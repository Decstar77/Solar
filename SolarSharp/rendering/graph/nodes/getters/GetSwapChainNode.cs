using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class GetSwapChainNode : Node
    {
        private ColourTargetPin colour = null;
        private DepthTargetPin depth = null;

        public GetSwapChainNode() : base("Swap chain")
        {
            colour = new ColourTargetPin("Colour", this, PinInputType.OUTPUT);
            depth = new DepthTargetPin("Depth", this, PinInputType.OUTPUT);
        }

        public override bool CreateResources(RenderGraph renderGraph)
        {
            colour.SetValue(RenderSystem.swapchain.renderTargetView);
            depth.SetValue(RenderSystem.swapchain.depthStencilView);

            return true;
        }

        public override void DrawUI()
        {
            depth.DrawUI();
            colour.DrawUI();
        }

        public override void Run(RenderGraph graph, Context context)
        {
            throw new NotImplementedException();
        }
    }
}
