using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Assimp;
using SolarSharp;
using SolarSharp.Assets;

namespace SolarEditor
{
    internal class ModelImporter
    {
        private static AssimpContext? importer = null;
        public bool Loaded { get; private set; } = false;
        private Scene scene;
        private string filePath;

        public ModelImporter(string filePath) 
        {
            if (!File.Exists(filePath)) { Logger.Error("File path does not exist + " + filePath); return; }

            if (importer == null)
                importer = new AssimpContext();

            if (importer == null) { Logger.Error("Assimp importer does not work"); return; }

            scene = importer.ImportFile(filePath, PostProcessSteps.JoinIdenticalVertices | PostProcessSteps.Triangulate);
            if (scene == null) { Logger.Error("Assimp scene is null"); return; }

            this.filePath = filePath;
            Loaded = true;
        }

        public List<MaterialAsset> LoadMaterials()
        {
            List<MaterialAsset> materials = new List<MaterialAsset>();
            foreach (Assimp.Material material in scene.Materials)
            {
                MaterialAsset m = new MaterialAsset();
                m.name = material.Name;
                m.AlbedoColour = new Vector4(material.ColorDiffuse.R, material.ColorDiffuse.G, material.ColorDiffuse.B, material.ColorDiffuse.A);
                m.SpecularColour = new Vector4(material.ColorSpecular.R, material.ColorSpecular.G, material.ColorSpecular.B, material.ColorSpecular.A);
                materials.Add(m);
            }

            return materials;
        }

        public ModelAsset LoadModel()
        {
            ModelAsset model = new ModelAsset();            
            model.meshes = new List<MeshAsset>(scene.MeshCount);
            model.alignedBox = new AlignedBox();

            foreach (Mesh m in scene.Meshes)
            {
                MeshAsset mesh = new MeshAsset();
                mesh.vertices = new List<float>(m.VertexCount);
                mesh.indices = new List<uint>(3 * m.FaceCount);

                Vector3 minPos = new Vector3(float.MaxValue);
                Vector3 maxPos = new Vector3(-float.MaxValue);

                List<Vector3D> verts = m.Vertices;
                List<Vector3D> norms = m.Normals;
                List<Vector3D> uvs = m.TextureCoordinateChannels[0];

                for (int i = 0; i < verts.Count; i++)
                {
                    Vector3D pos = verts[i];
                    Vector3D norm = norms[i];
                    Vector3D uv = uvs[i];

                    minPos = Vector3.Min(minPos, new SolarSharp.Vector3(pos.X, pos.Y, pos.Z));
                    maxPos = Vector3.Max(maxPos, new SolarSharp.Vector3(pos.X, pos.Y, pos.Z));

                    mesh.vertices.Add(pos.X);
                    mesh.vertices.Add(pos.Y);
                    mesh.vertices.Add(pos.Z);

                    mesh.vertices.Add(norm.X);
                    mesh.vertices.Add(norm.Y);
                    mesh.vertices.Add(norm.Z);

                    mesh.vertices.Add(uv.X);
                    mesh.vertices.Add(uv.Y);  
                }

                mesh.alignedBox = new AlignedBox(minPos, maxPos);
                model.alignedBox = AlignedBox.Combine(model.alignedBox, mesh.alignedBox);

                List<Face> faces = m.Faces;
                for (int i = 0; i < faces.Count; i++)
                {
                    Face f = faces[i];
                                
                    if (f.IndexCount != 3)
                    {
                        Logger.Error("Mesh is not trianglulated");
                        continue;
                    }

                    mesh.indices.Add((uint)(f.Indices[0]));
                    mesh.indices.Add((uint)(f.Indices[1]));
                    mesh.indices.Add((uint)(f.Indices[2]));
                }
                
                mesh.layout = VertexLayout.PNT;
                mesh.name = m.Name;
                mesh.materialName = scene.Materials[m.MaterialIndex].Name;
                model.meshes.Add(mesh);
            }

            model.name = Path.GetFileName(filePath);
            model.path = filePath;

            return model;
        }

        private static void GatherVertexCounts(Scene scene, out int vertexCount, out int indexCount, out int meshCount)
        {
            vertexCount = 0;
            indexCount = 0;
            meshCount = 0;

            foreach (Mesh m in scene.Meshes)
            {
                vertexCount += m.VertexCount;
                indexCount += 3 * m.FaceCount;
                meshCount++;
            }
        }
    }
}
