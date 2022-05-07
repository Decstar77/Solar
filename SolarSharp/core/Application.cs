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

namespace SolarSharp
{
    public class Application
    {
        public static FrameInput Input { get { return input; } }
        private static FrameInput input = new FrameInput();
        public static FrameInput OldInput { get; set; }

        private static string version;
        private static string descrip;

        private static bool initialized = false;

        public Application(ApplicationConfig config)
        {
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

            if (!config.OnInitializeCallback.Invoke()) {
                Logger.Error("Could not intialize project");
            }

            GameSystem.CurrentScene = new GameScene();
            GameSystem.CurrentScene.Name = "Untitled";

            while (Window.Running(ref input)) {

                config.OnUpdateCallback.Invoke();

                RenderSystem.BackupRenderer();

                //deviceContext.Context.SetVertexBuffers(mesh.VertexBuffer, mesh.StrideBytes);
                //deviceContext.Context.SetIndexBuffer(mesh.IndexBuffer, DXGIFormat.R32_UINT, 0);
                //deviceContext.Context.DrawIndexed(mesh.IndexCount, 0, 0);



                EventSystem.Fire(EventType.RENDER_END, null);
                RenderSystem.SwapBuffers();

                OldInput = input;
            }

            ImNodes.Shutdown();
            ImGuiAPI.ImGuiShutdown();


            //    if (initialized)
            //    {
            //        Logger.Error("Application is already running !!");
            //        return;
            //    }

            //    initialized = true;

            //    surfaceWidth = config.SurfaceWidth;
            //    surfaceHeight = config.SurfaceHeight;
            //    windowAspect = (float)surfaceWidth / (float)surfaceHeight;
            //    windowX = config.WindowXPos;
            //    windowY = config.WindowYPos;
            //    windowName = config.Title;
            //    version = config.Version;
            //    descrip = config.Description;

            //    if (config.OnUpdateCallback != null 
            //        && config.OnRenderCallback != null 
            //        && config.OnInitializeCallback != null 
            //        && config.OnShutdownCallback != null)
            //    {
            //        if (Win32API.CreateWindow_(windowName, surfaceWidth, surfaceHeight, windowY, windowX))
            //        {
            //            Logger.Trace("Window created");

            //            if (EngineAPI.Win32CreateRenderer())
            //            {
            //                Logger.Trace("Renderer created");
            //                if (EventSystem.Initialize())
            //                {
            //                    if (Renderer.Create())
            //                    {                                
            //                        if (config.OnInitializeCallback())
            //                        {
            //                            Logger.Info("Startup successful");

            //                            while (Win32API.PumpMessages_(ref input))
            //                            {
            //                                RenderPacket renderPacket = new RenderPacket();

            //                                //config.OnUpdateCallback();
            //                                //config.OnRenderCallback(renderPacket);

            //                                Renderer.Render(renderPacket);


            //                                oldInput = input;
            //                            }
            //                        }
            //                        else
            //                        {

            //                        }
            //                    }
            //                    else
            //                    {

            //                    }
            //                }
            //                else
            //                {

            //                }

            //                EngineAPI.Win32DestroyRenderer();
            //            }
            //            else
            //            {
            //                Logger.Error("Could not create win32 renderer");
            //            }

            //            Win32API.DestroyWindow_();
            //        }
            //        else
            //        {
            //            Logger.Error("Could not create win32 window");
            //        }
            //    }
            //    else
            //    {
            //        Logger.Error("Update/render/init/shutdown callback(s) is null");
            //    }
        }
    }
}
