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

        public static float SurfaceAspect { get { return windowAspect; } }
        private static float windowAspect;

        public static string WindowName { get { return windowName; } }
        private static string windowName;

        private static int windowX;   
        private static int windowY;        

        private static Input input = new Input();
        private static Input oldInput = new Input();

        private static Vector2 mouseDelta = new Vector2(0, 0);

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


                                    bool mouseUnlocked = true; // @NOTE: Used to stop snapping when first activating locking the mouse
                                    while (EngineAPI.Win32PumpMessages(input.keys, ref input.mouseIput))
                                    {
                                        RenderPacket renderPacket = new RenderPacket();

                                        if (input.mouseIput.mouseLocked)
                                        {                   
                                            if (mouseUnlocked)
                                            {
                                                mouseUnlocked = false;
                                                input.mouseIput.mouseXPositionNormalCoords = 0.5f;
                                                input.mouseIput.mouseYPositionNormalCoords = 0.5f;
                                            }

                                            mouseDelta.x = (float)(input.mouseIput.mouseXPositionNormalCoords - 0.5);
                                            mouseDelta.y = (float)(input.mouseIput.mouseYPositionNormalCoords - 0.5);
                                        }
                                        else
                                        {
                                            mouseUnlocked = true;
                                            mouseDelta.x = (float)(input.mouseIput.mouseXPositionNormalCoords - oldInput.mouseIput.mouseXPositionNormalCoords);
                                            mouseDelta.y = (float)(input.mouseIput.mouseYPositionNormalCoords - oldInput.mouseIput.mouseYPositionNormalCoords);
                                        }

                                        config.OnUpdateCallback();
                                        config.OnRenderCallback(renderPacket);

                                        Renderer.Render(renderPacket);

                                        oldInput.Copy(input);
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

        public static void Quit()
        {
            EngineAPI.Win32PostQuitMessage();
        }
        public static float GetDeltaTime()
        {
            return 0.016f;
        }
        public static bool IsKeyDown(ushort vkCode)
        {
            return input.keys[vkCode] == 1;
        }
        public static bool IsKeyJustDown(ushort vkCode)
        {
            return input.keys[vkCode] == 1 && oldInput.keys[vkCode] == 0;
        }
        public static bool IsKeyJustUp(ushort vkCode)
        {
            return input.keys[vkCode] == 0 && oldInput.keys[vkCode] == 1;
        }
        public static void EnableMouse()
        {
            input.mouseIput.mouseLocked = false;
        }
        public static void DisableMouse()
        {
            input.mouseIput.mouseLocked = true;
        }

        public static bool GetMouseDown(int num)
        {
            if (num == 1)
            {
                return input.mouseIput.mb1;
            }
            else if (num == 2)
            {
                return input.mouseIput.mb2;
            }
            else if (num == 3)
            {
                return input.mouseIput.mb3;
            }

            return false;
        }

        public static bool GetMouseJustDown(int num)
        {
            if (num == 1)
            {
                return input.mouseIput.mb1 && !oldInput.mouseIput.mb1;
            }
            else if (num == 2)
            {
                return input.mouseIput.mb2 && !oldInput.mouseIput.mb2; 
            }
            else if (num == 3)
            {
                return input.mouseIput.mb3 && !oldInput.mouseIput.mb3; 
            }

            return false;
        }
        public static Vector2 GetMouseDelta()
        {
            return mouseDelta;
        }
        public static Vector2 GetMousePixelPosition()
        {
            return new Vector2((float)input.mouseIput.mouseXPositionPixelCoords, (float)input.mouseIput.mouseYPositionPixelCoords);
        }
    }
}
