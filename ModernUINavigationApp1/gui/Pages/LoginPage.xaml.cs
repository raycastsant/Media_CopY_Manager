using MCP;
using MCP.gui;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp1.gui
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();

            tbxUser.Text = "super";
            tbxPass.Password = "super";

            AppMAnager.setDefaultForeColor(LLogin.Foreground);

            LLogin.Visibility = Visibility.Hidden;
            cbxPuntoCopia.Visibility = Visibility.Hidden;

            this.Loaded += ContentLoaded;
        }

        private void ContentLoaded(object sender, RoutedEventArgs e)
        {
            tbxUser.Focus();
            Keyboard.Focus(tbxUser);
        }

        /*private void refreshPuntosCombo(usuario user)
        {
            List<copia_puntos> list = new List<copia_puntos>();
            if (user.is_admin)
            {
                list = DBManager.PuntoCopyRepo.List;
            }
            else
            {
                list = user.copia_puntos.OrderBy(e => e.nombre).ToList();
            }

            cbxPuntoCopia.Items.Clear();
            foreach (copia_puntos cp in list)
            {
                cbxPuntoCopia.Items.Add(cp);
            }
        }*/

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            // object o = cbxPuntoCopia.SelectedItem;
            //if (o != null && o is copia_puntos)
            // {
                tryLogin();
           // }
           //  else
           //    LError.Visibility = Visibility.Visible;
        }

        private void tryLogin()
        {
            AppMAnager.Login(tbxUser.Text, tbxPass.Password, LError);
        }

        private void tbxUser_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Enter)
                tryLogin();

            /* usuario user = DBManager.UsuariosRepo.LoginUser(tbxUser.Text, tbxPass.Password);
             if(user != null)
             {
                 refreshPuntosCombo(user);
             }
             else
                 cbxPuntoCopia.Items.Clear();*/
        }

        /**
         * Abre la ventana de configuración de la conexión
         */
        private void BtnConfigConnection_Click(object sender, RoutedEventArgs e)
        {
            new ConnectionConfigDialog("Configurar Conexión").ShowDialog();
        }
    }
}
