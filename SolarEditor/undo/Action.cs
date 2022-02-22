using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEditor
{
    internal interface Action
    {
        internal bool Redo();
        internal bool Undo();
    }
}
