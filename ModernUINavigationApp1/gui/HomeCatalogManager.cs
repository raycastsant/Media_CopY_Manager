using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using MCP.db;
using MCP.gui.components;
using MCP.gui.components.IconItem;
using MCP.gui.Pages;

namespace MCP.gui
{
    public class HomeCatalogManager
    {
        public static readonly string TAG_FILTERED = "filtered";

        public static void ShowItemContenInvoke(List<media_files> mediaList)
        {
            ListView listView = PHome._PHome.getListView();

            listView.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                (Action)(() => {

                    PHome._PHome.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        (Action)(() => {
                            PHome._PHome.SetContentDescription(mediaList.Count + " elemento(s)");
                        }));

                    listView.Items.Clear();
                    ListViewMediaItem item;

                    foreach (media_files media in mediaList)
                    {
                        item = new ListViewMediaItem(media);
                        listView.Items.Add(item);
                    }
            }));
        }

        public static void ShowItemContent(List<media_files> mediaList)
        {
            ListView listView = PHome._PHome.getListView();

            PHome._PHome.SetContentDescription(mediaList.Count + " elemento(s)");

            listView.Items.Clear();
            ListViewMediaItem item;

            foreach (media_files media in mediaList)
            {
                item = new ListViewMediaItem(media);
                listView.Items.Add(item);
            }
        }

        //Realiza una busqueda para la categoria y el filtro seleccionados
      //  public static async Task<List<media_files>> FiltrarCatalogoAsync(int categId, string filtro)
     //   {
         //   if(categId > 0)
        //        return await DBManager.MediaFilesRepo.AplyFilterAsync(categId, filtro);
          //  else
          //      return await DBManager.MediaFilesRepo.AplyFilterAsync(filtro);  //Todas las categorias

            //Filtro del arbol del explorador
            /* TreeView tree = (TreeView)page.Content;

             tree.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                 (Action)(() => {
                     tree.Items.Clear();
                     IconItem nodo;
                     HomeIconItemClickHandler _iitemClickHandler = new HomeIconItemClickHandler();
                     foreach (media_files mf in resultList)
                     {
                         if (mf.is_folder) {
                             nodo = new IconItem(mf.titulo, mf, _iitemClickHandler);
                             tree.Items.Add(nodo);
                             AppMAnager.FillMovieChildrens(nodo, mf.categoria_id, mf.id, _iitemClickHandler, true);
                         }
                     }
                     tree.Tag = TAG_FILTERED;
                 }));*/

           // return resultList;
      //  }

     /*   public static void FiltrarCatalogo(TabItem page, string filtro)
        {
            if (page != null)
            {
                int categId = int.Parse(page.Tag.ToString());

                List<media_files> resultList;
                if (!string.IsNullOrEmpty(filtro))
                {
                    resultList = DBManager.MediaFilesRepo.AplyFilter(categId, filtro);
                    Console.WriteLine("Query result OK");

                    //Filtro del arbol del explorador
                    TreeView tree = (TreeView)page.Content;
                    tree.Items.Clear();
                    IconItem nodo;
                    HomeIconItemClickHandler _iitemClickHandler = new HomeIconItemClickHandler();
                    foreach (media_files mf in resultList)
                    {
                        if (mf.is_folder)
                        {
                            nodo = new IconItem(mf.titulo, mf, _iitemClickHandler);
                            tree.Items.Add(nodo);
                            AppMAnager.FillMovieChildrens(nodo, mf.categoria_id, mf.id, _iitemClickHandler, true);
                        }
                    }
                    tree.Tag = TAG_FILTERED;
                }
                else
                    resultList = DBManager.MediaFilesRepo.FindByCategoria(categId, -1, false);

                //Filtro de los contenidos
                ShowItemContent(resultList);
            }

            Console.WriteLine("Filter End");
        }*/

        /*
        // Carga el contenido de una carpeta MEDia File. Doble Click POster
        public static void LoadMediaFiles(int categId, int mediaParentId)
        {
            MediaFilesRepository repo = DBManager.MediaFilesRepo; // new MediaFilesRepository();
            List<media_files> mediaList = repo.FindByCategoria(categId, mediaParentId);
            WrapPanel container = dict_container[categId];
            if (container != null)
            {
                ((ModernButton)container.Tag).IsEnabled = true; //Boton de ir atras
                ((ModernButton)container.Tag).Tag = mediaParentId;

                RefreshMediaContainer(mediaList, categId, container);
                PHome._PHome.ClearPageSelection(false);  //false porque se crearan nuevos Posters
            }
        }*/
    }
}
