using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCP.gui.components
{
    public interface ICopyController
    {
        void Pause();
        void Continue();
        void Quit();
    }
}
