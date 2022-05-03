using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolarSharp;

namespace GameCode
{
    internal class Program
    {
        internal static bool OnInitialize()
        {
            Quaternion q = Quaternion.Normalize(new Quaternion(34, 23, 34, 12));
            Console.WriteLine(q);

            return true;
        }

        internal static void OnUpdate()
        {

        }

        internal static void OnShutdown()
        {

        }


        public static void Main()
        {
            ApplicationConfig config = new ApplicationConfig();
            config.Title = "Gaem";
            config.Description = "Gaem";
            config.SurfaceWidth = 1280;
            config.SurfaceHeight = 720;
            config.WindowXPos = 200;
            config.WindowYPos = 200;
            config.Version = "0.1";
            config.OnInitializeCallback = OnInitialize;
            config.OnUpdateCallback = OnUpdate;
            config.OnShutdownCallback = OnShutdown;

            Application app = new Application(config);

        }

    }
}
