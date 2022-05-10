using SolarSharp.Rendering.Graph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace SolarSharp.Assets
{
    public class EngineAsset
    {
        public Guid Guid = Guid.NewGuid();
    }

    public static class AssetSystem
    {
        public static List<ShaderAsset> ShaderAssets { get { return shaderAssets; } }
        private static List<ShaderAsset> shaderAssets = new List<ShaderAsset>();
        public static List<RenderGraph> RenderGraphs { get { return renderGraphs; } }
        private static List<RenderGraph> renderGraphs = new List<RenderGraph>();
        
        private static List<GameScene> gameScenes = new List<GameScene>();        
        private static List<ModelAsset> modelAssets = new List<ModelAsset>();
        private static List<TextureAsset> textureAssets = new List<TextureAsset>();

        public static ModelAsset? GetModelAsset(string name)
        {
            ModelAsset? model = null;

            lock (modelAssets)
            {
                model = modelAssets.Find(x => x.name == name);
            }

            return model;
        }

        public static ModelAsset? GetModelAsset(Guid id)
        {
            ModelAsset? model = null;

            lock (modelAssets)
            {
                model = modelAssets.Find(x => x.Guid == id);
            }

            return model;
        }

        public static List<ModelAsset> GetSortedModelAssets() {
            lock(modelAssets)
            {
                modelAssets.Sort((x, y) => (x.name.CompareTo(y.name)));
                List<ModelAsset> models = new List<ModelAsset>(modelAssets);
                return models;
            }             
        }
            
        public static void AddModelAsset(ModelAsset model)
        {
            lock (modelAssets)
            {                
                Logger.Trace($"Placing {model.name}");
                modelAssets.Add(model);
            }
        }

        public static void RemoveModelAsset(Guid id)
        {
            lock (modelAssets)
            {
                int index = modelAssets.FindIndex(x => x.Guid == id);
                if (index >= 0)
                {
                    Logger.Trace($"Removing {modelAssets[index].name}");
                    modelAssets.RemoveAt(index);
                }                
            }
        }

        public static List<TextureAsset> GetTextureAssets() => new List<TextureAsset>(textureAssets);
        public static void AddTextureAsset(TextureAsset texture)
        {
            lock(textureAssets)
            {
                Logger.Trace($"Placing {texture.name}");
                textureAssets.Add(texture);
            }
        }

        public static List<GameScene> GetGameScenes() => new List<GameScene>(gameScenes);
        public static void AddGameScene(GameScene gameScene)
        {
            lock (gameScenes)
            {
                Logger.Info($"Placing {gameScene.name}");
                gameScenes.Add(gameScene);
            }
        }


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

        public static void SaveGameSceneAsset(string path, GameScene gameScene)
        {
            Logger.Trace($"Saving {path}");
            string json = JsonSerializer.Serialize(gameScene);
            File.WriteAllText(path + gameScene.name + ".json", json);
        }

        public static GameScene LoadGameSceneAsset(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    string json = File.ReadAllText(path);
                    GameScene gameScene = JsonSerializer.Deserialize<GameScene>(json);
                    gameScene.name = Path.GetFileNameWithoutExtension(path);

                    lock(gameScenes)
                    {
                        gameScenes.Add(gameScene);
                    }

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
                    shaderAsset.Name = Path.GetFileNameWithoutExtension(path);
                    
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
