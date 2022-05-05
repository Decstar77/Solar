using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class SetViewPortNode : Node
    {
        private IntPin width = new IntPin("Width", true);
        private IntPin height = new IntPin("Height", true);

        public SetViewPortNode() : base("Set ViewPort")
        {
            AddFlowPins();

            InputPins.Add(width);
            InputPins.Add(height);
        }

        public override void CreateResources(RenderGraph renderGraph)
        {
        }

        public override void DrawUI()
        {
            DrawFlowPins();

            width.DrawUI();
            height.DrawUI();
        }

        public override void Run(RenderGraph graph, Context context)
        {
            context.SetViewPortState(width.GetValue(), height.GetValue());
        }
    }
}
