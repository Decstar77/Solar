using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolarSharp;
using SolarSharp.Rendering;
using SolarSharp.Rendering.Graph;
using SolarSharp.Assets;

namespace SolarEditor
{
    internal class EditorState
    {
        private List<Window> windows = new List<Window>();
        private List<Window> newWindows = new List<Window>();

        internal EditorState()
        {
            //AddWindow(new AssetSystemWindow());
            //AddWindow(new ShaderEditorWindow(AssetSystem.ShaderAssets[0]));
            //AddWindow(new RenderGraphWindow());
            EventSystem.Listen(EventType.RENDER_END, (EventType type, object context) => { UIDraw(); return false; });
        }

        private string path = "C:/Users/claud/OneDrive/Desktop/DeclanStuff/Solar/EngineAssets/";
        private string name = "";
        private bool open = false;

        internal void UIDraw()
        {
            ImGui.BeginFrame();
            DrawGlobalMenu();            

            //if (open)
            //{
            //    open = false;
            //    ImGui.OpenPopup("Create Shader");
            //}

            //if (ImGui.BeginPopupModal("Create Shader", ImGuiWindowFlags.NoResize))
            //{
            //    ImGui.InputText("Name", ref name);
            //    ImGui.InputText("Path", ref path);

            //    if (ImGui.Button("Create", 80, 0))
            //    {
            //        RenderSystem.graphicsShaders.Add(ShaderFactory.CreateGraphicsShader(name, path));
            //        ImGui.CloseCurrentPopup();
            //    }
            //    ImGui.SameLine();
            //    if (ImGui.Button("Cancel", 80, 0))
            //    {
            //        ImGui.CloseCurrentPopup();
            //    }

            //    ImGui.EndPopup();
            //}

            ShowWindows();
            ImGui.EndFrame();
        }

        internal void Update()
        {

        }

        internal void Shutdown()
        {
          
        }

        internal T AddWindow <T> (T window) where T : Window
        {
            foreach (Window w in windows)
                if (w.GetType() == window.GetType())
                    return (T)w;

            newWindows.Add(window);
            return window;
        }

        internal void ShowWindows()
        {
            windows.AddRange(newWindows);
            newWindows.Clear();
            windows.ForEach(x => x.Show(this));
            windows.RemoveAll(x => x.ShouldClose());            
        }

        private void DrawGlobalMenu()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.BeginMenu("New"))
                    {
                        if (ImGui.MenuItem("Graphics"))
                        {
                            //open = true;
                        }
                        if (ImGui.MenuItem("Compute"))
                        {

                        }

                        ImGui.EndMenu();
                    }

                    if (ImGui.MenuItem("Open"))
                    {

                    }

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("View"))
                {
                    if (ImGui.MenuItem("Shader Editor")) {
                        AddWindow(new ShaderEditorWindow(null));
                    }

                    if (ImGui.MenuItem("Assets")) {
                        AddWindow(new AssetSystemWindow());
                    }

                    if (ImGui.MenuItem("Render graph")) {
                        AddWindow(new RenderGraphWindow());
                    }

                    if (ImGui.MenuItem("Scene ")) {
                        AddWindow(new GameSceneWindow());
                    }                    

                    ImGui.EndMenu();
                }

                ImGui.EndMainMenuBar();
            }
        }
    }
}
