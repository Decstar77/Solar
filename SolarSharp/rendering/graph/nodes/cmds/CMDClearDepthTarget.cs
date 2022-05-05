using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class CMDClearDepthTargetNode : Node
    {
        public DepthTargetPin depthTargetPin = null;

        public CMDClearDepthTargetNode() : base("Clear Depth Target")
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

        public override Node Run(RenderGraph graph, Context context)
        {
            DepthStencilView depthStencilView = depthTargetPin.GetValue();

            if (depthStencilView != null) {
                context.ClearDepthStencilView(depthTargetPin.GetValue(), ClearFlag.D3D11_CLEAR_DEPTH, 0.0f, 0);
                return outFlowPin?.GetConnectedPin()?.Node;
            }

            return null;
        }
    }
}
