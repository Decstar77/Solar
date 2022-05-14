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
using PlaneGame;

/*
@TODO:
* Entity Creation
* Entity Deletion
* Console Scroll to bottom
* Undo redo can be a bit sticky
 */

namespace SolarEditor
{
    internal class Selection
    {
        public IReadOnlyList<EntityReference> SelectedEntities { get { return currentSelected; } }
        private List<EntityReference> currentSelected = new List<EntityReference>();        

        public List<Entity> GetValidEntities()
        {
            List<Entity> entities = new List<Entity>();
            foreach (EntityReference entity in SelectedEntities)
            {
                Entity? e = entity.GetEntity();
                if (e != null)
                {
                    entities.Add(e);
                }
            }

            return entities;
        }

        internal void Add(Entity selectedEntity, bool undoable)
        {
            if (undoable)
            {
                List<EntityReference> lastSelection = new List<EntityReference>(currentSelected);
                currentSelected.Add(selectedEntity.Reference);
                List<EntityReference> newSelection = new List<EntityReference>(currentSelected);

                UndoSystem.Add(new SelectionUndoAction(this, lastSelection, newSelection));
            }
            else
            {                
                currentSelected.Add(selectedEntity.Reference);
            }                
        }

        internal void Set(Entity selectedEntity, bool undoable)
        {
            if (!currentSelected.Contains(selectedEntity.Reference))
            {
                if (undoable)
                {
                    List<EntityReference> lastSelection = new List<EntityReference>(currentSelected);
                    currentSelected.Clear();
                    currentSelected.Add(selectedEntity.Reference);
                    List<EntityReference> newSelection = new List<EntityReference>(currentSelected);

                    UndoSystem.Add(new SelectionUndoAction(this, lastSelection, newSelection));
                }
                else
                {
                    currentSelected.Clear();
                    currentSelected.Add(selectedEntity.Reference);
                }
            }
        }

        internal void Set(List<EntityReference> entities, bool undoable)
        {
            if (entities.Count == 0)
            {
                Clear(undoable);
                return;
            }

            entities = entities.Except(SelectedEntities).ToList();
            if (entities.Count > 0)
            {
                if (undoable)
                {
                    List<EntityReference> lastSelection = new List<EntityReference>(currentSelected);
                    currentSelected.Clear();
                    currentSelected.AddRange(entities);
                    List<EntityReference> newSelection = new List<EntityReference>(currentSelected);

                    UndoSystem.Add(new SelectionUndoAction(this, lastSelection, newSelection));
                }
                else
                {
                    currentSelected.Clear();
                    currentSelected.AddRange(entities);
                }
            }
        }

        internal void Clear(bool undoable) 
        {
            if (undoable)
            {
                List<EntityReference> lastSelection = new List<EntityReference>(currentSelected);
                currentSelected.Clear();
                List<EntityReference> newSelection = new List<EntityReference>(currentSelected);

                UndoSystem.Add(new SelectionUndoAction(this, lastSelection, newSelection));
            }
            else
            {
                currentSelected.Clear();
            }
        }
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

        public AirGame airGame = new AirGame();

        public GameScene workingScene = new GameScene("NewScene");
        public GameScene paletteScene = new GameScene("Palette");
        public GameScene displayScene;

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
                    ModelImporter modelImporter = new ModelImporter(x);
                    if (modelImporter.Loaded)
                    {
                        ModelAsset modelAsset = modelImporter.LoadModel();
                        modelAsset.Guid = metaFileAsset.Guid;

                        List<MaterialAsset> materials = modelImporter.LoadMaterials();

                        AssetSystem.AddModelAsset(modelAsset);
                        AssetSystem.AddMaterialAssets(materials);
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

            workingScene = new GameScene(AssetSystem.LoadGameSceneAsset(Application.Config.AssetPath + "NewScene.json"));
            workingScene.Camera = camera;

            displayScene = workingScene;

            Logger.Info("Editor startup complete");
        }        

        private bool newModelDialog = false;
        private ModelAsset? newModelAsset = null;

        internal GameScene Update()
        {
            ImGui.BeginFrame();

            if (!camera.Operate())
            {
                DrawGlobalMenu();
                ShowWindows();

                if (Input.IskeyJustDown(KeyCode.S) && Input.IsKeyDown(KeyCode.CTRL_L))
                {
                    if (!EventSystem.Fire(EventType.ON_SAVE, null))
                    {
                        AssetSystem.SaveGameSceneAsset(Application.Config.AssetPath, displayScene.CreateSceneAsset());
                    }
                }

                gizmo.Operate(camera, selection.SelectedEntities.Select(x => x.GetEntity()).ToList());

                if (!ImGui.IsWindowHovered(ImGuiHoveredFlags.AnyWindow) && !ImGui.IsAnyItemHovered())
                {                    
                    if (Input.IsMouseButtonJustDown(MouseButton.MOUSE1))
                    {
                        Ray ray = camera.ShootRayFromMousePos();
                        Entity? selectedEntity = null;
                        float minDist = float.MaxValue;
                        foreach (var entity in displayScene.GetAllEntities())
                        {
                            RaycastInfo info;
                            if (Raycast.AlignedBox(ray, entity.WorldSpaceBoundingBox, out info))
                            {
                                if (info.t < minDist)
                                {
                                    minDist = info.t;
                                    selectedEntity = entity;
                                }
                            }
                        }

                        if (selectedEntity != null)
                        {
                            //selection.Set(selectedEntity);
                        }
                    }
                }

                if (!ImGui.WantCaptureKeyboard())
                {
                    if (Input.IsKeyDown(KeyCode.SHIFT_L) && Input.IskeyJustDown(KeyCode.A))
                    {
                        Entity entity = displayScene.CreateEntity();
                        selection.Set(entity, false);
                        AddWindow(new EntityWindow());

                        UndoSystem.Add(new CreateEntityUndoAction(displayScene, selection, selection.GetValidEntities() ));
                    }

                    if (Input.IskeyJustDown(KeyCode.DEL))
                    {
                        if (selection.SelectedEntities.Count > 0)
                        {
                            UndoSystem.Add(new DeleteEntityUndoAction(displayScene, selection, selection.GetValidEntities()));

                            foreach (EntityReference entityReference in selection.SelectedEntities)
                            {
                                displayScene.DestroyEntity(entityReference);
                            }
                            selection.Clear(false);
                        }                       
                    }

                    if (Input.IsKeyDown(KeyCode.SHIFT_L) && Input.IskeyJustDown(KeyCode.D))
                    {
                        if (selection.SelectedEntities.Count > 0)
                        {
                            List<Entity> newEntities = new List<Entity>();
                            foreach (EntityReference entityReference in selection.SelectedEntities)
                            {
                                Entity? entity = entityReference.GetEntity();
                                if (entity != null)
                                {
                                    Entity newEntity = displayScene.CreateEntity(entity.CreateEntityAsset());
                                    newEntity.Position += Vector3.UnitX; 
                                    newEntities.Add(newEntity);
                                }
                            }

                            selection.Set(newEntities.Select(x=> x.Reference).ToList(), false);

                            UndoSystem.Add(new CreateEntityUndoAction(displayScene, selection, newEntities));
                        }
                    }


                    if (Input.IsKeyDown(KeyCode.CTRL_L) && Input.IsKeyDown(KeyCode.SHIFT_L) && Input.IskeyJustDown(KeyCode.Z))
                    {
                        UndoSystem.Redo();
                    }
                    else if (Input.IsKeyDown(KeyCode.CTRL_L) && Input.IskeyJustDown(KeyCode.Z))
                    {
                        UndoSystem.Undo();
                    }

                    if (Input.IskeyJustDown(KeyCode.F1))
                    {
                        if (!HasWindow(typeof(ConsoleWindow)))
                        {
                            AddWindow(new ConsoleWindow());
                        }
                        else
                        {
                            GetWindow<ConsoleWindow>()?.Close();
                        }
                    }
                    else if (Input.IskeyJustDown(KeyCode.F2))
                    {
                        AddWindow(new ShaderEditorWindow(null));
                    }
                    else if (Input.IskeyJustDown(KeyCode.F3))
                    {
                        AddWindow(new RenderGraphWindow());
                    }
                    else if (Input.IskeyJustDown(KeyCode.F4))
                    {
                        AddWindow(new AssetSystemWindow());
                    }
                    else if (Input.IskeyJustDown(KeyCode.F6))
                    {
                        AddWindow(new GameSceneWindow());
                    }
                    else if (Input.IskeyJustDown(KeyCode.F7))
                    {
                        AddWindow(new EntityWindow());
                    }
                    else if (Input.IskeyJustDown(KeyCode.F8))
                    {
                        AddWindow(new DebugWindow());
                    }
                    else if (Input.IskeyJustDown(KeyCode.ESCAPE))
                    {
                        CloseAllWindows();
                    }
                    else if (Input.IskeyJustDown(KeyCode.TAB))
                    {
                        if (displayScene == paletteScene)
                        { 
                            displayScene = workingScene;
                            UndoSystem.Enabled = true;
                        }
                        else
                        {
                            displayScene = paletteScene;
                            if (paletteScene.EntityCount == 0) {

                            }
                            UndoSystem.Enabled = false;
                        }
                        
                    }
                }

                displayScene.GetAllEntities().ToList().ForEach(entity =>
                {
                    if (ShowBoundingBoxes)
                    {
                        DebugDraw.AlignedBox(entity.WorldSpaceBoundingBox);
                    }

                    if (ShowEmpties)
                    {
                        //if (entity.Material.ModelId == Guid.Empty) {
                        //    DebugDraw.Point(entity.Position);
                        //}
                    }
                });

               
            }

            return displayScene;
        }

        internal void CreatePaletteScene()
        {
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

        internal T AddWindow<T>(T window) where T : EditorWindow
        {
            foreach (EditorWindow w in windows)
                if (w.GetType() == window.GetType())
                    return (T)w;
            
            newWindows.Add(window);
            return window;
        }

        internal T? GetWindow<T>() where T : EditorWindow
        {
            Type windowType = typeof(T);

            foreach (EditorWindow w in windows)
                if (w.GetType() == windowType)
                    return (T)w;
            return null;
        }

        internal bool HasWindow(Type window)
        {
            foreach (EditorWindow w in windows)
                if (w.GetType() == window)
                    return true;
            return false;
        }

        internal void CloseAllWindows()
        {
            foreach (EditorWindow w in windows)
                w.Close();
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
                    if (ImGui.MenuItem("Console", "F1")) {
                        AddWindow(new ConsoleWindow());
                    }

                    if (ImGui.MenuItem("Shader Editor", "F2")) {
                        AddWindow(new ShaderEditorWindow(null));
                    }

                    if (ImGui.MenuItem("Render graph", "F3")) {
                        AddWindow(new RenderGraphWindow());
                    }

                    if (ImGui.MenuItem("Assets", "F4")) {
                        AddWindow(new AssetSystemWindow());
                    }

                    if (ImGui.MenuItem("Scene", "F6")) {
                        AddWindow(new GameSceneWindow());
                    }

                    if (ImGui.MenuItem("Entity inspector", "F7")) {
                        AddWindow(new EntityWindow());
                    }

                    if (ImGui.MenuItem("Debug", "F8")) {
                        AddWindow(new DebugWindow());
                    }

                    ImGui.EndMenu();
                }

                ImGui.EndMainMenuBar();
            }
        }
    }
}
