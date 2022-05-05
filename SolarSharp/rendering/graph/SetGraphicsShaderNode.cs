using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class SetGraphicsShaderNode : Node
    {
        private ShaderPin shaderPin = new ShaderPin("Shader", true);

        public SetGraphicsShaderNode() : base("Set Graphics Shader")
        {
            AddFlowPins();
        }

        public override void CreateResources(RenderGraph renderGraph)
        {
            throw new NotImplementedException();
        }

        public override void DrawUI()
        {
            DrawFlowPins();
            shaderPin.DrawUI();
        }

        public override void Run(RenderGraph graph, Context context)
        {
            GraphicsShader graphicsShader = shaderPin.GetValue();

            if (graphicsShader != null) {
                if (graphicsShader.IsValid()) {
                    context.SetInputLayout(graphicsShader.inputLayout);
                    context.SetVertexShader(graphicsShader.vertexShader);
                    context.SetPixelShader(graphicsShader.pixelShader);
                }
            }

        }
    }
}
