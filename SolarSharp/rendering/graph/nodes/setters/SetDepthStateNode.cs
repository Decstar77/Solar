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
        private DepthStencilState depthStencilState = null;
        private DepthStencilDesc createDesc = new DepthStencilDesc();

        public SetDepthStateNode() : base("Set Depth State")
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

        public override Node Run(RenderGraph graph, Context context)
        {
            context.SetDepthStencilState(depthStencilState);
            return outPin.GetConnectedPin().Node;
        }
    }

}
