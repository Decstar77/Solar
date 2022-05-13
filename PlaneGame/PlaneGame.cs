
using SolarSharp;
using SolarSharp.Core;

namespace PlaneGame
{
    public class AirGame : Game
    {
        public Guid PlaneModel { get; set; } = Guid.Empty;

        public bool Start()
        {
            return true;
        }

        public void FrameUpdate()
        {
            
        }

        public void TickUpdate()
        {
        }

        public void Shutdown()
        {
        }
    }

}