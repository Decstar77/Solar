using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp
{
    public static class GameSystem
    {
        public static GameScene CurrentScene { get; set; }

        public static bool Initialize() {
            return true;
        }
    }
}
