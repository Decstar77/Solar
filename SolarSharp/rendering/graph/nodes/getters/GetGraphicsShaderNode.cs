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
        public GraphicsShaderPin ShaderPin { get; set; }

        [RenderGraphSerializable]
        public string ShaderName {get; set;}

        public GetGraphicsShaderNode() : base("Get Graphics Shader")
        {
            ShaderPin = new GraphicsShaderPin("Shader", this, PinInputType.OUTPUT);
            ShaderName = "";
        }

        public override bool CreateResources(RenderGraph renderGraph, DXDevice device)
        {
            if (ShaderName != "") {
                GraphicsShader graphicsShader = renderGraph.CreateOrGetGraphicsShader(AssetSystem.ShaderAssets.Find(x => x.name == ShaderName), device);
                if (graphicsShader.IsValid()) {
                    ShaderPin.SetValue(graphicsShader.Name);
                    return true;
                }                
            }

            return false;
        }

        public override void DrawUI()
        {
            string[] names = AssetSystem.ShaderAssets.Select(x => x.name).ToArray();

            int currentItem = ShaderName == null ? -1 : Array.IndexOf(names, ShaderName);
            
            ShaderPin.DrawUI();

            ImGui.PushItemWidth(100);
            ImGui.Combo("Shader assets", ref currentItem, names);
            ImGui.PopItemWidth();

            if (currentItem != -1) {
                ShaderName = AssetSystem.ShaderAssets[currentItem].name;
            }

        }

        public override Node Run(RenderGraph graph, DXContext context)
        {
            return null;
        }
    }
}
