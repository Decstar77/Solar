using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolarSharp;
using SolarSharp.Rendering;
using SolarSharp.GameLogic;

namespace GameCode
{
    internal class Program
    {
        private static Room room;

        internal static bool OnInitialize()
        {
            room = new Room();
            room.camera = new GameCamera();
            room.player.camera = room.camera;

            return true;
        }

        internal static void OnUpdate()
        {
            room.player.Operate();
            ((GameCamera)room.camera).Follow(room.player);

            
            
        }

        internal static void OnRender(RenderPacket renderPacket)
        {
            renderPacket.viewMatrix = room.camera.GetViewMatrix();
            renderPacket.projectionMatrix = room.camera.GetProjectionMatrix();
            renderPacket.renderEntries.Add(new RenderEntry(room.player.Position, room.player.Orientation, new Vector3(1, 1, 1)));
            renderPacket.renderEntries.Add(new RenderEntry(new Vector3(0, -2, 0),Quaternion.Identity , new Vector3(10, 1, 10)));
        }

        internal static void OnShutdown()
        {

        }

        public static void Main()
        {

            ApplicationConfig config = new ApplicationConfig();
            config.Title = "Game";
            config.Description = "Game";
            config.SurfaceWidth = 1280;
            config.SurfaceHeight = 720;
            config.WindowXPos = 200;
            config.WindowYPos = 200;
            config.Version = "0.1";
            config.OnInitializeCallback = OnInitialize;
            config.OnUpdateCallback = OnUpdate;
            config.OnRenderCallback = OnRender;
            config.OnShutdownCallback = OnShutdown;

            Application app = new Application(config);

        }

    }
}
