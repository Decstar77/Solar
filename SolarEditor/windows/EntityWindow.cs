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
                if (editorState.selection.SelectedEntities.Count > 0)
                {
                    Entity = editorState.selection.SelectedEntities[0].GetEntity();
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

                        if (ImGui.CollapsingHeader("Material"))
                        {
                            var models = AssetSystem.GetSortedModelAssets();
                            string[] modelNames = models.Select(x => x.name).ToArray();
                            int modelIndex = models.FindIndex(x => x.Guid == Entity.RenderingState.ModelId);

                            if (ImGui.Combo("Model", ref modelIndex, modelNames))
                            {
                                Entity.RenderingState.ModelId = models[modelIndex].Guid;
                            }

                            //var textures = AssetSystem.GetSortedTextureAssets();
                            //string[] textureNames = textures.Select(x => x.name).ToArray();
                            //int textureIndex = textures.FindIndex(x => x.Guid == Entity.Material.AlbedoTexture);
                            //if (ImGui.Combo("Albedo", ref textureIndex, textureNames)) {
                            //    Entity.Material.AlbedoTexture = textures[textureIndex].Guid;
                            //}


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
