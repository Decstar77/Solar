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

        
        public RasterizerDesc CreateDesc { get; set; }

        public SetRasterizerStateNode() : base("Set Rasterizer State")
        {
            AddFlowPins();
            CreateDesc = new RasterizerDesc();
        }

        public override bool CreateResources(RenderGraph renderGraph)
        {
            rasterizerState = renderGraph.CreateOrGetRasterizerState(CreateDesc);
            return rasterizerState != null;
        }

        public override void DrawUI()
        {
           DrawFlowPins();
           CreateDesc = DrawStruct<RasterizerDesc>(CreateDesc);
        }

        public override Node Run(RenderGraph graph, Context context)
        {
            context.SetRasterizerState(rasterizerState);
            return outFlowPin?.GetConnectedPin()?.Node;
        }
    }
}
