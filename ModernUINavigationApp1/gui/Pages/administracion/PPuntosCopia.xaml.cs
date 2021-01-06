using MCP.db;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MCP.gui.Pages.administracion
{
    public partial class PPuntosCopia : UserControl
    {
        private int State { get; set; }

        private copia_puntos entity;

        public PPuntosCopia()
        {
            InitializeComponent();

            refreshGrid();
            lNombre.Tag = "Nombre";
            hideForm();
            State = AppMAnager.STATE_NULL;
            entity = null;
        }

        private void refreshGrid()
        {
            _dataGrid.ItemsSource = DBManager.PuntoCopyRepo.List;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            State = AppMAnager.STATE_INSERT;
            entity = new copia_puntos();
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
            AppMAnager.restoreDefaultTextBox(tbNombre);
            AppMAnager.restoreDefaulLabel(lNombre);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool hasError = false;

            if (tbNombre.Text.Trim().Length > 0)
            {
                AppMAnager.restoreDefaultTextBox(tbNombre);
                AppMAnager.restoreDefaulLabel(lNombre);

                if (DBManager.PuntoCopyRepo.FindByName(tbNombre.Text, entity.id) == null)
                {
                    entity.nombre = tbNombre.Text;
                }
                else
                {
                    hasError = true;
                    AppMAnager.SetErrorTextBox(tbNombre);
                    AppMAnager.SetLabel_Error(lNombre, "El punto de copia ya existe");
                }
            }
            else
            {
                hasError = true;
                AppMAnager.SetErrorTextBox(tbNombre);
                AppMAnager.SetEmptyLabel_Error(lNombre);
            }

            if (!hasError)
            {
                if(State == AppMAnager.STATE_INSERT)
                {
                    DBManager.PuntoCopyRepo.Add(entity);
                    AfterSave();
                }
                else
                if (State == AppMAnager.STATE_UPDATE)
                {
                    DBManager.PuntoCopyRepo.Update(entity);
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
                entity = DBManager.PuntoCopyRepo.FindById(id);
                if (entity != null)
                {
                    entity.inactivo = true;
                    entity.nombre = entity.nombre + "_deleted" + entity.id;
                    DBManager.PuntoCopyRepo.Update(entity);
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
                entity = DBManager.PuntoCopyRepo.FindById(id);
                if(entity != null)
                {
                    tbNombre.Text = entity.nombre;
                    State = AppMAnager.STATE_UPDATE;
                    showForm();
                }
            }
        }
    }
}
