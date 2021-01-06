using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows.Controls;
using MCP;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;

namespace ModernUINavigationApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        private static NotifyIcon nicon;


        public static MainWindow _MainWindow;

        public MainWindow(bool showAdminMenu)
        {
            InitializeComponent();

            nicon = new NotifyIcon();
            ShowInTaskbar = true;
            Stream iconStream = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/Resources/usb.ico")).Stream;
            nicon.Icon = new System.Drawing.Icon(iconStream);
            //nicon.Icon = new System.Drawing.Icon(@"../../Resources/usb.ico");
            // nicon.Icon = new System.Drawing.Icon(new Uri("pack://application:,,,/Resources/usb.ico"));
            nicon.Text = "Media Copy Manager";
            nicon.Visible = true;
            nicon.DoubleClick += delegate (object sender, EventArgs args)
            {
                this.Show();
                this.WindowState = WindowState.Maximized;
            };

            if (!showAdminMenu) //Usuario regular
            {
                LinkGroup catalogmenu = this.MenuLinkGroups.ElementAt(0);
                var editmenu = catalogmenu.Links.ElementAt(1);
              //  var scanmenu = catalogmenu.Links.ElementAt(2);
                catalogmenu.Links.Remove(editmenu);
             //   catalogmenu.Links.Remove(scanmenu);

                var configmenu = this.MenuLinkGroups.ElementAt(1);
                var adminmenu = this.MenuLinkGroups.ElementAt(2);
                this.MenuLinkGroups.Remove(configmenu);
                this.MenuLinkGroups.Remove(adminmenu);
            }

            //this.OnNavigateLink() += new RoutedEventHandler(Navigated);
            /* LinkGroup lg = new LinkGroup {
                 DisplayName = "Test"
             };

             Link l1 = new Link();
             l1.DisplayName = "Test1";
             l1.Source = new Uri("pack://application:,,,/gui/Pages/nomencladores/PCategorias.xaml");
             lg.Links.Add(l1);

             this.MenuLinkGroups.Add(lg);*/

            _MainWindow = this;
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                this.Hide();

            base.OnStateChanged(e);
        }

        /*  private void WActivated(object sender, EventArgs args)
          {

          }*/

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var frame = Template.FindName("ContentFrame", this) as ModernFrame;
            if (frame != null)
            {
                frame.CommandBindings.Add(new CommandBinding(FirstFloor.ModernUI.Windows.Navigation.LinkCommands.NavigateLink, OnNavigateLinkExecuted));
            }

            var tbStatus = Template.FindName("_statusBar_info", this) as TextBlock;
            if (tbStatus != null)
            {
                AppMAnager.tbStatus = tbStatus;
            }

            var statusLoader = Template.FindName("_statusBar_loader", this) as ModernProgressRing;
            if (statusLoader != null)
            {
                AppMAnager.statusLoader = statusLoader;
            }

            AppMAnager.StartContentChangesCheckig();
        }

        private void OnNavigateLinkExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter.ToString().Equals("Logout"))
            {
                AppMAnager.Logout();
            }
            else
            {
                OnNavigateLink(sender, e);
            }
        }

        private void OnNavigateLink(object sender, ExecutedRoutedEventArgs e)
        {
            if (LinkNavigator != null)
            {
                Uri uri;
                string parameter;
                string targetName;
                if (FirstFloor.ModernUI.Windows.Navigation.NavigationHelper.TryParseUriWithParameters(e.Parameter, out uri, out parameter, out targetName))
                    LinkNavigator.Navigate(uri, e.Source as FrameworkElement, parameter);
            }
        }

        public NotifyIcon NotifyIcon()
        {
            return nicon;
        }
    }
}
