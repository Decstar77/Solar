using SolarSharp.Assets;
using SolarSharp.Rendering.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace SolarSharp
{
    public class EntityReference
    { 
        public int EntityId { get; set; }
    }

    public class Entity
    {
        public int Id { get; }
        protected int id;

        protected string name = string.Empty;
        public string Name { get { return name; } set { name = value; } }


        protected Vector3 position = Vector3.Zero;
        public Vector3 Position { get { return position; } set { position = value; } }

        protected Quaternion orientation = Quaternion.Identity;
        public Quaternion Orientation { get { return orientation; } set { orientation = value; } }

        protected Vector3 scale = new Vector3(1, 1, 1);
        public Vector3 Scale { get { return scale; } set { scale = value; } }

        public Material Material { get; set; }
        public AlignedBox WorldSpaceBoundingBox { get; }
        public AlignedBox LocalSpaceBoundingBox { get { return GetLocalBoundingBox(); } }

        public EntityReference Parent { get; set; }
        public List<EntityReference> Children { get; set; }

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
                        return AlignedBox.Transform(modelAsset.alignedBox, position, orientation, scale);
                        //return modelAsset.alignedBox;
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

    public class GameScene : EngineAsset
    {
        public string name = "";
        public string path = "";

        public Camera Camera { get { return camera; } set { camera = value; } }
        private Camera camera = new Camera();
        
        public List<Entity> Entities { get { return entities; } set { entities = value; } }
        private List<Entity> entities = new List<Entity>();

        public RenderGraph RenderGraph { get { return renderGraph; } set { renderGraph?.Shutdown(); renderGraph = value; } }
        private RenderGraph renderGraph = null;
    }
}
