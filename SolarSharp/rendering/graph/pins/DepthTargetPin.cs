using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class DepthTargetPin : ValuePin<DepthStencilView>
    {
        public DepthTargetPin() : base("INVALID", null, PinInputType.INPUT)
        {
        }

        public DepthTargetPin(string name, Node node, PinInputType pinType) : base(name, node, pinType)
        {
        }

        public override bool CanConnect(Pin pin)
        {
            return pin is DepthTargetPin;
        }

        public override void DrawUI()
        {
            ImNodes.PushColorStyle(ImNodesCol.Pin, Util.ColourUnit(0.16f, 0.85f, 0.43f, 0.95f));
            DrawBasicPins();
            ImNodes.PopColorStyle();
        }
    }

}
