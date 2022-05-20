using SolarSharp.Rendering.Graph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SolarSharp.Assets
{
    public class EngineAsset
    {
        [JsonInclude]
        public Guid Guid = Guid.NewGuid();
    }

    public static class AssetSystem
    {
        public static List<ShaderAsset> ShaderAssets { get { return shaderAssets; } }
        private static List<ShaderAsset> shaderAssets = new List<ShaderAsset>();
        public static List<RenderGraph> RenderGraphs { get { return renderGraphs; } }
        private static List<RenderGraph> renderGraphs = new List<RenderGraph>();
        
        private static Dictionary<Guid, SceneAsset> scenes = new Dictionary<Guid, SceneAsset>();        
        private static Dictionary<Guid, ModelAsset> modelAssets = new Dictionary<Guid, ModelAsset>();
        private static List<TextureAsset> textureAssets = new List<TextureAsset>();


        private static Dictionary<Guid, MaterialAsset> materialAssets = new Dictionary<Guid, MaterialAsset>();
        private static HashSet<string> materialNames = new HashSet<string>();

        public static ModelAsset? GetModelAsset(string name)
        {
            lock (modelAssets)
            {
                foreach (var asset in modelAssets)
                {
                    if (asset.Value.name == name)
                    {
                        return asset.Value;
                    }
                }
            }

            return null;
        }

        public static ModelAsset? GetModelAsset(Guid id)
        {
            lock (modelAssets)
            {
                ModelAsset asset;
                if (modelAssets.TryGetValue(id, out asset))
                {
                    return asset;
                }
            }

            return null;
        }

        public static List<ModelAsset> GetSortedModelAssets() {
            lock (modelAssets)
            {
                var list = modelAssets.Values.ToList();
                list.Sort((x, y) => (x.name.CompareTo(y.name)));
                return list;
            }
        }
            
        public static void AddModelAsset(ModelAsset model)
        {
            lock (modelAssets)
            {                
                Logger.Trace($"Placing {model.name}");
                modelAssets.Add(model.Guid, model);
            }
        }

        public static void RemoveModelAsset(Guid id)
        {
            lock (modelAssets)
            {
                if (modelAssets.Remove(id))
                {
                    Logger.Trace($"Removed model {id}");
                }                   
            }
        }

        //////////////////////////////////////////////////
        //////////////////////////////////////////////////
        //////////////////////////////////////////////////
        //////////////////////////////////////////////////
        //////////////////////////////////////////////////
        
        public static List<TextureAsset> GetSortedTextureAssets()
        {
            lock (textureAssets)
            {
                textureAssets.Sort((x, y) => (x.name.CompareTo(y.name)));
                List<TextureAsset> textures = new List<TextureAsset>(textureAssets);
                return textures;
            }
        }

        public static void AddTextureAsset(TextureAsset texture)
        {
            lock(textureAssets)
            {
                Logger.Trace($"Placing {texture.name}");
                textureAssets.Add(texture);
            }
        }

        //////////////////////////////////////////////////
        //////////////////////////////////////////////////
        //////////////////////////////////////////////////
        //////////////////////////////////////////////////
        //////////////////////////////////////////////////

        public static List<SceneAsset> GetScenesAssets() => new List<SceneAsset>(scenes.Values);
        public static void AddGameScene(SceneAsset sceneAsset)
        {
            lock (scenes)
            {
                Logger.Trace($"Placing {sceneAsset.name}");
                if (sceneAsset.Guid != Guid.Empty)
                {
                    if (!scenes.ContainsKey(sceneAsset.Guid))
                    {
                        scenes.Add(sceneAsset.Guid, sceneAsset);
                    }                    
                }
                else
                {
                    Logger.Error("Adding scene with invalid guid");
                }
            }
        }

        public static void SaveGameSceneAsset(string path, SceneAsset sceneAsset)
        {
            Logger.Trace($"Saving {path + sceneAsset.name}");
            string json = JsonSerializer.Serialize(sceneAsset);
            File.WriteAllText(path + sceneAsset.name + ".json", json);
        }

        public static SceneAsset LoadGameSceneAsset(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    Logger.Trace($"Loading {path}");
                    string json = File.ReadAllText(path);
                    SceneAsset gameScene = JsonSerializer.Deserialize<SceneAsset>(json);
                    gameScene.name = Path.GetFileNameWithoutExtension(path);
                    AddGameScene(gameScene);

                    return gameScene;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                }
            }
            else
            {
                Logger.Error("LoadGameSceneAsset, File does not exist: " + path);
            }

            return null;
        }


        //////////////////////////////////////////////////
        //////////////////////////////////////////////////
        //////////////////////////////////////////////////
        //////////////////////////////////////////////////
        //////////////////////////////////////////////////

        public static void AddMaterialAsset(MaterialAsset materialAsset)
        {
            lock (materialAssets)
            {                
                if (!materialAssets.ContainsKey(materialAsset.Guid) && !materialNames.Contains(materialAsset.name))
                {
                    Logger.Trace($"Placing {materialAsset.name}");
                    materialAssets.Add(materialAsset.Guid, materialAsset);
                    materialNames.Add(materialAsset.name);
                }
                else
                {
                    Logger.Warn($"Material {materialAsset.name} already exists in the material registery");
                }
            }
        }

        public static void AddMaterialAssets(List<MaterialAsset> materials)
        {
            lock (materialAssets)
            {
                foreach (MaterialAsset materialAsset in materials)
                {
                    if (!materialAssets.ContainsKey(materialAsset.Guid) && !materialNames.Contains(materialAsset.name))
                    {
                        Logger.Trace($"Placing {materialAsset.name}");
                        materialAssets.Add(materialAsset.Guid, materialAsset);
                        materialNames.Add(materialAsset.name);
                    }
                    else
                    {
                        Logger.Warn($"Material {materialAsset.name} already exists in the material registery");
                    }
                }
            }
        }

        public static List<MaterialAsset> GetSortedMaterialAssets()
        {
            lock (materialAssets)
            {
                return materialAssets.Select(x => x.Value).ToList();
            }
        }
        public static MaterialAsset? GetMaterialAsset(Guid id)
        {
            lock (materialAssets)
            {
                if (materialAssets.TryGetValue(id, out var materialAsset))
                {
                    return materialAsset;
                }
            }

            return null;
        }

        public static MaterialAsset? GetMaterialAsset(string name)
        {
            lock(materialAssets)
            {
                return materialAssets.ToList().Find(x => x.Value.name == name).Value;
            }
        }

        //////////////////////////////////////////////////
        //////////////////////////////////////////////////
        //////////////////////////////////////////////////
        //////////////////////////////////////////////////
        //////////////////////////////////////////////////

        public static ShaderAsset? GetShaderAsset(string name)
        {
            lock (shaderAssets)
            {
                return shaderAssets.Find(x => x.name == name);
            }
        }

        //////////////////////////////////////////////////
        //////////////////////////////////////////////////
        //////////////////////////////////////////////////
        //////////////////////////////////////////////////
        //////////////////////////////////////////////////

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

                    lock (renderGraphs)
                    {
                        renderGraphs.Add(new RenderGraph(renderGraphSaveData));
                    }

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
                    shaderAsset.name = Path.GetFileNameWithoutExtension(path);
                    
                    StreamReader file = new StreamReader(path);
                    shaderAsset.Src = file.ReadToEnd();
                    file.Close();

                    lock (shaderAssets)
                    {
                        shaderAssets.Add(shaderAsset);
                    }                   

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
