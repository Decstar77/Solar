using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SolarSharp.Core;
using SolarSharp.Rendering;
using SolarSharp.EngineAPI;
using SolarSharp.Assets;
using SolarSharp.Rendering.Graph;
using SolarSharp.core;

namespace SolarSharp
{
    public class Application
    {
        public static float DeltaTime { get; set; }

        public static Clock clock = new Clock();

        public static FrameInput Input;
        public static FrameInput OldInput;

        private static string version;
        private static string descrip;

        private static bool initialized = false;

        public static ApplicationConfig Config { get; private set; }
        
        public Application(ApplicationConfig config)
        {
            Config = config;

            if (!EventSystem.Initialize()) {
                Logger.Error("Could not initialize event system");
            };
            
            if (!Window.Initialize(config.Title, config.SurfaceWidth, config.SurfaceHeight, config.WindowXPos, config.WindowYPos)) {
                Logger.Error("Could not open window");
            }

            if (!AssetSystem.Initialize()) {
                Logger.Error("Could not initalize assets");
            }

            if (!AssetSystem.LoadAllShaders(config.AssetPath)) {
                Logger.Error("Could not load shader assets");
            }

            if (!RenderSystem.Initialize()) {
                Logger.Error("Could not initalize renderer");
            }

            if (!AssetSystem.LoadAllRenderGraphs(config.AssetPath)) {
                Logger.Error("Could not load render graph assets");
            }

            if (!GameSystem.Initialize()) {
                Logger.Error("Could not initalize game systems");
            }

            if (!DebugDraw.Initialize(RenderSystem.device)) {
                Logger.Error("Could not initalize debug system");
            }


            GameSystem.CurrentScene = new GameScene();
            GameSystem.CurrentScene.name = "Untitled";

            if (!config.OnInitializeCallback.Invoke()) {
                Logger.Error("Could not intialize project");
            }

            
            clock.Start();
            while (Window.Running(ref Input)) {

                Clock t = new Clock();
                t.Start();
                config.OnUpdateCallback.Invoke();

                RenderSystem.BackupRenderer();                
                EventSystem.Fire(EventType.RENDER_END, null);
                Console.WriteLine(t.GetElapsedTime() * 1000);

                RenderSystem.SwapBuffers(false);

                OldInput = Input;
                DeltaTime = clock.GetElapsedTime();

                Console.WriteLine(DeltaTime * 1000);
            }

            ImNodes.Shutdown();
            ImGuiAPI.ImGuiShutdown();
        }
    }
}
