using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class RenderGraph
    {
        public List<Node> Nodes { get; set; }
        public string Name { get; set; }

        public RenderGraph(string name)
        {
            Name = name;
            Nodes = new List<Node>();
            Node cs = new Node("Create shader");
            Node ss = new Node("Set Shader");

            cs.OutputPins.Add(new Pin("ouput"));
            cs.InputPins.Add(new Pin("input"));
            ss.InputPins.Add(new Pin("input"));

            cs.OutputPins[0].Connect( ss.InputPins[0] );

            Nodes.Add(cs);
            Nodes.Add(ss);
        }

        public Node Find( Pin pin )
        {
            foreach ( Node node in Nodes )
            {
                if (node.InputPins.Contains(pin))
                    return node;
                if (node.OutputPins.Contains(pin))
                    return node;
            }

            return null;
        }

        public Pin FindPin(int pinId)
        {
            foreach (Node node in Nodes)
            {
                foreach (Pin pin in node.OutputPins)
                {
                    if (pin.Id == pinId) return pin;
                }

                foreach (Pin pin in node.InputPins)
                {
                    if (pin.Id == pinId) return pin;
                }
            }

            return null;
        }
    }
}
