using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolarSharp;
using SolarSharp.Rendering;
using SolarSharp.Rendering.Graph;

namespace SolarEditor
{
    internal class RenderGraphWindow : Window
    {
        public override void Show(EditorState editorState)
        {
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
                renderGraph.Nodes.ForEach(node =>
                {
                    node.OutputPins.ForEach(pin =>
                    {
                        if (pin.IsConnected() && pin.PinType == PinInputType.OUTPUT)
                        {
                            int c = pin.GetConnectedPin().Id;
                            ImNodes.Link(linkId++, pin.Id, c);
                        }
                    });
                });

                bool isEditorHovered = ImNodes.IsEditorHovered();

                ImNodes.EndNodeEditor();

                int startedAtPin = -1;
                int endedAtPin = -1;
                int ignore = -1;
                if (ImNodes.IsLinkCreated(ref startedAtPin, ref endedAtPin, ref ignore))
                {
                    Pin startPin = renderGraph.FindPin(startedAtPin);
                    Pin endPin = renderGraph.FindPin(endedAtPin);
                    
                    if (startPin != null && endPin != null)
                    {
                        startPin.Connect(endPin);
                    }
                }

                int killedPin = -1;
                if (ImNodes.IsLinkDropped(ref killedPin, true))
                {
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
                        if (ImGui.MenuItem("Graphics Shader"))
                        {
                            renderGraph.Nodes.Add(new SetGraphicsShaderNode().SetPositionScreenSpace(Application.Input.mousePositionPixelCoords));
                        }

                        if (ImGui.MenuItem("Depth"))
                        {
                            renderGraph.Nodes.Add(new SetDepthStateNode().SetPositionScreenSpace(Application.Input.mousePositionPixelCoords));
                        }

                        if (ImGui.MenuItem("Topology"))
                        {
                            renderGraph.Nodes.Add(new SetTopologyNode().SetPositionScreenSpace(Application.Input.mousePositionPixelCoords));
                        }

                        if (ImGui.MenuItem("Rasterizer"))
                        {
                            renderGraph.Nodes.Add(new SetRasterizerStateNode().SetPositionScreenSpace(Application.Input.mousePositionPixelCoords));
                        }

                        if (ImGui.MenuItem("Viewport"))
                        {
                            renderGraph.Nodes.Add(new SetViewPortNode().SetPositionScreenSpace(Application.Input.mousePositionPixelCoords));
                        }

                        if (ImGui.MenuItem("Render Targets"))
                        {
                            renderGraph.Nodes.Add(new SetRenderTargetsNode().SetPositionScreenSpace(Application.Input.mousePositionPixelCoords));
                        }

                        if (ImGui.MenuItem("Blend"))
                        {
                        }

                        ImGui.EndMenu();
                    }

                    if (ImGui.BeginMenu("Get"))
                    {
                        if (ImGui.MenuItem("Swap chain"))
                        {
                            renderGraph.Nodes.Add(new GetSwapChainNode().SetPositionScreenSpace(Application.Input.mousePositionPixelCoords));
                        }

                        if (ImGui.MenuItem("Graphics Shader"))
                        {
                            renderGraph.Nodes.Add(new GetGraphicsShaderNode().SetPositionScreenSpace(Application.Input.mousePositionPixelCoords));
                        }

                        ImGui.EndMenu();
                    }

                    if (ImGui.BeginMenu("Command"))
                    {
                        if (ImGui.MenuItem("Clear Colour Target"))
                        {
                            renderGraph.Nodes.Add(new CMDClearColourTargetNode().SetPositionScreenSpace(Application.Input.mousePositionPixelCoords));
                        }

                        if (ImGui.MenuItem("Clear Depth Target"))
                        {
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


            if (Input.IskeyJustDown(KeyCode.S) && Input.IsKeyDown(KeyCode.CTRL_L))
            {                
                renderGraph.Save("render.json");
            }

        }
    }
}
