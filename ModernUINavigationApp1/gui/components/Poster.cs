using MCP.gui.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MCP.gui.components
{
    public partial class Poster : StackPanel
    {
        private Button button;
        private TextBlock textblock;
        private int mediaId;
        private bool isFolder;
        private int categId;
        private Border b;

        public bool selected { get; set; }

        public Poster(string imgSource, string title, int mediaId, bool isFolder, int categId)
        {
            InitializeComponent();

            this.Width = 120;
            this.Orientation = Orientation.Vertical;

            b = new Border();
            button = new Button();
            b.Height = 100;
            b.Width = 100;
            button.BorderThickness = new Thickness(0);

            BrushConverter bc = new BrushConverter();
            b.BorderBrush = (Brush)bc.ConvertFrom("#FF5A7DF5");
            b.BorderThickness = new Thickness(0);
            b.CornerRadius = new CornerRadius(4);
           // b.Visibility = Visibility.Hidden;

            ImageBrush ib = new ImageBrush();
            ImageSourceConverter isc = new ImageSourceConverter();
            BitmapImage bmi;

            if (!File.Exists(imgSource))
            {
                if (isFolder)
                {
                    bmi = new BitmapImage(new Uri("pack://application:,,,/Resources/folder.ico"));
                }
                else
                {
                    bmi = new BitmapImage(new Uri("pack://application:,,,/Resources/movie.ico"));
                }

                ib.ImageSource = bmi;
            }
            else   
                ib.ImageSource = (ImageSource)isc.ConvertFromString(imgSource);

            button.Background = ib;
            button.Cursor = AppMAnager.HAND_CURSOR;
            button.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(DobleClick);
            button.Click += new RoutedEventHandler(click);

            textblock = new TextBlock();
            textblock.Text = title;
            textblock.TextWrapping = System.Windows.TextWrapping.Wrap;
            textblock.TextAlignment = TextAlignment.Center;

            b.Child = button;
            this.Children.Add(b);
            this.Children.Add(textblock);
            this.Margin = new System.Windows.Thickness(10,10,10,10);

            this.mediaId = mediaId;
            this.isFolder = isFolder;
            this.categId = categId;
            this.MinHeight = 120;
            this.MaxHeight = 150;
            this.ToolTip = title;
            this.selected = false;
        }

        private void click(object sender, RoutedEventArgs e)
        {
          /*  if (!this.selected)
            {
                SelectComponent();
            }
            else
            {
                UnselectComponent();
            }

            PHome._PHome.RefreshSelection(this);*/
        }

        public void SelectComponent()
        {
            this.selected = true;
            BrushConverter bc = new BrushConverter();
            this.Background = (Brush)bc.ConvertFrom(AppMAnager.COLOR_SELECTION);
        }

        public void UnselectComponent()
        {
            this.selected = false;
            this.Background = null;
        }

        private void DobleClick(object sender, MouseButtonEventArgs e)
        {
           /* if (isFolder)
                AppMAnager.LoadMediaFiles(categId, mediaId);*/
        }

        public void ShowBorder()
        {
            b.BorderThickness = new Thickness(1, 1, 1, 1);
        }

        public void HidesBorder()
        {
            b.BorderThickness = new Thickness(0);
        }

        public int MediaId()
        {
            return mediaId;
        }

        public int CategId()
        {
            return categId;
        }

        /*public Poster(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }*/
    }
}
