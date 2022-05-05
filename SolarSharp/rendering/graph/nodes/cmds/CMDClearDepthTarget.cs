using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class CMDClearDepthTargetNode : Node
    {
        private DepthTargetPin depthTargetPin = null;

        public CMDClearDepthTargetNode() : base("Clear Colour Target")
        {
            AddFlowPins();
            depthTargetPin = new DepthTargetPin("Depth target", this, PinInputType.INPUT);
        }

        public override bool CreateResources(RenderGraph renderGraph)
        {
            return true;
        }

        public override void DrawUI()
        {
            DrawFlowPins();
            depthTargetPin.DrawUI();
        }

        public override void Run(RenderGraph graph, Context context)
        {          

            throw new NotImplementedException();
        }
    }
}
