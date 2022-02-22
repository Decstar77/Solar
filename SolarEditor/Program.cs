using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SolarSharp;
using SolarSharp.Rendering;

namespace SolarEditor
{
    public class Program
    {
        private static EditorState? editorState;

        public static bool ImGuiDraw()
        {
            if (editorState != null)
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
                        if (ImGui.MenuItem("Assets"))
                        {
                            editorState.AddWindow(new AssetWindow());
                        }

                        if (ImGui.MenuItem("Room"))
                        {
                            editorState.AddWindow(new RoomWindow());
                        }

                        ImGui.EndMenu();
                    }

                    ImGui.EndMainMenuBar();
                }

               
                editorState.ShowWindows();

                ImGui.EndFrame();
            }

            return false;
        }


        public static bool OnInitialize()
        {
            editorState = new EditorState(new EditorConfig());

            EventSystem.Listen(EventType.RENDER_END, (EventType type, object context) => { return ImGuiDraw(); });
            return ImGui.Initialzie();
        }

        public static void OnUpdate()
        {
            if (editorState != null)
            {
                ImGui.BeginFrame();
                editorState.Update();     
            }
        }

        public static void OnRender(RenderPacket renderPacket)
        {
            if (editorState != null)
            {
                editorState.Render(renderPacket);
            }
        }

        public static void OnShutdown()
        {
            ImGui.Shutdown();
        }

        public static void Main()
        {
            ApplicationConfig config = new ApplicationConfig();
            config.Title = "Editor";
            config.Description = "Solar editor";
            config.SurfaceWidth = 1280;
            config.SurfaceHeight = 720;
            config.WindowXPos = 200;
            config.WindowYPos = 200;
            config.Version = "0.1";
            config.OnInitializeCallback = OnInitialize;
            config.OnUpdateCallback = OnUpdate; 
            config.OnRenderCallback = OnRender;
            config.OnShutdownCallback = OnShutdown;

            //EditorConfig configEditor = new EditorConfig();
            //System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(EditorConfig));
            //FileStream file = File.Create("EditorConfig.xml");
            //writer.Serialize(file, configEditor);
            //file.Close();

            Application app = new Application(config);
        }
    }
}
