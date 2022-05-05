using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class SetViewPortNode : Node
    {
        private IntPin width = null;
        private IntPin height = null;

        public SetViewPortNode() : base("Set ViewPort")
        {
            AddFlowPins();
            width = new IntPin("Width", this, PinInputType.INPUT);
            height = new IntPin("Height", this, PinInputType.INPUT);
        }

        public override bool CreateResources(RenderGraph renderGraph)
        {
            return true;
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
