using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class IntPin : Pin
    {
        private int value = 0;
        public IntPin(string name, bool isInputPin) : base(name, isInputPin)
        {
        }

        public override bool CanConnect(Pin pin)
        {
            return pin is IntPin;
        }

        public override void DrawUI()
        {            
            ImNodes.PushColorStyle(ImNodesCol.Pin, Util.ColourUnit(0.1f, 0.85f, 0.13f, 0.95f));
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
        public int GetValue()
        {
            if (IsConnected()) {
                return ((IntPin)connectedTo).GetValue();
            }

            return value;
        }

    }
}
