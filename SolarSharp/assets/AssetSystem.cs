using SolarSharp.Rendering.Graph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SolarSharp.Assets
{
    public static class AssetSystem
    {
        public static List<ShaderAsset> ShaderAssets { get { return shaderAssets; } }
        private static List<ShaderAsset> shaderAssets = new List<ShaderAsset>();

        public static List<RenderGraph> RenderGraphs { get { return renderGraphs; } }
        private static List<RenderGraph> renderGraphs = new List<RenderGraph>();

        public static bool Initialize()
        {
            return true;
        }

        public static bool LoadAllShaders(string path)
        {
            Directory.GetFiles(path, "*.hlsl", SearchOption.AllDirectories).ToList().ForEach(x=> LoadShaderAsset(x));
            return true;
        }

        public static bool LoadAllRenderGraphs(string path)
        {
            Directory.GetFiles(path, "*.rg", SearchOption.AllDirectories).ToList().ForEach(x => LoadRenderGraphAsset(x));
            return true;
        }

        public static RenderGraphAsset LoadRenderGraphAsset(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    string json = File.ReadAllText(path);
                    RenderGraphAsset renderGraphSaveData = JsonSerializer.Deserialize<RenderGraphAsset>(json);
                    renderGraphSaveData.Path = path;                    
                    renderGraphs.Add( new RenderGraph(renderGraphSaveData) );
                    return renderGraphSaveData;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                }
            }
            else
            {
                Logger.Error("LoadRenderGraphAsset, File does not exist: " + path);
            }

            return null;
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
                catch (Exception ex)
                { 
                    Logger.Error(ex.Message);
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
