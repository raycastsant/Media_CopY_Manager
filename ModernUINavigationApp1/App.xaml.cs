using MCP;
using System.Windows;

namespace ModernUINavigationApp1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            AppMAnager.DisposeNotIcon();
        }
    }
}
