using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolarSharp.Rendering;

namespace SolarSharp
{
    public class Application
    {
        public static int SurfaceWidth { get { return surfaceWidth; }  }
        private static int surfaceWidth;

        public static int SurfaceHeight { get { return surfaceHeight; } }
        private static int surfaceHeight;

        public static float WindowAspect { get { return windowAspect; } }
        private static float windowAspect;

        public static string WindowName { get { return windowName; } }
        private static string windowName;

        private static int windowX;   
        private static int windowY;        

        private static Input input = new Input();
        private static Input oldInput = new Input();

        private static string version;
        private static string descrip;

        private static bool initialized = false;

        public Application(ApplicationConfig config)
        {
            if (initialized)
            {
                Logger.Error("Application is already running !!");
                return;
            }

            initialized = true;

            surfaceWidth = config.SurfaceWidth;
            surfaceHeight = config.SurfaceHeight;
            windowAspect = (float)surfaceWidth / (float)surfaceHeight;
            windowX = config.WindowXPos;
            windowY = config.WindowYPos;
            windowName = config.Title;
            version = config.Version;
            descrip = config.Description;

            if (config.OnUpdateCallback != null 
                && config.OnRenderCallback != null 
                && config.OnInitializeCallback != null 
                && config.OnShutdownCallback != null)
            {
                if (EngineAPI.Win32CreateWindow(windowName, surfaceWidth, surfaceHeight, windowY, windowX))
                {
                    Logger.Trace("Window created");

                    if (EngineAPI.Win32CreateRenderer())
                    {
                        Logger.Trace("Renderer created");
                        if (EventSystem.Initialize())
                        {
                            if (Renderer.Create())
                            {                                
                                if (config.OnInitializeCallback())
                                {
                                    Logger.Info("Startup successful");

                                    while (EngineAPI.Win32PumpMessages(ref input))
                                    {
                                        RenderPacket renderPacket = new RenderPacket();

                                        config.OnUpdateCallback();
                                        config.OnRenderCallback(renderPacket);

                                        Renderer.Render(renderPacket);


                                        oldInput = input;
                                    }
                                }
                                else
                                {

                                }
                            }
                            else
                            {

                            }
                        }
                        else
                        {

                        }

                        EngineAPI.Win32DestroyRenderer();
                    }
                    else
                    {
                        Logger.Error("Could not create win32 renderer");
                    }

                    EngineAPI.Win32DestroyWindow();
                }
                else
                {
                    Logger.Error("Could not create win32 window");
                }
            }
            else
            {
                Logger.Error("Update/render/init/shutdown callback(s) is null");
            }
        }
    }
}
