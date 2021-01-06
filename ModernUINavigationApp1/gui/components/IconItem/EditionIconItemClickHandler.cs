using MCP.db;

namespace MCP.gui.components.IconItem
{
    class EditionIconItemClickHandler : IIconItemClickHandler
    {
        public void HandleIconItemClick(int mediaID, IconItem iitem)
        {
            media_files mf = DBManager.MediaFilesRepo.FindById(mediaID);
            if (mf != null)
            {
                AppMAnager.PEditarCatalogo_instance.LoadMediaFile(mf, iitem);
            }
            else
            {
                AppMAnager.PEditarCatalogo_instance.HideForm();
                AppMAnager.PEditarCatalogo_instance.DisableButtons();
            }
        }
    }
}
