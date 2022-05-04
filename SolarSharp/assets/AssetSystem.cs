using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Assets
{
    public static class AssetSystem
    {
        public static List<ShaderAsset> ShaderAssets { get { return shaderAssets; } }
        public static List<ShaderAsset> shaderAssets = new List<ShaderAsset>();

        public static bool Initialize()
        {
            return true;
        }

        public static void LoadEverything(string path)
        {
            LoadAllShaders(path);
        }

        private static void LoadAllShaders(string path)
        {
            string[] paths = Directory.GetFiles(path, "*.hlsl", SearchOption.AllDirectories);
            foreach (string p in paths) {
                LoadShaderAsset(p);
            }
        }

        public static ShaderAsset LoadShaderAsset(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    ShaderAsset shaderAsset = new ShaderAsset();
                    shaderAsset.Path = path;
                    shaderAsset.Name = Path.GetFileNameWithoutExtension(path);
                    
                    StreamReader file = new StreamReader(path);
                    shaderAsset.Src = file.ReadToEnd();
                    file.Close();

                    ShaderAssets.Add(shaderAsset);

                    return shaderAsset;
                }
                catch (Exception e)
                { 
                    Logger.Error(e.Message);
                }
            }
            else
            {
                Logger.Error("LoadShaderAsset, File does not exist: " + path);
            }

            return null;
        }
    }
}
