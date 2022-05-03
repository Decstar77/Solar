using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolarSharp;
using SolarSharp.Rendering;

namespace SolarEditor
{
    internal class EditorState
    {
        public string AssetPath;
        private List<Window> windows = new List<Window>();

        internal EditorState(EditorConfig config)
        {
            AssetPath = config.AssetPath;
            AddWindow(new ShaderEditorWindow());
            AddWindow(new AssetSystemWindow());
            EventSystem.Listen(EventType.RENDER_END, (EventType type, object context) => { UIDraw(); return false; });
        }

        internal void UIDraw()
        {
            ImGui.BeginFrame();
            DrawGlobalMenu();
            ShowWindows();
            ImGui.EndFrame();
        }

        internal void Update()
        {

        }

        internal void Shutdown()
        {
          
        }

        internal bool AddWindow(Window window)
        {
            windows.Add(window);
            return true;
        }

        internal void ShowWindows()
        {
            windows.ForEach(x => x.Show(this));
            windows.RemoveAll(x => x.ShouldClose());            
        }

        private void DrawGlobalMenu()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Open"))
                    {

                    }

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("View"))
                {
                    if (ImGui.MenuItem("Shader Editor"))
                    {

                    }

                    if (ImGui.MenuItem("Assets"))
                    {

                    }

                    ImGui.EndMenu();
                }

                ImGui.EndMainMenuBar();
            }
        }
    }
}
