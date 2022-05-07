using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class CMDClearColourTargetNode : Node
    {
        public ColourTargetPin ColourTargetPin { get; set; }

        public CMDClearColourTargetNode() : base("Clear Colour Target")
        {
            AddFlowPins();
            ColourTargetPin = new ColourTargetPin("Colour target", this, PinInputType.INPUT);
        }

        public override bool CreateResources(RenderGraph renderGraph, DXDevice device)
        {
            return true;
        }

        public override void DrawUI()
        {
            DrawFlowPins();
            ColourTargetPin.DrawUI();
        }

        public override Node Run(RenderGraph graph, DXContext context)
        {
            RenderTargetView renderTargetView = ColourTargetPin.GetValue();
            if (renderTargetView != null) {
                context.ClearRenderTargetView(renderTargetView, new Vector4(0.5f, 0.1f, 0.1f, 1.0f));
                return outFlowPin?.GetConnectedPin()?.Node;
            }

            return null;
        }
    }
}
