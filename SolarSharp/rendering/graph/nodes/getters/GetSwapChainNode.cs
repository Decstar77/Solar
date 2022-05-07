using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class GetSwapChainNode : Node
    {
        public ColourTargetPin ColourPin { get; set; }
        public DepthTargetPin DepthPin { get; set; }

        public GetSwapChainNode() : base("Swap chain")
        {
            ColourPin = new ColourTargetPin("Colour", this, PinInputType.OUTPUT);
            DepthPin = new DepthTargetPin("Depth", this, PinInputType.OUTPUT);
        }

        public override bool CreateResources(RenderGraph renderGraph, DXDevice device)
        {
            ColourPin.SetValue(RenderSystem.swapchain.renderTargetView);
            DepthPin.SetValue(RenderSystem.swapchain.depthStencilView);

            return true;
        }

        public override void DrawUI()
        {
            DepthPin.DrawUI();
            ColourPin.DrawUI();
        }

        public override Node Run(RenderGraph graph, DXContext context)
        {
            return null;
        }
    }
}
