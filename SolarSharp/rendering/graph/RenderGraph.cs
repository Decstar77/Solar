using SolarSharp.Assets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

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

        private Node root = null;

        public RenderGraph(string name, Device device, Context context)
        {
            Name = name;
            this.device = device;
            this.context = context;

            Nodes = new List<Node>();
            rasterizerStates = new List<RasterizerState>();
            depthStencilStates = new List<DepthStencilState>();
            graphicsShaders = new List<GraphicsShader>();

            CreateDummy();
        }

        public bool Create()
        {
            foreach (Node node in Nodes) {
                if (!node.CreateResources(this))
                    return false;
            }

            return true;
        }

        public void Run() 
        {
            Node node = root;

            while (node != null) {
                node = node.Run(this, context);
            }
        }


        public List<Pin> GetPins()
        {
            List<Pin> pins = new List<Pin>();
            foreach (Node node in Nodes)
            {
                pins.AddRange(node.InputPins);
                pins.AddRange(node.OutputPins);
            }

            return pins;
        }

        public void Print()
        {
            foreach (Node node in Nodes)
            {
                Console.Write(node.Id + " ");
               // Console.Write(node.Name + " ");
                foreach (Pin pin in node.InputPins)
                    Console.Write(pin.Id + " " );
                foreach (Pin pin in node.OutputPins)
                    Console.Write(pin.Id + " " );
            }

            Console.WriteLine();
        }

        public void Save()
        {
            string json = JsonSerializer.Serialize<List<SerializationNode>>(Nodes.Select(x => { return x.CreateSerNode(); }).ToList());
            
            File.WriteAllText("render.json", json);

            Print();

            var nodes = JsonSerializer.Deserialize<List<SerializationNode>>(json);
            Nodes = nodes.Select(x => Node.CreateFromSerNode(x)).ToList();
            Nodes.ForEach(x => x.SerializeConnections(this));

            Print();

            int a = 2;
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

        private void CreateDummy()
        {
            CMDClearColourTargetNode clearColourTargetNode = new CMDClearColourTargetNode();
            CMDClearDepthTargetNode clearDepthTargetNode = new CMDClearDepthTargetNode();
            CMDDrawSceneNode drawSceneNode = new CMDDrawSceneNode();

            GetGraphicsShaderNode getGraphicsShaderNode = new GetGraphicsShaderNode();
            GetSwapChainNode getSwapChainNode1 = new GetSwapChainNode();
            GetSwapChainNode getSwapChainNode2 = new GetSwapChainNode();

            SetDepthStateNode setDepthNode = new SetDepthStateNode();
            SetRasterizerStateNode setRasterizerNode = new SetRasterizerStateNode();
            SetGraphicsShaderNode setGraphicsShaderNode = new SetGraphicsShaderNode();
            SetTopologyNode setTopologyNode = new SetTopologyNode();
            SetViewPortNode setViewPortNode = new SetViewPortNode();
            SetRenderTargetsNode setRenderTargetsNode = new SetRenderTargetsNode();

            setViewPortNode.Width.SetValue(1900);
            setViewPortNode.Height.SetValue(1000);

            getSwapChainNode1.SetPositionScreenSpace(new Vector2(1200, 450));
            getGraphicsShaderNode.SetPositionScreenSpace(new Vector2(200, 450));

            setDepthNode.SetPositionScreenSpace(new Vector2(0, 150)).outFlowPin.Connect(setRasterizerNode.inFlowPin);
            setRasterizerNode.SetPositionScreenSpace(new Vector2(200, 150)).outFlowPin.Connect(setTopologyNode.inFlowPin);

            setTopologyNode.SetPositionScreenSpace(new Vector2(400, 150)).outFlowPin.Connect(setViewPortNode.inFlowPin);
            setViewPortNode.SetPositionScreenSpace(new Vector2(650, 150)).outFlowPin.Connect(setGraphicsShaderNode.inFlowPin);
            setGraphicsShaderNode.SetPositionScreenSpace(new Vector2(870, 150)).outFlowPin.Connect(setRenderTargetsNode.inFlowPin);
            getGraphicsShaderNode.SetPositionScreenSpace(new Vector2(870, 450));
            getGraphicsShaderNode.ShaderPin.Connect(setGraphicsShaderNode.ShaderPin);
            getGraphicsShaderNode.shaderAsset = AssetSystem.shaderAssets[0];

            getSwapChainNode2.SetPositionScreenSpace(new Vector2(870, 0));
            getSwapChainNode2.ColourPin.Connect(setRenderTargetsNode.RenderTargetPin);
            getSwapChainNode2.DepthPin.Connect(setRenderTargetsNode.DepthTargetPin);

            setRenderTargetsNode.SetPositionScreenSpace(new Vector2(1060, 150)).outFlowPin.Connect(clearColourTargetNode.inFlowPin);

            clearColourTargetNode.SetPositionScreenSpace(new Vector2(1260, 150)).outFlowPin.Connect(clearDepthTargetNode.inFlowPin);
            clearColourTargetNode.ColourTargetPin.Connect(getSwapChainNode1.ColourPin);

            clearDepthTargetNode.SetPositionScreenSpace(new Vector2(1470, 150)).outFlowPin.Connect(drawSceneNode.inFlowPin);
            clearDepthTargetNode.DepthTargetPin.Connect(getSwapChainNode1.DepthPin);

            drawSceneNode.SetPositionScreenSpace(new Vector2(1670, 150));

            Nodes.Add(clearColourTargetNode);
            Nodes.Add(clearDepthTargetNode);
            Nodes.Add(drawSceneNode);
            Nodes.Add(setDepthNode);
            Nodes.Add(setRasterizerNode);
            Nodes.Add(setTopologyNode);
            Nodes.Add(setViewPortNode);
            Nodes.Add(setRenderTargetsNode);
            Nodes.Add(getGraphicsShaderNode);
            Nodes.Add(setGraphicsShaderNode);
            Nodes.Add(getSwapChainNode1);
            Nodes.Add(getSwapChainNode2);

            root = setDepthNode;
        }
    }
}
