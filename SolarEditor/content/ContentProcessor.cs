using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using SolarSharp.GameLogic;

namespace SolarSharp
{
    public class ContentProcessor
    {
        public static string AssetPath = "F:/codes/Solar/Assets/base";

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

        public static void SaveRoomToTextFile(Room room)
        {
            StreamWriter fileWriter = new StreamWriter(AssetPath + "/room.txt");


            fileWriter.WriteLine("Version=0.1");
                        
            foreach (var prop in room.GetType().GetProperties())
            {
                fileWriter.Write(prop.Name);
                fileWriter.Write("=");
                fileWriter.WriteLine(prop.GetValue(room, null));
            }


            fileWriter.WriteLine("=========Entities=========");
            fileWriter.WriteLine("EntityCount={0}", room.entities.Count);
            foreach (Entity entity in room.entities)
            {
                fileWriter.WriteLine("ENTITY");

                var type = entity.GetType();                
                fileWriter.WriteLine("ClassName={0}", type.Name);

                var properties = type.GetProperties();
                foreach (var property in properties)
                {
                    var value = property.GetValue(entity, null);
                    if (value != null)
                    {
                        fileWriter.WriteLine("{0}={1}",property.Name, value.ToString());
                    }
                }
                
                fileWriter.WriteLine("END");
            }

            fileWriter.Close();
        }
    }
}
