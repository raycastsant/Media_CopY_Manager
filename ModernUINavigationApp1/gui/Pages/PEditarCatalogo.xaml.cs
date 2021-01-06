using FirstFloor.ModernUI.Windows.Controls;
using MCP.db;
using MCP.gui.components.IconItem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MCP.gui.Pages
{
    /// <summary>
    /// Interaction logic for PScanner.xaml
    /// </summary>

    public partial class PEditarCatalogo : UserControl, IListRefreshable
    {
        private GridLength c1Width;
        private readonly string TOPIC_GENEROS = "Géneros";
        private readonly string TOPIC_PAISES = "Paises";
        private List<genero> current_generos_list;
        private List<pais> current_paises_list;
        private media_files current_mf;
        private IconItem current_treeItem;
        private List<int> non_phisical_MF_ids;
        private bool content_loaded;

        public bool treeContentChanged { get; set; }

        public PEditarCatalogo()
        {
            InitializeComponent();

            HideForm();

            AppMAnager.PEditarCatalogo_instance = this;
            c1Width = _c1.Width;
            _c1.Width = GridLength.Auto;
            
            current_generos_list = new List<genero>();
            current_paises_list = new List<pais>();

            BtnGuardar.IsEnabled = false;
            BtnDelete.IsEnabled = false;

            notification_panel.Visibility = Visibility.Hidden;

            // _tab.SelectionChanged += new SelectionChangedEventHandler(TabSelectionChanged);

            treeContentChanged = true;
            content_loaded = true;

            this.Loaded += ContentLoaded;
            this.Unloaded += ContentUnloaded;
        }

        public void DisableButtons()
        {
            BtnGuardar.IsEnabled = false;
            BtnDelete.IsEnabled = false;
        }

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
                AppMAnager.current_page = "PEditarCatalogo";
            }
        }

        public async void reloadContent()
        {
            Console.WriteLine("Start EditPage Load...");
            //AppMAnager.SetWaitCursor();
            ShowLoader();

            await AppMAnager.Load_Movies_Tree(_tree, new EditionIconItemClickHandler(), Dispatcher, false);

            HideLoader();
            //AppMAnager.RestoreCursor();
            Console.WriteLine("EditPage Content Loaded");
            treeContentChanged = false;
        }

        private void ShowLoader()
        {
            AppMAnager.SetWaitCursor();
            _grid.Visibility = Visibility.Hidden;
            _LoaderGif.IsActive = true;
            _LoaderGif.Visibility = Visibility.Visible;
        }

        private void HideLoader()
        {
            AppMAnager.RestoreCursor();
            _grid.Visibility = Visibility.Visible;
            _LoaderGif.IsActive = false;
            _LoaderGif.Visibility = Visibility.Hidden;
        }

        public void HideForm()
        {
            _formControl.Visibility = Visibility.Hidden;
            _cForm.Width = new GridLength(0);
        }

        private void ShowForm()
        {
            _c1.Width = c1Width;
            _cForm.Width = new GridLength(680);
            _formControl.Visibility = Visibility.Visible;
        }

        /** Seleccionar Archivo de video o Carpeta */
        private void BtnOpenMovieFile(object sender, RoutedEventArgs e)
        {
            String selectedPath = "";
            string selected = "";
            if (!String.IsNullOrEmpty(tbFichero.Text))
            {
                if (current_mf.is_folder)
                    selectedPath = tbFichero.Text;
                else
                    selectedPath = Path.GetDirectoryName(tbFichero.Text);
            }

            if(current_mf.is_folder)
                selected = AppMAnager.showFolderBrowser(selectedPath);
            else
                selected = AppMAnager.ShowOpenMovieDialog(selectedPath);

            if (!String.IsNullOrEmpty(selected))
            {
                tbFichero.Text = selected;
            }
        }

        private void BtnOpenSubtitle_Click(object sender, RoutedEventArgs e)
        {
            String selectedPath = "";
            if (!String.IsNullOrEmpty(tbSubtitulo.Text))
            {
                if (current_mf.is_folder)
                    selectedPath = tbSubtitulo.Text;
                else
                    selectedPath = Path.GetDirectoryName(tbSubtitulo.Text);
            }

            string selected = AppMAnager.ShowOpenSubtitleDialog(selectedPath);
            if (!String.IsNullOrEmpty(selected))
            {
                tbSubtitulo.Text = selected;
            }
        }

        private void BtnOpenPortada_Click(object sender, RoutedEventArgs e)
        {
            OpenPortada();
        }

        private void OpenPortada()
        {
            String selectedPath = "";
            if (!String.IsNullOrEmpty(current_mf.fichero_portada))
            {
                if (current_mf.is_folder)
                    selectedPath = current_mf.fichero_portada;
                else
                    selectedPath = Path.GetDirectoryName(current_mf.fichero_portada);
            }

            string selected = AppMAnager.ShowOpenImageDialog(selectedPath);
            if (!String.IsNullOrEmpty(selected))
            {
                LoadPortadaFile(selected);
            }
        }
        
        /**Guardar Media File*/
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            AppMAnager.SetWaitCursor();

            current_treeItem.setTitle(tbTitulo.Text);
            current_mf.titulo = tbTitulo.Text;
            current_mf.file_url = tbFichero.Text;
            current_mf.str_file = tbSubtitulo.Text;

            /*  current_mf.titulo = tbTitulo.Text;
              current_mf.file_url = tbFichero.Text;
              current_mf.str_file = tbSubtitulo.Text;
              current_mf.fichero_trailer = tbTrailer.Text;

              if (IPortadaPreview.Tag != null && !String.IsNullOrEmpty(IPortadaPreview.Tag.ToString()))
                  current_mf.fichero_portada = IPortadaPreview.Tag.ToString();
              else
                  current_mf.fichero_portada = null;

              current_mf.director = tbDirector.Text;

              if(tbAnno.Text.Trim().Length > 0)
                  current_mf.anno = int.Parse(tbAnno.Text);

              if (tbDuracion.Text.Trim().Length > 0)
                  current_mf.duracion = int.Parse(tbDuracion.Text);

              current_mf.productora = tbProductora.Text;
              current_mf.reparto = tbReparto.Text;
              current_mf.sinopsis = tbSinopsis.Text;
              current_mf.premios = tbPremios.Text;

              //Generos
              media_generos mg;
              List<media_generos> mgList = new List<media_generos>();
              foreach (genero g in current_generos_list)
              {
                  mg = new media_generos(g.id, current_mf.id);
                  mgList.Add(mg);
              }

              //Paises
              media_paises mp;
              List<media_paises> mpList = new List<media_paises>();
              foreach (pais p in current_paises_list)
              {
                  mp = new media_paises(p.id, current_mf.id);
                  mpList.Add(mp);
              }*/

            List<media_generos> mgList = new List<media_generos>();
            List<media_paises> mpList = new List<media_paises>();
            ReadMediaFormDescriptions(current_mf, mgList, mpList);

            MediaFilesRepository repo = DBManager.MediaFilesRepo; 
            repo.Update(current_mf, mgList, mpList, true);

            if (chbxSaveRecursive.IsChecked == true)
            {
                DbContextTransaction transaction = DBManager.Context.Database.BeginTransaction();
                try
                {
                    SaveRecursiveMediaFiles(current_mf);
                   // AppMAnager.GlobalContentChanged();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    MessageBoxButton btn = MessageBoxButton.OK;
                    transaction.Rollback();
                    ModernDialog.ShowMessage("Ocurrió un error al realizar la operación." + "\n" + ex.Message, "Error", btn);
                }
            }

            PHome._PHome.SetContentChanged();
            AppMAnager.RestoreCursor();
        }

        /**Guarda las descripciones de un MediaFile en todos sus hijos*/
        public void SaveRecursiveMediaFiles(media_files mf)
        {
            MediaFilesRepository mediaRepo = DBManager.MediaFilesRepo;
            List<media_files> mediaList = mediaRepo.FindByCategoria(mf.categoria_id, mf.id, false);
            if (mediaList.Count > 0)
            {
                List<media_generos> mgList;
                List<media_paises> mpList;
                foreach (media_files child in mediaList)
                {
                    mgList = new List<media_generos>();
                    mpList = new List<media_paises>();
                    ReadMediaFormDescriptions(child, mgList, mpList);
                    mediaRepo.Update(child, mgList, mpList, false);

                    SaveRecursiveMediaFiles(child);
                }
            }
        }

        /**Lee la informacion de las decripciones (lo que es comun para todos los objetos en una carpeta)
         * del formulario y la establece 
         * en la variable MEdiaFile que se esta editando*/
        private void ReadMediaFormDescriptions(media_files mf, List<media_generos> mgList, List<media_paises> mpList)
        {
            mf.fichero_trailer = tbTrailer.Text;

            if (IPortadaPreview.Tag != null && !String.IsNullOrEmpty(IPortadaPreview.Tag.ToString()))
                mf.fichero_portada = IPortadaPreview.Tag.ToString();
            else
                mf.fichero_portada = null;

            mf.director = tbDirector.Text;

            if (tbAnno.Text.Trim().Length > 0)
                mf.anno = int.Parse(tbAnno.Text);

            if (tbDuracion.Text.Trim().Length > 0)
                mf.duracion = int.Parse(tbDuracion.Text);

            mf.productora = tbProductora.Text;
            mf.reparto = tbReparto.Text;
            mf.sinopsis = tbSinopsis.Text;
            mf.premios = tbPremios.Text;

            //Generos
            media_generos mg;
            foreach (genero g in current_generos_list)
            {
                mg = new media_generos(g.id, current_mf.id);
                mgList.Add(mg);
            }

            //Paises
            media_paises mp;
            foreach (pais p in current_paises_list)
            {
                mp = new media_paises(p.id, current_mf.id);
                mpList.Add(mp);
            }
        }

        /**Carga un mediaFile al formulario*/
        public void LoadMediaFile(media_files mf, IconItem iitem) 
        {
            if (_formControl.Visibility == Visibility.Hidden)
            {
                ShowForm();

              /* _c1.Width = c1Width;
                _cForm.Width = new GridLength(680);
                _formControl.Visibility = Visibility.Visible;*/
            }

            if (mf.is_folder)
            {
                lfichero.Text = "Carpeta";
                lDuracion.Visibility = Visibility.Hidden;
                tbDuracion.Visibility = Visibility.Hidden;
                _rowSubtitleLAbel.Height = new GridLength(0);
                _rowSubtitleTextField.Height = new GridLength(0);
                chbxSaveRecursive.Visibility = Visibility.Visible;
            }
            else
            {
                lfichero.Text = "Archivo de película";
                lDuracion.Visibility = Visibility.Visible;
                tbDuracion.Visibility = Visibility.Visible;
                _rowSubtitleLAbel.Height = new GridLength(25);
                _rowSubtitleTextField.Height = new GridLength(25);
                chbxSaveRecursive.Visibility = Visibility.Hidden;
            }

            tbTitulo.Text = mf.titulo;
            tbFichero.Text = mf.file_url;
            tbSubtitulo.Text = mf.str_file;

            LoadPortadaFile(mf.fichero_portada);

            tbDirector.Text = mf.director;
            tbAnno.Text = mf.anno.ToString();
            tbDuracion.Text = mf.duracion.ToString();
            tbProductora.Text = mf.productora;
            tbReparto.Text = mf.reparto;
            tbSinopsis.Text = mf.sinopsis;
            tbPremios.Text = mf.premios;
            tbTrailer.Text = mf.fichero_trailer;

            List<genero> generos_list = new List<genero>();
            foreach(media_generos mg in mf.media_generos)
            {
                generos_list.Add(mg.genero);
            }
            RefreshMediaGeneros(generos_list);

            List<pais> paises_list = new List<pais>();
            foreach (media_paises mp in mf.media_paises)
            {
                paises_list.Add(mp.pais);
            }
            RefreshMediaPaises(paises_list);

            current_mf = mf;
            current_treeItem = iitem;

            BtnOpenSubtitle.IsEnabled = !mf.is_folder;
            lSubtitulo.IsEnabled = !mf.is_folder;
           

            chbxSaveRecursive.IsChecked = false;
            // BtnOpenPortada.IsEnabled = !mf.is_folder;

            BtnGuardar.IsEnabled = true;
            BtnDelete.IsEnabled = true;
        }

        private void LoadPortadaFile(string path)
        {
            if (File.Exists(path))
            {
                ImageSourceConverter isc = new ImageSourceConverter();
                IPortadaPreview.Source = (ImageSource)isc.ConvertFromString(path);
                IPortadaPreview.Tag = path;
                BtnClearPOrtada.Visibility = Visibility.Visible;
            }
            else
            {
                IPortadaPreview.Tag = null;
                IPortadaPreview.Source = null;
                BtnClearPOrtada.Visibility = Visibility.Hidden;
            }
        }

        private void RefreshMediaGeneros(List<genero> list)
        {
            tbGeneros.Clear();
            current_generos_list = new List<genero>();
            if (list != null && list.Count > 0)
            {
                //IEnumerator<media_generos> list = mf.media_generos.GetEnumerator();
                //media_generos genero;
                bool isfirst = true;
                foreach (genero genero in list)
                {
                    if(!isfirst)
                        tbGeneros.Text += ", ";

                    //genero = list.Current;
                    tbGeneros.Text += genero.nombre;
                    isfirst = false;

                    current_generos_list.Add(genero);
                }
            }
        }

        private void RefreshMediaPaises(List<pais> list)
        {
            tbPaises.Clear();
            current_paises_list = new List<pais>();
            if (list != null && list.Count > 0)
            {
                bool isfirst = true;
                foreach (pais pais in list)
                {
                    if (!isfirst)
                        tbPaises.Text += ", ";

                    tbPaises.Text += pais.nombre;
                    isfirst = false;

                    current_paises_list.Add(pais);
                }
            }
        }

        private void BtnSetGeneros_Click(object sender, RoutedEventArgs e)
        {
            SetGeneros();
        }

        private void SetGeneros()
        {
            GenerosRepository repo = new GenerosRepository();
            List<string> current = new List<string>();
            foreach (genero g in current_generos_list)
                current.Add(g.nombre);

            ListSelectorDialog lsd = new ListSelectorDialog(TOPIC_GENEROS, this, repo.List, current);
            lsd.ShowDialog();
        }

        private void SetPaises()
        {
            PaisesRepository repo = new PaisesRepository();
            List<string> current = new List<string>();
            foreach (pais p in current_paises_list)
                current.Add(p.nombre);

            ListSelectorDialog lsd = new ListSelectorDialog(TOPIC_PAISES, this, repo.List, current);
            lsd.ShowDialog();
        }

        private void BtnPaises_Click(object sender, RoutedEventArgs e)
        {
           SetPaises();
        }

        /**Establece la lista de Generos/Paises desde un dialogo.*/
        public void RefresshList(string topic, IList entity_list)
        {
            if (entity_list != null && entity_list.Count > 0)
            {
                if (topic == TOPIC_GENEROS)
                {
                    List<genero> list = new List<genero>();
                    foreach (object obj in entity_list)
                    {
                        if (obj is genero)
                            list.Add((genero)obj);
                    }
                    RefreshMediaGeneros(list);
                }
                else
                if (topic == TOPIC_PAISES)
                {
                    List<pais> list = new List<pais>();
                    foreach (object obj in entity_list)
                    {
                        if (obj is pais)
                            list.Add((pais)obj);
                    }
                    RefreshMediaPaises(list);
                }
            }
        }

        private void lGéneros_MouseLeftButtonUp_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SetGeneros();
        }

        private void lPaises_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SetPaises();
        }

        private void BtnClearPOrtada_Click(object sender, RoutedEventArgs e)
        {
            IPortadaPreview.Source = null;
            IPortadaPreview.Tag = null;
            //current_mf.fichero_portada = null;
            BtnClearPOrtada.Visibility = Visibility.Hidden;
        }

        private void BtnOpenTrailer_Click(object sender, RoutedEventArgs e)
        {
            String selectedPath = "";
            if (!String.IsNullOrEmpty(tbTrailer.Text))
            {
                if (current_mf.is_folder)
                    selectedPath = tbTrailer.Text;
                else
                    selectedPath = Path.GetDirectoryName(tbTrailer.Text);
            }

            string selected = AppMAnager.ShowOpenMovieDialog(selectedPath);
            if (!String.IsNullOrEmpty(selected))
            {
                tbTrailer.Text = selected;
            }
        }

        private void lImagen_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OpenPortada();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if(current_mf != null)
            {
                MessageBoxResult res = MessageBox.Show("¿Confirma que desea eliminar el archivo del catálogo?", "Información", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                {
                    DBManager.MediaFilesRepo.DeleteEntity(current_mf);

                    IconItem item = (IconItem)_tree.SelectedItem;
                    if ( !(item.Parent is TreeView) )
                        ((IconItem)item.Parent).Items.Remove(item);

                    // ((TreeView)tab.Content).Items.Remove(item);
                    // else
                    //   ((IconItem)item.Parent).Items.Remove(item);

                    HideForm();

                    current_mf = null;
                    current_treeItem = null;

                    BtnGuardar.IsEnabled = false;
                    BtnDelete.IsEnabled = false;

                    AppMAnager.GlobalContentChanged();
                }
            }
        }

        private void BtnAddMedia_Click(object sender, RoutedEventArgs e)
        {

        }

        public void SetContentChanged()
        {
            treeContentChanged = true;
        }

        /**
         * Devuelve si el label que muestra el cartel de los MF eliminados se esta mostrando.
         */
        public bool LabelInfoDeletedMF_isHidden()
        {
            return notification_panel.Visibility == Visibility.Hidden;
        }

        /**
         * Notifica que existen Media Files que no se encuentran fisicamente, 
         * y da la opcion de eliminarlos del catalogo.
         */
        public void ShowDeletedMF_Notification()
        {
            notification_panel.Visibility = Visibility.Visible;
        }

        /**
         * Guarda en una lista el id de un MF para eliminarlo de la BD.
         */
        public void RegisterNonPhisicalMF_ID(int id)
        {
            if (non_phisical_MF_ids == null)
                non_phisical_MF_ids = new List<int>();

            non_phisical_MF_ids.Add(id);
        }

        /**
         * Elimina del catalogo los registros que no se encuentran fisicamente.
         */
        private async void btnDeleteUnphisicalsMF_Click(object sender, RoutedEventArgs e)
        {
            ShowLoader();

            DbContextTransaction transaction = DBManager.Context.Database.BeginTransaction();
            try
            {
                foreach (int mf_id in non_phisical_MF_ids)
                {
                    DBManager.MediaFilesRepo.Delete(mf_id);
                }

                transaction.Commit();
                AppMAnager.GlobalContentChanged();

                await AppMAnager.Load_Movies_Tree(_tree, new EditionIconItemClickHandler(), Dispatcher, false);
                notification_panel.Visibility = Visibility.Hidden;

            }
            catch (Exception ex)
            {
                MessageBoxButton btn = MessageBoxButton.OK;
                transaction.Rollback();
                ModernDialog.ShowMessage("Ocurrió un error al realizar la operación." + "\n" + ex.Message, "Error", btn);
            }

            HideLoader();
        }
    }
}
