using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class SetDepthNode : Node
    {
        private DepthStencilState depthStencilState = null;
        private DepthStencilDesc createDesc = new DepthStencilDesc();

        public SetDepthNode() : base("Set Depth")
        {
            AddFlowPins();
        }

        public override void DrawUI()
        {
            DrawFlowPins();
            createDesc = DrawStruct<DepthStencilDesc>(createDesc);
        }

        public override bool CreateResources(RenderGraph renderGraph)
        {
            depthStencilState = renderGraph.CreateOrGetDepthStencilState(createDesc);
            return depthStencilState != null;
        }

        public override void Run(RenderGraph graph, Context context)
        {
            context.SetDepthStencilState(depthStencilState);
        }
    }

}
