using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp
{
    public class ApplicationConfig
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string Title { get; set; }

        public int SurfaceWidth { get; set; }
        public int SurfaceHeight { get; set; }

        public int WindowXPos { get; set; }
        public int WindowYPos { get; set; }

        public string AssetPath { get; set; }

        public delegate bool OnInitialize();
        public OnInitialize OnInitializeCallback;

        public delegate GameScene OnUpdate();
        public OnUpdate OnUpdateCallback;

        //public delegate void OnRender(RenderPacket renderPacket);
        //public OnRender OnRenderCallback;

        public delegate void OnShutdown();
        public OnShutdown OnShutdownCallback;
    }
}
