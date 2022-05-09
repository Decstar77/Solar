using SolarSharp.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEditor
{
    internal class ConsoleWindow : EditorWindow
    {
        public override void Show(EditorState editorState)
        {
            if (ImGui.Begin("Console", ref show))
            {
                

            }

            ImGui.End();
        }
    }
}
