using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class CMDClearColourTargetNode : Node
    {
        private ColourTargetPin colourTargetPin = null;

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

        public override void Run(RenderGraph graph, Context context)
        {
            throw new NotImplementedException();
        }
    }
}
