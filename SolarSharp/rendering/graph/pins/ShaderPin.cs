using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class ShaderPin : Pin
    {
        public ShaderPin(string name, bool isInputPin) : base(name, isInputPin)
        {
        }

        public override bool CanConnect(Pin pin)
        {
            return pin is ShaderPin;
        }

        public override void DrawUI()
        {
            ImNodes.PushColorStyle(ImNodesCol.Pin, Util.ColourUnit( Util.ColourVec(185, 43, 204, 245) ));
            if (IsInputPin)
            {
                ImNodes.BeginInputAttribute(Id, ImNodesPinShape.CircleFilled);
                ImGui.Text(Name + " ");
                ImNodes.EndInputAttribute();
            }
            else
            {
                ImNodes.BeginInputAttribute(Id, ImNodesPinShape.CircleFilled);
                ImGui.Text(Name + " ");
                ImNodes.EndInputAttribute();
            }
            ImNodes.PopColorStyle();
        }
        public GraphicsShader GetValue()
        {
            if (IsConnected()) {
                return ((ShaderPin)connectedTo).GetValue();
            }

            return null;
        }

    }
}
