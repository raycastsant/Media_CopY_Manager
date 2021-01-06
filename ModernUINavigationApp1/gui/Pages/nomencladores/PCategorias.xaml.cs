using MCP.db;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MCP.gui.Pages.nomencladores
{
    /// <summary>
    /// Interaction logic for Categorias.xaml
    /// </summary>
    public partial class PCategorias : UserControl
    {
        //private CategoriasRepository repo;
        private int State { get; set; }

        private categoria entity;

        public PCategorias()
        {
            InitializeComponent();

         //   repo = new CategoriasRepository();

            refreshGrid();
            lCarpeta.Tag = "Carpeta";
            lNombre.Tag = "Nombre";
            hideForm();
            State = AppMAnager.STATE_NULL;
            entity = null;

            RefreshComboTiposCateg();
        }

        private void refreshGrid()
        {
            _dataGrid.ItemsSource = DBManager.CategoriasRepo.List.ToList();
        }

        private void RefreshComboTiposCateg()
        {
            List<tipo_categorias> list = DBManager.TipoCategoriasRepo.List;
            cbxTipoCateg.Items.Clear();
            foreach(tipo_categorias tc in list)
            {
                cbxTipoCateg.Items.Add(tc);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            State = AppMAnager.STATE_INSERT;
            entity = new categoria();
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
            tbCarpeta.Text = "";
            AppMAnager.restoreDefaultTextBox(tbCarpeta);
            AppMAnager.restoreDefaultTextBox(tbNombre);
            AppMAnager.restoreDefaulCombobox(cbxTipoCateg);
            AppMAnager.restoreDefaulLabel(lNombre);
            AppMAnager.restoreDefaulLabel(lCarpeta);
            AppMAnager.restoreDefaulLabel(lTipocateg);
            RefreshComboTiposCateg();
        }

        /**
         * Boton Guardar categoria
         */
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool hasError = false;

            //Validar Nombre
            if (tbNombre.Text.Trim().Length > 0)
            {
                AppMAnager.restoreDefaultTextBox(tbNombre);
                AppMAnager.restoreDefaulLabel(lNombre);

                if (DBManager.CategoriasRepo.FindByName(tbNombre.Text, entity.id) == null)
                {
                    entity.categoria1 = tbNombre.Text;
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

            //Validar carpeta
            if (tbCarpeta.Text.Trim().Length > 0)
                entity.carpeta = tbCarpeta.Text;
            else
            {
                hasError = true;
                AppMAnager.SetErrorTextBox(tbCarpeta);
                AppMAnager.SetEmptyLabel_Error(lCarpeta);
            }

            //Validar tipo de categoria
            if (cbxTipoCateg.SelectedItem != null)
                entity.tipo_categorias = (tipo_categorias)cbxTipoCateg.SelectedItem;
            else
            {
                hasError = true;
                AppMAnager.SetErrorCombobox(cbxTipoCateg);
                AppMAnager.SetEmptyLabel_Error(lTipocateg);
            }

            if (!hasError)
            {
                if(State == AppMAnager.STATE_INSERT)
                {
                    DBManager.CategoriasRepo.Add(entity);
                    BeforeSave();
                }
                else
                if (State == AppMAnager.STATE_UPDATE)
                {
                    DBManager.CategoriasRepo.Update(entity);
                    BeforeSave();
                }

                AppMAnager.GlobalContentChanged();
                DBManager.Reset_Context();
            }
        }

        private void BeforeSave()
        {
            refreshGrid();
            clearForm();
            hideForm();
            entity = null;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string selected = AppMAnager.showFolderBrowser("");
            if(selected != null && selected.Trim().Length > 0)
            {
                tbCarpeta.Text = selected;
            }
        }

        /**
         * Eliminar categoria
         */
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            if (id > 0)
            {
                DBManager.CategoriasRepo.Delete(id);
                refreshGrid();

                clearForm();
                hideForm();

                AppMAnager.GlobalContentChanged();
            }
        }

        /**
         * Click del boton actualizar categoria
         */
        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            if (id > 0)
            {
                entity = DBManager.CategoriasRepo.FindById(id);
                if(entity != null)
                {
                    tbNombre.Text = entity.categoria1;
                    tbCarpeta.Text = entity.carpeta;
                    cbxTipoCateg.SelectedItem = entity.tipo_categorias;
                    State = AppMAnager.STATE_UPDATE;
                    showForm();
                }
            }
        }
    }
}
