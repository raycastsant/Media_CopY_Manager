using MCP.db;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MCP.gui.Pages.nomencladores
{
    /// <summary>
    /// Interaction logic for Categorias.xaml
    /// </summary>
    public partial class PGeneros : UserControl
    {
        private GenerosRepository repo;
        private int State { get; set; }

        private genero entity;

        public PGeneros()
        {
            InitializeComponent();

            repo = new GenerosRepository();

            refreshGrid();
            lNombre.Tag = "Nombre";
            hideForm();
            State = AppMAnager.STATE_NULL;
            entity = null;
        }

        private void refreshGrid()
        {
            _dataGrid.ItemsSource = repo.List.ToList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            State = AppMAnager.STATE_INSERT;
            entity = new genero();
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

            //Validar nombre
            if (tbNombre.Text.Trim().Length > 0)
            {
                AppMAnager.restoreDefaultTextBox(tbNombre);
                AppMAnager.restoreDefaulLabel(lNombre);

                if (repo.FindByName(tbNombre.Text, entity.id) == null)
                {
                    entity.nombre = tbNombre.Text;
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

            if (!hasError)
            {
                if(State == AppMAnager.STATE_INSERT)
                {
                    repo.Add(entity);
                    BeforeSave();
                }
                else
                if (State == AppMAnager.STATE_UPDATE)
                {
                    repo.Update(entity);
                    BeforeSave();
                }

                AppMAnager.GlobalContentChanged();
            }
        }

        private void BeforeSave()
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
                repo.Delete(id);
                refreshGrid();

                clearForm();
                hideForm();
            }
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            if (id > 0)
            {
                entity = repo.FindById(id);
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
