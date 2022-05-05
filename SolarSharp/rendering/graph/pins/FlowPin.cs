using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class FlowPin : Pin
    {
        public FlowPin(string name, bool isInputPin) : base(name, isInputPin)
        {
        }

        public override bool CanConnect(Pin pin)
        {
            return pin is FlowPin;
        }

        public override void DrawUI()
        {
        }
    }
}
