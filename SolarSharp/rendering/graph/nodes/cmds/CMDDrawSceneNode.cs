using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class CMDDrawSceneNode : Node
    {
        public CMDDrawSceneNode() : base("Draw Scene")
        {
            AddFlowPins();
        }

        public override bool CreateResources(RenderGraph renderGraph, DXDevice device)
        {
            return true;
        }

        public override void DrawUI()
        {
            DrawFlowPins();
        }

        public override Node Run(RenderGraph graph, DXContext context)
        {
            return null;
        }
    }
}
