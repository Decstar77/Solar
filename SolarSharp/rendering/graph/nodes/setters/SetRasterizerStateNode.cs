using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class SetRasterizerStateNode : Node
    {
        private RasterizerState rasterizerState = null;
        private RasterizerDesc createDesc = new RasterizerDesc();

        public SetRasterizerStateNode() : base("Set Rasterizer State")
        {
            AddFlowPins();
        }

        public override bool CreateResources(RenderGraph renderGraph)
        {
            rasterizerState = renderGraph.CreateOrGetRasterizerState(createDesc);
            return rasterizerState != null;
        }

        public override void DrawUI()
        {
           DrawFlowPins();
           createDesc = DrawStruct<RasterizerDesc>(createDesc);
        }

        public override Node Run(RenderGraph graph, Context context)
        {
            context.SetRasterizerState(rasterizerState);
            return outFlowPin?.GetConnectedPin()?.Node;
        }
    }
}
