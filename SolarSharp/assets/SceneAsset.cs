using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SolarSharp.Assets
{
    public class EntityAsset
    {
        [JsonInclude] public string name;
        [JsonInclude] public Vector3 position = Vector3.Zero;
        [JsonInclude] public Quaternion orientation = Quaternion.Identity;
        [JsonInclude] public Vector3 scale = new Vector3(1, 1, 1);
        [JsonInclude] public EntityReference reference;
        [JsonInclude] public EntityReference parent;
        [JsonInclude] public List<EntityReference> children;
        [JsonInclude] public RenderingState renderingState;
        [JsonInclude] public Dictionary<string, object> properties;
    }

    public class SceneAsset : EngineAsset
    {
        [JsonInclude] public string name;
        [JsonInclude] public List<EntityAsset> entities;
    }
}
