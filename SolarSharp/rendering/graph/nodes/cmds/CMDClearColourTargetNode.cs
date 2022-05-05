using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class CMDClearColourTargetNode : Node
    {
        public ColourTargetPin colourTargetPin = null;

        public CMDClearColourTargetNode() : base("Clear Colour Target")
        {
            AddFlowPins();
            colourTargetPin = new ColourTargetPin("Colour target", this, PinInputType.INPUT);
        }

        public override bool CreateResources(RenderGraph renderGraph)
        {
            return true;
        }

        public override void DrawUI()
        {
            DrawFlowPins();
            colourTargetPin.DrawUI();
        }

        public override Node Run(RenderGraph graph, Context context)
        {
            context.ClearRenderTargetView(colourTargetPin.GetValue(), new Vector4(0.5f, 0.1f, 0.1f, 1.0f));
            return outPin.GetConnectedPin().Node;
        }
    }
}
