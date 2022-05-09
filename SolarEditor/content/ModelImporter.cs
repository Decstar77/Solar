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
        internal static ModelAsset? LoadFromFile(string filePath, MetaFileAsset metaFile)
        {
            if (!File.Exists(filePath)) { Logger.Error("File path does not exist + " + filePath); return null; }

            AssimpContext importer = new AssimpContext();
            if (importer == null) { Logger.Error("Assimp importer does not work"); return null; }

            Scene scene = importer.ImportFile(filePath, PostProcessSteps.JoinIdenticalVertices | PostProcessSteps.Triangulate);
            if (scene == null) { Logger.Error("Assimp scene is null"); return null; }

            Logger.Trace("Loading model " + filePath);
            ModelAsset model = LoadModel(scene);
            model.name = Path.GetFileName(filePath);
            model.path = filePath;
            model.Guid = metaFile.Guid;
            
            importer.Dispose();            

            return model;
        }
        
        private static ModelAsset LoadModel(Scene scene)
        {
            ModelAsset model = new ModelAsset();            
            model.meshes = new List<MeshAsset>(scene.MeshCount);

            foreach (Mesh m in scene.Meshes)
            {
                MeshAsset mesh = new MeshAsset();
                mesh.vertices = new List<float>(m.VertexCount);
                mesh.indices = new List<uint>(3 * m.FaceCount);

                List<Vector3D> verts = m.Vertices;
                List<Vector3D> norms = m.Normals;
                List<Vector3D> uvs = m.TextureCoordinateChannels[0];

                for (int i = 0; i < verts.Count; i++)
                {
                    Vector3D pos = verts[i];
                    Vector3D norm = norms[i];
                    Vector3D uv = uvs[i];

                    mesh.vertices.Add(pos.X);
                    mesh.vertices.Add(pos.Y);
                    mesh.vertices.Add(pos.Z);

                    mesh.vertices.Add(norm.X);
                    mesh.vertices.Add(norm.Y);
                    mesh.vertices.Add(norm.Z);

                    mesh.vertices.Add(uv.X);
                    mesh.vertices.Add(uv.Y);                    
                }

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

                model.meshes.Add(mesh);
            }


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
