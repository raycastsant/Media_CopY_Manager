using MCP.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MCP.gui.components.IconItem
{
    public class IconItem : TreeViewItem
    {
        private int mediaId;
        private Label l;
        private IIconItemClickHandler clickHandler;
        private bool file_exists;

        public bool StandBy {get; set;} //Para prevenir que cuando se de click se ejecute el click de los padres

        public IconItem(string header, media_files mf, IIconItemClickHandler clickHandler)
        {
            this.mediaId = mf.id;

            l = new Label();
            l.Content = header;
            l.Margin = new Thickness(5);

            Image img = new Image();
            img.Height = 22;
            img.Width = 22;
            BitmapImage bmi;

            if (mf.is_folder)
                bmi = new BitmapImage(new Uri("pack://application:,,,/Resources/folder.ico"));
            else
                bmi = new BitmapImage(new Uri("pack://application:,,,/Resources/cinema.png"));

            img.Source = bmi;

            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            sp.Children.Add(img);
            sp.Children.Add(l);
           // sp.MouseLeftButtonUp += CheckItem_MouseLeftButtonUp;

            this.Header = sp;
            this.MouseLeftButtonUp += CheckItem_MouseLeftButtonUp;
            this.StandBy = false;
            this.clickHandler = clickHandler;

            setFileExists(mf.FileExists());
        }

        public IconItem(string header, bool isFolder, IIconItemClickHandler clickHandler)
        {
            this.mediaId = -1;

            l = new Label();
            l.Content = header;
            l.Margin = new Thickness(5);

            Image img = new Image();
            img.Height = 22;
            img.Width = 22;
            BitmapImage bmi;

            if (isFolder)
                bmi = new BitmapImage(new Uri("pack://application:,,,/Resources/folder.ico"));
            else
                bmi = new BitmapImage(new Uri("pack://application:,,,/Resources/cinema.png"));

            img.Source = bmi;

            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            sp.Children.Add(img);
            sp.Children.Add(l);

            this.Header = sp;
            this.MouseLeftButtonUp += CheckItem_MouseLeftButtonUp;
            this.StandBy = false;
            this.clickHandler = clickHandler;

            setFileExists(true);
        }

        private void CheckItem_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!StandBy)
            {
                PreventParentsFireEvent(this, true);

                clickHandler.HandleIconItemClick(mediaId, this); //AppMAnager.EditMediaInfo(mediaId, this);
            }
            else
                this.StandBy = false;
        }

        private void PreventParentsFireEvent(IconItem item, bool flag)
        {
            if (item.Parent is IconItem)
            {
                ((IconItem)item.Parent).StandBy = flag;
                PreventParentsFireEvent((IconItem)item.Parent, flag);
            }
        }

        public int MediaId()
        {
            return mediaId;
        }

        public void setTitle(string title)
        {
            if (!String.IsNullOrEmpty(title))
            {
                l.Content = title;
            }
        }

        public bool FileExists()
        {
            return file_exists;
        }

        public void setFileExists(bool val)
        {
            file_exists = val;

            BrushConverter bc = new BrushConverter();
            if (!file_exists)
            {
                l.Foreground = (Brush)bc.ConvertFrom(AppMAnager.COLOR_ERROR_FOREGROUND);
            }
            else
                l.Foreground = AppMAnager.DefaultLabelForeColor();
        }
    }
}
