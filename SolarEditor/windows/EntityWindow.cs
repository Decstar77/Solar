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
            if (ImGui.Begin("Entity inspector", ref show))
            {                
                if (editorState.currentContext.selection.SelectedEntities.Count > 0)
                {
                    Entity = editorState.currentContext.selection.SelectedEntities[0].GetEntity();
                    if (Entity != null)
                    { 
                        if (ImGui.CollapsingHeader("Identifiers"))
                        {
                            ImGui.InputText("Name", ref Entity.Name);                        
                            ImGui.Text("Id " + Entity.Index.ToString());
                        }

                        if (ImGui.CollapsingHeader("Transform"))
                        {
                            Vector3 pos = Entity.Position;
                            if (ImGui.DragFloat3("Position", ref pos))
                            {
                                Entity.Position = pos;
                            }
                        }

                        if (ImGui.CollapsingHeader("Rendering"))
                        {
                            var models = AssetSystem.GetSortedModelAssets();
                            models.Select(model => model.name);
                        }

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
