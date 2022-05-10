using SolarSharp;
using SolarSharp.core;
using SolarSharp.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEditor
{
    internal class DebugWindow : EditorWindow
    {
        

        public override void Show(EditorState editorState)
        {
            if (ImGui.Begin("Debug window", ref show))  {
                ImGui.CheckBox("Show Bounding boxes", ref editorState.ShowBoundingBoxes );
                ImGui.CheckBox("Show empties", ref editorState.ShowEmpties);
            }
            ImGui.End();
        }

        public override void Shutdown()
        {
        }

        public override void Start()
        {
        }
    }
}
