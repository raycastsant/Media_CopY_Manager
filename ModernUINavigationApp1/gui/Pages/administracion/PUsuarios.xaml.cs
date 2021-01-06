using MCP.db;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MCP.gui.Pages.administracion
{
    /// <summary>
    /// Interaction logic for Categorias.xaml
    /// </summary>
    public partial class PUsuarios : UserControl
    {
        private int State { get; set; }

        private usuario user;

        public PUsuarios()
        {
            InitializeComponent();

            refreshGrid();
            lNombre.Tag = "Nombre";
            lPass.Tag = "Contraseña";
            lPassRepeat.Tag = "Contraseña";
            lPuntocopia.Tag = "Punto de copia";

//            AppMAnager.setDefaultForeColor(lNombre.Foreground);

            hideForm();
            State = AppMAnager.STATE_NULL;
            user = null;

            RefreshComboPuntoCopias();
        }

        private void refreshGrid()
        {
            _dataGrid.ItemsSource = DBManager.UsuariosRepo.List.ToList();
        }

        private void RefreshComboPuntoCopias()
        {
            List<copia_puntos> list = DBManager.PuntoCopyRepo.List;
            cbxPuntocopia.Items.Clear();
            foreach(copia_puntos pc in list)
            {
                cbxPuntocopia.Items.Add(pc);
            }
        }
        
        //Insertar nuevo
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            State = AppMAnager.STATE_INSERT;
            user = new usuario();
            clearForm();
            showForm();
        }

        private void showForm()
        {
            formPanel.Visibility = Visibility.Visible;
            _grid.ColumnDefinitions.Last().Width = new GridLength(300);

            tbPass.Tag = "0";
            tbPassRepeat.Tag = "0";
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
            tbPass.Password = "";
            tbPassRepeat.Password = "";

            AppMAnager.restoreDefaultTextBox(tbNombre);
            AppMAnager.restoreDefaultPasswordBox(tbPass);
            AppMAnager.restoreDefaultPasswordBox(tbPassRepeat);
            chbxIsdmin.IsChecked = false;
            AppMAnager.restoreDefaulCombobox(cbxPuntocopia);
            AppMAnager.restoreDefaulLabel(lNombre);
            AppMAnager.restoreDefaulLabel(lPass);
            AppMAnager.restoreDefaulLabel(lPassRepeat);
            AppMAnager.restoreDefaulLabel(lPuntocopia);

            cbxPuntocopia.IsEnabled = true;
            lPuntocopia.IsEnabled = true;
            chbxIsdmin.IsEnabled = true;

            RefreshComboPuntoCopias();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool hasError = false;
          
            //Validar Nombre
            if (tbNombre.Text.Trim().Length > 0)
            {
                if (DBManager.UsuariosRepo.FindByName(tbNombre.Text, user.id) == null)
                {
                    user.nombre = tbNombre.Text;
                    AppMAnager.restoreDefaultTextBox(tbNombre);
                    AppMAnager.restoreDefaulLabel(lNombre);
                }
                else
                {
                    hasError = true;
                    AppMAnager.SetErrorTextBox(tbNombre);
                    AppMAnager.SetLabel_Error(lNombre, "El usuario ya existe");
                }
            }
            else
            {
                hasError = true;
                AppMAnager.SetErrorTextBox(tbNombre);
                AppMAnager.SetEmptyLabel_Error(lNombre);
            }

            //Validar contraseña
            if (tbPass.Password.Trim().Length > 0)
            {
                AppMAnager.restoreDefaultPasswordBox(tbPass);
                AppMAnager.restoreDefaulLabel(lPass);

                if(tbPass.Tag.ToString().Equals("1"))
                {
                    user.pass = AppMAnager.getMD5(tbPass.Password);
                    if (tbPass.Password != tbPassRepeat.Password)
                    {
                        hasError = true;
                        AppMAnager.SetErrorPasswordBox(tbPassRepeat);
                        AppMAnager.SetLabel_Error(lPassRepeat, "Las contraseñas no coinciden");
                    }
                    else
                    {
                        AppMAnager.restoreDefaultPasswordBox(tbPassRepeat);
                        AppMAnager.restoreDefaulLabel(lPassRepeat);
                    }
                }
            }
            else
            {
                hasError = true;
                AppMAnager.SetErrorPasswordBox(tbPass);
                AppMAnager.SetEmptyLabel_Error(lPass);
            }

            user.is_admin = (bool)chbxIsdmin.IsChecked;

            //Validar punto de copia
            AppMAnager.restoreDefaulCombobox(cbxPuntocopia);
            AppMAnager.restoreDefaulLabel(lPuntocopia);
            if (!user.is_admin)
            {
                if (cbxPuntocopia.SelectedItem != null)
                {
                    user.copia_puntos.Clear();
                    user.copia_puntos.Add ( (copia_puntos)cbxPuntocopia.SelectedItem );
                }
                else
                {
                    hasError = true;
                    AppMAnager.SetErrorCombobox(cbxPuntocopia);
                    AppMAnager.SetEmptyLabel_Error(lPuntocopia);
                }
            }

            if (!hasError)
            {
                if(State == AppMAnager.STATE_INSERT)
                {
                    DBManager.UsuariosRepo.Add(user);
                    AfterSave();
                }
                else
                if (State == AppMAnager.STATE_UPDATE)
                {
                    DBManager.UsuariosRepo.Update(user);
                    AfterSave();
                }
            }
        }

        private void AfterSave()
        {
            refreshGrid();
            clearForm();
            hideForm();
            user = null;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            if (id > 0)
            {
                user = DBManager.UsuariosRepo.FindById(id);
                if (user != null)
                {
                    if (!user.is_owner)
                    {
                        user.eliminado = true;
                        user.nombre = user.nombre + "_deleted"+user.id;
                        DBManager.UsuariosRepo.Update(user);
                        refreshGrid();

                        clearForm();
                        hideForm();
                    }
                    else
                    {
                        MessageBox.Show("El usuario principal del sistema no se puede eliminar");
                    }
                }
            }
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            if (id > 0)
            {
                user = DBManager.UsuariosRepo.FindById(id);
                if(user != null)
                {
                    tbNombre.Text = user.nombre;
                    tbPass.Password = user.pass;
                    tbPassRepeat.Password = user.pass;
                    chbxIsdmin.IsChecked = user.is_admin;

                    if (user.is_owner)
                    {
                        chbxIsdmin.IsEnabled = false;
                    }

                    if (user.is_admin)
                    {
                        cbxPuntocopia.IsEnabled = false;
                        lPuntocopia.IsEnabled = false;
                    }
                    else
                    {
                        cbxPuntocopia.SelectedItem = user.copia_puntos.FirstOrDefault();
                    }
                    
                    State = AppMAnager.STATE_UPDATE;
                    showForm();
                }
            }
        }

        private void tbPass_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (tbPass.Tag.ToString().Equals("0"))
            {
                tbPass.Tag = "1";
                tbPassRepeat.Tag = "1";
                tbPassRepeat.Password = "";
            }
        }

        private void tbPassRepeat_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (tbPassRepeat.Tag.ToString().Equals("0"))
            {
                tbPassRepeat.Tag = "1";
                tbPass.Tag = "1";
                tbPass.Password = "";
            }
        }

        private void tbPass_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbPass.Tag.ToString().Equals("0"))
            {
                tbPass.Password = "";
            }
        }

        private void tbPass_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbPass.Tag.ToString().Equals("0"))
            {
                tbPass.Password = user.pass;
            }
        }

        private void tbPassRepeat_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbPassRepeat.Tag.ToString().Equals("0"))
            {
                tbPassRepeat.Password = "";
            }
        }

        private void tbPassRepeat_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbPassRepeat.Tag.ToString().Equals("0"))
            {
                tbPassRepeat.Password = user.pass;
            }
        }

        private void chbxIsdmin_Checked(object sender, RoutedEventArgs e)
        {
            cbxPuntocopia.IsEnabled = !(bool)chbxIsdmin.IsChecked;
            lPuntocopia.IsEnabled = !(bool)chbxIsdmin.IsChecked;
        }
    }
}
