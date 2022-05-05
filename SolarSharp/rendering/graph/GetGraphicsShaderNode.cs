using SolarSharp.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class GetGraphicsShaderNode : Node
    {
        private ShaderAsset shaderAsset = null;
        private GraphicsShaderPin shaderPin = null;

        public GetGraphicsShaderNode() : base("Get Graphics Shader")
        {
            shaderPin = new GraphicsShaderPin("Shader", this, PinInputType.OUTPUT);
        }

        public override bool CreateResources(RenderGraph renderGraph)
        {
            if (shaderAsset != null) {
                GraphicsShader graphicsShader = renderGraph.CreateOrGetGraphicsShader(shaderAsset);                
                shaderPin.SetValue(graphicsShader);
            }

            return false;
        }

        public override void DrawUI()
        {
            
        }

        public override void Run(RenderGraph graph, Context context)
        {
            
        }
    }
}
