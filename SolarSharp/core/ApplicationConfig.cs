using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolarSharp.Rendering;

namespace SolarSharp
{
    public class ApplicationConfig
    {
        public string Name;
        public string Description;
        public string Version;
        public string Title;

        public int SurfaceWidth;
        public int SurfaceHeight;

        public int WindowXPos;
        public int WindowYPos;

        public delegate bool OnInitialize();
        public OnInitialize OnInitializeCallback;

        public delegate void OnUpdate();
        public OnUpdate OnUpdateCallback;

        public delegate void OnRender(RenderPacket renderPacket);
        public OnRender OnRenderCallback;

        public delegate void OnShutdown();
        public OnShutdown OnShutdownCallback;
    }
}
