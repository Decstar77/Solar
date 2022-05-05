using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class IntPin : ValuePin<int>
    {
        public IntPin(string name, Node node, PinInputType inputType) : base(name, node, inputType)
        {
        }

        public override bool CanConnect(Pin pin)
        {
            return pin is IntPin;
        }

        public override void DrawUI()
        {            
            ImNodes.PushColorStyle(ImNodesCol.Pin, Util.ColourUnit(0.1f, 0.85f, 0.13f, 0.95f));
            
            if (PinType == PinInputType.INPUT)
            {
                ImNodes.BeginInputAttribute(Id, ImNodesPinShape.CircleFilled);
                ImGui.Text(Name + " ");
                ImNodes.EndInputAttribute();
            }
            else if (PinType == PinInputType.OUTPUT)
            {
                ImNodes.BeginInputAttribute(Id, ImNodesPinShape.CircleFilled);
                ImGui.Text(Name + " ");
                ImNodes.EndInputAttribute();
            }

            ImNodes.PopColorStyle();

            ImGui.SameLine();
            if (!IsConnected()) {
                ImGui.PushItemWidth(100);
                ImGui.PushId(Id);                
                ImGui.InputInt("", ref value);
                ImGui.PopId();
                ImGui.PopItemWidth();
            }
            else {
                ImGui.Text(((IntPin)connectedTo).GetValue().ToString());
            }
        }
 

    }
}
