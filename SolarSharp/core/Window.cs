using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SolarSharp.EngineAPI;

namespace SolarSharp.Core
{
    public static class Window
    {
        public static int SurfaceWidth { get { return surfaceWidth; } }
        private static int surfaceWidth;

        public static int SurfaceHeight { get { return surfaceHeight; } }
        private static int surfaceHeight;

        public static float WindowAspect { get { return windowAspect; } }
        private static float windowAspect;

        public static string WindowTitle { get { return windowTitle; } }
        private static string windowTitle;

        public static bool Initialize(string title, int surfaceWidth_, int surfaceHeight_, int startX, int startY)
        {            
            windowTitle = title; ;
            surfaceWidth = surfaceWidth_;
            surfaceHeight = surfaceHeight_; 
            windowAspect = (float)surfaceWidth / (float)surfaceHeight;

            return Win32API.CreateWindow_(windowTitle, surfaceWidth, surfaceHeight, startX, startY);
        }

        public static bool Running(ref FrameInput input)
        {
            return Win32API.PumpMessages_(ref input);
        }
    }
}
