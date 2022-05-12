using SolarSharp.Assets;
using SolarSharp.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEditor
{
    internal class ModelViewerWindow : EditorWindow
    {
        public ModelAsset? modelAsset { get; set; } = null;

        public ModelViewerWindow(ModelAsset modelAsset)
        {
            this.modelAsset = modelAsset;
        }

        public override void Show(EditorState editorState)
        {
            if (ImGui.Begin("Model viewer", ref show))
            {
                if (modelAsset != null)
                {
                    ImGui.Text("Name: " + modelAsset.name);
                    ImGui.Text("Path: " + modelAsset.path);
                    ImGui.Text("Guid: " + modelAsset.Guid);
                    if (ImGui.CollapsingHeader("Meshes"))
                    {
                        foreach (MeshAsset mesh in modelAsset.meshes)
                        {
                            ImGui.Separator();
                            ImGui.Text("MeshName: " + mesh.name);
                            ImGui.Text("VertexLayout: " + mesh.layout.ToString());
                            ImGui.Text("MaterialName: " + mesh.materialName);
                            ImGui.Text("Guid: " + mesh.Guid);
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
