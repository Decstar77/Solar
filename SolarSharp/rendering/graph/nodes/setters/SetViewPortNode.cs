using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class SetViewPortNode : Node
    {       
        public IntPin Width { get; set; }
        public IntPin Height { get; set; }

        public SetViewPortNode() : base("Set ViewPort State")
        {
            AddFlowPins();
            Width = new IntPin("Width", this, PinInputType.INPUT);
            Height = new IntPin("Height", this, PinInputType.INPUT);
        }

        public override bool CreateResources(RenderGraph renderGraph)
        {
            return true;
        }

        public override void DrawUI()
        {
            DrawFlowPins();

            Width.DrawUI();
            Height.DrawUI();
        }

        public override Node Run(RenderGraph graph, Context context)
        {
            context.SetViewPortState(Width.GetValue(), Height.GetValue());
            return outFlowPin?.GetConnectedPin()?.Node;
        }
    }
}
