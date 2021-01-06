using MCP.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MCP.gui.Pages
{
    /// <summary>
    /// Interaction logic for Categorias.xaml
    /// </summary>
    public partial class PRegistroCopias : UserControl
    {
        private bool prepared;
        //private bool contentChanged;

        public PRegistroCopias()
        {
            InitializeComponent();

            //combo puntos de copia
            if (!AppMAnager.CurrentUser().is_admin)
            {
                _spPuntoCopia.Visibility = Visibility.Hidden;
            }
            else
            {
                List<copia_puntos> puntosList = DBManager.PuntoCopyRepo.List;
                cbxPuntoCopia.Items.Add("- TODOS - ");
                foreach (copia_puntos cp in puntosList)
                {
                    cbxPuntoCopia.Items.Add(cp);
                }
            }

            //combo categorias
            List<string> categs = DBManager.RegistroCopiasRepo.ListDistinctCategValues();
            cbxCategoria.Items.Add("- TODAS -");
            foreach (string categ in categs)
            {
                cbxCategoria.Items.Add(categ);
            }

            DateTime now = DateTime.Now;
            DateTime ini = now.Date + new TimeSpan(0, 0, 0);
            _datePickerIni.SelectedDate = ini;
            DateTime end = now.Date + new TimeSpan(11, 59, 0);
            _datePickerEnd.SelectedDate = end;

            prepared = false;
            //contentChanged = true;

            this.IsVisibleChanged += ScheduleUserControl_IsVisibleChanged;
        }

        private void ScheduleUserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Console.WriteLine(e.NewValue);
            if(AppMAnager.CurrentUser() != null)
            {
                prepared = true;
                refreshGrid();
            }
        }

        private async void refreshGrid()
        {
            ShowLoader();

            //Filtros
            /*DateTime fdesde = new DateTime();
            if (_datePickerIni.SelectedDate != null)
                fdesde = (DateTime)_datePickerIni.SelectedDate;*/

            DateTime fdesde = (DateTime)_datePickerIni.SelectedDate;
            fdesde = fdesde.Date + new TimeSpan(0, 0, 0);
            DateTime fhasta = (DateTime)_datePickerEnd.SelectedDate;
            fhasta = fhasta.Date + new TimeSpan(23, 59, 0);

            int ptoCopiaId = -1;
            int user_id = -1;
            if (!AppMAnager.CurrentUser().is_admin)
            {
                ptoCopiaId = AppMAnager.CurrentUser().copia_puntos.FirstOrDefault().id;
                user_id = AppMAnager.CurrentUser().id;
            }
            else
            if (cbxPuntoCopia.SelectedIndex > 0)
            {
                ptoCopiaId = ((copia_puntos)cbxPuntoCopia.SelectedItem).id;
            }

            string categoria = "";
            if (cbxCategoria.SelectedIndex > 0)
            {
                categoria = cbxCategoria.SelectedItem.ToString();
            }

            List<registro_copias> list = await DBManager.RegistroCopiasRepo.FindAsync(fdesde, fhasta, ptoCopiaId, categoria, user_id);

            _dataGrid.ItemsSource = list;
            _ltotalFiles.Text = list.Count.ToString();

            await Task.Run(() =>
             {
                 double totalSize = 0;
                 foreach (registro_copias rc in list)
                 {
                     Thread.Sleep(2);
                     totalSize += rc.Size;
                 }

                 _lPesoTotal.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                     (Action)(() =>
                     {
                         _lPesoTotal.Text = Math.Round(totalSize, 2).ToString() + " Gb";
                     }));
             });

            /*Task.Run(() =>
                DBManager.RegistroCopiasRepo.FindAsync(fdesde, fhasta, ptoCopiaId, categoria, user_id).ContinueWith(
                    task => {
                        List<registro_copias> list = task.Result;
                        
                        _dataGrid.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                            (Action)(() =>
                            {
                                _dataGrid.ItemsSource = list;
                            }));

                        _ltotalFiles.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                            (Action)(() =>
                            {
                                _ltotalFiles.Text = list.Count.ToString();
                            }));

                        double totalSize = 0;
                        foreach (registro_copias rc in list)
                        {
                            Thread.Sleep(500);
                            totalSize += rc.Size;
                        }

                        _lPesoTotal.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                            (Action)(() =>
                            {
                                _lPesoTotal.Text = Math.Round(totalSize, 2).ToString() + " Gb";
                            }));
                    }).Wait()
            );*/

            HideLoader();
        }

        private void ShowLoader()
        {
            _grid.Visibility = Visibility.Hidden;
            _LoaderGif.IsActive = true;
            _LoaderGif.Visibility = Visibility.Visible;
        }

        private void HideLoader()
        {
            _grid.Visibility = Visibility.Visible;
            _LoaderGif.IsActive = false;
            _LoaderGif.Visibility = Visibility.Hidden;
        }

        private void FilterChanged(object sender, SelectionChangedEventArgs e)
        {
            if(prepared)
                refreshGrid();
        }

        private void BtnViewCopy_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            if (id > 0)
            {
                registro_copias rc = DBManager.RegistroCopiasRepo.FindById(id);
                if(rc != null)
                {
                    new CopyListViewDialog(rc.copia_id).ShowDialog();
                }
            }
            
        }
    }
}
