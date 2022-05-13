using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp
{
    public class Logger
    {
        private static List<string> logs = new List<string>();

        public static List<string> GetLogs()
        {
            lock (logs)
            {
                return new List<string>(logs);
            }
        }

        public static void ClearLogs()
        {
            lock (logs)
            {
                logs.Clear();
            }
        }


        public static void Error(string message)
        {
            lock(logs)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                string msg = "ERROR: " + message;
                logs.Add(msg);
                Console.WriteLine(msg);
            }            
        }

        public static void Warn(string message)
        {
            lock (logs)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                string msg = "WARN:  " + message;
                logs.Add(msg);
                Console.WriteLine(msg);
            }
        }

        public static void Info(string message)
        {
            lock (logs)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                string msg = "INFO:  " + message;
                logs.Add(msg);
                Console.WriteLine(msg);
            }
        }

        public static void Trace(string message)
        {
            lock (logs)
            {
                Console.ForegroundColor = ConsoleColor.White;
                string msg = "TRACE: " + message;
                logs.Add(msg);
                Console.WriteLine(msg);
            }
        }
    }
}
