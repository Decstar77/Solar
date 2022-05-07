using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Assets
{
    public struct SerNode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IDictionary<string, object> Pins { get; set; }
        public string ClassName { get; set; }
        public IDictionary<string, object> ClassData { get; set; }
        public float PositionEditorSpaceX { get; set; }
        public float PositionEditorSpaceY { get; set; }
    }

    public class RenderGraphAsset
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public int RootId { get; set; }
        public List<SerNode> Nodes { get; set; }
    }
}
