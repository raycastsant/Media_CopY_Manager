using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using MCP.db;
using ModernUINavigationApp1;
using MCP.gui;
using System;
using MCP.gui.components;
using MCP.gui.Pages;
using System.Windows.Input;
using System.Windows;
using Ookii.Dialogs.Wpf;
using System.Security.Cryptography;
using System.Globalization;
using Microsoft.Win32;
using MCP.gui.components.IconItem;
using System.Windows.Threading;
using System.Threading.Tasks;
using MCP.USB;
using System.IO;
using FirstFloor.ModernUI.Windows.Controls;

namespace MCP
{
    class AppMAnager
    {
        private static copia_puntos copia_punto;
        private static usuario _current_user;
        private static Brush default_fore_color;
        private static Dictionary<int, WrapPanel> dict_container = new Dictionary<int, WrapPanel>(); //< id de categoria, componente dentro del TAB>
        private static MainWindow mw;
        private static DispatcherTimer changes_timer;

        public static TextBlock tbStatus;
        public static ModernProgressRing statusLoader;

        public static readonly string TYPES_MEDIA = "*.mp4,*.avi,*.mpg,*.mpeg,*.mov,*.vob,*.mkv";
        public static readonly string TYPES_SUBTITLE = "*.srt,*.sub";
        public static readonly string TYPES_IMAGE = "*.jpg,*.jpeg,*.png,*.ico";

        public static readonly string COLOR_ERROR_BACKGROUND = "#FFFFDADA";
        public static readonly string COLOR_ERROR_FOREGROUND = "#FFCF2929";
        public static readonly string COLOR_SELECTION = "#FF0885CD";

        public static LoginDialog _loginDialog;
        public static Cursor HAND_CURSOR;
        public static PEditarCatalogo PEditarCatalogo_instance;
        public static int RUNNING_COPYS_COUNT = 0;
        public static string current_page;
        public static bool userLogged = false;

        public static readonly int STATE_NULL = 0;
        public static readonly int STATE_INSERT = 1;
        public static readonly int STATE_UPDATE = 2;
        public static readonly int STATE_COPYING_FILES = 3;

        /** Si un nodo es un directorio y se selecciona/deselecciona entonces
         se refresca la seleccion de todos sus hijos*/
        public static void ActualizarSeleccion(CheckItem node)
        {
            foreach (CheckItem n in node.Items)
            {
                n.SetChecked(node.IsChecked());
                ActualizarSeleccion(n);
            }
        }

        //----- Mostrar el catalogo en la pagina principal-----------------------------------------
        /**Muestra el catalogo en la pagina principal en forma de posters*/
        /*public static void ShowMainCatalogPosters(TabControl tabControl)
         {
             dict_container.Clear();
           //  dict_category_current_parent.Clear();
             tabControl.Items.Clear();
             tabControl.ClipToBounds = true;
             CategoriasRepository repo = DBManager.CategoriasRepo;
             List<categoria> ListaCategorias = repo.List;
             if (ListaCategorias.Count > 0)
             {
                 List<media_files> mediaList;
                 TabItem page;
                 WrapPanel container;
                 ModernButton buttonBack;
                 Grid grid;
                 ScrollViewer scroll;
                 foreach (categoria categ in ListaCategorias)
                 {
                     page = new TabItem();
                     page.Header = categ.categoria1;
                     page.Tag = categ.id;  //PAra las busquedas en la ventana proncipal del catalogo

                     mediaList = repo.ListFirstMedias(categ.media_files);

                     scroll = new ScrollViewer();
                     scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                     scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                     container = new WrapPanel();
                     container.Orientation = Orientation.Horizontal;
                     scroll.Content = container;
                     // container.ViewportWidth = scroll.ViewportWidth;

                     buttonBack = GetBackButton();
                     buttonBack.Tag = -1; //En el tag del boton guardo el ID del padre
                     grid = new Grid();
                     RowDefinition r0 = new RowDefinition();
                     r0.Height = new GridLength(35);
                     grid.RowDefinitions.Add(r0);
                     grid.RowDefinitions.Add(new RowDefinition());
                     grid.Tag = categ.id;  //Para el click del boton Atras

                     container.Tag = buttonBack; //Para poder acceder a el luego

                     dict_container.Add(categ.id, container);

                     grid.Children.Add(buttonBack);
                     Grid.SetRow(buttonBack, 0);
                     grid.Children.Add(scroll);
                     Grid.SetRow(scroll, 1);

                     page.Content = grid;
                     tabControl.Items.Add(page);

                     RefreshMediaContainer(mediaList, categ.id, container);
                 }
             }
             else
                 ((Grid) ((Grid)tabControl.Parent).Parent).Visibility = Visibility.Hidden;
         }

         // Realiza una busqueda para la categoria y el filtro seleccionados
         public static void FiltrarCatalogo(TabItem page, string filtro)
         {
             if (page != null)
             {
                 int categId = int.Parse(page.Tag.ToString());

                 MediaFilesRepository mrepo = DBManager.MediaFilesRepo;
                 CategoriasRepository categrepo = DBManager.CategoriasRepo;

                 List<media_files> mediaList;
                 if (!String.IsNullOrEmpty(filtro))
                     mediaList = mrepo.AplyFilter(categId, filtro);
                 else
                     mediaList = categrepo.ListFirstMedias(categId);

                 RefreshMediaContainer(mediaList, categId, dict_container[categId]);
             }
         }

         private static ModernButton GetBackButton()
         {
             GeometryConverter geomConvert = new GeometryConverter();
             Geometry iconData = (Geometry)geomConvert.ConvertFromString("F1 M 57,42L 57,34L 32.25,34L 42.25,24L 31.75,24L 17.75,38L 31.75,52L 42.25,52L 32.25,42L 57,42 Z ");
             ModernButton buttonBack = new ModernButton();
             buttonBack.EllipseDiameter = 30;
             buttonBack.IconHeight = 20;
             buttonBack.IconWidth = 20;
             buttonBack.ToolTip = "Atrás";
             buttonBack.Click += new System.Windows.RoutedEventHandler(BackFolderClick);
             buttonBack.IconData = iconData;
             buttonBack.IsEnabled = false;

             return buttonBack;
         }

         // Click de ir atras en el catalogo
         private static void BackFolderClick(object sender, RoutedEventArgs e)
         {
             MediaFilesRepository mediarepo = DBManager.MediaFilesRepo; // new MediaFilesRepository();
             int categId = int.Parse( ((Grid)(((ModernButton)sender).Parent)).Tag.ToString() );
             int parentId = int.Parse( ((ModernButton)sender).Tag.ToString() );
             media_files mediafile =  mediarepo.FindById(parentId);

             if(mediafile != null)
             {
                 ((ModernButton)sender).Tag = mediafile.parent_id;
                 List<media_files> mediaList;
                 CategoriasRepository repo = DBManager.CategoriasRepo;
                 categoria categ = repo.FindById(categId);

                 if (categ != null)
                 {
                     if (mediafile.parent_id <= 0)
                     {
                         mediaList = repo.ListFirstMedias(categ.media_files);
                         ((ModernButton)sender).IsEnabled = false;
                     }
                     else
                     {
                         mediaList = mediarepo.FindByCategoria(categId, (int)mediafile.parent_id);
                     }

                     RefreshMediaContainer(mediaList, categId, dict_container[categId]);
                     PHome._PHome.ClearPageSelection(false);  //false porque se crearan nuevos Posters
                 }
             }
         }

         //Carga el contenido de una carpeta MEDia File. Doble Click POster
         public static void LoadMediaFiles(int categId, int mediaParentId)
         {
             MediaFilesRepository repo = DBManager.MediaFilesRepo; // new MediaFilesRepository();
             List<media_files>  mediaList = repo.FindByCategoria(categId, mediaParentId);
             WrapPanel container = dict_container[categId];
             if (container != null)
             {
                 ((ModernButton)container.Tag).IsEnabled = true; //Boton de ir atras
                 ((ModernButton)container.Tag).Tag = mediaParentId;

                 RefreshMediaContainer(mediaList, categId, container);
                 PHome._PHome.ClearPageSelection(false);  //false porque se crearan nuevos Posters
             }
         }

         private static void RefreshMediaContainer(List<media_files> mediaList, int categId, WrapPanel container)
         {
             container.Children.Clear();
             Poster poster;

             foreach (media_files media in mediaList)
             {
                 poster = new Poster(media.fichero_portada, media.titulo, media.id, media.is_folder, categId);
                 poster.MouseEnter += PosterMouseEnter;
                 poster.MouseLeave += PosterMouseLeave;
                 container.Children.Add(poster);
             }
         }

         private static void PosterMouseEnter(object sender, MouseEventArgs e)
         {
             ((Poster)sender).ShowBorder();
         }

         private static void PosterMouseLeave(object sender, MouseEventArgs e)
         {
             ((Poster)sender).HidesBorder();
         }

         public static WrapPanel GetHomeWrapPanel(int categId)
         {
             if(dict_container != null && dict_container.Keys.Contains(categId))
                 return dict_container[categId];

             return null;
         }*/

        //------- Edicion de los registros de MovieFiles en la BD------------------------------------------
        /**Carga los movie files en el tab control Para la edicion*/
        public static async Task Load_Movies_Tree(TreeView treeView, IIconItemClickHandler _iitemClickHandler, Dispatcher _dispatcher, bool foldersOnly)
        {
           // changes_timer.Stop();

            treeView.Items.Clear();
            List<categoria> ListaCategorias = await DBManager.CategoriasRepo.ListAsync;

            List<media_files> mediaList;
            if (ListaCategorias.Count > 0)
            {
                IconItem nodo;
                IconItem nodoCateg;

                await Task.Run(async () =>
                {
                     foreach (categoria categ in ListaCategorias)
                     {
                        /*_dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background,
                           (Action)(() => {
                                
                           }));*/

                         mediaList = DBManager.MediaFilesRepo.FindByCategoria(categ.id, -1, foldersOnly);

                         await _dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                         (Action)(() =>
                         {
                             nodoCateg = new IconItem(categ.categoria1, true, _iitemClickHandler);  //Nodo categoria padre
                             nodoCateg.Tag = categ.id;
                             treeView.Items.Add(nodoCateg);
                             foreach (media_files mf in mediaList)
                             {
                                 nodo = new IconItem(mf.titulo, mf, _iitemClickHandler);
                                 nodoCateg.Items.Add(nodo);   //treeView.Items.Add(nodo);

                                if (mf.is_folder)
                                     FillMovieChildrens(nodo, categ.id, mf.id, _iitemClickHandler, foldersOnly);

                                //Verificar si el MF no existe fisicamente lo notifico
                                CheckMF_Exists(mf);
                             }
                         }));
                     }
                 });
            }
            else
            {
                ((Grid)treeView.Parent).Visibility = Visibility.Hidden;
            }

          //  changes_timer.Start();
        }
        
        /**Verifica si el MovieFile tiene hijos y los inserta en el TreeItem*/
        public static async void FillMovieChildrens(IconItem nodo, int categId, int mediaPArentId, IIconItemClickHandler _iitemClickHandler, bool foldersOnly)
        {
            MediaFilesRepository mediaRepo = DBManager.MediaFilesRepo; // new MediaFilesRepository();
            List<media_files> mediaList = await mediaRepo.FindByCategoriaAsync(categId, mediaPArentId, foldersOnly);
            if (mediaList.Count > 0)
            {
                IconItem child;
                foreach (media_files mf in mediaList)
                {
                    child = new IconItem(mf.titulo, mf, _iitemClickHandler);
                    nodo.Items.Add(child);
                    if (mf.is_folder)
                        FillMovieChildrens(child, categId, mf.id, _iitemClickHandler, foldersOnly);

                    //Verificar si el MF no existe fisicamente lo notifico
                    CheckMF_Exists(mf);
                }
            }
        }

        private static void CheckMF_Exists(media_files mf)
        {
            if (!mf.FileExists())
            {
                if (PEditarCatalogo_instance != null)
                {
                    if (PEditarCatalogo_instance.LabelInfoDeletedMF_isHidden())
                        PEditarCatalogo_instance.ShowDeletedMF_Notification();

                    //Se guarda para si el usuario lo desea eliminarlo más adelante
                    PEditarCatalogo_instance.RegisterNonPhisicalMF_ID(mf.id);
                }
            }
        }

        //------------- Metodos utiles ----------------------------------------------------
        public static async void Login(string username, string pass, TextBlock LError)
        {
            SetWaitCursor();

            DateTime dt = DateTime.ParseExact("30/05/2021", "dd/MM/yyyy", CultureInfo.InvariantCulture);
            if (DateTime.Now.CompareTo(dt) > 0)
            {
                LError.Text = "Application Expired";

                RegistryKey microsoft = Registry.CurrentUser.OpenSubKey("Software", true).OpenSubKey("Microsoft", true);
                if (!inArray(microsoft.GetSubKeyNames(), "mcm"))
                {
                    RegistryKey mcm = microsoft.CreateSubKey("mcm");
                    mcm.SetValue("mcm_auth", "0");
                }
                else
                {
                    RegistryKey mcm = microsoft.OpenSubKey("mcm", true);
                    mcm.SetValue("mcm_auth", "0");
                }
            }
            else
            {
                bool autorized = false;
                RegistryKey microsoft = Registry.CurrentUser.OpenSubKey("Software", true).OpenSubKey("Microsoft", true);
                if (!inArray(microsoft.GetSubKeyNames(), "mcm"))
                {
                    RegistryKey mcm = microsoft.CreateSubKey("mcm");
                    mcm.SetValue("mcm_auth", "1");
                    autorized = true;
                }
                else
                {
                    RegistryKey mcm = Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("Microsoft").OpenSubKey("mcm");
                    if(mcm != null)
                    {
                        autorized = mcm.GetValue("mcm_auth").Equals("1");
                    }
                }

                if (autorized)
                {
                    try
                    {
                        DBManager.Context = new media_managerEntities();

                        usuario user = await DBManager.UsuariosRepo.LoginUser(username, pass);
                        if (user != null)
                        {
                            _current_user = user;
                            if (user.is_admin)
                                copia_punto = DBManager.PuntoCopyRepo.List.FirstOrDefault();
                            else
                                copia_punto = user.copia_puntos.FirstOrDefault();

                            mw = new MainWindow(user.is_admin);

                          //  UsbMonitorWindow usbmw = new UsbMonitorWindow();
                            //usbmw.Initialize();
                           // UsbMonitorManager usb_manager = new UsbMonitorManager(usbmw, true);

                            _loginDialog.Hide();
                            mw.Show();
                            userLogged = true;
                            _loginDialog.Close(); 

                            //LoadScannerSettings();

                            SetAppStatus("Aplicación iniciada.", false);
                            USBManager.StartUsbDeviceWatcher();
                        }
                        else
                            LError.Text = "Usuario o contraseña incorrectos";
                    }
                    catch (Exception e)
                    {
                        LError.Text = "Fallo al iniciar la aplicación"+"\n"+e.StackTrace;
                        MessageBox.Show(e.Message);
                    }
                }
                else
                    LError.Text = "Application Expired";
            }

            RestoreCursor();
        }

        private static bool inArray(string[] array, string value)
        {
            foreach(string s in array)
            {
                if (s.Equals(value))
                    return true;
            }

            return false;
        }

        public static bool Logout()
        {
            bool noProblem = false;
            if (RUNNING_COPYS_COUNT > 0)
            {
                MessageBoxResult res = MessageBox.Show("Existen copias en curso. ¿Confirma que desea cancelarlas?", "Información", MessageBoxButton.YesNo);
                if(res == MessageBoxResult.Yes)
                {
                    PHome._PHome.CancelRunningCopys();
                    noProblem = true;
                }
            }
            else
                noProblem = true;

            if (noProblem)
            {
                _current_user = null;
                copia_punto = null;

                MainWindow mw = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                mw.Hide();
                _loginDialog = new LoginDialog();
                _loginDialog.Show();
                mw.Close();
                userLogged = false;
            }

            return noProblem;

           // else
             //   NavigationHelper.ToUri(NavigationCommands.PreviousPage);
        }

        public static usuario CurrentUser()
        {
            return _current_user;
        }

        public static copia_puntos CurrentPuntoCopia()
        {
            return copia_punto;
        }

        public static void SetWaitCursor()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            //System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
        }

        public static void RestoreCursor()
        {
            Mouse.OverrideCursor = null;
          //  System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
        }

        public static string showFolderBrowser(string selectedPath)
        {
            /* System.Windows.Forms.FolderBrowserDialog FBD = new System.Windows.Forms.FolderBrowserDialog();
             if (Directory.Exists(selectedPath))
                 FBD.SelectedPath = selectedPath;
             else
                 FBD.SelectedPath = lastPath;

             if (FBD.ShowDialog() == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(FBD.SelectedPath))
             {
                 lastPath = FBD.SelectedPath;
                 return FBD.SelectedPath;
             }

             return null;*/

            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            dialog.Description = "Please select a folder.";
            dialog.UseDescriptionForTitle = true; // This applies to the Vista style dialog only, not the old dialog.

            if (!VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
                MessageBox.Show("Error", "Because you are not using Windows Vista or later, the regular folder browser dialog will be used. Please use Windows Vista to see the new dialog.");

            dialog.ShowDialog();

            return dialog.SelectedPath;
        }

        private static string ShowOpenDialog(string defaultDir, string filter)
        {
            System.Windows.Forms.OpenFileDialog d = new System.Windows.Forms.OpenFileDialog();

            if (!String.IsNullOrEmpty(filter))
                d.Filter = filter; 

            d.Multiselect = false;

            if (!String.IsNullOrEmpty(defaultDir))
                d.InitialDirectory = defaultDir;

            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return d.FileName;
            }

            return null;
        }

        public static string ShowOpenMovieDialog(string defaultDir)
        {
            return ShowOpenDialog(defaultDir, "Archivos de video|"+TYPES_MEDIA.Replace(',' , ';'));
        }

        public static string ShowOpenSubtitleDialog(string defaultDir)
        {
            return ShowOpenDialog(defaultDir, "Archivos de subtítulos|" + TYPES_SUBTITLE.Replace(',', ';'));
        }

        public static string ShowOpenImageDialog(string defaultDir)
        {
            return ShowOpenDialog(defaultDir, "Archivos de imagen|" + TYPES_IMAGE.Replace(',', ';'));
        }

        public static void SetErrorTextBox(TextBox tb)
        {
            BrushConverter bc = new BrushConverter();
            tb.Background = (Brush)bc.ConvertFrom(COLOR_ERROR_BACKGROUND);
        }

        public static void SetErrorPasswordBox(PasswordBox tb)
        {
            BrushConverter bc = new BrushConverter();
            tb.Background = (Brush)bc.ConvertFrom(COLOR_ERROR_BACKGROUND);
        }

        public static void SetErrorCombobox(ComboBox cbx)
        {
            BrushConverter bc = new BrushConverter();
            cbx.Background = (Brush)bc.ConvertFrom(COLOR_ERROR_BACKGROUND);
        }

        public static void setDefaultForeColor(Brush value)
        {
            default_fore_color = value;
        }

        public static void SetEmptyLabel_Error(TextBlock l)
        {
          //  default_fore_color = l.Foreground;
            BrushConverter bc = new BrushConverter();
            l.Foreground = (Brush)bc.ConvertFrom(COLOR_ERROR_FOREGROUND);
            l.Text = "Inserte el valor de : " + l.Tag;
        }

        public static void SetLabel_Error(TextBlock l, string error)
        {
           // default_fore_color = l.Foreground;
            BrushConverter bc = new BrushConverter();
            l.Foreground = (Brush)bc.ConvertFrom(COLOR_ERROR_FOREGROUND);
            l.Text = error;
        }

        public static void restoreDefaultTextBox(TextBox tb)
        {
            BrushConverter bc = new BrushConverter();
            tb.Background = (Brush)bc.ConvertFrom("#FFFFFFFF");
        }

        public static void restoreDefaultPasswordBox(PasswordBox tb)
        {
            BrushConverter bc = new BrushConverter();
            tb.Background = (Brush)bc.ConvertFrom("#FFFFFFFF");
        }

        public static void restoreDefaulCombobox(ComboBox cbx)
        {
            BrushConverter bc = new BrushConverter();
            cbx.Background = (Brush)bc.ConvertFrom("#FFFFFFFF");
        }

        public static void restoreDefaulLabel(TextBlock l)
        {
            BrushConverter bc = new BrushConverter();
            l.Foreground = default_fore_color; // (Brush)bc.ConvertFrom("#FF333333");
            l.Text = l.Tag.ToString();
        }

        public static Brush DefaultLabelForeColor()
        {
            return default_fore_color;
        }

        public static string NameWithoutExt(string val)
        {
            if (val != null && val.Length > 4)
                return val.Substring(0, val.Length - 4);
            else
                return val;
        }

        public static string getMD5(string value)
        {
            MD5 md5 = MD5.Create();
            byte[] encoded = new System.Text.UTF8Encoding().GetBytes(value);
            byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encoded);

            return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
        }

        public static void GlobalContentChanged()
        {
            PHome._PHome.SetContentChanged();

            if (PEditarCatalogo_instance != null)
                PEditarCatalogo_instance.SetContentChanged();

            if (PScanner._PScanner != null)
                PScanner._PScanner.SetContentChanged();

            if ("PHome" == current_page)
                PHome._PHome.reloadContent();
           // else
          //  if ("PEditarCatalogo" == current_page && PEditarCatalogo_instance != null)
              //  PEditarCatalogo_instance.reloadContent();
        }

        /** Busca si existe algun fichero de subtitulo asociado al fichero 
         pasado por parametros*/
        public static string FindSubtitleFile(FileInfo fi)
        {
            if (fi.Exists)
            {
                List<string> list = Directory.GetFiles(fi.DirectoryName, fi.Name.Replace(fi.Extension, "") + ".*", SearchOption.TopDirectoryOnly).Where(
                             s => AppMAnager.TYPES_SUBTITLE.Contains(Path.GetExtension(s).ToLower())
                            ).ToList();

                if (list.Count > 0)
                {
                    return new FileInfo(list[0]).FullName;
                }
            }

            return null;
        }

        /** Busca si existe algun fichero de imagen asociado al fichero 
         pasado por parametros*/
        public static string FindPosterFile(FileInfo fi)
        {
            if (fi.Exists)
            {
                List<string> list = Directory.GetFiles(fi.DirectoryName, fi.Name.Replace(fi.Extension, "") + ".*", SearchOption.TopDirectoryOnly).Where(
                             s => AppMAnager.TYPES_IMAGE.Contains(Path.GetExtension(s).ToLower())
                            ).ToList();

                if (list.Count > 0)
                {
                    return new FileInfo(list[0]).FullName;
                }
            }

            return null;
        }

        /** Busca si existe algun fichero de imagen asociado al fichero 
        pasado por parametros*/
        public static string FindTrailerFile(FileInfo fi)
        {
            if (fi.Exists)
            {
                List<string> list = Directory.GetFiles(fi.DirectoryName, "*Trailer*", SearchOption.TopDirectoryOnly).Where(s => AppMAnager.TYPES_MEDIA.Contains(Path.GetExtension(s).ToLower())).ToList();

                if (list.Count > 0)
                {
                    return new FileInfo(list[0]).FullName;
                }
            }

            return null;
        }

        /**
         * Activa el chequeo de la tabla de Preferencias, por si hay que actualizar el contenido.
         */
        public static void StartContentChangesCheckig()
        {
            /*  System.Timers.Timer _timer = new System.Timers.Timer();
              _timer.Interval = (25000);
              _timer.Elapsed += OnTimedEvent;
              _timer.Enabled = true;*/

            changes_timer = new DispatcherTimer();
            changes_timer.Tick += new EventHandler(OnTimedEvent);
            changes_timer.Interval = new TimeSpan(0, 0, 20);
            changes_timer.Start();
        }

       // private static int test = 0;
        private static async void OnTimedEvent(object sender, EventArgs e) //(Object source, System.Timers.ElapsedEventArgs e)
        {
            DBManager.Context = new media_managerEntities();

             int valor_int = await DBManager.Context.Database.SqlQuery<int>("select valor_int from preferencias where nombre='" + PreferencesRepository.CONTENT_CHANGED_KEY + "'").FirstOrDefaultAsync();
            //            int valor_int = DBManager.Context.Database.SqlQuery<int>("select valor_int from preferencias where nombre='" + PreferencesRepository.CONTENT_CHANGED_KEY + "'").FirstOrDefault();

            if (valor_int == 1)
            {
                DBManager.PreferenciasRepo.SetContentUnchangedRegistry();
                GlobalContentChanged();
            }
        }

        /**Determina si un directorio contiene videos*/
        public static bool DirectoryHasMediaFiles(string dir)
        {
            return Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories).Where(
                                s => AppMAnager.TYPES_MEDIA.Contains(Path.GetExtension(s).ToLower())).Where(
                                s => Path.GetExtension(s).Length > 0)
                            .ToList().Count > 0;
        }

        public static void SetAppStatus(string status, bool showStatusLoader)
        {
            tbStatus.Text = status;  
            statusLoader.IsActive = showStatusLoader;
        }

        /**
         *Establecer el estado de la aplicacion cuando se esta debtro de un Thread.
         */
        public static void SetAppStatus_OnTask(string status)
        {
            tbStatus.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background,
                         (Action)(() => {
                             tbStatus.Text = status;
                         }));
        }

        public static void DisposeNotIcon()
        {
            if (mw != null)
                mw.NotifyIcon().Dispose();
        }

        /* public static double GetWindowWidth()
         {
             //double res = 0;
             Console.WriteLine(Application.Current.MainWindow.WindowState);
             if(Application.Current.MainWindow.WindowState == WindowState.Maximized)
             {
                 return System.Windows.SystemParameters.PrimaryScreenWidth;
             }
             else
             {
                 return ((System.Windows.Controls.Panel)Application.Current.MainWindow.Content).ActualWidth;
             }

           //  return 0;
         }*/
    }
}
