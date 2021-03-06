using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SolarSharp;
using PlaneGame;
using SolarSharp.Core;

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

        public static GameScene OnUpdate()
        {
            return editorState?.Update();
        }

        public static void OnShutdown()
        {
            editorState?.Shutdown();
        }

        public static void Main()
        {
            ApplicationConfig config = new ApplicationConfig();
            config.Title = "Editor";
            config.Description = "Solar editor";
            config.SurfaceWidth = 1900;
            config.SurfaceHeight = 1000;
            config.WindowXPos = 20;
            config.WindowYPos = 30;
            config.Version = "0.1";
            config.OnInitializeCallback = OnInitialize;
            config.OnUpdateCallback = OnUpdate; 
            config.OnShutdownCallback = OnShutdown;
            config.AssetPath = "F:/codes/Solar/EngineAssets/";

            //string json = JsonSerializer.Serialize(config);
            //File.WriteAllText("appConfig.json", json);

            Application app = new Application(config);
        }
    }
}
