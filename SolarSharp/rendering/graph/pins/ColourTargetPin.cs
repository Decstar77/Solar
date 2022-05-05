using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class ColourTargetPin : ValuePin<RenderTargetView>
    {
        public ColourTargetPin(string name, Node node, PinInputType pinType) : base(name, node, pinType)
        {
        }

        public override bool CanConnect(Pin pin)
        {
           return pin is ColourTargetPin;
        }

        public override void DrawUI()
        {
            ImNodes.PushColorStyle(ImNodesCol.Pin, Util.ColourUnit(0.66f, 0.15f, 0.33f, 0.95f));
            DrawBasicPins();
            ImNodes.PopColorStyle();
        }
    }

}
