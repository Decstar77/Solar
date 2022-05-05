using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class GraphicsShaderPin : ValuePin<GraphicsShader>
    {
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

            if (PinType.HasFlag(PinInputType.INPUT))
            {
                ImNodes.BeginInputAttribute(Id, ImNodesPinShape.CircleFilled);
                ImGui.Text(Name + " ");
                ImNodes.EndInputAttribute();
            }
            else if (PinType.HasFlag(PinInputType.OUTPUT))
            {
                ImNodes.BeginInputAttribute(Id, ImNodesPinShape.CircleFilled);
                ImGui.Text(Name + " ");
                ImNodes.EndInputAttribute();
            }

            ImNodes.PopColorStyle();
        }
    }
}
