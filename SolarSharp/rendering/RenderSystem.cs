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
                //FillMode = RasterizerFillMode.WIREFRAME
            });
            blendState = deviceContext.Device.CreateBlendState(new BlendDesc());

            samplerState0 = deviceContext.Device.CreateSamplerState(new SamplerDesc { Filter = Filter.MIN_MAG_MIP_POINT });
            samplerState1 = deviceContext.Device.CreateSamplerState(new SamplerDesc { Filter = Filter.MIN_MAG_LINEAR_MIP_POINT });
            samplerState2 = deviceContext.Device.CreateSamplerState(new SamplerDesc { Filter = Filter.MIN_MAG_MIP_LINEAR });


            quad = new StaticMesh(device, MeshFactory.CreateQuad(-1, 1, 2, 2, 0));
            cube = new StaticMesh(device, MeshFactory.CreateBox(1, 1, 1, 1));

            shader = new GraphicsShader().Create(device, Assets.AssetSystem.ShaderAssets[0]); 
            
            constBuffer0 = new ConstBuffer(device, 16 * 3).SetVS(context, 0);
            constBuffer1 = new ConstBuffer(device, 16 * 3).SetVS(context, 1);
            materialConstBuffer = new ConstBuffer(device, 16).SetPS(context, 0);

            ImGui.Initialzie();
            ImGuiTextEditor.Initialize();
            ImNodes.Initialzie();

            return true;
        }

        private static bool temp = false;
        public static void BackupRenderer(GameScene scene)
        {
            if ( !temp && AssetSystem.GetModelAsset("windmill.fbx") != null)
            {
                ModelAsset model = AssetSystem.GetModelAsset("windmill.fbx");

                cube = new StaticMesh(device, model.meshes[0]);
                temp = true;
            };

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

                    meshes.Add(meshAsset.Guid, new StaticMesh(device, meshAsset));
                }
                meshesToAdd.Clear();
            }

            lock (texturesToAdd)
            {
                foreach (TextureAsset textureAsset in texturesToAdd)
                {
                    Logger.Trace($"Uploading texture {textureAsset.name}");
                    textures.Add(textureAsset.Guid, new StaticTexture(device, textureAsset));
                }
                texturesToAdd.Clear();
            }


            Camera camera = scene.Camera;

            Matrix4 proj = camera.GetProjectionMatrix();
            Matrix4 view = camera.GetViewMatrix();

            context.ClearRenderTargetView(swapchain.renderTargetView, new Vector4(0.2f, 0.2f, 0.2f, 1.0f)); 
            context.ClearDepthStencilView(swapchain.depthStencilView, ClearFlag.D3D11_CLEAR_DEPTH, 1.0f, 0);
            context.SetRenderTargets(swapchain.depthStencilView, swapchain.renderTargetView);

            context.SetViewPortState(Window.SurfaceWidth, Window.SurfaceHeight); // @DONE
            context.SetPrimitiveTopology(PrimitiveTopology.TRIANGLELIST); // @DONE
            context.SetDepthStencilState(depthStencilState); // @DONE
            context.SetRasterizerState(rasterizerState); // @DONE
            //context.SetBlendState(blendState);

            context.SetPSSampler(samplerState0, 0);
            context.SetPSSampler(samplerState1, 1);
            context.SetPSSampler(samplerState2, 2);

            constBuffer1.Reset().Prepare(proj).Prepare(view).Prepare(Matrix4.Identity).Upload(context);

            DebugVariables.DrawCalls = 0;
            DebugVariables.IndexCount = 0;

            if (shader != null && cube != null)
            {
                if (shader.IsValid())
                {
                    context.SetInputLayout(shader.inputLayout);   
                    context.SetVertexShader(shader.vertexShader); 
                    context.SetPixelShader(shader.pixelShader);   

                    Entity[] entities = scene.GetAllEntities();
                    foreach (Entity entity in entities)
                    {
                        if (entity.RenderingState.ModelId != Guid.Empty)
                        {
                            ModelAsset modelAsset = AssetSystem.GetModelAsset(entity.RenderingState.ModelId);
                            if (modelAsset != null)
                            {
                                foreach (MeshAsset meshAsset in modelAsset.meshes)
                                {
                                    StaticMesh mesh;
                                    if (meshes.TryGetValue(meshAsset.Guid, out mesh))
                                    {
                                        Matrix4 m = entity.ComputeTransformMatrix();
                                        Matrix4 mvp = proj * view * m;

                                        MaterialAsset materialAsset = AssetSystem.GetMaterialAsset(meshAsset.materialName);

                                        constBuffer0.Reset().Prepare(mvp).Prepare(m).Prepare(m.Inverse).Upload(context);
                                        materialConstBuffer.Reset().Prepare(materialAsset.AlbedoColour).Upload(context);

                                        context.SetVertexBuffers(mesh.VertexBuffer, mesh.StrideBytes);
                                        context.SetIndexBuffer(mesh.IndexBuffer, TextureFormat.R32_UINT, 0);
                                        context.DrawIndexed(mesh.IndexCount, 0, 0);
                                        DebugVariables.DrawCalls++;
                                        DebugVariables.IndexCount += (int)mesh.IndexCount;
                                    }

                                }

                            }
                            //{
                            //    StaticTexture texture;
                            //    if (textures.TryGetValue(entity.Material.AlbedoTexture, out texture))
                            //    {
                            //        Matrix4 mvp = proj * view * entity.ComputeModelMatrix();

                            //        constBuffer0.Reset().Prepare(mvp).Upload(context);
                            //        constBuffer1.Reset().Prepare(proj).Prepare(view).Prepare(Matrix4.Identity).Upload(context);

                            //        context.SetPSShaderResources(texture.srv, 0);

                            //        context.SetVertexBuffers(mesh.VertexBuffer, mesh.StrideBytes);
                            //        context.SetIndexBuffer(mesh.IndexBuffer, TextureFormat.R32_UINT, 0);
                            //        context.DrawIndexed(mesh.IndexCount, 0, 0);
                            //    }
                            //}  
                        }
                    }

                }
            }

            //Logger.Info("Draw calls: " + drawCallCount);
            DebugDraw.Flush(context);
        }

        public static void SwapBuffers(bool vysnc)
        {
            swapchain.Present(vysnc);
        }

    }
}
