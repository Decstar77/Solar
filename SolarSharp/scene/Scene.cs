using SolarSharp.Assets;
using SolarSharp.Rendering;
using SolarSharp.Rendering.Graph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;



namespace SolarSharp
{
    public struct EntityReference
    { 
        public static EntityReference Invalid { get { return new EntityReference(-1, -1, null); } }
        public int EntityIndex { get; set; }
        public int EntityGeneration { get; set; }
        [JsonIgnore] public GameScene Scene { get; set; }

        public EntityReference(int id, int gen, GameScene scene)
        {
            EntityIndex = id;
            EntityGeneration = gen;
            Scene = scene;
        }

        public Entity? GetEntity()
        {
            if (Scene != null)
            {
                Entity[] entities = Scene.GetAllEntities();
                foreach (Entity entity in entities)
                {
                    if (entity.Index == EntityIndex)
                        return entity;
                }
            }

            return null;
        }

        public static bool operator ==(EntityReference left, EntityReference right)
        {
            return left.EntityIndex == right.EntityIndex && left.EntityGeneration == right.EntityGeneration && left.Scene == right.Scene;
        }

        public static bool operator !=(EntityReference left, EntityReference right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj is EntityReference entityReference)
                return entityReference == this;
            return false;
        }
    }


    public struct MeshState
    {
        public Guid MeshId;
        public Guid MaterialId;
    }

    public class RenderingState
    {
        private Guid modelId;
        private List<MeshState> meshStates = new List<MeshState>();        
        private bool loadMeshes = false;

        public Guid GetModelId() => modelId;
        public List<MeshState> GetMeshStates() => meshStates;

        public void SetModel(Guid modelId)
        {
            this.modelId = modelId;
            loadMeshes = false;
            RetrieveModelMeshes();
        }

        public void SetModel(Guid modelId, List<MeshState> meshStates)
        {
            this.modelId= modelId;
            this.meshStates.Clear();
            this.meshStates.AddRange(meshStates);
            loadMeshes = true;
        }
        
        public void SetMeshMaterial(int meshIndex, Guid materialId)
        {
            meshStates[meshIndex] = new MeshState { MaterialId = materialId, MeshId = meshStates[meshIndex].MeshId };
        }

        public bool IsValid()
        {
            if (modelId == Guid.Empty) return false;
            if (!loadMeshes) RetrieveModelMeshes();
            return loadMeshes;
        }

        private void RetrieveModelMeshes()
        {
            ModelAsset modelAsset = AssetSystem.GetModelAsset(modelId);
            if (modelAsset != null)
            {
                foreach (MeshAsset mesh in modelAsset.meshes)
                {
                    Guid meshId = mesh.Guid;
                    Guid materialId = Guid.Empty;

                    MaterialAsset materialAsset = AssetSystem.GetMaterialAsset(mesh.materialName);
                    if (materialAsset != null)
                    {
                        materialId = materialAsset.Guid;
                    }
                    meshStates.Add(new MeshState { MeshId = meshId, MaterialId = materialId });
                }
                loadMeshes = true;
            }
        }
    }

    public class Entity
    {
        public string Name = "Untitled";         
        public int Index = -1;
        public int Generation = -1;        
        public Vector3 Position = Vector3.Zero;        
        public Quaternion Orientation = Quaternion.Identity;
        public Vector3 Scale = new Vector3(1, 1, 1);

        public EntityReference Reference { get { return new EntityReference(Index, Generation, Scene); } }
        public EntityReference Parent { get; set; } = EntityReference.Invalid;
        public List<EntityReference> Children { get; set; }
        public RenderingState RenderingState { get; set; } = new RenderingState();

        public AlignedBox WorldSpaceBoundingBox { get { return GetWorldBoundingBox(); } }
        //public AlignedBox LocalSpaceBoundingBox { get { return GetLocalBoundingBox(); } }
        public GameScene Scene { get; set; } = null;

        public Entity(int index, int generation, GameScene scene)
        {
            Index = index;
            Generation = generation;
            Scene = scene;
        }

        public Entity(FreeList.Element element, GameScene scene)
        {
            Index = element.idx;
            Generation = element.gen;
            Scene = scene;
        }

        //public Entity Clone()
        //{
        //    Entity entity = new Entity();
        //    entity.Id = Id;
        //    entity.Position = Position + Vector3.UnitX;
        //    entity.Orientation= Orientation;
        //    entity.Scale = Scale;                            
        //    entity.Name = Name + " Clone";
        //    entity.RenderingState.ModelId = RenderingState.ModelId;
        //
        //    //entity.RenderingState.Flags = RenderingState.Flags;
        //    //entity.RenderingState.AlbedoTexture = RenderingState.AlbedoTexture;
        //    //entity.RenderingState.NormalTexture = RenderingState.NormalTexture;
        //    //entity.RenderingState.ShaderId = RenderingState.ShaderId;
        //
        //    return entity;
        //}

        public EntityAsset CreateEntityAsset()
        {
            EntityAsset asset = new EntityAsset();
            asset.name = Name;
            asset.modelId = RenderingState.GetModelId();
            asset.meshStates = new List<MeshState>(RenderingState.GetMeshStates());
            asset.reference = Reference;
            asset.position = Position;
            asset.orientation = Orientation;
            asset.scale = Scale;
            asset.parent = Parent;
            asset.children = Children;
            
            return asset;
        }

        public void SetFromEntityAsset(EntityAsset asset)
        {
            Name = asset.name;
            RenderingState.SetModel(asset.modelId, asset.meshStates);            
            Position = asset.position;
            Orientation = asset.orientation;
            Scale = asset.scale;
            Parent = asset.parent;
            Children = asset.children;
        }

        public void SetTransform(Matrix4 m)
        {
            Matrix4.Decompose(m, out Position, out Orientation, out Scale);
        }

        public Vector3 GetForward() 
        {
            return Quaternion.ToBasis(Orientation).forward;
        }

        public Matrix4 ComputeTransformMatrix()
        {
            Matrix4 translation = Matrix4.TranslateRH(Matrix4.Identity, Position);
            Matrix4 rotation = Quaternion.ToMatrix4(Orientation);
            Matrix4 scaling = Matrix4.ScaleCardinal(Matrix4.Identity, Scale);

            Matrix4 transform = translation * rotation * scaling;

            return transform;
        }

        protected AlignedBox GetLocalBoundingBox()
        {
            //if (Material?.ModelId != null)
            //{
            //    if (Material.ModelId != Guid.Empty)
            //    {
            //        ModelAsset modelAsset = AssetSystem.GetModelAsset(Material.ModelId);
            //        if (modelAsset != null)
            //        {                        
            //            return modelAsset.alignedBox;
            //        }
            //    }
            //}

            return new AlignedBox();
        }

        protected AlignedBox GetWorldBoundingBox()
        {
            if (RenderingState != null && RenderingState.GetModelId() != Guid.Empty)
            {
                ModelAsset modelAsset = AssetSystem.GetModelAsset(RenderingState.GetModelId());
                if (modelAsset != null) {
                    return AlignedBox.Transform(modelAsset.alignedBox, Position, Orientation);
                }
            }

            return new AlignedBox();
        }
    }

    public class FreeList
    {
        public struct Element
        {
            public int idx;
            public int gen;
        }

        public int Count { get; set; }        
        private int[] generations;
        private Stack<int> freeList;

        public FreeList(int size)
        {
            freeList = new Stack<int>(size);
            generations = new int[size];
            Clear();
        }

        public Element GetNext()
        {
            Count++;
            int index = freeList.Pop();
            int generation = ++generations[index];
            return new Element { idx = index, gen = generation };
        }

        public void Remove(int index)
        {
            Count--;
            freeList.Push(index);
        }

        public void Clear()
        {
            freeList.Clear();
            CreateFreeListIndices();
            Count = 0;
            Array.Fill(generations, 0);
        }

        private void CreateFreeListIndices()
        {
            for (int i = generations.Length - 1; i >= 0; i--)
            {
                freeList.Push(i);
            }
        }
    }



    public class GameScene
    {
        public string name = "";
        public string path = "";

        public Camera Camera { get { return camera; } set { camera = value; } }
        private Camera camera = new Camera();

        public static readonly int MaxEntityCount = 100000;
        
        private FreeList freeList = new FreeList(MaxEntityCount);
        private Entity[] entities = new Entity[MaxEntityCount];
        private RenderPacket packet = new RenderPacket();

        public int EntityCount { get { return freeList.Count; } }

        public GameScene()
        {
        }

        public GameScene(string name)
        {
            this.name = name;
        }

        public Entity CreateEntity()
        {            
            Entity entity = new Entity(freeList.GetNext(), this);
            Debug.Assert(entities[entity.Index] == null);
            entities[entity.Index] = entity;
            return entity;
        }

        public Entity CreateEntity(EntityAsset entityAsset)
        {
            Entity entity = CreateEntity();
            entity.SetFromEntityAsset(entityAsset);

            return entity;
        }

        public GameScene(SceneAsset sceneAsset)
        {
            if (sceneAsset != null)
            {
                name = sceneAsset.name;
                sceneAsset.entities?.ForEach(e => CreateEntity(e));
            }
            else
            {
                name = "Unknown_scene";
            }
        }

        public void DestroyEntity(EntityReference entityReference)
        {
            if (entityReference != EntityReference.Invalid)
            {
                freeList.Remove(entityReference.EntityIndex);
                entities[entityReference.EntityIndex] = null;
            }            
        }

        public void DestroyAllEntities() 
        {
            freeList.Clear();
            Array.Fill(entities, null);
        }

        public SceneAsset CreateSceneAsset()
        {
            SceneAsset sceneAsset = new SceneAsset();
            sceneAsset.name = name;
            sceneAsset.entities = entities.Where(x => x != null).Select(x => x.CreateEntityAsset()).ToList();

            return sceneAsset;
        }

        public RenderPacket CreateRenderPacket()
        {
            packet.renderPacketEntries.Clear();
            packet.cameraPosition = camera.Position;
            packet.projectionMatrix = camera.GetProjectionMatrix();
            packet.viewMatrix = camera.GetViewMatrix();

            foreach(Entity entity in entities)
            {
                if (entity != null && entity.RenderingState.IsValid()) 
                {
                    foreach (MeshState state in entity.RenderingState.GetMeshStates())
                    {
                        packet.renderPacketEntries.Add(new RenderPacketEntry
                        {
                            meshId = state.MeshId,
                            materialId = state.MaterialId,
                            transform = entity.ComputeTransformMatrix()
                        });
                    }
                }
            }

            return packet;
        }

        public Entity[] GetAllEntities() => entities.Where(x => x != null).ToArray();

        public RenderGraph RenderGraph { get { return renderGraph; } set { renderGraph?.Shutdown(); renderGraph = value; } }
        private RenderGraph renderGraph = null;
    }
}
