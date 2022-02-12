using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp
{
    public class Logger
    {
        public static void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR: " + message);
        }

        public static void Info(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("INFO:  " + message);
        }

        public static void Trace(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("TRACE: " + message);
        }
    }
}
