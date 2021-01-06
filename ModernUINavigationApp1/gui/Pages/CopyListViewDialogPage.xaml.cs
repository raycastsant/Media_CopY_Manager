using FirstFloor.ModernUI.Windows.Controls;
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

namespace MCP.gui.Pages
{
    /// <summary>
    /// Interaction logic for CopyListViewDialogPage.xaml
    /// </summary>
    public partial class CopyListViewDialogPage : UserControl
    {
        public CopyListViewDialogPage()
        {
            InitializeComponent();

            this.Loaded += ContentLoaded;
        }

        private void ContentLoaded(object sender, RoutedEventArgs e)
        {
            var w = Window.GetWindow(this);
            if(w != null && w is CopyListViewDialog)
            {
                int id = ((CopyListViewDialog)w).getDBCopyId();

                 copia c = DBManager.CopiasRepo.FindById(id);
                 if (c != null)
                 {
                     tbCodigo.Text = "#" + c.codigo;
                     tbTipoPago.Text = c.tipos_pago.nombre;
                     tbFecha.Text = c.fecha.ToString();
                     tbMontoSystema.Text = "$"+c.monto_sistema.ToString();
                     tbMontoReal.Text = "$" + c.monto_real.ToString();

                     foreach (registro_copias rc in DBManager.CopiasRepo.ListRegistros(id))
                     {
                         _listSeleccion.Items.Add("(" + rc.nombre_categoria + ") "+rc.titulo + ", [" + Math.Round(rc.Size, 2) + " GB], " +rc.destino_url  );
                     }
                 }
            }
        }
    }
}
