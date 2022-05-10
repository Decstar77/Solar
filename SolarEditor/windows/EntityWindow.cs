using SolarSharp;
using SolarSharp.Assets;
using SolarSharp.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEditor
{
    internal class EntityWindow : EditorWindow
    {
        public Entity Entity { get; set; }

        public override void Show(EditorState editorState)
        {
            Entity = GameSystem.CurrentScene.Entities[0];

            if (ImGui.Begin("Entity inspector", ref show))
            {
                if (Entity != null)
                {
                    Vector3 pos = Entity.Position;
                    if (ImGui.DragFloat3("Position", ref pos)) {
                        Entity.Position = pos;
                    }

                    var models = AssetSystem.GetSortedModelAssets();

                    string[] modelNames = models.Select(x => x.name).ToArray();
                    int index = models.FindIndex(x => x.Guid == Entity.Material.ModelId);                    

                    if (ImGui.Combo("Model", ref index, modelNames))
                    {
                         Entity.Material.ModelId = models[index].Guid;
                    }                    
                }
            }

            ImGui.End();
        }

        public override void Shutdown()
        {
        }

        public override void Start()
        {
        }
    }
}
