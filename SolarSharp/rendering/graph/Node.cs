using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class Node
    {
        private static int IdCounter = 0;
        public int Id { get { return id; } }
        private int id = -1;

        public List<Pin> OutputPins { get; set; }
        public List<Pin> InputPins { get; set; }

        public string Name { get; set; }

        public Node()
        {
            id = IdCounter++;
            OutputPins = new List<Pin>();
            InputPins = new List<Pin>();
        }

        public Node(string name)
        {
            id = IdCounter++;
            Name = name;
            OutputPins = new List<Pin>();
            InputPins = new List<Pin>();
        }
    }
}
