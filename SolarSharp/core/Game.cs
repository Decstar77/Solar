using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Core
{
    public interface Game
    {
        public bool Start();
        public void FrameUpdate();
        public void TickUpdate();
        public void Shutdown();
    }
}
