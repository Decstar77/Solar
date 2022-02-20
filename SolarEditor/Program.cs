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
        private static EditorState editorState;
        
        static Vector3 v = new Vector3();
        static Matrix4 model = Matrix4.Identity;
        
        public static bool ImGuiDraw()
        {
            ImGui.BeginFrame();

            ImGui.GizmoEnable(true);
            ImGui.GizmoSetRect(0, 0, Application.SurfaceWidth, Application.SurfaceHeight);

            ImGui.GizmoManipulate(camera, ref model, 2, 1);
       


            if (ImGui.DragFloat3("Label", ref v))
            {
                
            }

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

                    ImGui.EndMenu();
                }

                ImGui.EndMainMenuBar();
            }

            editorState.ShowWindows();


            ImGui.EndFrame();

            return false;
        }


        public static bool OnInitialize()
        {
            editorState = new EditorState(new EditorConfig());

            camera = new FlyCamera();

            Console.WriteLine(camera.GetProjectionMatrix());

            EventSystem.Listen(EventType.RENDER_END, (EventType type, object context) => { return ImGuiDraw(); });
            return ImGui.Initialzie();
        }

        static FlyCamera camera;
        static Ray ray = new Ray();
        public static void OnUpdate()
        {
            if (Application.IsKeyJustDown(0x1B))
            {
                Application.Quit();
            }
            
            camera.Operate();

            if (Application.GetMouseJustDown(1))
            {
                ray = camera.ShootRayFromMousePos();
            }
            Debug.DrawRay(ray);
        }

        public static void OnRender(RenderPacket renderPacket)
        {
            Vector3 position;
            Quaternion orientation;
            Vector3 scale;
            Matrix4.Decompose(model.Transpose, out position, out orientation, out scale);

            //renderPacket.renderEntries.Add(new RenderEntry(new Vector3(5, 0, 0), Quaternion.Identity, new Vector3(0.1f, 1, 1)));
            renderPacket.renderEntries.Add(new RenderEntry(position, orientation, scale));

            renderPacket.viewMatrix = camera.GetViewMatrix();
            renderPacket.projectionMatrix = camera.GetProjectionMatrix();
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
