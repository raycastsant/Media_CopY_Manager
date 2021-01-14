using MCP.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.AxHost;

namespace MCP.gui.Pages.administracion
{
    /// <summary>
    /// Interaction logic for PClientes.xaml
    /// </summary>
    public partial class PClientes : UserControl
    {
        private int State { get; set; }

        private cliente cliente;

        public PClientes()
        {
            InitializeComponent();

            refreshGrid();
            lNombre.Tag = "Nombre";
            lApellido.Tag = "Apellido";
            lTelefono.Tag = "Telefono";

            //            AppMAnager.setDefaultForeColor(lNombre.Foreground);

            hideForm();
            State = AppMAnager.STATE_NULL;
            cliente = null;

           
        }

        private void refreshGrid()
        {
            List<cliente> clientes = DBManager.ClienteRepo.List.ToList();
            foreach(cliente cliente in clientes)
            {
                Console.WriteLine(cliente.nombre_cliente);
            }
            _dataGrid.ItemsSource =clientes;
        }


        //Insertar nuevo
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            State = AppMAnager.STATE_INSERT;
            cliente = new cliente();
            clearForm();
            showForm();
        }

        private void showForm()
        {
            formPanel.Visibility = Visibility.Visible;
            _grid.ColumnDefinitions.Last().Width = new GridLength(300);

            tbApellidos.Tag = "0";
            tbTelefono.Tag = "0";
        }

        private void hideForm()
        {
            formPanel.Visibility = Visibility.Hidden;
            _grid.ColumnDefinitions.Last().Width = new GridLength(0);
            State = AppMAnager.STATE_NULL;
        }

        private void clearForm()
        {
            tbNombre.Text = "";
            tbApellidos.Text = "";
            tbTelefono.Text = "";

            AppMAnager.restoreDefaultTextBox(tbNombre);
            AppMAnager.restoreDefaulLabel(lNombre);
            AppMAnager.restoreDefaulLabel(lApellido);
            AppMAnager.restoreDefaulLabel(lTelefono);

          
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool hasError = false;

            //Validar Nombre
            if (tbNombre.Text.Trim().Length > 0)
            {
                cliente.nombre_cliente = tbNombre.Text;
                AppMAnager.restoreDefaultTextBox(tbNombre);
                AppMAnager.restoreDefaulLabel(lNombre);


            }
            else
            {
                hasError = true;
                AppMAnager.SetErrorTextBox(tbNombre);
                AppMAnager.SetLabel_Error(lNombre, "Debe introducir el nombre del cliente");
            }
          

            //Validar contraseña
            if (tbApellidos.Text.Trim().Length > 0)
            {

                cliente.apellidos_cliente = tbApellidos.Text;
                AppMAnager.restoreDefaultTextBox(tbApellidos);
                AppMAnager.restoreDefaulLabel(lApellido);

            }
            else
            {
                hasError = true;
                AppMAnager.SetErrorTextBox(tbApellidos);
                AppMAnager.SetLabel_Error(lApellido, "Debe introducir el apellido del cliente");
            }

            if (tbTelefono.Text.Trim().Length > 0)
            {
                if (DBManager.ClienteRepo.FindByPhone(tbTelefono.Text) == null)
                {
                    cliente.telefono = tbTelefono.Text;
                    AppMAnager.restoreDefaultTextBox(tbTelefono);
                    AppMAnager.restoreDefaulLabel(lTelefono);
                }
                else
                {
                    hasError = true;
                    AppMAnager.SetErrorTextBox(tbTelefono);
                    AppMAnager.SetLabel_Error(lTelefono, "El cliente ya existe");
                }
            }
            else
            {
                hasError = true;
                AppMAnager.SetErrorTextBox(tbTelefono);
                AppMAnager.SetLabel_Error(lTelefono, "Debe introducir el teléfono del cliente");
            }
          

         

            if (!hasError)
            {
                if (State == AppMAnager.STATE_INSERT)
                {
                    DBManager.ClienteRepo.Add(cliente);
                    AfterSave();
                }
                else
                if (State == AppMAnager.STATE_UPDATE)
                {
                    DBManager.ClienteRepo.Update(cliente);
                    AfterSave();
                }
            }
        }

        private void AfterSave()
        {
            refreshGrid();
            clearForm();
            hideForm();
            cliente = null;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            if (id > 0)
            {
                cliente = DBManager.ClienteRepo.FindById(id);
                if (cliente != null)
                {
                        DBManager.ClienteRepo.Update(cliente);
                        refreshGrid();

                        clearForm();
                        hideForm();
                    
                }
            }
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            if (id > 0)
            {
                cliente = DBManager.ClienteRepo.FindById(id);
                if (cliente != null)
                {
                    tbNombre.Text = cliente.nombre_cliente;
                    tbApellidos.Text = cliente.apellidos_cliente;
                    tbTelefono.Text = cliente.telefono;
                  
                    State = AppMAnager.STATE_UPDATE;
                    showForm();
                }
            }
        }

        private void tbApellidos_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (tbApellidos.Tag.ToString().Equals("0"))
            {
                tbApellidos.Tag = "1";
                tbTelefono.Tag = "1";
            }
        }

        private void tbTelefono_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (tbTelefono.Tag.ToString().Equals("0"))
            {
                tbTelefono.Tag = "1";
                tbApellidos.Tag = "1";
            }
        }

        private void tbApellidos_GotFocus(object sender, RoutedEventArgs e)
        {
            /*if (tbPass.Tag.ToString().Equals("0"))
            {
                tbPass.Password = "";
            }*/
        }

        private void tbApellidos_LostFocus(object sender, RoutedEventArgs e)
        {
            /*if (tbPass.Tag.ToString().Equals("0"))
            {
                tbPass.Password = user.pass;
            }*/
        }

        private void tbTelefono_GotFocus(object sender, RoutedEventArgs e)
        {
           /* if (tbPassRepeat.Tag.ToString().Equals("0"))
            {
                tbPassRepeat.Password = "";
            }*/
        }

        private void tbTelefono_LostFocus(object sender, RoutedEventArgs e)
        {
            /*if (tbPassRepeat.Tag.ToString().Equals("0"))
            {
                tbPassRepeat.Password = user.pass;
            }*/
        }

    }
}
