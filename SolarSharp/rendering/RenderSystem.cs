using SolarSharp.Core;
using System;
using System.Collections.Generic;
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

        public static List<GraphicsShader> graphicsShaders = new List<GraphicsShader>();
        private static DXDepthStencilState depthStencilState;
        private static DXRasterizerState rasterizerState;
        private static DXBlendState blendState;

        private static StaticMesh mesh;
        private static GraphicsShader shader;

        public static bool Initialize()
        {
            DeviceContext deviceContext = new DeviceContext();
            swapchain = new DXSwapchain();

            deviceContext.Create();
            swapchain.Create();

            device = deviceContext.Device;
            context = deviceContext.Context;

            depthStencilState = deviceContext.Device.CreateDepthStencilState(new DepthStencilDesc());
            rasterizerState = deviceContext.Device.CreateRasterizerState(new RasterizerDesc());
            blendState = deviceContext.Device.CreateBlendState(new BlendDesc());
            DXSamplerState samplerState = deviceContext.Device.CreateSamplerState(new SamplerDesc());


            mesh = StaticMesh.CreateScreenSpaceQuad(deviceContext.Device);

            shader = new GraphicsShader(device);
            shader.Create(Assets.AssetSystem.ShaderAssets[0]);

            Camera camera = new Camera();
            camera.Position.x = -1;
            camera.Position.z = -5;
           
            Matrix4 mvp = camera.GetViewMatrix() * camera.GetProjectionMatrix();
            ConstBuffer constBuffer0 = new ConstBuffer(device, 16 * 3).Prepare(mvp).SetVS(context, 0).Upload(context);
           
            ImGui.Initialzie();
            ImGuiTextEditor.Initialize();
            ImNodes.Initialzie();

            return true;
        }

        public static void BackupRenderer()
        {
            context.ClearRenderTargetView(swapchain.renderTargetView, new Vector4(0.2f, 0.2f, 0.2f, 1.0f)); 
            context.ClearDepthStencilView(swapchain.depthStencilView, ClearFlag.D3D11_CLEAR_DEPTH, 0.0f, 0);
            context.SetRenderTargets(swapchain.depthStencilView, swapchain.renderTargetView);

            context.ClearRenderTargetView(swapchain.renderTargetView, new Vector4(0.2f, 0.2f, 0.2f, 1.0f)); // @DONE
            context.ClearDepthStencilView(swapchain.depthStencilView, ClearFlag.D3D11_CLEAR_DEPTH, 0.0f, 0); // @DONE
            context.SetRenderTargets(swapchain.depthStencilView, swapchain.renderTargetView); // @DONE
            context.SetViewPortState(Window.SurfaceWidth, Window.SurfaceHeight); // @DONE
            context.SetPrimitiveTopology(PrimitiveTopology.TRIANGLELIST); // @DONE
            context.SetDepthStencilState(depthStencilState); // @DONE
            context.SetRasterizerState(rasterizerState); // @DONE
            //context.SetBlendState(blendState);

            context.SetInputLayout(shader.inputLayout);   // @DONE
            context.SetVertexShader(shader.vertexShader); // @DONE
            context.SetPixelShader(shader.pixelShader);   // @DONE
            context.SetVertexBuffers(mesh.VertexBuffer, mesh.StrideBytes);
            context.SetIndexBuffer(mesh.IndexBuffer, DXGIFormat.R32_UINT, 0);
            context.DrawIndexed(mesh.IndexCount, 0, 0);
        }

        public static void SwapBuffers()
        {
            swapchain.Present(true);
        }

    }
}
