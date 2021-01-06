using FirstFloor.ModernUI.Windows.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Windows;

namespace MCP.gui
{
    /// <summary>
    /// Interaction logic for ListDialog.xaml
    /// </summary>
    public partial class ConnectionConfigDialog : ModernDialog
    {
        public ConnectionConfigDialog(string title)
        {
            InitializeComponent();

            this.Title = title;

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            string cs = config.ConnectionStrings.ConnectionStrings["media_managerEntities"].ConnectionString;
            if(cs.Length > 20)
            {
                string user = "";
                string server = "";
                string db = "";

                string []result = cs.Split(';');

                server = result[2];
                int start = server.IndexOf("server=")+ "server=".Length;
                int count = server.Length - start;
                server = server.Substring(start, count);

                user = result[3];
                start = user.IndexOf("user id=") + "user id=".Length;
                count = user.Length - start;
                user = user.Substring(start, count);

                db = result[6];
                start = db.IndexOf("database=") + "database=".Length;
                count = db.Length - 1 - start;
                db = db.Substring(start, count);

                tbxUser.Text = user;
                tbxServer.Text = server;
                tbxDB.Text = db;
            }
        }

        /**
         * Click del botón Guardar Conexión
         */
        private void BtnSaveConnection_Click(object sender, RoutedEventArgs e)
        {
            bool error = false;
            string user = "";
            string pass = "";
            string server = "";
            string db = "";

            AppMAnager.SetWaitCursor();

            //Validar Usuario
            if (tbxUser.Text.Length <= 0) { 
                AppMAnager.SetErrorTextBox(tbxUser);
                AppMAnager.SetLabel_Error(lUser, "Inserte un usuario");
                error = true;
            }
            else
            {
                AppMAnager.restoreDefaulLabel(lUser);
                AppMAnager.restoreDefaultTextBox(tbxUser);
                user = tbxUser.Text;
            }

            //Validar Servidor
            if (tbxServer.Text.Length <= 0)
            {
                AppMAnager.SetErrorTextBox(tbxServer);
                AppMAnager.SetLabel_Error(lServer, "Inserte el servidor");
                error = true;
            }
            else
            {
                AppMAnager.restoreDefaulLabel(lServer);
                AppMAnager.restoreDefaultTextBox(tbxServer);
                server = tbxServer.Text;
            }

            //Validar Base de Datos
            if (tbxDB.Text.Length <= 0)
            {
                AppMAnager.SetErrorTextBox(tbxDB);
                AppMAnager.SetLabel_Error(lDB, "Inserte la base de datos");
                error = true;
            }
            else
            {
                AppMAnager.restoreDefaulLabel(lDB);
                AppMAnager.restoreDefaultTextBox(tbxDB);
                db = tbxDB.Text;
            }

            if(!error)
            {
                pass = tbxPass.Password;

                //Test Connection
                //var sqlCon = new SqlConnection();
                var connectionString = "datasource=mysqlip;host="+server+";port=3306;database="+db+";username="+user+";password="+pass;
                var mySQLConn = new MySqlConnection(connectionString);
                //sqlCon.Open();

                try
                {
                    mySQLConn.Open();
                    var temp = mySQLConn.State.ToString();
                    if ("Open" == temp)
                    {
                        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                        config.ConnectionStrings.ConnectionStrings["media_managerEntities"].ConnectionString = "metadata=res://*/db.Model.csdl|res://*/db.Model.ssdl|res://*/db.Model.msl;provider=MySql.Data.MySqlClient;provider connection string='server=" + server + ";user id=" + user + ";password=" + pass + ";persistsecurityinfo=True;database=" + db + "'";
//                        config.ConnectionStrings.ConnectionStrings["media_managerEntities"].ConnectionString = "metadata=res://*/db.Model.csdl|res://*/db.Model.ssdl|res://*/db.Model.msl;provider=MySql.Data.MySqlClient;provider connection string=&quot;server=" + server + ";user id=" + user + ";Password=" + pass + ";persistsecurityinfo=True;database=" + db + "&quot;";
                        config.Save(ConfigurationSaveMode.Modified, false);
                        ConfigurationManager.RefreshSection("connectionStrings");

                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(@"Please check connection string");
                    }
                }
                catch(Exception exc)
                {
                    MessageBox.Show(@"Error de conexión con la base de datos."+"\n"+exc.Message);
                }

                /*  var settings = ConfigurationManager.ConnectionStrings[0];
                  var fi = typeof(ConfigurationElement).GetField("_bReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
                  fi.SetValue(settings, false);
                  settings.ConnectionString = "Data Source=Something";*/
            }

            AppMAnager.RestoreCursor();
        }
    }
}
