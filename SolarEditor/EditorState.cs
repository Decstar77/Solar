using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolarSharp;
using SolarSharp.Rendering;
using SolarSharp.Rendering.Graph;
using SolarSharp.Assets;

namespace SolarEditor
{
    internal class EditorState
    {
        private List<Window> windows = new List<Window>();
        private List<Window> newWindows = new List<Window>();

        internal EditorState()
        {
            AddWindow(new AssetSystemWindow());
            EventSystem.Listen(EventType.RENDER_END, (EventType type, object context) => { UIDraw(); return false; });
        }

        private string path = "C:/Users/claud/OneDrive/Desktop/DeclanStuff/Solar/EngineAssets/";
        private string name = "";
        private bool open = false;


        internal void UIDraw()
        {
            ImGui.BeginFrame();
            DrawGlobalMenu();
            
            RenderGraph renderGraph = Application.renderGraph;

            if (ImGui.Begin(renderGraph.Name))
            {
                ImNodes.BeginNodeEditor();

                renderGraph.Nodes.ForEach(node =>
                {
                    ImNodes.BeginNode(node.Id);
                    ImNodes.BeginNodeTitleBar();
                    ImGui.Text(node.Name);
                    ImNodes.EndNodeTitleBar();

                    node.DrawUI();

                    ImNodes.EndNode();
                });

                int linkId = 0;
                renderGraph.Nodes.ForEach(node => {
                    node.OutputPins.ForEach(pin => { 
                        if (pin.IsConnected() && pin.PinType == PinInputType.OUTPUT) {
                            ImNodes.Link(linkId++, pin.Id, pin.GetConnectedPin().Id);
                        }
                    });
                });

                bool isEditorHovered = ImNodes.IsEditorHovered();

                ImNodes.EndNodeEditor();

                int startedAtPin = -1;
                int endedAtPin = -1;
                int ignore = -1;
                if (ImNodes.IsLinkCreated(ref startedAtPin, ref endedAtPin, ref ignore)) {
                    Pin startPin = renderGraph.FindPin(startedAtPin);
                    Pin endPin = renderGraph.FindPin(endedAtPin);

                    if (startPin != null && endPin != null)
                    {
                        startPin.Connect(endPin);    
                    }                    
                }

                int killedPin = -1;
                if (ImNodes.IsLinkDropped(ref killedPin, true)) {
                    renderGraph.FindPin(killedPin)?.Disconnect();
                }

                if (Input.IsMouseButtonJustDown(MouseButton.MOUSE2) && isEditorHovered)
                {
                    ImGui.OpenPopup("Create Node");
                }

                if (ImGui.BeginPopup("Create Node"))
                {
                    if (ImGui.BeginMenu("Set"))
                    {
                        if (ImGui.MenuItem("Graphics Shader")) {
                            renderGraph.Nodes.Add(new SetGraphicsShaderNode().SetPositionScreenSpace(Application.Input.mousePositionPixelCoords));
                        }

                        if (ImGui.MenuItem("Depth")) {
                            renderGraph.Nodes.Add(new SetDepthStateNode().SetPositionScreenSpace(Application.Input.mousePositionPixelCoords));
                        }

                        if (ImGui.MenuItem("Topology")) {
                            renderGraph.Nodes.Add(new SetTopologyNode().SetPositionScreenSpace(Application.Input.mousePositionPixelCoords));
                        }

                        if (ImGui.MenuItem("Rasterizer")) {
                            renderGraph.Nodes.Add(new SetRasterizerStateNode().SetPositionScreenSpace(Application.Input.mousePositionPixelCoords));
                        }

                        if (ImGui.MenuItem("Viewport")) {
                            renderGraph.Nodes.Add(new SetViewPortNode().SetPositionScreenSpace(Application.Input.mousePositionPixelCoords));
                        }

                        if (ImGui.MenuItem("Render Targets"))
                        {
                            renderGraph.Nodes.Add(new SetRenderTargetsNode().SetPositionScreenSpace(Application.Input.mousePositionPixelCoords));
                        }

                        if (ImGui.MenuItem("Blend")) {
                        }

                        ImGui.EndMenu();
                    }

                    if (ImGui.BeginMenu("Get"))
                    {
                        if (ImGui.MenuItem("Swap chain")) {
                            renderGraph.Nodes.Add(new GetSwapChainNode().SetPositionScreenSpace(Application.Input.mousePositionPixelCoords));
                        }

                        if (ImGui.MenuItem("Graphics Shader")) {
                            renderGraph.Nodes.Add(new GetGraphicsShaderNode().SetPositionScreenSpace(Application.Input.mousePositionPixelCoords));
                        }

                        ImGui.EndMenu();
                    }

                    if (ImGui.BeginMenu("Command"))
                    {
                        if (ImGui.MenuItem("Clear Colour Target")) {
                            renderGraph.Nodes.Add(new CMDClearColourTargetNode().SetPositionScreenSpace(Application.Input.mousePositionPixelCoords));
                        }

                        if (ImGui.MenuItem("Clear Depth Target")) {
                            renderGraph.Nodes.Add(new CMDClearDepthTargetNode().SetPositionScreenSpace(Application.Input.mousePositionPixelCoords));
                        }

                        if (ImGui.MenuItem("Draw Scene"))
                        {
                            renderGraph.Nodes.Add(new CMDDrawSceneNode().SetPositionScreenSpace(Application.Input.mousePositionPixelCoords));
                        }

                        ImGui.EndMenu();
                    }

                    ImGui.EndPopup();
                }
            }
            ImGui.End();


            if (open)
            {
                open = false;
                ImGui.OpenPopup("Create Shader");
            }

            if (ImGui.BeginPopupModal("Create Shader", ImGuiWindowFlags.NoResize))
            {
                ImGui.InputText("Name", ref name);
                ImGui.InputText("Path", ref path);

                if (ImGui.Button("Create", 80, 0))
                {
                    RenderSystem.graphicsShaders.Add(ShaderFactory.CreateGraphicsShader(name, path));
                    ImGui.CloseCurrentPopup();
                }
                ImGui.SameLine();
                if (ImGui.Button("Cancel", 80, 0))
                {
                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndPopup();
            }

            ShowWindows();
            ImGui.EndFrame();
        }

        internal void Update()
        {

        }

        internal void Shutdown()
        {
          
        }

        internal bool AddWindow(Window window)
        {
            foreach (Window w in windows)
                if (w.GetType() == window.GetType())
                    return false;

            newWindows.Add(window);
            return true;
        }

        internal void ShowWindows()
        {
            windows.AddRange(newWindows);
            newWindows.Clear();
            windows.ForEach(x => x.Show(this));
            windows.RemoveAll(x => x.ShouldClose());            
        }

        private void DrawGlobalMenu()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.BeginMenu("New"))
                    {
                        if (ImGui.MenuItem("Graphics"))
                        {
                            open = true;
                        }
                        if (ImGui.MenuItem("Compute"))
                        {

                        }

                        ImGui.EndMenu();
                    }

                    if (ImGui.MenuItem("Open"))
                    {

                    }

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("View"))
                {
                    if (ImGui.MenuItem("Shader Editor"))
                    {

                    }

                    if (ImGui.MenuItem("Assets"))
                    {

                    }

                    ImGui.EndMenu();
                }

                ImGui.EndMainMenuBar();
            }
        }
    }
}
