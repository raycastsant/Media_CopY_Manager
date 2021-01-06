using MCP.db;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MCP.gui.Pages.administracion
{
    public partial class PCoeficientes : UserControl
    {
        private int State { get; set; }

        public PCoeficientes()
        {
            InitializeComponent();

            refreshGrid();
            State = AppMAnager.STATE_NULL;
        }

        private void refreshGrid()
        {
            ObservableCollection<CoeficienteObject> list = new ObservableCollection<CoeficienteObject>();
            CoeficienteObject cob;
            foreach (coeficientes_pago cp in DBManager.CoeficientesRepo.List)
            {
                cob = new CoeficienteObject();
                cob.id = cp.id;
                cob.tipo = cp.tipos_pago.nombre;
                cob.categoria = cp.tipo_categorias.nombre;
                cob.value = cp.valor;
                list.Add(cob);
            }

            _dataGrid.ItemsSource = list;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<CoeficienteObject> list = (ObservableCollection<CoeficienteObject>)_dataGrid.ItemsSource;
            coeficientes_pago CP;
            foreach (CoeficienteObject co in list)
            {
                CP = DBManager.CoeficientesRepo.FindById(co.id);
                if(CP != null)
                {
                    CP.valor = co.value;
                    DBManager.CoeficientesRepo.Update(CP);
                }
            }
        }
    }

    public class CoeficienteData
    {
        public ObservableCollection<CoeficienteObject> _lista { get; set; }

        public CoeficienteData()
        {
            _lista = new ObservableCollection<CoeficienteObject>();
        }

        public void add(CoeficienteObject co)
        {
            _lista.Add(co);
        }
    }

    public class CoeficienteObject
    {
        public int id { get; set; }
        public string tipo { get; set; }
        public string categoria { get; set; }
        public double value { get; set; }
    }
}
