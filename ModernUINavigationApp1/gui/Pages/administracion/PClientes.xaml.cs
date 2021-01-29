using MCP.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Button = System.Windows.Controls.Button;
using MessageBox = System.Windows.Forms.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

namespace MCP.gui.Pages.administracion
{
    /// <summary>
    /// Interaction logic for PClientes.xaml
    /// </summary>
    public partial class PClientes : UserControl
    {
        private int State { get; set; }
        public object BtnSelect { get; }

        private cliente cliente;
        private usb usb;
        private bool usbToClient;

        public PClientes()
        {
            InitializeComponent();

            refreshGrid();
            lNombre.Tag = "Nombre";
            lApellido.Tag = "Apellido";
            lTelefono.Tag = "Telefono";
            usbToClient = false;

            //            AppMAnager.setDefaultForeColor(lNombre.Foreground);

            hideForm();
            State = AppMAnager.STATE_NULL;
            cliente = null;
            

           
        }

        public PClientes(usb usb)
        {
            InitializeComponent();

            refreshGrid();
            lNombre.Tag = "Nombre";
            lApellido.Tag = "Apellido";
            lTelefono.Tag = "Telefono";
            usbToClient = true;

            //            AppMAnager.setDefaultForeColor(lNombre.Foreground);

            hideForm();
            this.usb = usb;
            State = AppMAnager.STATE_NULL;
            cliente = new cliente();



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
               
                cliente.telefono = tbTelefono.Text;
                AppMAnager.restoreDefaultTextBox(tbTelefono);
                AppMAnager.restoreDefaulLabel(lTelefono);
                
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
                    cliente c = DBManager.ClienteRepo.Add(cliente);
                    
                    if (usbToClient)
                    {
                        usb.id_cliente = c.id_cliente;
                        usb.cliente = c;
                        DBManager.UsbRepo.Add(usb);
                    }
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
            usbToClient = false;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            if (id > 0)
            {
                cliente = DBManager.ClienteRepo.FindById(id);
                if (cliente != null)
                {
                        DBManager.ClienteRepo.Delete(id);
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

      

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            if (usbToClient)
            {
                cliente = (cliente)_dataGrid.CurrentCell.Item;
                Console.WriteLine(cliente.nombre_cliente);
                DialogResult result = MessageBox.Show("¿El dispositvo pertenece a " + cliente.nombre_cliente + " " + cliente.apellidos_cliente + "?", "Elegir cliente", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    this.usb.id_cliente = cliente.id_cliente;
                    this.usb.cliente = cliente;
                    DBManager.UsbRepo.Add(usb);
                }
                else
                {
                    showForm();
                    State = AppMAnager.STATE_INSERT;
                }
            }
        }

        private void SetVisible(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (usbToClient)
            {
               
                btn.Visibility = Visibility.Visible;
            }
            else
            {
                btn.Visibility = Visibility.Hidden;
            }
        }

        private void SetVisibleDelete(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (usbToClient)
            {

                btn.Visibility = Visibility.Hidden;
            }
            else
            {
                btn.Visibility = Visibility.Visible;
            }
        }

        private void Set_Visible_Update(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (usbToClient)
            {

                btn.Visibility = Visibility.Hidden;
            }
            else
            {
                btn.Visibility = Visibility.Visible;
            }
        }
    }
}
