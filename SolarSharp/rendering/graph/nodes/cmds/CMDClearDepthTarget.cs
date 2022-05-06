using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class CMDClearDepthTargetNode : Node
    {
        public DepthTargetPin DepthTargetPin { get; set; } 

        public CMDClearDepthTargetNode() : base("Clear Depth Target")
        {
            AddFlowPins();
            DepthTargetPin = new DepthTargetPin("Depth target", this, PinInputType.INPUT);
        }

        public override bool CreateResources(RenderGraph renderGraph)
        {
            return true;
        }

        public override void DrawUI()
        {
            DrawFlowPins();
            DepthTargetPin.DrawUI();
        }

        public override Node Run(RenderGraph graph, Context context)
        {
            DepthStencilView depthStencilView = DepthTargetPin.GetValue();

            if (depthStencilView != null) {
                context.ClearDepthStencilView(DepthTargetPin.GetValue(), ClearFlag.D3D11_CLEAR_DEPTH, 0.0f, 0);
                return outFlowPin?.GetConnectedPin()?.Node;
            }

            return null;
        }
    }
}
