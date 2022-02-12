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

        public static bool ImGuiDraw()
        {
            ImGui.BeginFrame();
            
            if (ImGui.DragFloat3("Label", ref v))
            {
                Console.WriteLine(v);
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
            Quaternion q = Quaternion.Normalize(new Quaternion(2, 23, 34, 12));

            Console.WriteLine(Matrix3.ToQuaternion(Quaternion.ToMatrix3(q)));
            Console.WriteLine(Matrix4.ToQuaternion(Quaternion.ToMatrix4(q)));

            Matrix4 m = new Matrix4();
            m.col1 = new Vector4(1, 2, 3, 4);
            m.col2 = new Vector4(5, 6, 7, 8);
            m.col3 = new Vector4(9, 10, 11, 12);
            m.col4 = new Vector4(13, 14, 15, 16);

            Console.WriteLine(m);
            Console.WriteLine(m.col1);
            Console.WriteLine(m.col2);
            Console.WriteLine(m.col3);
            Console.WriteLine(m.col4);

            EventSystem.Listen(EventType.RENDER_END, (EventType type, object context) => { return ImGuiDraw(); });
            return ImGui.Initialzie();
        }

        static FlyCamera camera;
        public static void OnUpdate()
        {
            if (Application.IsKeyJustDown(0x1B))
            {
                Application.Quit();
            }
            camera.Operate();
        }

        public static void OnRender(RenderPacket renderPacket)
        {
            //renderPacket.renderEntries.Add(new RenderEntry(new Vector3(5, 0, 0), Quaternion.Identity, new Vector3(0.1f, 1, 1)));
            renderPacket.renderEntries.Add(new RenderEntry(Vector3.Zero, Quaternion.Identity, new Vector3(1,1,1)));

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
