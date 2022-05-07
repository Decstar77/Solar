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
        public string Path { get; set; }
        public bool Valid { get { return created;  } }

        private bool created = false;

        private List<DXDepthStencilState> depthStencilStates;
        private List<DXRasterizerState> rasterizerStates;
        public List<GraphicsShader> graphicsShaders;

        private Node root = null;

        public RenderGraph(string name)
        {
            Name = name;

            Nodes = new List<Node>();
            rasterizerStates = new List<DXRasterizerState>();
            depthStencilStates = new List<DXDepthStencilState>();
            graphicsShaders = new List<GraphicsShader>();            
        }

        public RenderGraph(RenderGraphAsset renderGraphAsset)
        {
            Nodes = new List<Node>();
            rasterizerStates = new List<DXRasterizerState>();
            depthStencilStates = new List<DXDepthStencilState>();
            graphicsShaders = new List<GraphicsShader>();

            Nodes = renderGraphAsset.Nodes.Select(x => Node.CreateFromSerNode(x)).ToList();
            Nodes.ForEach(x => x.SerializeConnections(this));

            root = Nodes.Find(x => renderGraphAsset.RootId == x.Id);
            Name = renderGraphAsset.Name;
            Path = renderGraphAsset.Path;
        }

        public void Shutdown()
        {
            Logger.Info("Shutting down render graph: " + Name);

            rasterizerStates.ForEach(x => x.Release());
            depthStencilStates.ForEach(x => x.Release());
            graphicsShaders.ForEach(x => x.Release());

            rasterizerStates.Clear();
            depthStencilStates.Clear();
            graphicsShaders.Clear();

            created = false;
        }

        public bool Create(DXDevice device)
        {
            Logger.Info("Creating render graph: " + Name);

            foreach (Node node in Nodes) {
                if (!node.CreateResources(this, device))
                    return false;
            }

            created = true;
            return true;
        }

        public void Run(DXContext context) 
        {
            if (created) {
                Node node = root;

                while (node != null)
                {
                    node = node.Run(this, context);
                }
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
            Save(Path);
        }

        public void Save(string path)
        {
            Logger.Info("Saving: " + path);

            RenderGraphAsset renderGraphSaveData = new RenderGraphAsset();
            renderGraphSaveData.Nodes = Nodes.Select(x => { return x.CreateSerNode(); }).ToList();
            renderGraphSaveData.RootId = root.Id;
            renderGraphSaveData.Name = "A name";

            string json = JsonSerializer.Serialize<RenderGraphAsset>(renderGraphSaveData);            
            File.WriteAllText(path, json);
        }

        public void Load(string path)
        {
            Path = path;

            string json = File.ReadAllText(path);
            RenderGraphAsset renderGraphSaveData = JsonSerializer.Deserialize<RenderGraphAsset>(json);

            Nodes = renderGraphSaveData.Nodes.Select(x => Node.CreateFromSerNode(x)).ToList();
            Nodes.ForEach(x => x.SerializeConnections(this));

            root = Nodes.Find(x => renderGraphSaveData.RootId == x.Id);
            Name = renderGraphSaveData.Name;
        }

        public DXDepthStencilState CreateOrGetDepthStencilState(DepthStencilDesc desc, DXDevice device)
        {
            DXDepthStencilState state = depthStencilStates.Find(x => { return x.Description == desc; });

            if (state == null) {
                Logger.Trace("Creating new depth stencil state");
                state = device.CreateDepthStencilState(desc);
                depthStencilStates.Add(state);
            }

            return state;            
        }

        public DXRasterizerState CreateOrGetRasterizerState(RasterizerDesc desc, DXDevice device)
        {
            DXRasterizerState state = rasterizerStates.Find(x => { return x.Description == desc; });

            if (state == null) {
                Logger.Trace("Creating new rasterizer state");
                state = device.CreateRasterizerState(desc);
                rasterizerStates.Add(state);
            }

            return state;
        }

        public GraphicsShader CreateOrGetGraphicsShader(ShaderAsset shaderAsset, DXDevice device) 
        {
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

            setViewPortNode.WidthPin.SetValue(1900);
            setViewPortNode.HeightPin.SetValue(1000);

            getSwapChainNode1.SetPositionScreenSpace(new Vector2(1200, 450));
            getGraphicsShaderNode.SetPositionScreenSpace(new Vector2(200, 450));

            setDepthNode.SetPositionScreenSpace(new Vector2(0, 150)).outFlowPin.Connect(setRasterizerNode.inFlowPin);
            setRasterizerNode.SetPositionScreenSpace(new Vector2(200, 150)).outFlowPin.Connect(setTopologyNode.inFlowPin);

            setTopologyNode.SetPositionScreenSpace(new Vector2(400, 150)).outFlowPin.Connect(setViewPortNode.inFlowPin);
            setViewPortNode.SetPositionScreenSpace(new Vector2(650, 150)).outFlowPin.Connect(setGraphicsShaderNode.inFlowPin);
            setGraphicsShaderNode.SetPositionScreenSpace(new Vector2(870, 150)).outFlowPin.Connect(setRenderTargetsNode.inFlowPin);
            getGraphicsShaderNode.SetPositionScreenSpace(new Vector2(870, 450));
            getGraphicsShaderNode.ShaderPin.Connect(setGraphicsShaderNode.ShaderPin);
            getGraphicsShaderNode.ShaderName = AssetSystem.ShaderAssets[0].Name;

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
