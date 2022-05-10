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

/*
@TODO:
* Console
* Selection undo redo.
* Entity Creation
* Entity Deletion
 */

namespace SolarEditor
{
    internal class Selection
    {
        public List<EntityReference> SelectedEntities = new List<EntityReference>();

        internal void Add(Entity selectedEntity)
        {
            SelectedEntities.Add(selectedEntity.Reference);
        }

        internal void Set(Entity selectedEntity)
        {
            SelectedEntities.Clear();
            SelectedEntities.Add(selectedEntity.Reference);
        }

        internal void Clear() => SelectedEntities.Clear();
    }


    internal class EditorState
    {
        private List<EditorWindow> windows = new List<EditorWindow>();
        private List<EditorWindow> newWindows = new List<EditorWindow>();
        private FlyCamera camera = new FlyCamera();
        private Gizmo gizmo = new Gizmo();

        public Selection selection = new Selection();

        public bool ShowBoundingBoxes = false;
        public bool ShowEmpties = false;

        internal EditorState()
        {
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
            
            Entity entity = gameScene.CreateEntity();
            entity.Name = "Bike";
            entity.Material = new Material();
            entity.Material.ModelId = Guid.Parse("10209316-f57b-41f7-88cf-8ea81614bdb2");
            entity.Material.AlbedoTexture = Guid.Parse("5b767a8f-5e2e-428d-81d2-8b350bf556fc");
            entity.Position = new Vector3(0, 0, -3);
            //entity.Orientation = Quaternion.RotateLocalZ(Quaternion.Identity, Util.DegToRad(45.0f));
            
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
                if (!EventSystem.Fire(EventType.ON_SAVE, null)) {
                    AssetSystem.SaveGameSceneAsset(Application.Config.AssetPath + "scene", GameSystem.CurrentScene);
                }
            }

            if (!ImGui.IsWindowHovered(ImGuiHoveredFlags.AnyWindow) && !ImGui.IsAnyItemHovered())
            {
                if (selection.SelectedEntities.Count > 0 && gizmo.Operate(camera, selection.SelectedEntities[0].GetEntity() ))
                {
                }
                else if (Input.IsMouseButtonJustDown(MouseButton.MOUSE1))
                {
                    Ray ray = camera.ShootRayFromMousePos();
                    Entity? selectedEntity = null;
                    float minDist = float.MaxValue;
                    foreach (var entity in GameSystem.CurrentScene.GetAllEntities()) {
                        RaycastInfo info;
                        if (Raycast.AlignedBox(ray, entity.WorldSpaceBoundingBox, out info)) {
                            if (info.t < minDist)
                            {
                                minDist = info.t;
                                selectedEntity = entity;
                            }
                        }
                    }

                    if (selectedEntity != null)
                    {
                        selection.Set(selectedEntity);
                    }
                }
            }

            if (!ImGui.WantCaptureKeyboard())
            {
                if (Input.IsKeyDown(KeyCode.SHIFT_L) && Input.IskeyJustDown(KeyCode.A))
                {
                    Entity entity = GameSystem.CurrentScene.CreateEntity();
                    selection.Set((Entity)entity);
                    AddWindow(new EntityWindow());
                }

                if (Input.IsKeyDown(KeyCode.SHIFT_L) && Input.IskeyJustDown(KeyCode.D))
                {
                    //if (selectedEntities.Count > 0)
                    //{
                    //    List<Entity> newEntities = new List<Entity>();
                    //    for (int i = 0; i < selectedEntities.Count; i++)
                    //    {
                    //        Entity entity = GameSystem.CurrentScene.CreateEntity();
                    //        entity.Position = selectedEntities[i].Position + Vector3.UnitX;
                    //        entity.Orientation= selectedEntities[i].Orientation;
                    //        entity.Scale = selectedEntities[i].Scale;                            
                    //        entity.Name = selectedEntities[i].Name + " Clone";
                    //        entity.Material.Flags = selectedEntities[i].Material.Flags;
                    //        entity.Material.ModelId = selectedEntities[i].Material.ModelId;
                    //        entity.Material.AlbedoTexture = selectedEntities[i].Material.AlbedoTexture;
                    //        entity.Material.NormalTexture = selectedEntities[i].Material.NormalTexture;
                    //        entity.Material.ShaderId = selectedEntities[i].Material.ShaderId;

                    //        newEntities.Add(entity);
                    //    }

                    //    selectedEntities.Clear();
                    //    selectedEntities.AddRange(newEntities); 
                    //}
                }

                if (Input.IskeyJustDown(KeyCode.DEL))
                {
                    //selectedEntities.ForEach(x => GameSystem.CurrentScene.DeleteEntity(x.Id));
                    //selectedEntities.Clear();
                }

                if (Input.IsKeyDown(KeyCode.CTRL_L) && Input.IsKeyDown(KeyCode.SHIFT_L) && Input.IskeyJustDown(KeyCode.Z))
                {
                    UndoSystem.Redo();
                }
                else if (Input.IsKeyDown(KeyCode.CTRL_L) && Input.IskeyJustDown(KeyCode.Z))
                {
                    UndoSystem.Undo();
                }
            }
            
            GameSystem.CurrentScene.GetAllEntities().ToList().ForEach(entity => {
                if (ShowBoundingBoxes) {
                    DebugDraw.AlignedBox(entity.WorldSpaceBoundingBox);
                }

                if (ShowEmpties) {
                    if (entity.Material.ModelId == Guid.Empty) {
                        DebugDraw.Point(entity.Position);
                    }
                }

            });
            

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
