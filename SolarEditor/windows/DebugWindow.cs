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
        public static bool ShowBoundingBoxes = false;

        public override void Show(EditorState editorState)
        {
            if (ImGui.Begin("Debug window", ref show))
            {
                ImGui.CheckBox("Show Bounding boxes", ref ShowBoundingBoxes);
            }
            ImGui.End();

            if (ShowBoundingBoxes) {
                GameSystem.CurrentScene.Entities.ForEach(entity => {
                    DebugDraw.AlignedBox(entity.WorldSpaceBoundingBox);
                });

            }
        }

        public override void Shutdown()
        {
        }

        public override void Start()
        {
        }
    }
}
