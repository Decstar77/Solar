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
        private bool compileOnSave = false;
        public RenderGraph RenderGraph { get; set; }

        public override void Show(EditorState editorState)
        {
            if (ImGui.Begin("Render Graph", ref show, (int)ImGuiWindowFlags.MenuBar) && RenderGraph != null)
            {
                if (ImGui.BeginMenuBar())
                {
                    if (ImGui.BeginMenu("File"))
                    {
                        if (ImGui.MenuItem("Open", "Ctrl+O"))
                        {
                        }
                        if (ImGui.MenuItem("Save", "Ctrl+S"))
                        {
                            Save();
                        }

                        ImGui.EndMenu();
                    }

                    if (ImGui.BeginMenu("Edit"))
                    {
                        if (ImGui.MenuItem("Compile", "F5"))
                        {
                            Save();
                            Compile();
                        }

                        if (ImGui.MenuItem("Compile on save", "", compileOnSave))
                        {
                            compileOnSave = !compileOnSave;
                        }

                        ImGui.EndMenu();
                    }

                    ImGui.EndMenuBar();
                }

                if (!RenderGraph.Valid)
                    ImGui.Text("Error !!");

                ImNodes.BeginNodeEditor();

                RenderGraph.Nodes.ForEach(node =>
                {
                    ImNodes.BeginNode(node.Id);
                    ImNodes.BeginNodeTitleBar();
                    ImGui.Text(node.Name);
                    ImNodes.EndNodeTitleBar();

                    node.DrawUI();

                    ImNodes.EndNode();
                });

                int linkId = 0;
                RenderGraph.Nodes.ForEach(node =>
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
                    Pin startPin = RenderGraph.FindPin(startedAtPin);
                    Pin endPin = RenderGraph.FindPin(endedAtPin);

                    if (startPin != null && endPin != null)
                    {
                        startPin.Connect(endPin);
                    }
                }

                int killedPin = -1;
                if (ImNodes.IsLinkDropped(ref killedPin, true))
                {
                    RenderGraph.FindPin(killedPin)?.Disconnect();
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
                            RenderGraph.Nodes.Add(new SetGraphicsShaderNode().SetPositionScreenSpace(Application.Input.mousePositionPixelCoords));
                        }

                        if (ImGui.MenuItem("Depth"))
                        {
                            RenderGraph.Nodes.Add(new SetDepthStateNode().SetPositionScreenSpace(Application.Input.mousePositionPixelCoords));
                        }

                        if (ImGui.MenuItem("Topology"))
                        {
                            RenderGraph.Nodes.Add(new SetTopologyNode().SetPositionScreenSpace(Application.Input.mousePositionPixelCoords));
                        }

                        if (ImGui.MenuItem("Rasterizer"))
                        {
                            RenderGraph.Nodes.Add(new SetRasterizerStateNode().SetPositionScreenSpace(Application.Input.mousePositionPixelCoords));
                        }

                        if (ImGui.MenuItem("Viewport"))
                        {
                            RenderGraph.Nodes.Add(new SetViewPortNode().SetPositionScreenSpace(Application.Input.mousePositionPixelCoords));
                        }

                        if (ImGui.MenuItem("Render Targets"))
                        {
                            RenderGraph.Nodes.Add(new SetRenderTargetsNode().SetPositionScreenSpace(Application.Input.mousePositionPixelCoords));
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
                            RenderGraph.Nodes.Add(new GetSwapChainNode().SetPositionScreenSpace(Application.Input.mousePositionPixelCoords));
                        }

                        if (ImGui.MenuItem("Graphics Shader"))
                        {
                            RenderGraph.Nodes.Add(new GetGraphicsShaderNode().SetPositionScreenSpace(Application.Input.mousePositionPixelCoords));
                        }

                        ImGui.EndMenu();
                    }

                    if (ImGui.BeginMenu("Command"))
                    {
                        if (ImGui.MenuItem("Clear Colour Target"))
                        {
                            RenderGraph.Nodes.Add(new CMDClearColourTargetNode().SetPositionScreenSpace(Application.Input.mousePositionPixelCoords));
                        }

                        if (ImGui.MenuItem("Clear Depth Target"))
                        {
                            RenderGraph.Nodes.Add(new CMDClearDepthTargetNode().SetPositionScreenSpace(Application.Input.mousePositionPixelCoords));
                        }

                        if (ImGui.MenuItem("Draw Scene"))
                        {
                            RenderGraph.Nodes.Add(new CMDDrawSceneNode().SetPositionScreenSpace(Application.Input.mousePositionPixelCoords));
                        }

                        ImGui.EndMenu();
                    }

                    ImGui.EndPopup();
                }
            }
            ImGui.End();

            if (Input.IskeyJustDown(KeyCode.F5))
            {
                Compile();
            }

            if (Input.IskeyJustDown(KeyCode.S) && Input.IsKeyDown(KeyCode.CTRL_L))
            {
                Save();
            }
        }

        private void Compile()
        {
            if (GameSystem.CurrentScene.RenderGraph == RenderGraph)
            {
                RenderGraph.Shutdown();
                RenderGraph.Create(RenderSystem.device);
            }
            else
            {
                Logger.Warn($"Did not compile render graph, {RenderGraph.Name}, as it is not currently the active graph in the open scene");
            }
        }

        private void Save()
        {
            RenderGraph.Save();
            if (compileOnSave)
                Compile();
        }
    }
}
