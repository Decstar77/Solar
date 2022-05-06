using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
     public class FlowPin : Pin
    {
        public FlowPin() : base("INVALID", null, PinInputType.INPUT)
        {
        }

        public FlowPin(string name, Node node, PinInputType pinType) : base(name, node, pinType)
        {
        }

        public override bool CanConnect(Pin pin)
        {
            return pin is FlowPin;
        }

        public override void DrawUI()
        {
            //ImNodes.BeginInputAttribute(InputPinId, ImNodesPinShape.CircleFilled);
            //ImGui.Text("In");
            //ImNodes.EndInputAttribute();
            //ImGui.SameLine();
            //ImNodes.BeginOutputAttribute(OutputPinId, ImNodesPinShape.CircleFilled);
            //ImGui.Text("Out");
            //ImNodes.EndOutputAttribute();
        }
    }
}
