using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class SetTopologyNode : Node
    {
        private PrimitiveTopology topology = PrimitiveTopology.TRIANGLELIST;

        public SetTopologyNode() : base("Set Topology State")
        {
            AddFlowPins();
        }

        public override bool CreateResources(RenderGraph renderGraph)
        {
            return true;
        }

        public override void DrawUI()
        {
            DrawFlowPins();

            Type type = typeof(PrimitiveTopology);

            string[] names = type.GetEnumNames();
            string curName = type.GetEnumName(topology);
            Array values = type.GetEnumValues();

            int index = Array.IndexOf(names, curName);

            ImGui.PushItemWidth(100);
            ImGui.Combo(type.Name, ref index, names);
            ImGui.PopItemWidth();

            topology = (PrimitiveTopology)Enum.ToObject(type, values.GetValue(index));
        }

        public override Node Run(RenderGraph graph, Context context)
        {
            context.SetPrimitiveTopology(topology);
            return outPin.GetConnectedPin().Node;
        }
    }
}
