using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class GraphicsShaderPin : ValuePin<string>
    {
        [RenderGraphSerializable]
        public string ShaderName { get { return GetValue(); } set { SetValue(value); } }


        public GraphicsShaderPin() : base("INVALID", null, PinInputType.INPUT)
        {
        }

        public GraphicsShaderPin(string name, Node node, PinInputType pinInputType) : base(name, node, pinInputType)
        {
        }

        public override bool CanConnect(Pin pin)
        {
            return pin is GraphicsShaderPin;
        }

        public override void DrawUI()
        {
            ImNodes.PushColorStyle(ImNodesCol.Pin, Util.ColourUnit( Util.ColourVec(185, 43, 204, 245) ));
            DrawBasicPins();
            ImNodes.PopColorStyle();
        }
    }
}
