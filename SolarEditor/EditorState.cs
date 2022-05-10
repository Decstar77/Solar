using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolarSharp;
using SolarSharp.Rendering;
using SolarSharp.Rendering.Graph;
using SolarSharp.Assets;
using SolarSharp.core;
using SolarSharp.Core;

namespace SolarEditor
{

    internal class EditorState
    {
        private List<EditorWindow> windows = new List<EditorWindow>();
        private List<EditorWindow> newWindows = new List<EditorWindow>();
        private FlyCamera camera = new FlyCamera();

        internal EditorState()
        {
            //AddWindow(new AssetSystemWindow());
            //AddWindow(new ShaderEditorWindow(AssetSystem.ShaderAssets[0]));
            //AddWindow(new RenderGraphWindow());
            EventSystem.AddListener(EventType.RENDER_END, (EventType type, object context) => { UIDraw(); return false; }, this);

            Task.Run(() => 
            {
                Directory.GetFiles(Application.Config.AssetPath, "*.fbx", SearchOption.AllDirectories)
                .Concat(Directory.GetFiles(Application.Config.AssetPath, "*.obj", SearchOption.AllDirectories))
                .ToList().ForEach(x => {               
                    Logger.Info($"Loading model {x}");
                    MetaFileAsset metaFileAsset = MetaFileAsset.GetOrCreateMetaFileAsset(x, AssetType.MODEL);
                    ModelAsset? modelAsset = ModelImporter.LoadFromFile(x, metaFileAsset);
                    if (modelAsset != null)
                    {
                        AssetSystem.AddModelAsset(modelAsset);
                        RenderSystem.RegisterModel(modelAsset);
                    }
                });
            });

            Task.Run(() =>
            {
                Directory.GetFiles(Application.Config.AssetPath, "*.png", SearchOption.AllDirectories).ToList().ForEach(x => {
                    Logger.Info($"Loading texture {x}");
                    MetaFileAsset metaFileAsset = MetaFileAsset.GetOrCreateMetaFileAsset(x, AssetType.TEXTURE);
                    TextureAsset? textureAsset = TextureImporter.LoadFromFile(x, metaFileAsset);
                    if (textureAsset != null)
                    {
                        AssetSystem.AddTextureAsset(textureAsset);
                        RenderSystem.RegisterTexture(textureAsset);
                    }
                });
            });


            GameScene gameScene = new GameScene();
            gameScene.Camera = camera;
            
            Entity entity = new Entity();
            entity.Name = "Bike";
            entity.Material = new Material();
            entity.Material.ModelId = Guid.Parse("10209316-f57b-41f7-88cf-8ea81614bdb2");
            entity.Material.AlbedoTexture = Guid.Parse("5b767a8f-5e2e-428d-81d2-8b350bf556fc");
            entity.Position = new Vector3(0, 0, -3);
            //entity.Orientation = Quaternion.RotateLocalZ(Quaternion.Identity, Util.DegToRad(45.0f));

            gameScene.Entities.Add(entity);
            GameSystem.CurrentScene = gameScene;
            

            AssetSystem.AddGameScene(gameScene);

            Logger.Info("Editor startup complete");
        }        

        private bool newModelDialog = false;
        private ModelAsset? newModelAsset = null;

        internal void Update()
        {
            ImGui.BeginFrame();
            DrawGlobalMenu();
            ShowWindows();

            if (Input.IskeyJustDown(KeyCode.S) && Input.IsKeyDown(KeyCode.CTRL_L)) {
                EventSystem.Fire(EventType.ON_SAVE, null);

                AssetSystem.SaveGameSceneAsset(Application.Config.AssetPath + "scene", GameSystem.CurrentScene);
            }

            GameSystem.CurrentScene.Entities.ForEach(entity => {
                DebugDraw.AlignedBox(entity.WorldSpaceBoundingBox);
            });

            if (Input.IsMouseButtonJustDown(MouseButton.MOUSE1) && !ImGui.WantMouseInput())
            {
                Ray ray = camera.ShootRayFromMousePos();

                GameSystem.CurrentScene.Entities.ForEach(entity => {
                    if (Raycast.AlignedBox(ray, entity.WorldSpaceBoundingBox, out _))
                    {
                        Console.WriteLine("hit");
                    }
                });
            }

            Entity entity = GameSystem.CurrentScene.Entities[0];
            ImGizmo.Enable(true);
            ImGizmo.SetRect(0, 0, Window.SurfaceWidth, Window.SurfaceHeight);

            Matrix4 modelMatrix = entity.ComputeModelMatrix().Transpose;
            if (ImGizmo.Manipulate(camera.GetProjectionMatrix().Transpose, camera.GetViewMatrix().Transpose ,ref modelMatrix, ImGizmoOperation.TRANSLATE, ImGizmoMode.LOCAL))
            {
                Vector3 pos;
                Quaternion rot;
                Vector3 scl;
                Matrix4.Decompose(modelMatrix.Transpose, out pos, out rot, out scl);

                entity.Position = pos;
                entity.Orientation = rot;
                entity.Scale = scl;
            }

            camera.Operate();
        }

        internal void Shutdown()
        {

        }

        internal void UIDraw()
        {
           
                     

            //if (open)
            //{
            //    open = false;
            //    ImGui.OpenPopup("Create Shader");
            //}

            if (newModelDialog)
            {
                newModelDialog = false;
                ImGui.OpenPopup("Create Model Asset");
            }

            
            if (ImGui.BeginPopupModal("Create Model Asset"))
            {
                string path = Application.Config.AssetPath + newModelAsset.name;

                ImGui.InputText("Path", ref path);
                if (ImGui.Button("Create", 200, 0))
                { 
                    File.Copy(newModelAsset.path,  path);

                    //MeshAsset mesh = newModelAsset.meshes[0];
                    //StaticMesh newMesh = new StaticMesh(RenderSystem.device, mesh);
                    //RenderSystem.cube = newMesh;

                    newModelAsset = null;

                    ImGui.CloseCurrentPopup();
                }
                ImGui.SameLine();
                if (ImGui.Button("Cancel", 200, 0))
                {
                    newModelAsset = null;
                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndPopup();
            }
 
    
            ImGui.EndFrame();
        }  

        internal T AddWindow <T> (T window) where T : EditorWindow
        {
            foreach (EditorWindow w in windows)
                if (w.GetType() == window.GetType())
                    return (T)w;

            newWindows.Add(window);
            return window;
        }

        internal void ShowWindows()
        {
            newWindows.ForEach(x => x.Start());
            windows.AddRange(newWindows);
            newWindows.Clear();
            
            windows.ForEach(x => x.Show(this));

            windows.RemoveAll(x =>
            {
                if (x.ShouldClose())
                {
                    x.Shutdown();
                    return true;
                }
                return false;
            });
        }

        private void DrawGlobalMenu()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.BeginMenu("New"))
                    {
                        if (ImGui.MenuItem("Graphics"))
                        {
                            //open = true;
                        }
                        if (ImGui.MenuItem("Compute"))
                        {

                        }

                        ImGui.EndMenu();
                    }

                    if (ImGui.MenuItem("Open"))
                    {
                        string file = SolarSharp.Core.Window.OpenNativeFileDialog();

                        if (file != null && file != "")
                        {
                            string ext = Path.GetExtension(file);
                            if (ext == ".fbx" || ext == ".obj")
                            {
                                //Task.Run(() => {
                                //    Logger.Info($"Loading model {file}");
                                //    ModelAsset? modelAsset = ModelImporter.LoadFromFile(file);
                                //    if (modelAsset != null)
                                //    {
                                //        newModelDialog = true;
                                //        newModelAsset = modelAsset;
                                //    }
                                //});                             
                            }
                            else if (ext == ".png")
                            {
                                Logger.Error("We don't support textures... yet.");
                            }
                            else
                            {
                                Logger.Error("Unkown extension, " + ext);
                            }
                        }
                    }

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("View"))
                {
                    if (ImGui.MenuItem("Shader Editor")) {
                        AddWindow(new ShaderEditorWindow(null));
                    }

                    if (ImGui.MenuItem("Render graph")) {
                        AddWindow(new RenderGraphWindow());
                    }

                    if (ImGui.MenuItem("Assets")) {
                        AddWindow(new AssetSystemWindow());
                    }

                    if (ImGui.MenuItem("Scene ")) {
                        AddWindow(new GameSceneWindow());
                    }

                    if (ImGui.MenuItem("Entity inspector ")) {
                        AddWindow(new EntityWindow());
                    }

                    if (ImGui.MenuItem("Debug")) {
                        AddWindow(new DebugWindow());
                    }

                    ImGui.EndMenu();
                }

                ImGui.EndMainMenuBar();
            }
        }
    }
}
