using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class SetGraphicsShaderNode : Node
    {
        public GraphicsShaderPin ShaderPin { get; set; }

        public SetGraphicsShaderNode() : base("Set Graphics Shader")
        {
            AddFlowPins();
            ShaderPin = new GraphicsShaderPin("Shader", this, PinInputType.INPUT);
        }

        public override bool CreateResources(RenderGraph renderGraph, DXDevice device)
        {
            return true;
        }

        public override void DrawUI()
        {
            DrawFlowPins();
            ShaderPin.DrawUI();
        }

        public override Node Run(RenderGraph graph, DXContext context)
        {
            GraphicsShader graphicsShader = graph.graphicsShaders.Find(x => x.Name == ShaderPin.GetValue());

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
