using FirstFloor.ModernUI.Windows.Controls;
using MCP.gui.components;
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

namespace ModernUINavigationApp1.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : UserControl
    {
        WrapPanel container;
        ScrollViewer scroll;
        ModernButton buttonBack;
        Grid grid;

        public SettingsPage()
        {
            InitializeComponent();

         /*   ScrollViewer scroll = new ScrollViewer();
            scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            container = new WrapPanel();
            container.Orientation = Orientation.Vertical;
            scroll.Content = container;

            TabItem tab = new TabItem();
            tab.Header = "TAB 1";

            grid = new Grid();
            RowDefinition rd = new RowDefinition();
            
            //grid.RowDefinitions.Add()

            buttonBack = GetBackButton();
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            sp.Children.Add(buttonBack);
            container.Children.Add(sp);
            tab.Content = scroll;

            _fcontent.Items.Add(tab);*/
        }

        private ModernButton GetBackButton()
        {
            GeometryConverter geomConvert = new GeometryConverter();
            Geometry iconData = (Geometry)geomConvert.ConvertFromString("F1 M 57,42L 57,34L 32.25,34L 42.25,24L 31.75,24L 17.75,38L 31.75,52L 42.25,52L 32.25,42L 57,42 Z ");
            ModernButton buttonBack = new ModernButton();
            buttonBack.EllipseDiameter = 30;
            buttonBack.IconHeight = 20;
            buttonBack.IconWidth = 20;
            buttonBack.ToolTip = "Atrás";
            buttonBack.IconData = iconData;
            buttonBack.IsEnabled = false;

            return buttonBack;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           /* Border b = new Border();
            BrushConverter bc = new BrushConverter();
            b.BorderBrush = (Brush)bc.ConvertFrom("#FFFFFFFF");
            b.BorderThickness = new Thickness(1, 1, 1, 1);
            b.CornerRadius = new CornerRadius(2);
            b.Margin = new Thickness(10);
            Expander exp = new Expander();
            exp.IsExpanded = true;
            exp.BorderThickness = new Thickness(0);
            exp.Header = "General";
            exp.Content = "Add content here";
            b.Child = exp;*/

            Poster p = new Poster("", "POster", 1, true, 1);

            container.Children.Add(p);
        }
    }
}
