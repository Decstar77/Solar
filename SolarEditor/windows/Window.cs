using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolarSharp;
using SolarSharp.Rendering;

namespace SolarEditor
{
    internal interface Window
    {
        void Show(EditorState editorState);
        bool ShouldRemove();
    }

    internal class AssetWindow : Window
    {
        private bool show = true;

        public bool ShouldRemove() => !show;
        public void Show(EditorState editorState)
        {
            if (ImGui.Begin("Assets", ref show))
            {   
                if (ImGui.InputText("Base asset path", ref editorState.AssetPath, 0))
                {
                }

                ImGui.SameLine();
                if (ImGui.Button("Browse"))
                {
                    //string path =  editorState.AssetPath + "testPlane.obj";

                    //ModelResource? modelResource =  ModelImporter.LoadFromFile(path);
                    //if (modelResource != null)
                    //{
                    //    Renderer.testMesh = new StaticMesh(modelResource.meshes[0].vertices.ToArray(),
                    //        modelResource.meshes[0].indices.ToArray(),
                    //        modelResource.meshes[0].layout);

                    //    foreach (MeshResource meshResource in modelResource.meshes)
                    //    {
                    //        for (int i = 0; i < meshResource.indices.Count; i++)
                    //        {
                    //            Console.Write(meshResource.indices[i] + " ");
                    //        }
                    //        Console.WriteLine();
                    //    }
                    //}


                }
            }

            ImGui.End();
        }              
    }




}
