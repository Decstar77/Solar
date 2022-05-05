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

        public SetTopologyNode() : base("Set Topology")
        {
            AddFlowPins();
        }

        public override void CreateResources(RenderGraph renderGraph)
        {
            throw new NotImplementedException();
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

        public override void Run(RenderGraph graph, Context context)
        {
            throw new NotImplementedException();
        }
    }
}
