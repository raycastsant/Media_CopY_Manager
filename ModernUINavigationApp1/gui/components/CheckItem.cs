using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MCP.gui.components
{
    public class CheckItem : TreeViewItem
    {
        private CheckBox chb;
        private bool exists;

        public CheckItem(String header, bool fileExists, bool isFolder)
        {
            exists = fileExists;

            chb = new CheckBox();
            chb.IsChecked = true;

            TextBlock tb = new TextBlock();
            tb.Text = header;

            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;

            if (!fileExists)
            {
                chb.Click += new RoutedEventHandler(ClickNodo);
                sp.Children.Add(chb);
            }
            else
            {
                chb.IsChecked = false;
                if (isFolder)
                    sp.Children.Add(chb);

                BrushConverter bc = new BrushConverter();
                tb.Foreground = (Brush)bc.ConvertFrom(AppMAnager.COLOR_ERROR_FOREGROUND);
            }

            sp.Children.Add(tb);

            this.Header = sp;
            this.MouseLeftButtonUp += CheckItem_MouseLeftButtonUp;
        }

        private void CheckItem_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
           // AppMAnager.ClickNodo(this);
        }

        public CheckItem AddChild(string header, bool exists, bool childIsFolder)
        {
            CheckItem child = new CheckItem(header, exists, childIsFolder);
            this.Items.Add(child);

            return child;
        }

        private void ClickNodo(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(chb.IsChecked);
            AppMAnager.ActualizarSeleccion(this);
        }

        public bool IsChecked()
        {
            return (bool)chb.IsChecked;
        }

        public bool FileExists()
        {
            return exists;
        }

        public void SetChecked(bool val)
        {
            chb.IsChecked = val;
        }
    }
}
