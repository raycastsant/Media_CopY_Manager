using FirstFloor.ModernUI.Windows;
using FirstFloor.ModernUI.Windows.Controls;
using MCP.db;
using MCP.gui.components;
using MCP.gui.components.IconItem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;

namespace MCP.gui.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class PHome : UserControl
    {
        private bool firstAddition, colorChange;
        private Brush original_brush;
        private BrushConverter _BrushConverter;
        private double costoLista;
        private long listCopySize;
        private int tpagoId;
        private bool querying;
        private bool pending_query;
        private bool treeContentChanged;
        private bool content_loaded;

        public static PHome _PHome;

        public PHome()
        {
            InitializeComponent();
            
            AppMAnager.HAND_CURSOR = _treeView.Cursor;
           
//            _fcontent.MouseLeftButtonUp += new MouseButtonEventHandler(TabMouseClick);

            _PHome = this;
            _infoPanel.Visibility = Visibility.Hidden;
           // _listSeleccion.MaxHeight = System.Windows.SystemParameters.MaximizedPrimaryScreenHeight - 270;

            //_MediaPayer.Hide();
           // firstRender = true;   //Para controlar los triggers cuando se renderizan los tabs
            firstAddition = true;  //Para mostrar el panel de la lista de copia la primera vez que se añade
            lCostoReal.Tag = lCostoReal.Text;

            _cListCopys.MinWidth = 0;
            _cBtnHideList.MinWidth = 0;
            _cTree.MinWidth = 150;
            _cFormInfo.MinWidth = 250;
            _cListCopys.Width = new GridLength(0);
            _cBtnHideList.Width = new GridLength(0);
            _cTree.Width = new GridLength(300);
            _cFormInfo.Width = new GridLength(350);

            tpagoId = DBManager.TipoPagosRepo.FindByCode("TA").id;  //Pago por tipo de archivo
            restartCostInfo();

            _listViewContent.SelectionChanged += new SelectionChangedEventHandler(ListContentSelectionChanged);

            colorChange = false;
            original_brush = btnLista.Foreground;
            _BrushConverter = new BrushConverter();
            DispatcherTimer dt = new DispatcherTimer();
            dt.Tick += new EventHandler(TimerTic);
            dt.Interval = new TimeSpan(0, 0, 1);
           // dt.Start();

            //Buscador de cambioes externos
           // StartContentChangesCheckig();

            querying = false;
            pending_query = false;

            //ShowLoader();

            // cload = new ContentLoad();
            //this.DataContext = cload;

            //_fcontent.SelectionChanged += new SelectionChangedEventHandler(TabSelectionChanged);

            treeContentChanged = true;
            content_loaded = true;

            this.Loaded += ContentLoaded;
            this.Unloaded += ContentUnloaded;

            _comboCategorias.SelectionChanged += new SelectionChangedEventHandler(ComboCategoriasChanged);
            _comboAnnos.SelectionChanged += new SelectionChangedEventHandler(ComboAnnosChanged);
            _comboGeneros.SelectionChanged += new SelectionChangedEventHandler(ComboGenerosChanged);

            //this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(visibiliyChanged);

            //LoadContent();
        }

        /*   public void StartContentChangesCheckig()
           {
               DispatcherTimer changes_timer = new DispatcherTimer();
               changes_timer.Tick += new EventHandler(OnTimedEvent);
               changes_timer.Interval = new TimeSpan(0, 0, 15);
               changes_timer.Start();
           }
           int valor_int = 0;
           private void OnTimedEvent(object sender, EventArgs e) //(Object source, System.Timers.ElapsedEventArgs e)
           {
               //int valor_int = DBManager.Context.Database.SqlQuery<int>("select valor_int from preferencias where nombre='" + PreferencesRepository.CONTENT_CHANGED_KEY + "'").FirstOrDefault();

               if (valor_int == 1)
               {
                  // DBManager.PreferenciasRepo.SetContentUnchangedRegistry();
                   AppMAnager.GlobalContentChanged();
               }

               valor_int++;
           }*/

        /* private void visibiliyChanged(object sender, DependencyPropertyChangedEventArgs args)
         {
             LoadContent();
         }*/

        private void ContentUnloaded(object sender, RoutedEventArgs e)
        {
            /** Hago esto porque el evento Loaded(...) por alguna razón es llamado también cuando se va hacia otra página, 
             creando conflicto si la variable treeContentChanged es TRUE, ya que trata de actualizar el contenido de esta página 
             en paralelo con la otra que va a ser cargada. */
            content_loaded = !content_loaded;
        }

        private void ContentLoaded(object sender, RoutedEventArgs e)
        {
            if (treeContentChanged && content_loaded)
            {
                reloadContent();
                AppMAnager.current_page = "PHome";
            }
        }

        public async void reloadContent()
        {
            Console.WriteLine("Start Home Content Load...");

            //DBManager.Reset_Context();
            TbxSearch.IsEnabled = false;
            BtnClear.IsEnabled = false;
            ShowLoader();

            await AppMAnager.Load_Movies_Tree(_treeView, new HomeIconItemClickHandler(), Dispatcher, true);

            refreshComboCategorias();
            refreshAnnos();
            refreshGeneros();

            TbxSearch.IsEnabled = true;
            BtnClear.IsEnabled = true;
            HideLoader();
            Console.WriteLine("Home Content Loaded");
            treeContentChanged = false;
        }

        private async void ComboCategoriasChanged(object sender, SelectionChangedEventArgs args)
        {
            if (!treeContentChanged)
            {
                _comboCategoriasHint.Visibility = Visibility.Hidden;
                BtnClearCateg.Visibility = Visibility.Visible;
                await Filtrar();
            }
        }

        private async void ComboAnnosChanged(object sender, SelectionChangedEventArgs args)
        {
            if (!treeContentChanged)
            {
                _comboAnnosHint.Visibility = Visibility.Hidden;
                BtnClearAnno.Visibility = Visibility.Visible;
                await Filtrar();
            }
        }

        private async void ComboGenerosChanged(object sender, SelectionChangedEventArgs args)
        {
            if (!treeContentChanged)
            {
                _comboGenerosHint.Visibility = Visibility.Hidden;
                BtnClearGenero.Visibility = Visibility.Visible;
                await Filtrar();
            }
        }

        private void refreshComboCategorias()
        {
            _comboCategoriasHint.Visibility = Visibility.Visible;
            _comboCategorias.Items.Clear();
            BtnClearCateg.Visibility = Visibility.Hidden;
            List<categoria> list = DBManager.CategoriasRepo.List;
            foreach(categoria categ in list)
            {
                _comboCategorias.Items.Add(categ);
            }
        }

        private void refreshAnnos()
        {
            _comboAnnosHint.Visibility = Visibility.Visible;
            _comboAnnos.Items.Clear();
            BtnClearAnno.Visibility = Visibility.Hidden;
            List<int?> list = DBManager.MediaFilesRepo.ListMediaYears();
            list.Sort();
            foreach (object year in list)
            {
                if(year != null)
                    _comboAnnos.Items.Add(year);
            }
        }

        private void refreshGeneros()
        {
            _comboGenerosHint.Visibility = Visibility.Visible;
            _comboGeneros.Items.Clear();
            BtnClearGenero.Visibility = Visibility.Hidden;
            GenerosRepository repo = new GenerosRepository();
            List<genero> list = repo.List;
            
            foreach (genero genero in list)
            {
                _comboGeneros.Items.Add(genero);
            }
        }

        private async void BtnClearCateg_Click(object sender, RoutedEventArgs e)
        {
            _comboCategorias.SelectedIndex = -1;
            BtnClearCateg.Visibility = Visibility.Hidden;
            _comboCategoriasHint.Visibility = Visibility.Visible;
            await Filtrar();
        }

        private async void BtnClearAnno_Click(object sender, RoutedEventArgs e)
        {
            _comboAnnos.SelectedIndex = -1;
            BtnClearAnno.Visibility = Visibility.Hidden;
            _comboAnnosHint.Visibility = Visibility.Visible;
            await Filtrar();
        }

        private async void BtnClearGenero_Click(object sender, RoutedEventArgs e)
        {
            _comboGeneros.SelectedIndex = -1;
            BtnClearGenero.Visibility = Visibility.Hidden;
            _comboGenerosHint.Visibility = Visibility.Visible;
            await Filtrar();
        }

        private void restartCostInfo()
        {
            costoLista = 0;
            lCosto.Text = "Costo calculado: 0";
            tbReal.Text = "0";
            
            listCopySize = 0;
            lCopySize.Text = "0";

            AppMAnager.restoreDefaulLabel(lCostoReal);
            AppMAnager.restoreDefaultTextBox(tbReal);
            lCostoReal.Text = "Real:";
        }

        private async void TbxSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (!querying)
                await Filtrar();
            else
            {
                pending_query = true;
            }
        }

        private void ShowLoader()
        {
            AppMAnager.SetWaitCursor();
            _MainGrid.Visibility = Visibility.Hidden;
           // _MainGrid.IsEnabled = false;
            _LoaderGif.IsActive = true;
            _LoaderGif.Visibility = Visibility.Visible;
           
        }

        private void HideLoader()
        {
            
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                (Action)(() => {
                    AppMAnager.RestoreCursor();
                }));

            _MainGrid.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
              (Action)(() => {
                  _MainGrid.Visibility = Visibility.Visible;

              }));

            _LoaderGif.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                (Action)(() => {
                    _LoaderGif.IsActive = false;
                    _LoaderGif.Visibility = Visibility.Hidden;
                }));

          /*  TbxSearch.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
               (Action)(() => {
                   TbxSearch.IsEnabled = true;
               }));

            BtnClear.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
              (Action)(() => {
                  BtnClear.IsEnabled = true;
              }));*/
        }

        private async Task Filtrar()
        {
            string text = TbxSearch.Text;

            //object o = _fcontent.SelectedItem;
            //if (o != null)
            //{
            querying = true;
            Console.WriteLine("Start query: "+ text);
            ShowLoader();
            _listViewContent.Items.Clear();

            // TabItem page = (TabItem)o;
            //if(page != null)
            //{_comboCategorias

            int categId = -1;  // int.Parse(page.Tag.ToString());
            if(_comboCategorias.SelectedIndex >= 0)
            {
                categId = ((categoria)_comboCategorias.SelectedItem).id;
            }

            int year = -1;  
            if (_comboAnnos.SelectedIndex >= 0)
            {
                year = (int)_comboAnnos.SelectedItem;
            }

            int generoId = -1;  
            if (_comboGeneros.SelectedIndex >= 0)
            {
                generoId = ((genero)_comboGeneros.SelectedItem).id;
            }

            if (!string.IsNullOrEmpty(text) || categId > 0  || year > 0 || generoId > 0)
            {
                if (!pending_query)
                {
                    List<media_files> mf_list = await DBManager.MediaFilesRepo.AplyFilterAsync(categId, text, year, generoId);  // HomeCatalogManager.FiltrarCatalogoAsync(categId, text);

                    //  await Task.Run(async () =>
                    //{
                    //  await _listViewContent.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                    // (Action)(() =>
                    // {

                    ListViewMediaItem item;
                            foreach (media_files mf in mf_list)
                            {
                                if (!pending_query)
                                {
                                    var addItem = new Action(() => _listViewContent.Items.Add(item = new ListViewMediaItem(mf)));
                                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, addItem);

                                  //  item = new ListViewMediaItem(mf);
                                   // _listViewContent.Items.Add(item);
                                }
                                else
                                {
                                    break;
                                }
                            }
                       // }));

                        if (!pending_query)
                        {
                            _lContentDesc.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                            (Action)(() =>
                            {
                                _lContentDesc.Text = "0 elementos";
                            }));

                            Console.WriteLine("End Task Operations" + "\n");
                            HideLoader();
                            querying = false;
                        }
                   // });

                    if (pending_query)
                    {
                        pending_query = false;
                        await Filtrar();
                    }
                }
                else
                {
                    pending_query = false;
                    await Filtrar();
                }
            }
            else
            {
                Console.WriteLine("End Task Operations"+"\n");
                HideLoader();
                querying = false;
            }
        }

        private void ListContentSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectionChanged();
        }

        public async void selectionChanged()
        {
            if (_listViewContent.SelectedItems.Count > 0)
            {
                //btnAdd.IsEnabled = true;
                if (_listViewContent.SelectedItems.Count == 1) //Mostrar info del Item seleccionado
                {
                    int selectedMediaId = ((ListViewMediaItem)_listViewContent.SelectedItem).MediaId();
                    media_files mf = await DBManager.MediaFilesRepo.FindByIdAsync(selectedMediaId);
                    showInfoPanel(mf);
                }
                else
                {
                    _infoPanel.Visibility = Visibility.Hidden;
                }

                btnAdd.IsEnabled = checkSelectedFilesExistence();
            }
            else  //Si no hay contenido seleccionado veo si hay seleccion en el explorador
            {
                IconItem iitem = (IconItem)_treeView.SelectedItem;
                _infoPanel.Visibility = Visibility.Hidden;
                if (iitem != null)
                {
                    btnAdd.IsEnabled = iitem.FileExists();
                }
                else
                {
                    btnAdd.IsEnabled = false;
                }
            }
        }

        //Verificar lo seleccionado. TRUE si todos existen. Si existe algun item que no existe devuelve FALSE
        private bool checkSelectedFilesExistence()
        {
            foreach (ListViewMediaItem item in _listViewContent.SelectedItems)
            {
                if (!item.FileExists())
                {
                    return false;
                }
            }

            return true;
        }

        //Selecciona la carpeta del treeView seleccionado, correspondiente al mediaId pasado por parametros
        public void SelectTreeViewFolder(int mediaId)
        {
            foreach(IconItem iitem in _treeView.Items)
            {
                if(iitem.MediaId() == mediaId)
                {
                    iitem.IsSelected = true;
                    iitem.IsExpanded = true;
                    break;
                }
                else
                    seekToSelectItem(iitem.Items, mediaId);
            }
        }

        private void seekToSelectItem(ItemCollection list, int mediaId)
        {
            foreach (IconItem iitem in list)
            {
                if (iitem.MediaId() == mediaId)
                {
                    iitem.IsSelected = true;
                    iitem.IsExpanded = true;

                    IconItem parent = (IconItem)iitem.Parent;
                    while (parent != null)
                    {
                        parent.IsExpanded = true;
                        if (parent.Parent is IconItem)
                            parent = (IconItem)parent.Parent;
                        else
                            parent = null;
                    }

                    break;
                }
                else
                    seekToSelectItem(iitem.Items, mediaId);
            }
        }

        //Actualiza la informacion del formulario  de un MediaFile
        private void showInfoPanel(media_files mf)
        {
            if (mf != null)
            {
                _infoPanel.Visibility = Visibility.Visible;
                tbTitulo.Text = mf.titulo;
                tbUrlFile.Text = mf.file_url;

                if (File.Exists(mf.fichero_portada))
                {
                    ImageSourceConverter isc = new ImageSourceConverter();
                    IPortadaPreview.Source = (ImageSource)isc.ConvertFromString(mf.fichero_portada);
                    IPortadaPreview.Tag = mf.fichero_portada;
                    //BtnClearPOrtada.Visibility = Visibility.Visible;
                }
                else
                {
                    IPortadaPreview.Tag = null;
                    IPortadaPreview.Source = null;
                    //BtnClearPOrtada.Visibility = Visibility.Hidden;
                }

                if (!string.IsNullOrEmpty(mf.str_file))
                    tbSubtitulos.Text = mf.str_file;
                else
                    tbSubtitulos.Clear();

                if (!string.IsNullOrEmpty(mf.fichero_trailer))
                    tbTrailer.Text = mf.fichero_trailer;
                else
                    tbTrailer.Clear();

                if (!string.IsNullOrEmpty(mf.director))
                    tbDirector.Text = mf.director;
                else
                    tbDirector.Clear();

                if (!string.IsNullOrEmpty(mf.anno.ToString()))
                    tbAnno.Text = mf.anno.ToString();
                else
                    tbAnno.Clear();

                if (!string.IsNullOrEmpty(mf.duracion.ToString()))
                    tbDuracion.Text = mf.duracion.ToString();
                else
                    tbDuracion.Clear();

                if (!string.IsNullOrEmpty(mf.sinopsis))
                    tbSinopsis.Text = mf.sinopsis;
                else
                    tbSinopsis.Clear();

                if (!string.IsNullOrEmpty(mf.premios))
                    tbPremios.Text = mf.premios;
                else
                    tbPremios.Clear();

                if (!string.IsNullOrEmpty(mf.productora))
                    tbCadena.Text = mf.productora;
                else
                    tbCadena.Clear();

                if (!string.IsNullOrEmpty(mf.reparto))
                    tbReparto.Text = mf.reparto;
                else
                    tbReparto.Clear();

                tbGeneros.Clear();
                bool isfirst = true;
                foreach (media_generos mg in mf.media_generos)
                {
                    if (!isfirst)
                        tbGeneros.Text += ", ";

                    tbGeneros.Text += mg.genero;
                    isfirst = false;
                }

                tbPaises.Clear();
                isfirst = true;
                foreach (media_paises mp in mf.media_paises)
                {
                    if (!isfirst)
                        tbPaises.Text += ", ";

                    tbPaises.Text += mp.pais;
                    isfirst = false;
                }

                /*if (!mf.is_folder)
                {
                    _MediaPayer.LoadMediaFile(mf.file_url);
                }
                else
                {
                    _MediaPayer.Hide();
                    _MediaPayer.Stop();
                }*/
            }
        }

        /** COPIAR LISTA SELECCIONADA */
        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            string realValue = tbReal.Text.Replace(" ", "");
         
            while (realValue.StartsWith("0"))
                realValue = realValue.Substring(1);

            if (string.IsNullOrWhiteSpace(realValue) || double.Parse(realValue) <= 0)
            {
                AppMAnager.SetLabel_Error(lCostoReal, "Real:");
                AppMAnager.SetErrorTextBox(tbReal);
                MessageBox.Show("El costo real debe ser mayor que 0");
                return;
            }

            string destination = AppMAnager.showFolderBrowser("");
            if (Directory.Exists(destination))
            {
                string root = Directory.GetDirectoryRoot(destination);
                long freespace = new DriveInfo(root).AvailableFreeSpace;

                List<MediaFile_Basic_Info> mf_list = new List<MediaFile_Basic_Info>();
                MediaFile_Basic_Info MFBI;
                foreach (StackPanel sp in _listSeleccion.Items)
                {
                    MFBI = (MediaFile_Basic_Info)sp.Tag;
                    mf_list.Add(MFBI);
                    //copySize += MFBI.getTotalSize();
                }

                if (freespace < listCopySize)
                {
                    double free = Math.Round( StorageConverter.Convert(Differential.ByteToGiga, freespace, StorageBase.BASE2) , 2);
                    double need = Math.Round( StorageConverter.Convert(Differential.ByteToGiga, listCopySize, StorageBase.BASE2) , 2);
                    MessageBox.Show("No hay espacio suficiente en el dispositivo seleccionado"+"\n"+
                                    "Necesario: "+need+"Gb       Disponible: "+free+"Gb");
                }
                else
                {
                    if (_copySplitter.Visibility == Visibility.Hidden)
                    {
                        _copysRow.Height = new GridLength(100);
                        _copySplitter.Visibility = Visibility.Visible;
                    }

                  //  ProgressInfo pinfo = new ProgressInfo();
                   // int pos = _copysContainer.Children.Add(pinfo);  //Uso la posicion del componente para enlazarlo con el BackgroundMediaCopy
                   // pinfo.Tag = pos;

                    double montoReal = 0;
                    if (!string.IsNullOrEmpty(realValue))
                        montoReal = double.Parse(realValue);

                    copia c = new copia
                    {
                        user_id = AppMAnager.CurrentUser().id,
                        punto_copia_id = AppMAnager.CurrentPuntoCopia().id,
                        codigo = DBManager.CopiasRepo.NextSerie().ToString(),
                        tipo_pago_id = tpagoId,
                        fecha = DateTime.Now,
                        monto_sistema = costoLista,
                        monto_real = montoReal
                    };
                    copia the_copy = DBManager.CopiasRepo.Add(c);

                    if (the_copy != null)
                    {
                        BackgroundMediaFileCopy copier = new BackgroundMediaFileCopy(the_copy.id);
                        ProgressInfo pinfo = new ProgressInfo(copier);
                        _copysContainer.Children.Add(pinfo);
                        copier.StartCopyWorker(mf_list, destination, pinfo);
                        AppMAnager.RUNNING_COPYS_COUNT++;

                        //ClearPageSelection(true);
                        // AppMAnager.thread_copys.Add(pos, copier);

                        _listSeleccion.Items.Clear();
                        btnCopy.IsEnabled = false;

                        restartCostInfo();
                    }
                }
            }
        }

        /** Muestra el panel de lista de copias*/
        private void btnLista_Click(object sender, RoutedEventArgs e)
        {
            ShowCopyList();
        }
        
        /** Muestra el panel de lista de copias*/
        private void ShowCopyList()
        {
            int w = 300;
            _cBtnHideList.MinWidth = 30;
            _cBtnHideList.Width = new GridLength(30);
            _cBtnShowList.MinWidth = 0;
            _cBtnShowList.Width = new GridLength(0);

            new Thread(() => {
                int i = 0;
                while (i <= w)
                {
                    _cListCopys.Dispatcher.Invoke(() => {
                        _cListCopys.MinWidth = i;
                    });

                    Thread.Sleep(1);
                    i = i + 2;
                }
            }).Start();
        }

        /** Oculta el panel de lista de copias*/
        private void btnHideList_Click(object sender, RoutedEventArgs e)
        {
            int w = 300;
            _cBtnHideList.MinWidth = 0;
            _cBtnHideList.Width = new GridLength(0);
            _cBtnShowList.MinWidth = 30;
            _cBtnShowList.Width = new GridLength(30);

            new Thread(() => {
                int i = 0;
                while (i <= w)
                {
                    _cListCopys.Dispatcher.Invoke(() => {
                        _cListCopys.MinWidth = w;
                    });

                    Thread.Sleep(1);
                    w = w-2;
                }
            }).Start();
        }

        /** Adicionar elemento a la lista de copia*/
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (firstAddition)
            {
                ShowCopyList();
                firstAddition = false;
            }

            media_files mf;
            string btnxaml = XamlWriter.Save(btnDelete);
            if(_listViewContent.SelectedItems.Count > 0)
            {
                bool fileExist;
                foreach (ListViewMediaItem item in _listViewContent.SelectedItems)
                {
                    mf = DBManager.MediaFilesRepo.FindById(item.MediaId());

                    if (mf.is_folder)
                        fileExist = Directory.Exists(mf.file_url);
                    else
                        fileExist = File.Exists(mf.file_url);

                    if (mf != null && !inList(mf.id) && fileExist)
                    {
                        btnCopy.IsEnabled = true;
                        addMediaToCopyList(mf, btnxaml);
                    }
                }
            }
            else  //Si no hay contenido seleccionado veo si hay seleccion en el explorador
            {
                IconItem iitem = (IconItem)_treeView.SelectedItem;
                if (iitem != null)
                {
                    mf = DBManager.MediaFilesRepo.FindById(iitem.MediaId());
                    if (mf != null && !inList(mf.id))
                    {
                        btnCopy.IsEnabled = true;
                        addMediaToCopyList(mf, btnxaml);
                    }
                }
            }
        }

        //metodo auxiliar para adicionar un elemento a la lista de copia
        private void addMediaToCopyList(media_files mf, string btnxaml)
        {
            StringReader sreader = new StringReader(btnxaml);
            XmlReader xmlreader = XmlReader.Create(sreader);
            ModernButton delBtn = (ModernButton)XamlReader.Load(xmlreader);
            delBtn.Visibility = Visibility.Visible;
            delBtn.Click += btnDelete_Click;
            delBtn.Tag = _listSeleccion.Items.Count;  //Guardo la posicion para localizar el item a la hora de eliminar

            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            sp.Children.Add(delBtn);

            MediaFile_Basic_Info MFBI = new MediaFile_Basic_Info();
            MFBI.ReadMediaFile(mf);
            sp.Tag = MFBI;
            sp.ToolTip = MFBI.ToString();
            TextBlock tb = new TextBlock();
            tb.Foreground = lListTile.Foreground;
            tb.Text = MFBI.ToString();
            sp.Children.Add(tb);

            double cost = MFBI.getCosto(tpagoId);
            UpdateListCost(cost);
            UpdateListSize( MFBI.getTotalSize() );

            _listSeleccion.Items.Add(sp);
        }

        private void UpdateListCost(double cost)
        {
            costoLista += cost;
            lCosto.Text = "Costo calculado: $" + costoLista;
        }

        private void UpdateListSize(long size)
        {
            listCopySize += size;
            lCopySize.Text = "["+Math.Round( StorageConverter.Convert(Differential.ByteToGiga, listCopySize, StorageBase.BASE2) , 2 )+" Gb]";
        }

        /** Verificar si un elemento ya existe en la lista de copias*/
        private bool inList(int mfId)
        {
            MediaFile_Basic_Info MFBI;
            foreach (StackPanel sp in _listSeleccion.Items)
            {
                MFBI = (MediaFile_Basic_Info)sp.Tag;
                if (MFBI.id == mfId)
                {
                    return true;
                }
            }

            return false;
        }

        /** Eliminar elemento de la lista de copia*/
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            string t = ((ModernButton)sender).Tag.ToString();
            int pos = int.Parse(t);
            
            if (pos >= 0 && pos < _listSeleccion.Items.Count)
            {
                //rebajar primero el costo total de la lista
                StackPanel spanel = (StackPanel)_listSeleccion.Items.GetItemAt(pos);
                MediaFile_Basic_Info MFBI = (MediaFile_Basic_Info)spanel.Tag;
                UpdateListCost( MFBI.getCosto(tpagoId) *-1 );  //Para que reste

                //rebajar el Size de la lista
                UpdateListSize( MFBI.getTotalSize() *-1);

                //Eliminar de la lista
                _listSeleccion.Items.RemoveAt(pos);

                //Actualizar index en los botones
                int i = 0;
                foreach (StackPanel sp in _listSeleccion.Items)
                {
                    var en = sp.Children.GetEnumerator();
                    en.MoveNext();
                    ((ModernButton)en.Current).Tag = i;
                    i++;
                }
            }

            if(_listSeleccion.Items.Count <= 0)
                btnCopy.IsEnabled = false;
        }

        /** Limpiar el filtro*/
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearFilter();
        }

        public async void ClearFilter()
        {
            TbxSearch.Clear();
            await Filtrar();
        }

        public ListView getListView()
        {
            return _listViewContent;
        }

        public void SetContentDescription(string value)
        {
            _lContentDesc.Text = value;
        }

        private void tbReal_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Regex regex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");  // 
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
            /* if (regex.IsMatch(e.Text) && !(e.Text == "." && ((TextBox)sender).Text.Contains(e.Text)))
                 e.Handled = false;  //regex.IsMatch(e.Text);
             else
                 e.Handled = true;*/
        }

        private void tbReal_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void TimerTic(object sender, EventArgs e)
        {
            if (_listSeleccion.Items.Count > 0)
            {
                if (colorChange)
                    btnLista.Foreground = (Brush)_BrushConverter.ConvertFrom("#FF00FFFF");
                else
                    btnLista.Foreground = original_brush;

                colorChange = !colorChange;
            }
            else
                btnLista.Foreground = original_brush;

           /* //Buscador de cambios externos
            changesTicCount++;
            if (changesTicCount == changesTicCount_trigger_value)
            {
                changesTicCount = 0;
                AppMAnager.GlobalContentChanged();
            }*/
        }

        public void CancelRunningCopys()
        {
            foreach(ProgressInfo pinfo in _copysContainer.Children)
            {
                pinfo.CancelCopy();
            }
        }

        

        public void SetContentChanged()
        {
            treeContentChanged = true;
        }
    }
}
