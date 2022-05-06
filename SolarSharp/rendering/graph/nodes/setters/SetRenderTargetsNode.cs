using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class SetRenderTargetsNode : Node
    {
        public ColourTargetPin RenderTargetPin { get; set; }
        public DepthTargetPin DepthTargetPin { get; set; }

        public SetRenderTargetsNode() : base("Set Render Targets")
        {
            AddFlowPins();

            RenderTargetPin = new ColourTargetPin("Colour 0", this, PinInputType.INPUT);
            DepthTargetPin = new DepthTargetPin("Depth", this, PinInputType.INPUT);
        }

        public override bool CreateResources(RenderGraph renderGraph)
        {
            return true;
        }

        public override void DrawUI()
        {
            DrawFlowPins();
            DepthTargetPin.DrawUI();
            RenderTargetPin.DrawUI();
        }

        public override Node Run(RenderGraph graph, Context context)
        {
            DepthStencilView depthStencilView = DepthTargetPin.GetValue();
            RenderTargetView renderTargetView = RenderTargetPin.GetValue();
            if (depthStencilView != null && renderTargetView != null) {
                context.SetRenderTargets(DepthTargetPin.GetValue(), RenderTargetPin.GetValue());
                return outFlowPin?.GetConnectedPin()?.Node;
            }
            return null;
        }
    }
}
