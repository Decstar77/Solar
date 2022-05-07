using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class SetDepthStateNode : Node
    {
        private DXDepthStencilState depthStencilState = null;

        [RenderGraphSerializable]
        public DepthStencilDesc CreateDesc { get; set; }

        public SetDepthStateNode() : base("Set Depth State")
        {
            AddFlowPins();
            CreateDesc = new DepthStencilDesc();
        }

        public override void DrawUI()
        {
            DrawFlowPins();
            CreateDesc = DrawStruct<DepthStencilDesc>(CreateDesc);
        }

        public override bool CreateResources(RenderGraph renderGraph, DXDevice device)
        {
            depthStencilState = renderGraph.CreateOrGetDepthStencilState(CreateDesc, device);
            return depthStencilState != null;
        }

        public override Node Run(RenderGraph graph, DXContext context)
        {
            context.SetDepthStencilState(depthStencilState);
            return outFlowPin?.GetConnectedPin()?.Node;
        }
    }

}
