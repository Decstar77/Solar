using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Core
{
    public class Clock
    {
        private double lastLoopTime;

        public void Start()
        {
            lastLoopTime = GetTime();
        }

        public double GetTime()
        {
            double timestamp = Stopwatch.GetTimestamp();
            double nanoseconds = 1_000_000_000.0 * timestamp / Stopwatch.Frequency;

            return nanoseconds;
        }

        public float GetElapsedTime()
        {
            double time = GetTime();
            double elapsedTime = (time - lastLoopTime);
            lastLoopTime = time;
            return (float)(elapsedTime * 0.000000001);
        }

        public double GetLastLoopTime()
        {
            return lastLoopTime;
        }
    }
}
