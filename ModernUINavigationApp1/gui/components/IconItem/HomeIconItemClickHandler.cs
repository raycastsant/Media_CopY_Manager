using MCP.db;
using MCP.gui.Pages;
using System.Collections.Generic;
using System.IO;

namespace MCP.gui.components.IconItem
{
    //Handler del evento click del TreeView
    class HomeIconItemClickHandler : IIconItemClickHandler
    {
        public async void HandleIconItemClick(int mediaID, IconItem iitem)
        {
            PHome._PHome.ClearFilter();
            if (mediaID == -1)   //Es un nodo Categoria
            {
                int categId = (int)iitem.Tag;
                List<media_files> mfList = await DBManager.MediaFilesRepo.FindByCategoriaAsync(categId, mediaID, false);

                HomeCatalogManager.ShowItemContent(mfList);
                PHome._PHome.selectionChanged();
            }
            else
            {
                media_files mf = DBManager.MediaFilesRepo.FindById(mediaID);
                if (mf != null)
                {
                    iitem.setFileExists(mf.FileExists());
                    List<media_files> mfList = await DBManager.MediaFilesRepo.FindByCategoriaAsync(mf.categoria_id, mediaID, false);

                    HomeCatalogManager.ShowItemContent(mfList);
                    PHome._PHome.selectionChanged();
                }
            }
        }
    }
}
