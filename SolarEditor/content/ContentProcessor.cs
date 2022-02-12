using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SolarSharp
{
    public class ContentProcessor
    {
        public static string AssetPath = "F:/codes/Learning/SolarSharp/Assets/base";

        public static void ProcessModels()
        {
            Dictionary<string, int> materials = new Dictionary<string, int>();

            string[] materialFiles = null;
            if (Directory.Exists(AssetPath))
            {
                materialFiles = Directory.GetFiles(AssetPath, @"*.obj", SearchOption.AllDirectories);
                foreach (string materialFile in materialFiles)
                {
                    string[] lines= File.ReadAllLines(materialFile);

                    foreach (string line in lines)
                    {
                        if (line.StartsWith("newmtl"))
                        {
                            string name = line.Split(' ')[1];
                            materials.Add(name, 1);
                        }
                    }
                }
            }
            else
            {
                Logger.Error("Could not find asset path: " + AssetPath);
            }  
        }
    }
}
