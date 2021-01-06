using FirstFloor.ModernUI.Windows.Controls;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MCP.gui
{
    /// <summary>
    /// Interaction logic for ListSelectorDialog.xaml
    /// </summary>
    public partial class ListSelectorDialog : ModernDialog
    {
        private string topic;
        private IListRefreshable lr;
        private IList mainList;

        public ListSelectorDialog(string topic, IListRefreshable lr, IList mainList, List<string> selectedItems)
        {
            InitializeComponent();

            // define the dialog buttons
            this.Buttons = null; //new Button[] { this.OkButton, this.CancelButton };
            this.CloseButton.Visibility = Visibility.Hidden;
            this.Title = "Selección de :";

            this.topic = topic;
            this.lr = lr;
            this.mainList = mainList;
            _lText.Text = topic;

            CheckBox chb;
            foreach(object item in mainList)
            {
                chb = new CheckBox();
                chb.Content = item.ToString();

                if (selectedItems.Contains(item.ToString()))
                    chb.IsChecked = true;

                chb.Tag = item;
                _listBox.Items.Add(chb);
            }
        }

        private void _BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (lr != null)
            {
                List<object> lista = new List<object>();
                ListItem li;
                foreach(CheckBox item in _listBox.Items)
                {
                    if ((bool)item.IsChecked)
                    {
                        li = new ListItem();
                        lista.Add(item.Tag);
                    }
                }

                lr.RefresshList(topic, lista);
                this.Close();
            }
        }

        private void _BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
