using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SolarSharp;
using SolarSharp.EngineAPI;
using System.Text.Json;

namespace SolarEditor
{
    public class Program
    {
        private static EditorState? editorState;
        public static bool OnInitialize()
        {
            editorState = new EditorState();           
            return true;
        }

        public static void OnUpdate()
        {
            editorState?.Update();
        }

        //public static void OnRender(RenderPacket renderPacket)
        //{
        //    renderPacket.renderEntries.Add(new RenderEntry(new Vector3(5, 0, 0), Quaternion.Identity, new Vector3(0.1f, 1, 1)));
        //    renderPacket.renderEntries.Add(new RenderEntry(new Vector3(0, 0, 0), Quaternion.Identity, new Vector3(1,1,1)));
        //}

        public static void OnShutdown()
        {
            editorState?.Shutdown();
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
            config.OnShutdownCallback = OnShutdown;
            config.AssetPath = "C:/Users/claud/OneDrive/Desktop/DeclanStuff/Solar/EngineAssets/";

            //string json = JsonSerializer.Serialize(config);
            //File.WriteAllText("appConfig.json", json);

            Application app = new Application(config);
        }
    }
}
