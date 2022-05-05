using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class SetViewPortNode : Node
    {
        public IntPin width = null;
        public IntPin height = null;

        public SetViewPortNode() : base("Set ViewPort State")
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

        public override Node Run(RenderGraph graph, Context context)
        {
            context.SetViewPortState(width.GetValue(), height.GetValue());
            return outFlowPin?.GetConnectedPin()?.Node;
        }
    }
}
