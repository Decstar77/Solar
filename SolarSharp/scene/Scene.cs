using SolarSharp.Assets;
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


    public class RenderingState
    {
        public Guid ModelId { get; set; }
        public string MaterialReference { get; set; }
    }

    public class Entity
    {
        public int Index { get; set; } = -1;
        public int Generation { get; set; } = -1;

        public string Name = "Untitled";

        protected Vector3 position = Vector3.Zero;
        public Vector3 Position { get { return position; } set { position = value; } }

        protected Quaternion orientation = Quaternion.Identity;
        public Quaternion Orientation { get { return orientation; } set { orientation = value; } }

        protected Vector3 scale = new Vector3(1, 1, 1);
        public Vector3 Scale { get { return scale; } set { scale = value; } }
        
        public EntityReference Reference { get { return new EntityReference(Index, Generation, Scene); } }
        public RenderingState RenderingState { get; set; } = new RenderingState();

        public AlignedBox WorldSpaceBoundingBox { get { return GetWorldBoundingBox(); } }
        public AlignedBox LocalSpaceBoundingBox { get { return GetLocalBoundingBox(); } }
        public GameScene Scene { get; set; } = null;

        public EntityReference Parent { get; set; } = EntityReference.Invalid;
        public List<EntityReference> Children { get; set; }

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
            asset.renderingState = new RenderingState();
            asset.renderingState.ModelId = RenderingState.ModelId;
            asset.renderingState.MaterialReference = RenderingState.MaterialReference;
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
            RenderingState.ModelId = asset.renderingState.ModelId;
            RenderingState.MaterialReference = asset.renderingState.MaterialReference;
            Position = asset.position;
            Orientation = asset.orientation;
            Scale = asset.scale;
            Parent = asset.parent;
            Children = asset.children;
        }
        
        public void SetTransform(Matrix4 m) => Matrix4.Decompose(m, out position, out orientation, out scale);

        public Matrix4 ComputeModelMatrix()
        {
            Matrix4 translation = Matrix4.TranslateRH(Matrix4.Identity, position);
            Matrix4 rotation = Quaternion.ToMatrix4(orientation);
            Matrix4 scaling = Matrix4.ScaleCardinal(Matrix4.Identity, scale);

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
            //if (Material?.ModelId != null)
            //{
            //    if (Material.ModelId != Guid.Empty)
            //    {
            //        ModelAsset modelAsset = AssetSystem.GetModelAsset(Material.ModelId);
            //        if (modelAsset != null)
            //        {
            //            return AlignedBox.Transform(modelAsset.alignedBox, position, Orientation);
            //        }
            //    }
            //}

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
            Count = 0;
            CreateFreeListIndices();
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
        
        private FreeList freeList = new FreeList(1000);
        private Entity[] entities = new Entity[1000];

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
            
        }

        public SceneAsset CreateSceneAsset()
        {
            SceneAsset sceneAsset = new SceneAsset();
            sceneAsset.name = name;
            sceneAsset.entities = entities.Where(x => x != null).Select(x => x.CreateEntityAsset()).ToList();

            return sceneAsset;
        }

        public Entity[] GetAllEntities() => entities.Where(x => x != null).ToArray();

        public RenderGraph RenderGraph { get { return renderGraph; } set { renderGraph?.Shutdown(); renderGraph = value; } }
        private RenderGraph renderGraph = null;
    }
}
