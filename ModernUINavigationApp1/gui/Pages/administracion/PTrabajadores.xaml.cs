using MCP.db;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MCP.gui.Pages.administracion
{
    public partial class PTrabajadores : UserControl
    {
        private int State { get; set; }

        private trabajadore entity;

        public PTrabajadores()
        {
            InitializeComponent();

            lNombre.Tag = "Nombre";
            lUsuario.Tag = "Usuario";

            this.Loaded += ContentLoaded;
        }

        private void ContentLoaded(object sender, RoutedEventArgs e)
        {
            hideForm();
            State = AppMAnager.STATE_NULL;
            entity = null;

            refreshGrid();
            RefreshComboUsuarios();
        }

        private void refreshGrid()
        {
            _dataGrid.ItemsSource = DBManager.TrabajadoresRepo.List;
        }

        private void RefreshComboUsuarios()
        {
            List<usuario> list = DBManager.UsuariosRepo.ListUnused;
            cbxUser.Items.Clear();
            foreach (usuario u in list)
            {
                cbxUser.Items.Add(u);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            State = AppMAnager.STATE_INSERT;
            entity = new trabajadore();
            clearForm();
            showForm();
        }

        private void showForm()
        {
            formPanel.Visibility = Visibility.Visible;
            _grid.ColumnDefinitions.Last().Width = new GridLength(300);
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
            tbTelefono.Text = "";
            tbDireccion.Text = "";
            RefreshComboUsuarios();

            AppMAnager.restoreDefaultTextBox(tbNombre);
            AppMAnager.restoreDefaulLabel(lNombre);
            AppMAnager.restoreDefaulLabel(lUsuario);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool hasError = false;

            //Validar nombre
            if (tbNombre.Text.Trim().Length > 0)
            {
                AppMAnager.restoreDefaultTextBox(tbNombre);
                AppMAnager.restoreDefaulLabel(lNombre);

                if (DBManager.TrabajadoresRepo.FindByName(tbNombre.Text, entity.id) == null)
                {
                    entity.nombre_apell = tbNombre.Text;
                    entity.telefono = tbTelefono.Text;
                    entity.direccion = tbDireccion.Text;
                }
                else
                {
                    hasError = true;
                    AppMAnager.SetErrorTextBox(tbNombre);
                    AppMAnager.SetLabel_Error(lNombre, "El nombre ya existe");
                }
            }
            else
            {
                hasError = true;
                AppMAnager.SetErrorTextBox(tbNombre);
                AppMAnager.SetEmptyLabel_Error(lNombre);
            }

            //Validar punto de copia
            AppMAnager.restoreDefaulCombobox(cbxUser);
            AppMAnager.restoreDefaulLabel(lUsuario);
            if (cbxUser.SelectedItem != null)
            {
                entity.usuario = (usuario)cbxUser.SelectedItem;
            }
            else
            {
                hasError = true;
                AppMAnager.SetErrorCombobox(cbxUser);
                AppMAnager.SetEmptyLabel_Error(lUsuario);
            }

            if (!hasError)
            {
                if(State == AppMAnager.STATE_INSERT)
                {
                    DBManager.TrabajadoresRepo.Add(entity);
                    AfterSave();
                }
                else
                if (State == AppMAnager.STATE_UPDATE)
                {
                    DBManager.TrabajadoresRepo.Update(entity);
                    AfterSave();
                }
            }
        }

        private void AfterSave()
        {
            refreshGrid();
            clearForm();
            hideForm();
            entity = null;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            if (id > 0)
            {
                entity = DBManager.TrabajadoresRepo.FindById(id);
                if (entity != null)
                {
                    DBManager.TrabajadoresRepo.Delete(id);
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
                entity = DBManager.TrabajadoresRepo.FindById(id);
                if(entity != null)
                {
                    tbNombre.Text = entity.nombre_apell;
                    tbTelefono.Text = entity.telefono;
                    tbDireccion.Text = entity.direccion;

                    RefreshComboUsuarios();
                    if (!cbxUser.Items.Contains(entity.usuario))
                        cbxUser.Items.Add(entity.usuario);

                    cbxUser.SelectedItem = entity.usuario;

                    State = AppMAnager.STATE_UPDATE;
                    showForm();
                }
            }
        }
    }
}
