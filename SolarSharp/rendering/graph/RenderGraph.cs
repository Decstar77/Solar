using SolarSharp.Assets;
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

        private List<DepthStencilState> depthStencilStates;
        private List<RasterizerState> rasterizerStates;
        private List<GraphicsShader> graphicsShaders;

        private Device device;
        private Context context;

        public RenderGraph(string name, Device device, Context context)
        {
            Name = name;
            this.device = device;
            this.context = context;

            Nodes = new List<Node>();
            Node cs = new SetDepthNode();
            Node ss = new SetDepthNode();

            rasterizerStates = new List<RasterizerState>();
            depthStencilStates = new List<DepthStencilState>();
            graphicsShaders = new List<GraphicsShader>();

            //cs.OutputPins[0].Connect( ss.InputPins[0] );

            Nodes.Add(cs);
            Nodes.Add(new SetRasterizerNode());

            //Nodes.Add(ss);
        }

        public DepthStencilState CreateOrGetDepthStencilState(DepthStencilDesc desc)
        {
            DepthStencilState state = depthStencilStates.Find(x => { return x.Description == desc; });

            if (state == null) {
                Logger.Trace("Creating new depth stencil state");
                state = device.CreateDepthStencilState(desc);
                depthStencilStates.Add(state);
            }

            return state;            
        }

        public RasterizerState CreateOrGetRasterizerState(RasterizerDesc desc)
        {
            RasterizerState state = rasterizerStates.Find(x => { return x.Description == desc; });

            if (state == null) {
                Logger.Trace("Creating new rasterizer state");
                state = device.CreateRasterizerState(desc);
                rasterizerStates.Add(state);
            }

            return state;
        }

        public GraphicsShader CreateOrGetGraphicsShader(ShaderAsset shaderAsset) {
            GraphicsShader shader = graphicsShaders.Find(x => { return x.Name == shaderAsset.Name; });

            if (shader == null) {
                Logger.Trace("Creating new graphics shader " + shaderAsset.Name);
                shader = new GraphicsShader(device).Create(shaderAsset);
                graphicsShaders.Add(shader);
            }

            return shader;
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
