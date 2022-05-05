﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class GraphicsShaderPin : ValuePin<GraphicsShader>
    {
        public GraphicsShaderPin(string name, Node node, PinInputType pinInputType) : base(name, node, pinInputType)
        {
        }

        public override bool CanConnect(Pin pin)
        {
            return pin is GraphicsShaderPin;
        }

        public override void DrawUI()
        {
            ImNodes.PushColorStyle(ImNodesCol.Pin, Util.ColourUnit( Util.ColourVec(185, 43, 204, 245) ));
            DrawBasicPins();
            ImNodes.PopColorStyle();
        }
    }
}
