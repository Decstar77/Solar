using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SolarSharp.Assets;

namespace SolarSharp.Rendering
{
    public class GraphicsShader
    {
        public string Name { get; set; }
        public DXVertexShader vertexShader;
        public DXPixelShader pixelShader;
        public DXInputLayout inputLayout;
        private DXDevice device = null;

        public GraphicsShader() {

        }

        public GraphicsShader(DXDevice device)
        {
            this.device = device;
        }

        public GraphicsShader Create(ShaderAsset shaderAsset)
        {
            Name = shaderAsset.Name;

            DXBlob vertexBlob = device.CompileShader(shaderAsset.Src, "VSmain", "vs_5_0");
            DXBlob pixelBlob = device.CompileShader(shaderAsset.Src, "PSmain", "ps_5_0");

            if (vertexBlob.Ptr != IntPtr.Zero && pixelBlob.Ptr != IntPtr.Zero)
            {
                InputElementDesc[] inputElementDescs = GetInputElementDescs();

                inputLayout = device.CreateInputLayout(vertexBlob, inputElementDescs);

                vertexShader = device.CreateVertexShader(vertexBlob);
                pixelShader = device.CreatePixelShader(pixelBlob);

                vertexBlob.Release();
                pixelBlob.Release();
            }
            else
            {
                Logger.Error("Could not compile shader");
            }

            return this;
        }

        public GraphicsShader Release()
        {
            vertexShader?.Release();
            pixelShader?.Release();
            inputLayout?.Release();

            return this;
        }

        public bool IsValid()
        {
            return vertexShader != null && pixelShader != null && inputLayout != null && 
                vertexShader.Ptr != IntPtr.Zero && pixelShader.Ptr != IntPtr.Zero && inputLayout.Ptr != IntPtr.Zero;
        }

        private InputElementDesc[] GetInputElementDescs()
        {
            InputElementDesc pDesc = new InputElementDesc();
            pDesc.SemanticName = "Position";
            pDesc.SemanticIndex = 0;
            pDesc.Format = DXGIFormat.R32G32B32_FLOAT;
            pDesc.InputSlot = 0;
            pDesc.AlignedByteOffset = 0;
            pDesc.InputSlotClass = InputClassification.PER_VERTEX_DATA;
            pDesc.InstanceDataStepRate = 0;

            InputElementDesc nDesc = new InputElementDesc();
            nDesc.SemanticName = "Normal";
            nDesc.SemanticIndex = 0;
            nDesc.Format = DXGIFormat.R32G32B32_FLOAT;
            nDesc.InputSlot = 0;
            nDesc.AlignedByteOffset = 0xffffffff;
            nDesc.InputSlotClass = InputClassification.PER_VERTEX_DATA;
            nDesc.InstanceDataStepRate = 0;

            InputElementDesc tDesc = new InputElementDesc();
            tDesc.SemanticName = "TexCord";
            tDesc.SemanticIndex = 0;
            tDesc.Format = DXGIFormat.R32G32_FLOAT;
            tDesc.InputSlot = 0;
            tDesc.AlignedByteOffset = 0xffffffff;
            tDesc.InputSlotClass = InputClassification.PER_VERTEX_DATA;
            tDesc.InstanceDataStepRate = 0;

            return new InputElementDesc[] { pDesc, nDesc, tDesc };
        }
    }
}
