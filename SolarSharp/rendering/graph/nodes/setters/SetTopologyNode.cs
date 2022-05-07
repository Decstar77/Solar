using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class SetTopologyNode : Node
    {
        [RenderGraphSerializable]
        public PrimitiveTopology Topology { get; set; }
        

        public SetTopologyNode() : base("Set Topology State")
        {
            AddFlowPins();
            Topology = PrimitiveTopology.TRIANGLELIST;
        }

        public override bool CreateResources(RenderGraph renderGraph, DXDevice device)
        {
            return true;
        }

        public override void DrawUI()
        {
            DrawFlowPins();

            Type type = typeof(PrimitiveTopology);

            string[] names = type.GetEnumNames();
            string curName = type.GetEnumName(Topology);
            Array values = type.GetEnumValues();

            int index = Array.IndexOf(names, curName);

            ImGui.PushItemWidth(100);
            ImGui.Combo(type.Name, ref index, names);
            ImGui.PopItemWidth();

            Topology = (PrimitiveTopology)Enum.ToObject(type, values.GetValue(index));
        }

        public override Node Run(RenderGraph graph, DXContext context)
        {
            context.SetPrimitiveTopology(Topology);
            return outFlowPin?.GetConnectedPin()?.Node;
        }
    }
}
