using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class SetRenderTargetsNode : Node
    {
        public ColourTargetPin renderTargetPin = null;
        public DepthTargetPin depthTargetPin = null;

        public SetRenderTargetsNode() : base("Set Render Targets")
        {
            AddFlowPins();

            renderTargetPin = new ColourTargetPin("Colour 0", this, PinInputType.INPUT);
            depthTargetPin = new DepthTargetPin("Depth", this, PinInputType.INPUT);
        }

        public override bool CreateResources(RenderGraph renderGraph)
        {
            return true;
        }

        public override void DrawUI()
        {
            DrawFlowPins();
            depthTargetPin.DrawUI();
            renderTargetPin.DrawUI();
        }

        public override Node Run(RenderGraph graph, Context context)
        {
            context.SetRenderTargets(depthTargetPin.GetValue(), renderTargetPin.GetValue());
            return outPin.GetConnectedPin().Node;
        }
    }
}
