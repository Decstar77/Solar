using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class SetGraphicsShaderNode : Node
    {
        public GraphicsShaderPin shaderPin = null;

        public SetGraphicsShaderNode() : base("Set Graphics Shader")
        {
            AddFlowPins();
            shaderPin = new GraphicsShaderPin("Shader", this, PinInputType.INPUT);
        }

        public override bool CreateResources(RenderGraph renderGraph)
        {
            return true;
        }

        public override void DrawUI()
        {
            DrawFlowPins();
            shaderPin.DrawUI();
        }

        public override Node Run(RenderGraph graph, Context context)
        {
            GraphicsShader graphicsShader = shaderPin.GetValue();

            if (graphicsShader != null) {
                if (graphicsShader.IsValid()) {
                    context.SetInputLayout(graphicsShader.inputLayout);
                    context.SetVertexShader(graphicsShader.vertexShader);
                    context.SetPixelShader(graphicsShader.pixelShader);
                    return outFlowPin?.GetConnectedPin()?.Node;
                }
            }

            return null;            
        }
    }
}
