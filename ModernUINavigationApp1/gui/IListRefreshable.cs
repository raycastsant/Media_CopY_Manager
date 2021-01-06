using System.Collections;

namespace MCP.gui
{
    /**Para refrescar las listas desde un dialogo de seleccion*/
    public interface IListRefreshable
    {
        void RefresshList(string topic, IList entity_list);
    }
}
