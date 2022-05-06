using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SolarSharp.Core;
using SolarSharp.Rendering;
using SolarSharp.EngineAPI;
using SolarSharp.Assets;
using SolarSharp.Rendering.Graph;

namespace SolarSharp
{
    public class Application
    {
        public static int SurfaceWidth { get { return surfaceWidth; }  }
        private static int surfaceWidth;

        public static int SurfaceHeight { get { return surfaceHeight; } }
        private static int surfaceHeight;

        public static float WindowAspect { get { return windowAspect; } }
        private static float windowAspect;

        public static string WindowName { get { return windowName; } }
        private static string windowName;

        private static int windowX;   
        private static int windowY;        

        public static FrameInput Input { get { return input; } }
        private static FrameInput input = new FrameInput();
        public static FrameInput OldInput { get; set; }

        private static string version;
        private static string descrip;

        private static bool initialized = false;

        public static RenderGraph renderGraph = null;

        public Application(ApplicationConfig config)
        {
            if (!EventSystem.Initialize()) {
                Logger.Error("Could not initialize event system");
            };
            
            Window window = new Window();
            if (!window.Open(config.Title, config.SurfaceWidth, config.SurfaceHeight, config.WindowXPos, config.WindowYPos)) {
                Logger.Error("Could not open window");
            }

            if (!AssetSystem.Initialize()) {
                Logger.Error("Could not initalize assets");
            }

            AssetSystem.LoadEverything(config.AssetPath);

            if (!RenderSystem.Initialize()) {
                Logger.Error("Could not initalize renderer");
            }


            DeviceContext deviceContext = new DeviceContext();
            Swapchain swapchain = new Swapchain();

            deviceContext.Create();
            swapchain.Create();

            ShaderFactory.Initialize(deviceContext.Device);

            DepthStencilState depthStencilState = deviceContext.Device.CreateDepthStencilState(new DepthStencilDesc());
            RasterizerState rasterizerState = deviceContext.Device.CreateRasterizerState(new RasterizerDesc());
            BlendState blendState = deviceContext.Device.CreateBlendState(new BlendDesc());
            SamplerState samplerState = deviceContext.Device.CreateSamplerState(new SamplerDesc());


            GraphicsShader shader = new GraphicsShader(deviceContext.Device).Create(AssetSystem.ShaderAssets[0]);

            RenderSystem.graphicsShaders.Add( shader );

            StaticMesh mesh = StaticMesh.CreateScreenSpaceQuad(deviceContext.Device);

            DirectXBuffer constBuffer0 = deviceContext.Device.CreateBuffer(new BufferDesc { 
                BindFlags = (uint)BufferBindFlag.CONSTANT_BUFFER,
                Usage = BufferUsage.DEFAULT,
                ByteWidth = sizeof(float) * 16 * 3
            });

            Matrix4 proj = Matrix4.CreatePerspectiveLH((MathF.PI / 180) * 45.0f, window.WindowAspect, 0.1f, 100.0f);
            Matrix4 cam = Matrix4.CreateLookAtLH(new Vector3(0, 0, -5), Vector3.Zero, Vector3.UnitY).Inverse;
            Matrix4 mvp = cam * proj;

            int index = 0;
            float[] constBufferData = new float[16 * 3];
            constBufferData[index++] = mvp.m11; constBufferData[index++] = mvp.m12; constBufferData[index++] = mvp.m13; constBufferData[index++] = mvp.m14;
            constBufferData[index++] = mvp.m21; constBufferData[index++] = mvp.m22; constBufferData[index++] = mvp.m23; constBufferData[index++] = mvp.m24;
            constBufferData[index++] = mvp.m31; constBufferData[index++] = mvp.m32; constBufferData[index++] = mvp.m33; constBufferData[index++] = mvp.m34;
            constBufferData[index++] = mvp.m41; constBufferData[index++] = mvp.m42; constBufferData[index++] = mvp.m43; constBufferData[index++] = mvp.m44;

            deviceContext.Context.UpdateSubresource(constBuffer0, constBufferData);
            deviceContext.Context.SetVSConstBuffer(constBuffer0, 0);

            ImGuiAPI.ImGuiInitialzie();
            ImGuiTextEditor.Initialize();
            ImNodes.Initialzie();

            RenderSystem.device = deviceContext.Device;
            RenderSystem.context = deviceContext.Context;
            RenderSystem.swapchain = swapchain;

            config.OnInitializeCallback.Invoke();

            renderGraph = new RenderGraph("My Render Graph", RenderSystem.device, RenderSystem.context);
            renderGraph.Load(config.AssetPath + "BasicRenderGraph.json");
            renderGraph.Create();

            while (window.Running(ref input)) {

                renderGraph.Run();
                deviceContext.Context.SetVertexBuffers(mesh.VertexBuffer, mesh.StrideBytes);
                deviceContext.Context.SetIndexBuffer(mesh.IndexBuffer, DXGIFormat.R32_UINT, 0);
                deviceContext.Context.DrawIndexed(mesh.IndexCount, 0, 0);

                //deviceContext.Context.ClearRenderTargetView(swapchain.renderTargetView, new Vector4(0.2f, 0.2f, 0.2f, 1.0f)); // @DONE
                //deviceContext.Context.ClearDepthStencilView(swapchain.depthStencilView, ClearFlag.D3D11_CLEAR_DEPTH, 0.0f, 0); // @DONE
                //deviceContext.Context.SetRenderTargets(swapchain.depthStencilView, swapchain.renderTargetView); // @DONE
                //deviceContext.Context.SetViewPortState(window.SurfaceWidth, window.SurfaceHeight); // @DONE
                //deviceContext.Context.SetPrimitiveTopology(PrimitiveTopology.TRIANGLELIST); // @DONE
                //deviceContext.Context.SetDepthStencilState(depthStencilState); // @DONE
                //deviceContext.Context.SetRasterizerState(rasterizerState); // @DONE
                //deviceContext.Context.SetBlendState(blendState);

                //if (shader.IsValid())
                //{
                //    deviceContext.Context.SetInputLayout(shader.inputLayout);   // @DONE
                //    deviceContext.Context.SetVertexShader(shader.vertexShader); // @DONE
                //    deviceContext.Context.SetPixelShader(shader.pixelShader);   // @DONE

                //    deviceContext.Context.SetVertexBuffers(mesh.VertexBuffer, mesh.StrideBytes);
                //    deviceContext.Context.SetIndexBuffer(mesh.IndexBuffer, DXGIFormat.R32_UINT, 0);
                //    deviceContext.Context.DrawIndexed(mesh.IndexCount, 0, 0);
                //}

                EventSystem.Fire(EventType.RENDER_END, null);

                swapchain.Present(true);

                OldInput = input;
            }

            ImNodes.Shutdown();
            ImGuiAPI.ImGuiShutdown();


            //    if (initialized)
            //    {
            //        Logger.Error("Application is already running !!");
            //        return;
            //    }

            //    initialized = true;

            //    surfaceWidth = config.SurfaceWidth;
            //    surfaceHeight = config.SurfaceHeight;
            //    windowAspect = (float)surfaceWidth / (float)surfaceHeight;
            //    windowX = config.WindowXPos;
            //    windowY = config.WindowYPos;
            //    windowName = config.Title;
            //    version = config.Version;
            //    descrip = config.Description;

            //    if (config.OnUpdateCallback != null 
            //        && config.OnRenderCallback != null 
            //        && config.OnInitializeCallback != null 
            //        && config.OnShutdownCallback != null)
            //    {
            //        if (Win32API.CreateWindow_(windowName, surfaceWidth, surfaceHeight, windowY, windowX))
            //        {
            //            Logger.Trace("Window created");

            //            if (EngineAPI.Win32CreateRenderer())
            //            {
            //                Logger.Trace("Renderer created");
            //                if (EventSystem.Initialize())
            //                {
            //                    if (Renderer.Create())
            //                    {                                
            //                        if (config.OnInitializeCallback())
            //                        {
            //                            Logger.Info("Startup successful");

            //                            while (Win32API.PumpMessages_(ref input))
            //                            {
            //                                RenderPacket renderPacket = new RenderPacket();

            //                                //config.OnUpdateCallback();
            //                                //config.OnRenderCallback(renderPacket);

            //                                Renderer.Render(renderPacket);


            //                                oldInput = input;
            //                            }
            //                        }
            //                        else
            //                        {

            //                        }
            //                    }
            //                    else
            //                    {

            //                    }
            //                }
            //                else
            //                {

            //                }

            //                EngineAPI.Win32DestroyRenderer();
            //            }
            //            else
            //            {
            //                Logger.Error("Could not create win32 renderer");
            //            }

            //            Win32API.DestroyWindow_();
            //        }
            //        else
            //        {
            //            Logger.Error("Could not create win32 window");
            //        }
            //    }
            //    else
            //    {
            //        Logger.Error("Update/render/init/shutdown callback(s) is null");
            //    }
        }
    }
}
