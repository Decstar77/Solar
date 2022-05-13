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
        public static EntityReference Invalid { get { return new EntityReference(-1); } }
        public int EntityId { get; set; }

        public EntityReference(int id)
        {
            EntityId = id;
        }

        public Entity? GetEntity()
        {
            Entity[] entities = GameSystem.CurrentScene.GetAllEntities();
            foreach (Entity entity in entities)
            {
                if (entity.Id == EntityId)
                    return entity;
            }

            return null;
        }

        public static bool operator ==(EntityReference left, EntityReference right)
        {
            return left.EntityId == right.EntityId;
        }

        public static bool operator !=(EntityReference left, EntityReference right)
        {
            return left.EntityId != right.EntityId;
        }

        public override bool Equals(object obj)
        {
            if (obj is EntityReference entityReference)
                return entityReference.EntityId == EntityId;
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
        public int Id { get; set; } = -1;

        [JsonInclude]
        public string Name = "Untitled";

        protected Vector3 position = Vector3.Zero;
        public Vector3 Position { get { return position; } set { position = value; } }

        protected Quaternion orientation = Quaternion.Identity;
        public Quaternion Orientation { get { return orientation; } set { orientation = value; } }

        protected Vector3 scale = new Vector3(1, 1, 1);
        public Vector3 Scale { get { return scale; } set { scale = value; } }
        
        public EntityReference Reference { get { return new EntityReference(Id); } }
        public RenderingState RenderingState { get; set; } = new RenderingState();

        [JsonIgnore]
        public AlignedBox WorldSpaceBoundingBox { get { return GetWorldBoundingBox(); } }

        [JsonIgnore]
        public AlignedBox LocalSpaceBoundingBox { get { return GetLocalBoundingBox(); } }

        public EntityReference Parent { get; set; }
        public List<EntityReference> Children { get; set; }

        public Entity()
        {
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


    public class FreeList<T> where T : class
    {
        public int Count { get; set; }  
                
        private T[] data;
        private Stack<int> freeList;

        public FreeList(int size)
        {
            freeList = new Stack<int>(size);
            data = new T[size];
            Count = 0;
            
            for (int i = size - 1; i >= 0; i--) {
                freeList.Push(i);
            }
            
        }

        public int GetNextFreeIndex() => freeList.Peek();
        public T[] GetValues() => data;

        public void Add(T t)
        {
            data[freeList.Pop()] = t;
            Count++;
        }

        public void Remove(int index)
        {
            freeList.Push(index);
            Count--;
            data[index] = null;
        }
    }



    public class GameScene
    {
        public string name = "";
        public string path = "";

        public Camera Camera { get { return camera; } set { camera = value; } }
        private Camera camera = new Camera();
        
        private FreeList<Entity> entities = new FreeList<Entity>(1000);

        public GameScene()
        {
        }

        public GameScene(SceneAsset sceneAsset)
        {
            if (sceneAsset != null)
            {
                name = sceneAsset.name;
                sceneAsset.entities?.ForEach(e => { e.Id = entities.GetNextFreeIndex(); PlaceEntity(e); });
            }            
        }

        public Entity CreateEntity()
        {
            Entity entity = new Entity();
            entity.Id = entities.GetNextFreeIndex();
            entities.Add(entity);
            return entity;
        }

        public Entity CreateEntity(EntityAsset entityAsset)
        {
            Entity entity = new Entity();
            entity.Id = entities.GetNextFreeIndex();
            entity.SetFromEntityAsset(entityAsset);
            entities.Add(entity);

            return entity;
        }

        public void PlaceEntity(Entity entity)
        {
            Debug.Assert(entity.Id == entities.GetNextFreeIndex());
            entities.Add(entity);
        }

        public void DestroyEntity(EntityReference entityReference)
        {
            entities.Remove(entityReference.EntityId);
        }

        public void DestroyEntity(int id) 
        {
            entities.Remove(id);
        }

        public SceneAsset CreateSceneAsset()
        {
            SceneAsset sceneAsset = new SceneAsset();
            sceneAsset.name = name;
            sceneAsset.entities = entities.GetValues().Where(x => x != null).ToList();

            return sceneAsset;
        }
        
        public Entity[] GetAllEntities() => entities.GetValues().Where(x => x != null).ToArray();
                

        public RenderGraph RenderGraph { get { return renderGraph; } set { renderGraph?.Shutdown(); renderGraph = value; } }
        private RenderGraph renderGraph = null;
    }
}
