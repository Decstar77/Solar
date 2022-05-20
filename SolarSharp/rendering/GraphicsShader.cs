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

        public GraphicsShader() {

        }

        public GraphicsShader Create(DXDevice device, ShaderAsset shaderAsset, VertexLayout layout)
        {
            Name = shaderAsset.name;

            DXBlob vertexBlob = device.CompileShader(shaderAsset.Src, "VSmain", "vs_5_0");
            DXBlob pixelBlob = device.CompileShader(shaderAsset.Src, "PSmain", "ps_5_0");

            if (vertexBlob.Ptr != IntPtr.Zero && pixelBlob.Ptr != IntPtr.Zero)
            {
                InputElementDesc[] inputElementDescs = GetInputElementDescs(layout);

                inputLayout = device.CreateInputLayout(vertexBlob, inputElementDescs);

                vertexShader = device.CreateVertexShader(vertexBlob);
                pixelShader = device.CreatePixelShader(pixelBlob);

                vertexBlob.Release();
                pixelBlob.Release();
            }
            else
            {
                Logger.Error($"Could not compile {Name}");
            }

            return this;
        }

        public GraphicsShader Create(DXDevice device, string name, string src, VertexLayout vertexLayout)
        {
            Name = name;

            DXBlob vertexBlob = device.CompileShader(src, "VSmain", "vs_5_0");
            DXBlob pixelBlob = device.CompileShader(src, "PSmain", "ps_5_0");

            if (vertexBlob.Ptr != IntPtr.Zero && pixelBlob.Ptr != IntPtr.Zero)
            {
                InputElementDesc[] inputElementDescs = GetInputElementDescs(vertexLayout);

                inputLayout = device.CreateInputLayout(vertexBlob, inputElementDescs);
                vertexShader = device.CreateVertexShader(vertexBlob);
                pixelShader = device.CreatePixelShader(pixelBlob);

                vertexBlob.Release();
                pixelBlob.Release();
            }
            else
            {
                Logger.Error($"Could not compile {Name}");
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

        private InputElementDesc[] GetInputElementDescs(VertexLayout vertexLayout)
        {
            InputElementDesc pDesc = new InputElementDesc();
            pDesc.SemanticName = "Position";
            pDesc.SemanticIndex = 0;
            pDesc.Format = TextureFormat.R32G32B32_FLOAT;
            pDesc.InputSlot = 0;
            pDesc.AlignedByteOffset = 0;
            pDesc.InputSlotClass = InputClassification.PER_VERTEX_DATA;
            pDesc.InstanceDataStepRate = 0;

            InputElementDesc nDesc = new InputElementDesc();
            nDesc.SemanticName = "Normal";
            nDesc.SemanticIndex = 0;
            nDesc.Format = TextureFormat.R32G32B32_FLOAT;
            nDesc.InputSlot = 0;
            nDesc.AlignedByteOffset = 0xffffffff;
            nDesc.InputSlotClass = InputClassification.PER_VERTEX_DATA;
            nDesc.InstanceDataStepRate = 0;

            InputElementDesc tDesc = new InputElementDesc();
            tDesc.SemanticName = "TexCord";
            tDesc.SemanticIndex = 0;
            tDesc.Format = TextureFormat.R32G32_FLOAT;
            tDesc.InputSlot = 0;
            tDesc.AlignedByteOffset = 0xffffffff;
            tDesc.InputSlotClass = InputClassification.PER_VERTEX_DATA;
            tDesc.InstanceDataStepRate = 0;

            switch (vertexLayout)
            {
                case VertexLayout.INVALID:
                    return null;
                case VertexLayout.P:
                    return new InputElementDesc[] { pDesc };
                case VertexLayout.P_PAD:
                    return null;
                case VertexLayout.PNT:
                    return new InputElementDesc[] { pDesc, nDesc, tDesc };
                case VertexLayout.PNTC:
                    return null;
                case VertexLayout.PNTM:
                    return null;
                case VertexLayout.TEXT:
                    return null;
                case VertexLayout.PC:
                    return null;
            }

            return null;
        }
    }
}
