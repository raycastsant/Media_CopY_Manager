using MCP.db;
using MCP.gui.components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using FirstFloor.ModernUI.Windows.Controls;
using MCP.gui.components.IconItem;

namespace MCP.gui.Pages
{
    /// <summary>
    /// Interaction logic for PScanner.xaml
    /// </summary>
    public partial class PScanner : UserControl
    {
        //  public MediaFilePlayer _MFPlayer;
        //private BackgroundWorker worker;
        // private BackgroundWorker worker;

        private bool contentChanged;
        private int totalSaveOperations;
        private int i;
        //private MediaFile_Basic_Info selectedList;
        private Dictionary<categoria, MediaFile_Basic_Info> selectedList;
        private List<string> existingFilesInDB;
        private bool content_loaded;
        private int nextMediaId;

        public static PScanner _PScanner;

        public PScanner()
        {
            InitializeComponent();

            _PScanner = this;
            // _MFPlayer = _media;

            //AppMAnager.loadCategoriasToTABS(_mTab);
            contentChanged = true;
            content_loaded = true;
            //this.IsVisibleChanged += ScheduleUserControl_IsVisibleChanged;

            //_PBar = pBar;

            this.Loaded += ContentLoaded;
            this.Unloaded += ContentUnloaded;
        }

        private void ContentUnloaded(object sender, RoutedEventArgs e)
        {
            /** Hago esto porque el evento Loaded(...) por alguna razón es llamado también cuando se va hacia otra página, 
             creando conflicto si la variable treeContentChanged es TRUE, ya que trata de actualizar el contenido de esta página 
             en paralelo con la otra que va a ser cargada. */
            content_loaded = !content_loaded;
        }

        private async void ContentLoaded(object sender, RoutedEventArgs e)
        {
            /* List<categoria> ListaCategorias = await DBManager.CategoriasRepo.ListAsync;
             if (!scheduled || ListaCategorias.Count != _tab.Items.Count)
                 initializeControls(ListaCategorias);*/
           
            if (contentChanged && content_loaded)
            {
                ShowLoader();

               // await AppMAnager.Load_Movies_Tree(_treeView, new EditionIconItemClickHandler(), Dispatcher, false);

                List<categoria> ListaCategorias = await DBManager.CategoriasRepo.ListAsync;

                 _tab.Items.Clear();
                 BtnScan.IsEnabled = true;
                 BtnSaveMovies.IsEnabled = false;

                 _pBar.Value = 0;
                 _cProgress.Height = new GridLength(0);

                 if (ListaCategorias.Count > 0)
                 {
                     TabItem page;
                     TreeView tree;

                     await Task.Run(async () =>
                     {
                         foreach (categoria categ in ListaCategorias)
                         {
                             await Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                               (Action)(() => {
                                   page = new TabItem
                                   {
                                       Header = categ.categoria1,
                                       Tag = categ
                                   };

                                   tree = new TreeView();
                                   page.Content = tree;
                                   _tab.Items.Add(page);
                               }));
                         }
                     });

                     contentChanged = false;
                 }

                HideLoader();
            }
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

        /**
         * Click del Boton Escanear
         */
        private void BtnScan_Click(object sender, RoutedEventArgs e)
        {
            AppMAnager.SetWaitCursor();
            _cProgress.Height = new GridLength(20);
            // _pBar.Value = 0;
            _pBar.Minimum = 0;
            _pBar.Maximum = 100;

            //_tab.ClipToBounds = true;
            BtnScan.IsEnabled = false;

            existingFilesInDB = new List<string>();

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += DoWork;
            worker.RunWorkerCompleted += workerEnded;
            worker.WorkerReportsProgress = true;
            worker.ProgressChanged += workerProgressChanged;
            worker.RunWorkerAsync();
        }

        //Escanear Categorias
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            List<categoria> ListaCategorias = DBManager.CategoriasRepo.List;
            if (ListaCategorias.Count > 0)
            {
                int Total = ListaCategorias.Count;
                int i = 0;
                int progress = 0;

                TreeView tree;
                foreach (categoria categ in ListaCategorias)
                {
                    _tab.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                    (Action)(() => {
                        TabItem page = (TabItem)_tab.Items.GetItemAt(i);
                        tree = (TreeView)page.Content;

                        tree.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        (Action)(() => {
                            tree.Items.Clear();

                            if (Directory.Exists(categ.carpeta))
                                LoadDirectory(categ, tree);
                        }));
                    }));
                        

                    i++;
                    progress = (i * 100) / Total;
                    (sender as BackgroundWorker).ReportProgress(progress);
                    Thread.Sleep(100);
                }
            }
            else
                ((Grid)_tab.Parent).Visibility = Visibility.Hidden; 
        }

        private void workerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _pBar.Value = e.ProgressPercentage;
            Console.WriteLine(e.ProgressPercentage);
        }

        private void workerEnded(object sender, RunWorkerCompletedEventArgs e)
        {
            AppMAnager.RestoreCursor();
            BtnSaveMovies.IsEnabled = true;
            // contentChanged = false;

            if (existingFilesInDB.Count > 0)
            {
                //new ModernWindow1().Show();
                Thread.Sleep(100);
                new ListDialog("Información", "Los siguientes archivos ya existen en el sistema y no serán guardados", existingFilesInDB).ShowDialog();
            }
        }

        /**Carga el directorio pasado por parametros al treeView*/
        private void LoadDirectory(categoria categ, TreeView treeView1)
        {
            //DirectoryInfo di = new DirectoryInfo(categ.carpeta);

            LoadSubDirectories(categ.carpeta, null, treeView1, categ.categoria1);
            LoadFiles(categ.carpeta, null, treeView1, categ.categoria1);
        }

        /**Carga el directorio pasado por parametros al TreeNode del treeView*/
        private void LoadSubDirectories(string dir, CheckItem td, TreeView treeView, string categName)
        {
            // Get all subdirectories  
            string[] subdirectoryEntries = Directory.GetDirectories(dir);
            // Loop through them to see if they have any other subdirectories  
            DirectoryInfo di;
            CheckItem tds;
            bool exists;
            foreach (string subdirectory in subdirectoryEntries)
            {
                if (AppMAnager.DirectoryHasMediaFiles(subdirectory))
                {
                    di = new DirectoryInfo(subdirectory);
                    exists = DBManager.MediaFilesRepo.Exists(di.FullName);

                    if (td != null)
                        tds = td.AddChild(di.Name, exists, true);
                    else
                    {
                        tds = new CheckItem(di.Name, exists, true);
                        treeView.Items.Add(tds);
                    }

                    //tds.StateImageIndex = 0;
                    tds.Tag = di;

                    LoadSubDirectories(subdirectory, tds, null, categName);
                    LoadFiles(subdirectory, tds, null, categName);
                    //UpdateProgress();
                }
            }
        }

        /**Carga los ficheros encontrados hacia un nodo del treeView*/
        private void LoadFiles(string dir, CheckItem td, TreeView treeView, string categName)
        {
            IEnumerable<string> Files = Directory.GetFiles(dir, "*.*", SearchOption.TopDirectoryOnly).Where(s => AppMAnager.TYPES_MEDIA.Contains(Path.GetExtension(s).ToLower()));

            // Loop through them to see files  
            bool exists;
            foreach (string file in Files)
            {
                FileInfo fi = new FileInfo(file);
                CheckItem tds;

                if (fi.Extension.Length > 0)
                {
                    exists = DBManager.MediaFilesRepo.Exists(fi.FullName);
                    if (exists)
                        existingFilesInDB.Add("("+categName+")"+fi.FullName);

                    if (td != null && fi.Extension.Length > 0)
                        tds = td.AddChild(fi.Name, exists, false);
                    else
                    {
                        tds = new CheckItem(fi.Name, exists, false);
                        treeView.Items.Add(tds);
                    }

                    tds.Tag = fi;
                    //tds.StateImageIndex = 1;
                    //  UpdateProgress();
                }
            }
        }

    //CLick del boton Guardar-----------------------------------------------------------------------------
        private void BtnSaveMovies_Click(object sender, RoutedEventArgs e)
        {
            AppMAnager.SetWaitCursor();

            _pBar.Value = 0;
            nextMediaId = 0;
            BtnSaveMovies.IsEnabled = false;
            BackgroundWorker bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.DoWork += SaveMoviesDoWork;
            bw.ProgressChanged += SaveMoviewProgressChanged;
            bw.RunWorkerCompleted += SaveMoviewWorkerEnded;
            bw.RunWorkerAsync();
        }

        private void GuardarCatalogo(TabControl tabControl, BackgroundWorker worker)
        {
            string result = DBManager.Context.Database.SqlQuery<string>("select max(id) as max from media_files").FirstOrDefault();
            if (result != null)
                nextMediaId = Int16.Parse(result);

            if (nextMediaId <= 0)
            {
                DBManager.Context.Database.ExecuteSqlCommand("ALTER TABLE media_files AUTO_INCREMENT = 1");
                nextMediaId = 0;
            }

            DbContextTransaction transaction = DBManager.Context.Database.BeginTransaction();
            try
            {
                MediaFile_Basic_Info MFBI;
                foreach (categoria categ in selectedList.Keys)
                {
                    Console.WriteLine("Leyendo categoría: " + categ.categoria1);

                    MFBI = selectedList[categ];
                    SaveSelectedNodes(MFBI.Childrens, categ, -1, worker);

                    Console.WriteLine("---------");
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                MessageBoxButton btn = MessageBoxButton.OK;
                transaction.Rollback();
                ModernDialog.ShowMessage("Error guardando datos en escaneo automático." + "\n" + ex.Message, "Error", btn);
            }
        }

        private void SaveMoviesDoWork(object sender, DoWorkEventArgs e)
        {
            totalSaveOperations = 0;
            i = 0;
            selectedList = new Dictionary<categoria, MediaFile_Basic_Info>();
            
            _tab.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                (Action)(() => {
                    MediaFile_Basic_Info mfbi;
                    foreach (TabItem page in _tab.Items)
                    {
                        TreeView tree = (TreeView)page.Content;
                        if (tree != null)
                        {
                            mfbi = new MediaFile_Basic_Info();
                            selectedList.Add((categoria)page.Tag, mfbi);
                            totalSaveOperations += GetSelectionCount(tree.Items, mfbi, (categoria)page.Tag);
                        }
                    }
                }));

            //Thread.Sleep(100);
            GuardarCatalogo(_tab, (BackgroundWorker)sender); 
        }

        private void SaveMoviewProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _pBar.Value = e.ProgressPercentage;
        }

        private void SaveMoviewWorkerEnded(object sender, RunWorkerCompletedEventArgs e)
        {
            AppMAnager.RestoreCursor();
            AppMAnager.GlobalContentChanged();
            BtnScan.IsEnabled = true;

          /*  if(existingFilesInDB.Count > 0)
            {
                new SimpleListDialog("Información", "Los siguientes archivos ya existen en el sistema y no fueron guardados", existingFilesInDB).ShowDialog();
            }*/

            Console.WriteLine("Save Movies Worker Ends");
        }

        //Determinar el total de operaciones a realizar segun lo seleccionado en el Tree
        private int GetSelectionCount(ItemCollection items, MediaFile_Basic_Info parent, categoria categ)
        {
            int res = 0;
            MediaFile_Basic_Info child; 
            foreach (CheckItem item in items)
            {
                if (item.IsChecked())
                {
                    child = new MediaFile_Basic_Info();
                    child.file_exists = item.FileExists();

                    if (item.Tag is FileInfo)  //Es un fichero
                    {
                        res++;
                        child._fileSystemInfo = (FileInfo)item.Tag;
                        parent.Childrens.Add(child);
                    }
                    else
                    if (item.Tag is DirectoryInfo)  //Es un directorio
                    {
                        child._fileSystemInfo = (DirectoryInfo)item.Tag;
                        parent.Childrens.Add(child);
                        res += GetSelectionCount(item.Items, child, categ);
                    }
                }
            }

            return res;
        }

        //Lee lo seleccionado en el arbol de categorias escaneado y lo guarda a la bd
        private void SaveSelectedNodes(List<MediaFile_Basic_Info> List, categoria categ, int parentId, BackgroundWorker worker)
        {
            FileInfo fi;
            DirectoryInfo di;
            string subtitle;
            string poster;
            string trailer;

            MediaFilesRepository mediaRepo = DBManager.MediaFilesRepo; // new MediaFilesRepository();
            int ptoCopia_id = AppMAnager.CurrentPuntoCopia().id;

            //media_files mf;
            int progress;
            foreach (MediaFile_Basic_Info item in List)
            {
                if (item._fileSystemInfo is FileInfo)  //Es un fichero
                {
                    fi = (FileInfo)item._fileSystemInfo;

                    Console.WriteLine(fi.DirectoryName + "  |  " + fi.Name);

                    /*mf = new media_files
                    {
                        categoria_id = categ.id,
                        punto_copia_id = AppMAnager.CurrentPuntoCopia().id, // copia_punto.id,
                        titulo = AppMAnager.NameWithoutExt(fi.Name),
                        is_folder = false
                    };*/

                    subtitle = AppMAnager.FindSubtitleFile(fi);
                    poster = AppMAnager.FindPosterFile(fi);
                    trailer = AppMAnager.FindTrailerFile(fi);
                    /*
                                        if (poster != null)
                                            mf.fichero_portada = poster;

                                        mf.parent_id = parentId;
                                        mf.file_url = fi.FullName;
                                        if (subtitle != null)
                                            mf.str_file = subtitle;

                                        mf = mediaRepo.Add(mf);*/

                    nextMediaId++;
                    DBManager.Context.Database.ExecuteSqlCommand("insert into media_files(id, categoria_id, punto_copia_id, parent_id, file_url, str_file, fichero_portada, titulo, fichero_trailer, is_folder) " +
                                                                 "values(" + nextMediaId + "," + categ.id + "," + ptoCopia_id + "," + parentId + ",'" + getScapedString(fi.FullName) + "','" + getScapedString(subtitle) + "','" +
                                                                 getScapedString(poster) + "','" + getScapedString(AppMAnager.NameWithoutExt(fi.Name)) + "','" + getScapedString(trailer) + "', false)");

                    i++;
                    progress = (i * 100) / totalSaveOperations;
                    worker.ReportProgress(progress);
                }
                else
                if (item._fileSystemInfo is DirectoryInfo)  //Es un directorio
                {
                    di = (DirectoryInfo)item._fileSystemInfo;

                    Console.WriteLine("Leyendo directorio: " + di.FullName);

                    if (!item.file_exists)  //No existe el directorio
                    {
                        /* mf = new media_files
                          {
                              categoria_id = categ.id,
                              punto_copia_id = AppMAnager.CurrentPuntoCopia().id,
                              titulo = di.Name,
                              is_folder = true,
                              parent_id = parentId,
                              file_url = di.FullName
                          };
                          mf = mediaRepo.Add(mf);*/

                        nextMediaId++;
                        DBManager.Context.Database.ExecuteSqlCommand("insert into media_files(id, categoria_id, punto_copia_id, parent_id, file_url, titulo, is_folder) " +
                                                                     "values(" + nextMediaId + "," + categ.id + "," + ptoCopia_id + "," + parentId + ",'" + getScapedString(di.FullName) + "','" + getScapedString(di.Name) + "', true)");

                        SaveSelectedNodes(item.Childrens, categ, nextMediaId, worker);
                    }
                  /*  else
                        mf = mediaRepo.FindByUrl(di.FullName);

                    SaveSelectedNodes(item.Childrens, categ, mf.id, worker); */
                }
            }
        }

        private string getScapedString(string value)
        {
            if (value != null)
            {
                value = value.Replace("'", "''");
                value = value.Replace("\\", "\\\\");

                return value;
            }

            else
                return "";
        }

        public void SetContentChanged()
        {
            contentChanged = true;
        }
    }
}
