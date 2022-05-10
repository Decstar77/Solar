﻿using SolarSharp.Assets;
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
    }

    public class Entity
    {
        public int Id { get { return id; } }
        protected int id;

        [JsonInclude]
        public string Name = "Untitled";

        protected Vector3 position = Vector3.Zero;
        public Vector3 Position { get { return position; } set { position = value; } }

        protected Quaternion orientation = Quaternion.Identity;

        public Quaternion Orientation { get { return orientation; } set { orientation = value; } }

        protected Vector3 scale = new Vector3(1, 1, 1);
        public Vector3 Scale { get { return scale; } set { scale = value; } }
        public EntityReference Reference { get { return new EntityReference(id); } }
        public void SetTransform(Matrix4 m) => Matrix4.Decompose(m, out position, out orientation, out scale);
        public Material Material { get; set; } = new Material();

        [JsonIgnore]
        public AlignedBox WorldSpaceBoundingBox { get { return GetWorldBoundingBox(); } }

        [JsonIgnore]
        public AlignedBox LocalSpaceBoundingBox { get { return GetLocalBoundingBox(); } }

        public EntityReference Parent { get; set; }
        public List<EntityReference> Children { get; set; }

        public Entity()
        {

        }

        public Entity(int id)
        {
            this.id = id;
        }

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
            if (Material?.ModelId != null)
            {
                if (Material.ModelId != Guid.Empty)
                {
                    ModelAsset modelAsset = AssetSystem.GetModelAsset(Material.ModelId);
                    if (modelAsset != null)
                    {                        
                        return modelAsset.alignedBox;
                    }
                }
            }

            return new AlignedBox();
        }

        protected AlignedBox GetWorldBoundingBox()
        {
            if (Material?.ModelId != null)
            {
                if (Material.ModelId != Guid.Empty)
                {
                    ModelAsset modelAsset = AssetSystem.GetModelAsset(Material.ModelId);
                    if (modelAsset != null)
                    {
                        return AlignedBox.Transform(modelAsset.alignedBox, position, Orientation);
                    }
                }
            }

            return new AlignedBox();
        }
    }

    [Flags]
    public enum MaterialFlag
    { 
        SHADOW = 0x1,
        TRANSPARENT = 0x2
    }

    public class Material
    {
        public MaterialFlag Flags { get; set; }
        public Guid ModelId { get; set; }
        public Guid ShaderId { get; set; }
        public Guid AlbedoTexture { get; set; }
        public Guid NormalTexture { get; set; }
    }

    public class FreeList<T> where T : class
    {
        [JsonInclude]
        public int Count { get; private set; }
        
        [JsonInclude]
        private Queue<int> freeList;

        [JsonInclude]
        private T[] data;

        public FreeList(int size)
        {
            freeList = new Queue<int>(size);
            data = new T[size];
            Count = 0;

            for (int i = 0 ; i < size; i++) {
                freeList.Enqueue(i);
            }
        }

        public int GetNextFreeIndex() => freeList.Peek();
        public T[] GetValues() => data;

        public void Add(T t)
        {
            data[freeList.Dequeue()] = t;
            Count++;
        }

        public void Remove(int index)
        {
            freeList.Enqueue(index);
            Count--;
            data[index] = null;
        }
    }

    public class GameScene : EngineAsset
    {
        public string name = "";
        public string path = "";

        public Camera Camera { get { return camera; } set { camera = value; } }
        private Camera camera = new Camera();

        private FreeList<Entity> entities = new FreeList<Entity>(1000);

        public Entity CreateEntity()
        {
            Entity entity = new Entity(entities.GetNextFreeIndex());
            entities.Add(entity);
            return entity;
        }

        public void PlaceEntity(Entity entity)
        {
            Debug.Assert(entity.Id == entities.GetNextFreeIndex());
            entities.Add(entity);
        }

        public void DeleteEntity(int id) => entities.Remove(id);
        public Entity[] GetAllEntities() => entities.GetValues().Where(x => x != null).ToArray();

        public RenderGraph RenderGraph { get { return renderGraph; } set { renderGraph?.Shutdown(); renderGraph = value; } }
        private RenderGraph renderGraph = null;
    }
}
