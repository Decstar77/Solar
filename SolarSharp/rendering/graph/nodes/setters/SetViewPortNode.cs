using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class SetViewPortNode : Node
    {
        [RenderGraphSerializable]
        public int Width { get { return WidthPin.GetValue(); } set { WidthPin.SetValue(value); } }
        [RenderGraphSerializable]
        public int Height { get { return HeightPin.GetValue(); } set { HeightPin.SetValue(value); } }

        public IntPin WidthPin { get; set; }
        public IntPin HeightPin { get; set; }

        public SetViewPortNode() : base("Set ViewPort State")
        {
            AddFlowPins();
            WidthPin = new IntPin("Width", this, PinInputType.INPUT);
            HeightPin = new IntPin("Height", this, PinInputType.INPUT);
        }

        public override bool CreateResources(RenderGraph renderGraph)
        {
            return true;
        }

        public override void DrawUI()
        {
            DrawFlowPins();

            WidthPin.DrawUI();
            HeightPin.DrawUI();
        }

        public override Node Run(RenderGraph graph, Context context)
        {
            context.SetViewPortState(WidthPin.GetValue(), HeightPin.GetValue());
            return outFlowPin?.GetConnectedPin()?.Node;
        }
    }
}
