using FirstFloor.ModernUI.Windows.Controls;
using System.Windows.Controls;

namespace MCP.gui
{
    /// <summary>
    /// Interaction logic for LoginDialog.xaml
    /// </summary>
    public partial class LoginDialog : ModernWindow
    {
        public LoginDialog()
        {
            InitializeComponent();
            AppMAnager._loginDialog = this;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var sbar = Template.FindName("_statusBarWrapper", this) as Border;
            if (sbar != null)
            {
                //Oculto el Status Bar
                sbar.Visibility = System.Windows.Visibility.Hidden;
            }
        }
    }
}
