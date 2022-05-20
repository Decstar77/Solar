using SolarSharp.Assets;
using SolarSharp.core;
using SolarSharp.Core;
using SolarSharp.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering
{
    unsafe struct Foo
    {
        fixed int x[4];
    };

    struct Bar
    {
        int[] x = new int[4];
        public Bar()
        {

        }
    }

    public static class RenderSystem
    {
        public static DXDevice device;
        public static DXContext context;
        public static DXSwapchain swapchain;

        private static DXDepthStencilState depthStencilState;
        private static DXRasterizerState rasterizerState;
        private static DXBlendState blendState;

        private static StaticMesh quad;
        public static StaticMesh cube;

        public static GraphicsShader shader;

        public static GraphicsShader packedMaterialShader;

        private static ConstBuffer constBuffer0;
        private static ConstBuffer constBuffer1;

        private static ConstBuffer materialConstBuffer;

        private static DXSamplerState samplerState0;
        private static DXSamplerState samplerState1;
        private static DXSamplerState samplerState2;

        private static Dictionary<Guid, StaticMesh> meshes = new Dictionary<Guid, StaticMesh>();
        private static List<MeshAsset> meshesToAdd = new List<MeshAsset>();
        private static List<Guid> meshesToRemove = new List<Guid>();

        private static Dictionary<Guid, StaticTexture> textures = new Dictionary<Guid, StaticTexture>();
        private static List<TextureAsset> texturesToAdd = new List<TextureAsset>();
        private static List<TextureAsset> texturesToRemove = new List<TextureAsset>();

        public static void RegisterModel(ModelAsset modelAsset)
        {
            lock (meshesToAdd)
            {
                meshesToAdd.AddRange(modelAsset.meshes);
            }
        }

        public static void DeregisterModel(ModelAsset modelAsset)
        {
            lock (meshesToRemove)
            {
                meshesToRemove.AddRange(modelAsset.meshes.Select(x => x.Guid));
            }
        }

        public static void RegisterTexture(TextureAsset textureAsset)
        {
            lock (texturesToAdd)
            {
                Logger.Trace($"Registering {textureAsset.name}");
                texturesToAdd.Add(textureAsset);
            }
        }

        class InstancedDrawCall
        {
            public StaticMesh mesh;
            public List<Matrix4> transforms = new List<Matrix4>();
        }

        class MaterialDrawCall
        {
            public Vector4 albedo;
            public Dictionary<Guid, InstancedDrawCall> meshCalls = new Dictionary<Guid, InstancedDrawCall>();
        }

        private static Dictionary<Guid, MaterialDrawCall> drawCalls = new Dictionary<Guid, MaterialDrawCall>();

        public static bool Initialize()
        {
            DeviceContext deviceContext = new DeviceContext();
            swapchain = new DXSwapchain();

            deviceContext.Create();
            swapchain.Create();

            device = deviceContext.Device;
            context = deviceContext.Context;

            depthStencilState = deviceContext.Device.CreateDepthStencilState(new DepthStencilDesc { 
                depthTest = true,
                depthWrite = true,
                DepthFunc = DepthComparisonFunc.LESS_EQUAL
            });

            rasterizerState = deviceContext.Device.CreateRasterizerState(new RasterizerDesc { 
                CullMode = RasterizerCullMode.BACK,
                FillMode = RasterizerFillMode.WIREFRAME
            });
            blendState = deviceContext.Device.CreateBlendState(new BlendDesc());

            samplerState0 = deviceContext.Device.CreateSamplerState(new SamplerDesc { Filter = Filter.MIN_MAG_MIP_POINT });
            samplerState1 = deviceContext.Device.CreateSamplerState(new SamplerDesc { Filter = Filter.MIN_MAG_LINEAR_MIP_POINT });
            samplerState2 = deviceContext.Device.CreateSamplerState(new SamplerDesc { Filter = Filter.MIN_MAG_MIP_LINEAR });


            quad = new StaticMesh(device, MeshFactory.CreateQuad(-1, 1, 2, 2, 0), VertexLayout.PNT);
            cube = new StaticMesh(device, MeshFactory.CreateBox(1, 1, 1, 1), VertexLayout.PNT);

            shader = new GraphicsShader().Create(device, AssetSystem.GetShaderAsset("FirstShader"), VertexLayout.PNT);

            constBuffer0 = new ConstBuffer(device, 16 * 3).SetVS(context, 0);
            constBuffer1 = new ConstBuffer(device, 16 * 3).SetVS(context, 1);
            materialConstBuffer = new ConstBuffer(device, 16).SetPS(context, 0);

            ImGui.Initialzie();
            ImGuiTextEditor.Initialize();
            ImNodes.Initialzie();

            return true;
        }

        public static void BackupRenderer(RenderPacket renderPacket)
        {
            CreateAndDestroyMeshes();
            CreateAndDestroyTextures();

            context.ClearRenderTargetView(swapchain.renderTargetView, new Vector4(0.2f, 0.2f, 0.2f, 1.0f)); 
            context.ClearDepthStencilView(swapchain.depthStencilView, ClearFlag.D3D11_CLEAR_DEPTH, 1.0f, 0);
            context.SetRenderTargets(swapchain.depthStencilView, swapchain.renderTargetView);

            context.SetViewPortState(Window.SurfaceWidth, Window.SurfaceHeight); 
            context.SetPrimitiveTopology(PrimitiveTopology.TRIANGLELIST); 
            context.SetDepthStencilState(depthStencilState);
            context.SetRasterizerState(rasterizerState); 
            //context.SetBlendState(blendState);

            context.SetPSSampler(samplerState0, 0);
            context.SetPSSampler(samplerState1, 1);
            context.SetPSSampler(samplerState2, 2);

            constBuffer1.Reset().
                Prepare(renderPacket.projectionMatrix).
                Prepare(renderPacket.viewMatrix).
                Prepare(Matrix4.Identity).
                Upload(context);

            DebugVariables.DrawCalls = 0;
            DebugVariables.IndexCount = 0;

            if (shader != null && shader.IsValid())
            {
                context.SetInputLayout(shader.inputLayout); 
                context.SetVertexShader(shader.vertexShader);
                context.SetPixelShader(shader.pixelShader);

                foreach (RenderPacketEntry entry in renderPacket.renderPacketEntries)
                {
                    if (meshes.TryGetValue(entry.meshId, out StaticMesh mesh))
                    {
                        MaterialAsset material = AssetSystem.GetMaterialAsset(entry.materialId);

                        if (material != null)
                        {
                            Matrix4 mvp = renderPacket.projectionMatrix * renderPacket.viewMatrix * entry.transform;

                            constBuffer0.Reset().
                                Prepare(mvp).
                                Prepare(entry.transform).
                                Prepare(entry.transform.Inverse).
                                Upload(context);

                            materialConstBuffer.Reset().
                                Prepare(material.AlbedoColour).
                                Upload(context);

                            context.SetVertexBuffers(mesh.VertexBuffer, mesh.StrideBytes);
                            context.SetIndexBuffer(mesh.IndexBuffer, TextureFormat.R32_UINT, 0);
                            context.DrawIndexed(mesh.IndexCount, 0, 0);

                            DebugVariables.DrawCalls++;
                            DebugVariables.IndexCount += (int)mesh.IndexCount;
                        }
                    }
                }
            }
         
            DebugDraw.Flush(context);
        }

        public static void SwapBuffers(bool vysnc)
        {
            swapchain.Present(vysnc);
        }

        private static void CreateAndDestroyMeshes()
        {
            lock (meshesToRemove)
            {
                foreach (Guid modelAsset in meshesToRemove)
                {
                    Logger.Trace($"Removing mesh {modelAsset}");
                    StaticMesh mesh;
                    if (meshes.TryGetValue(modelAsset, out mesh))
                    {
                        mesh.Release();
                        meshes.Remove(modelAsset);
                    }
                }

                meshesToRemove.Clear();
            }

            lock (meshesToAdd)
            {
                foreach (MeshAsset meshAsset in meshesToAdd)
                {
                    Logger.Trace($"Uploading mesh {meshAsset.name}");

                    // @TODO: Probably not a failure case but I want to investigate later 
                    Debug.Assert(!meshes.ContainsKey(meshAsset.Guid));

                    StaticMesh mesh = new StaticMesh(device, meshAsset, VertexLayout.PNT);
                    //drawCalls.Add(mesh.mater)


                    meshes.Add(meshAsset.Guid, mesh);
                }
                meshesToAdd.Clear();
            }
        }

        private static void CreateAndDestroyTextures()
        {
            lock (texturesToAdd)
            {
                foreach (TextureAsset textureAsset in texturesToAdd)
                {
                    Logger.Trace($"Uploading texture {textureAsset.name}");
                    textures.Add(textureAsset.Guid, new StaticTexture(device, textureAsset));
                }
                texturesToAdd.Clear();
            }
        }
    }
}
