using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SolarSharp.EngineAPI;

namespace SolarSharp.Core
{
    public class Window
    {
        public int SurfaceWidth { get { return surfaceWidth; } }
        private int surfaceWidth;

        public int SurfaceHeight { get { return surfaceHeight; } }
        private int surfaceHeight;

        public float WindowAspect { get { return windowAspect; } }
        private float windowAspect;

        public string WindowTitle { get { return windowTitle; } }
        private string windowTitle;

        public Window()
        {

        }

        public bool Open(string title, int surfaceWidth, int surfaceHeight, int startX, int startY)
        {            
            windowTitle = title; ;
            this.surfaceWidth = surfaceWidth;
            this.surfaceHeight = surfaceHeight; 
            this.windowAspect = (float)surfaceWidth / (float)surfaceHeight;

            return Win32API.CreateWindow_(windowTitle, surfaceWidth, surfaceHeight, startX, startY);
        }

        public bool Running(ref FrameInput input)
        {
            return Win32API.PumpMessages_(ref input);
        }
    }
}
