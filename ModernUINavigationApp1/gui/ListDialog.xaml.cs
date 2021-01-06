using FirstFloor.ModernUI.Windows.Controls;
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

namespace MCP.gui
{
    /// <summary>
    /// Interaction logic for ListDialog.xaml
    /// </summary>
    public partial class ListDialog : ModernDialog
    {
        public ListDialog(string title, string subtitle, List<string> list)
        {
            InitializeComponent();

            this.Title = title;
            _lText.Text = subtitle;

            foreach (string s in list)
            {
                _listView.Items.Add(s);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if(false == AppMAnager.userLogged)
                Application.Current.Shutdown();
        }
    }
}
