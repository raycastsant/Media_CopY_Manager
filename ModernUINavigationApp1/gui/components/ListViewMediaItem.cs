using MCP.db;
using MCP.gui.Pages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MCP.gui.components
{
    public class ListViewMediaItem : ContentControl
    {
        private int mediaId;
        private int categoriaId;
        private bool is_folder;
        private bool file_exists;

        //public bool StandBy { get; set; }

        public ListViewMediaItem(media_files mf)
        {
            this.mediaId = mf.id;
            this.categoriaId = mf.categoria_id;
            this.is_folder = mf.is_folder;

            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;

            Label l = new Label();
            l.Content = mf.titulo+"  |  "+mf.file_url;
            l.Margin = new Thickness(5);

            Image img = new Image();
            img.Height = 22;
            img.Width = 18;
            BitmapImage bmi;

            if (mf.is_folder)
                bmi = new BitmapImage(new Uri("pack://application:,,,/Resources/folder.ico"));
            else
                bmi = new BitmapImage(new Uri("pack://application:,,,/Resources/cinema.png"));

            img.Source = bmi;

            sp.Children.Add(img);
            sp.Children.Add(l);
            this.Content = sp;

            file_exists = mf.FileExists();
            BrushConverter bc = new BrushConverter();
            if (!file_exists)
            {
                l.Foreground = (Brush)bc.ConvertFrom(AppMAnager.COLOR_ERROR_FOREGROUND);
            }
            else
                l.Foreground = AppMAnager.DefaultLabelForeColor();

            this.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(Item_MouseDoubleClick);
           // this.StandBy = false;
        }

        private void Item_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (is_folder)
            {
                PHome._PHome.ClearFilter();

                List<media_files> mfList = DBManager.MediaFilesRepo.FindByCategoria(categoriaId, mediaId, false);
                HomeCatalogManager.ShowItemContent(mfList);
                PHome._PHome.SelectTreeViewFolder(mediaId);
            }
            else
            if(file_exists)
            {
                media_files mf = DBManager.MediaFilesRepo.FindById(mediaId);
                if(mf != null)
                {
                    Process.Start(@mf.file_url);
                    //File.e mf.file_url;
                }
            }
        }

        public int MediaId()
        {
            return mediaId;
        }

        public bool FileExists()
        {
            return file_exists;
        }
    }
}
