using SolarSharp.Assets;
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

        private static Dictionary<Guid, StaticMesh> meshes = new Dictionary<Guid, StaticMesh>();
        private static List<ModelAsset> modelsToAdd = new List<ModelAsset> ();

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

            rasterizerState = deviceContext.Device.CreateRasterizerState(new RasterizerDesc());
            blendState = deviceContext.Device.CreateBlendState(new BlendDesc());
            DXSamplerState samplerState = deviceContext.Device.CreateSamplerState(new SamplerDesc());

            quad = new StaticMesh(device, MeshFactory.CreateQuad(-1, 1, 2, 2, 0));
            cube = new StaticMesh(device, MeshFactory.CreateBox(1, 1, 1, 1));

            //mesh = StaticMesh.CreateScreenSpaceQuad(deviceContext.Device);

            shader = new GraphicsShader(device);
            shader.Create(Assets.AssetSystem.ShaderAssets[0]);
            constBuffer0 = new ConstBuffer(device, 16 * 3).SetVS(context, 0);
           
            ImGui.Initialzie();
            ImGuiTextEditor.Initialize();
            ImNodes.Initialzie();

            return true;
        }


        private static bool temp = false;
        public static void BackupRenderer()
        {
            if ( !temp && AssetSystem.GetModelAsset("ionG_gravelBike_full.fbx") != null)
            {
                ModelAsset model = AssetSystem.GetModelAsset("ionG_gravelBike_full.fbx");

                cube = new StaticMesh(device, model.meshes[0]);
                temp = true;
            };
            
            foreach (ModelAsset modelAsset in modelsToAdd) {
                meshes.Add(modelAsset.Guid, new StaticMesh(device, modelAsset.meshes[0]));
            }
            modelsToAdd.Clear();

            Camera camera = GameSystem.CurrentScene.Camera;
            Matrix4 mvp = camera.GetProjectionMatrix() * camera.GetViewMatrix();
            constBuffer0.Reset().Prepare(mvp).Upload(context);
            
            context.ClearRenderTargetView(swapchain.renderTargetView, new Vector4(0.2f, 0.2f, 0.2f, 1.0f)); 
            context.ClearDepthStencilView(swapchain.depthStencilView, ClearFlag.D3D11_CLEAR_DEPTH, 1.0f, 0);
            context.SetRenderTargets(swapchain.depthStencilView, swapchain.renderTargetView);

            context.SetViewPortState(Window.SurfaceWidth, Window.SurfaceHeight); // @DONE
            context.SetPrimitiveTopology(PrimitiveTopology.TRIANGLELIST); // @DONE
            context.SetDepthStencilState(depthStencilState); // @DONE
            context.SetRasterizerState(rasterizerState); // @DONE
            //context.SetBlendState(blendState);

            if (shader != null && cube != null)
            {
                if (shader.IsValid())
                {
                    context.SetInputLayout(shader.inputLayout);   // @DONE
                    context.SetVertexShader(shader.vertexShader); // @DONE
                    context.SetPixelShader(shader.pixelShader);   // @DONE

                    GameScene scene = GameSystem.CurrentScene;

                    foreach (Entity entity in scene.Entities)
                    {
                        if (entity.Material != null)
                        {
                            StaticMesh mesh;
                            if (meshes.TryGetValue(entity.Material.ModelId, out mesh))
                            {
                                context.SetVertexBuffers(mesh.VertexBuffer, mesh.StrideBytes);
                                context.SetIndexBuffer(mesh.IndexBuffer, DXGIFormat.R32_UINT, 0);
                                context.DrawIndexed(mesh.IndexCount, 0, 0);
                            }
                            else
                            {
                                ModelAsset asset = AssetSystem.GetModelAsset(entity.Material.ModelId);
                                if (asset != null) 
                                {
                                    modelsToAdd.Add(asset);
                                }                                
                            }
                        }
                    }

                    //context.SetVertexBuffers(cube.VertexBuffer, cube.StrideBytes);
                    //context.SetIndexBuffer(cube.IndexBuffer, DXGIFormat.R32_UINT, 0);
                    //context.DrawIndexed(cube.IndexCount, 0, 0);
                }
            }
        }

        public static void SwapBuffers(bool vysnc)
        {
            swapchain.Present(vysnc);
        }

    }
}
