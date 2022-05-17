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
* Base Rendering:
*   - Pack material index into vertex data
*   - All material data does into a const buffer
*   - Remove asset system lookup
* Base Lighting:
*   - PBR
*   - IBL
*   - SHADOWS
*   - SSAO
* Mouse picking
* Clip board
* Selection indication - draw bb
* Gizmo for multple entities
* 
*/

namespace SolarEditor
{
    internal class Selection
    {
        public IReadOnlyList<EntityReference> SelectedEntities { get { return currentSelected; } }
        private List<EntityReference> currentSelected = new List<EntityReference>();
        private UndoSystem undoSystem;

        public Selection(UndoSystem undoSystem)
        {
            this.undoSystem = undoSystem;
        }

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

                undoSystem.Add(new SelectionUndoAction(this, lastSelection, newSelection));
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

                    undoSystem.Add(new SelectionUndoAction(this, lastSelection, newSelection));
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


            if (entities.Count > 0)
            {
                if (undoable)
                {
                    List<EntityReference> lastSelection = new List<EntityReference>(currentSelected);
                    currentSelected.Clear();
                    currentSelected.AddRange(entities);
                    List<EntityReference> newSelection = new List<EntityReference>(currentSelected);

                    undoSystem.Add(new SelectionUndoAction(this, lastSelection, newSelection));
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

                undoSystem.Add(new SelectionUndoAction(this, lastSelection, newSelection));
            }
            else
            {
                currentSelected.Clear();
            }
        }
    }

    internal class ClipBoard
    { 
        public List<EntityAsset> copiedEntities = new List<EntityAsset>();
    }


    internal class EditorContext
    {
        public UndoSystem undoSystem;
        public Selection selection; 
        public FlyCamera camera;
        public GameScene scene;

        public EditorContext()
        {
            undoSystem = new UndoSystem();
            selection = new Selection(undoSystem);
            camera = new FlyCamera();
        }

        public void SetScene(GameScene scene)
        {
            this.scene = scene;
            scene.Camera = this.camera;
        }

        public void AddEntities(params Entity[] newEntities)
        {
            selection.Set(newEntities.Select(x => x.Reference).ToList(), false);
            undoSystem.Add(new CreateEntityUndoAction(scene, selection, newEntities.ToList()));
        }
    }


    internal class EditorState
    {
        private Gizmo gizmo = new Gizmo();
        private ClipBoard clipboard = new ClipBoard();
        private List<EditorWindow> windows = new List<EditorWindow>();
        private List<EditorWindow> newWindows = new List<EditorWindow>();
        

        public bool ShowBoundingBoxes = false;
        public bool ShowEmpties = false;

        public AirGame airGame = new AirGame();

        public EditorContext currentContext = new EditorContext();
        public EditorContext workingContext = new EditorContext();
        public EditorContext paletteContext = new EditorContext();        

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

            workingContext.SetScene(new GameScene(AssetSystem.LoadGameSceneAsset(Application.Config.AssetPath + "NewScene.json")));
            currentContext = workingContext;

            paletteContext.SetScene(new GameScene(AssetSystem.LoadGameSceneAsset(Application.Config.AssetPath + "Palette.json")));

            Logger.Info("Editor startup complete");
        }

        internal GameScene Update()
        {
            ImGui.BeginFrame();

            if (!currentContext.camera.Operate())
            {
                DrawGlobalMenu();
                ShowWindows();

                if (Input.IskeyJustDown(KeyCode.S) && Input.IsKeyDown(KeyCode.CTRL_L))
                {
                    if (!EventSystem.Fire(EventType.ON_SAVE, null))
                    {
                        AssetSystem.SaveGameSceneAsset(Application.Config.AssetPath, currentContext.scene.CreateSceneAsset());
                    }
                }

                bool usingGizmo = gizmo.Operate(currentContext.camera, 
                    currentContext.selection.SelectedEntities.Select(x => x.GetEntity()).ToList(), 
                    currentContext.undoSystem);

                if (!ImGui.IsWindowHovered(ImGuiHoveredFlags.AnyWindow) && !ImGui.IsAnyItemHovered() && !usingGizmo)
                {                    
                    if (Input.IsMouseButtonJustDown(MouseButton.MOUSE1))
                    {
                        Ray ray = currentContext.camera.ShootRayFromMousePos();
        
                        Entity? selectedEntity = null;
                        float minDist = float.MaxValue;
                        foreach (var entity in currentContext.scene.GetAllEntities())
                        {
                            RaycastInfo info;

                            if (Raycast.AlignedBox(ray, entity.WorldSpaceBoundingBox, out info))
                            {
                                if (info.t > minDist)
                                    continue;

                                if (entity.RenderingState != null && entity.RenderingState.ModelId != Guid.Empty)
                                {
                                    ModelAsset? modelAsset = AssetSystem.GetModelAsset(entity.RenderingState.ModelId);
                                    if (modelAsset != null)
                                    {
                                        Matrix4 transformMatrix = entity.ComputeTransformMatrix();
                                        foreach (MeshAsset meshAsset in modelAsset.meshes)
                                        {
                                            List<Triangle> triangles = meshAsset.BuildTriangles();
                                            foreach (Triangle triangle in triangles)
                                            {
                                                if (Raycast.Triangle(ray, Triangle.Transform(triangle, transformMatrix), out info))
                                                {
                                                    if (info.t < minDist)
                                                    {
                                                        minDist = info.t;
                                                        selectedEntity = entity;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }  
                            }
                        }

                        if (selectedEntity != null)
                        {
                            if (Input.IsKeyDown(KeyCode.SHIFT_L))
                            {
                                currentContext.selection.Add(selectedEntity, true);
                            }
                            else
                            {
                                currentContext.selection.Set(selectedEntity, true);
                            }
                        }
                        else
                        {
                            currentContext.selection.Clear(true);                            
                        }
                    }
                }

                if (!ImGui.WantCaptureKeyboard())
                {
                    if (Input.IsKeyDown(KeyCode.SHIFT_L) && Input.IskeyJustDown(KeyCode.A))
                    {
                        Entity entity = currentContext.scene.CreateEntity();
                        currentContext.selection.Set(entity, false);
                        AddWindow(new EntityWindow());

                        currentContext.undoSystem.Add(
                            new CreateEntityUndoAction(currentContext.scene, 
                            currentContext.selection, 
                            currentContext.selection.GetValidEntities()));
                    }

                    if (Input.IskeyJustDown(KeyCode.DEL))
                    {
                        if (currentContext.selection.SelectedEntities.Count > 0)
                        {
                            currentContext.undoSystem.Add(
                                new DeleteEntityUndoAction(currentContext.scene, 
                                currentContext.selection, 
                                currentContext.selection.GetValidEntities()));

                            foreach (EntityReference entityReference in currentContext.selection.SelectedEntities)
                            {
                                currentContext.scene.DestroyEntity(entityReference);
                            }
                            currentContext.selection.Clear(false);
                        }                       
                    }

                    if (Input.IsKeyDown(KeyCode.SHIFT_L) && Input.IskeyJustDown(KeyCode.D))
                    {
                        if (currentContext.selection.SelectedEntities.Count > 0)
                        {
                            List<Entity> newEntities = new List<Entity>();
                            foreach (EntityReference entityReference in currentContext.selection.SelectedEntities)
                            {
                                Entity? entity = entityReference.GetEntity();
                                if (entity != null)
                                {
                                    Entity newEntity = currentContext.scene.CreateEntity(entity.CreateEntityAsset());
                                    newEntity.Position += Vector3.UnitX; 
                                    newEntities.Add(newEntity);
                                }
                            }

                            currentContext.AddEntities(newEntities.ToArray());
                        }
                    }


                    if (Input.IsKeyDown(KeyCode.CTRL_L) && Input.IsKeyDown(KeyCode.SHIFT_L) && Input.IskeyJustDown(KeyCode.Z))
                    {
                        currentContext.undoSystem.Redo();
                    }
                    else if (Input.IsKeyDown(KeyCode.CTRL_L) && Input.IskeyJustDown(KeyCode.Z))
                    {
                        currentContext.undoSystem.Undo();
                    }

                    if (Input.IsKeyDown(KeyCode.CTRL_L) && Input.IskeyJustDown(KeyCode.C)) 
                    {
                        if (currentContext.selection.SelectedEntities.Count > 0)
                        {
                            Logger.Trace($"Copied {currentContext.selection.SelectedEntities.Count } entties");
                            clipboard.copiedEntities = currentContext.selection.GetValidEntities().Select(x => x.CreateEntityAsset()).ToList();
                        }
                    }

                    if (Input.IsKeyDown(KeyCode.CTRL_L) && Input.IskeyJustDown(KeyCode.V))
                    {
                        //Logger.Trace($"Copied {currentContext.selection.SelectedEntities.Count } entties");
                        if (clipboard.copiedEntities.Count > 0)
                        {
                            List<Entity> newEntities = new List<Entity>(); 

                            foreach (var entity in clipboard.copiedEntities)
                            {
                                Entity e = currentContext.scene.CreateEntity(entity);
                                e.Position += Vector3.UnitX;
                                newEntities.Add(e);
                            }

                            currentContext.AddEntities(newEntities.ToArray());
                        }
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
                    else if (Input.IskeyJustDown(KeyCode.F9))
                    {
                        AddWindow(new LevelGeneratorWindow());
                    }
                    else if (Input.IskeyJustDown(KeyCode.ESCAPE))
                    {
                        CloseAllWindows();
                    }
                    else if (Input.IskeyJustDown(KeyCode.TAB))
                    {
                        if (currentContext == workingContext)
                        {
                            if (paletteContext.scene.EntityCount == 0) {
                                CreatePaletteScene();
                            }
                            currentContext = paletteContext;
                        }
                        else
                        {
                            currentContext = workingContext;
                        }
                    }
                }

                currentContext.scene.GetAllEntities().ToList().ForEach(entity =>
                {
                    if (ShowBoundingBoxes)
                    {

                        //DebugDraw.AlignedBox(entity.WorldSpaceBoundingBox);
                    }

                    if (ShowEmpties)
                    {
                        //if (entity.Material.ModelId == Guid.Empty) {
                        //    DebugDraw.Point(entity.Position);s
                        //}
                    }
                });
               
                foreach (EntityReference reff in currentContext.selection.SelectedEntities)
                {
                    Entity? entity = reff.GetEntity();
                    if (entity != null)
                        DebugDraw.AlignedBox(entity.WorldSpaceBoundingBox);
                }

            }

        

            return currentContext.scene;
        }

        internal void CreatePaletteScene()
        {
            paletteContext.scene.DestroyAllEntities();
            float x = -30;
            float z = -30;

            AssetSystem.GetSortedModelAssets().ForEach(asset => {
                AlignedBox ab = asset.alignedBox;
                //float width = ab.Extent.x * 2;
                x += 5;

                Entity entity = paletteContext.scene.CreateEntity();
                entity.RenderingState.ModelId = asset.Guid;
                entity.Position += new Vector3(x, 0, z);
                entity.Name = asset.name;
                if (x > 30)
                {
                    z += 5;
                    x = -30;
                }
            });
        }

        internal void Shutdown()
        {

        }

        internal void UIDraw()
        {
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

                    if (ImGui.MenuItem("Level Generator", "F9")) {
                        AddWindow(new LevelGeneratorWindow());
                    }

                    ImGui.EndMenu();
                }

                ImGui.EndMainMenuBar();
            }
        }
    }
}
