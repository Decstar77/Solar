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
        public ShaderAsset shaderAsset = null;
        public GraphicsShaderPin shaderPin = null;

        public GetGraphicsShaderNode() : base("Get Graphics Shader")
        {
            shaderPin = new GraphicsShaderPin("Shader", this, PinInputType.OUTPUT);
        }

        public override bool CreateResources(RenderGraph renderGraph)
        {
            if (shaderAsset != null) {
                GraphicsShader graphicsShader = renderGraph.CreateOrGetGraphicsShader(shaderAsset);
                if (graphicsShader.IsValid()) {
                    shaderPin.SetValue(graphicsShader);
                    return true;
                }                
            }

            return false;
        }

        public override void DrawUI()
        {
            string[] names = AssetSystem.ShaderAssets.Select(x => x.Name).ToArray();

            int currentItem = shaderAsset == null ? -1 : Array.IndexOf(names, shaderAsset.Name);
            
            shaderPin.DrawUI();

            ImGui.PushItemWidth(100);
            ImGui.Combo("Shader assets", ref currentItem, names);
            ImGui.PopItemWidth();

            if (currentItem != -1) {
                shaderAsset = AssetSystem.ShaderAssets[currentItem];
            }

        }

        public override Node Run(RenderGraph graph, Context context)
        {
            return null;
        }
    }
}
