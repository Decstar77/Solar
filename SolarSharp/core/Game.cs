using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Core
{
    public interface Game
    {
        public bool Start(GameScene scene);
        public void FrameUpdate(GameScene scene);
        public void TickUpdate(GameScene scene);
        public void Shutdown(GameScene scene);
    }
}
