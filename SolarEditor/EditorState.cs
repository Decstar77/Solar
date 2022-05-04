﻿using System;
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
        
        public RenderGraph renderGraph = new RenderGraph("My Render Graph");

        internal void UIDraw()
        {
            ImGui.BeginFrame();
            DrawGlobalMenu();

            if (ImGui.Begin(renderGraph.Name))
            {
                ImNodes.BeginNodeEditor();

                renderGraph.Nodes.ForEach(node =>
                {
                    ImNodes.BeginNode(node.Id);
                    ImNodes.BeginNodeTitleBar();
                    ImGui.Text(node.Name);
                    ImNodes.EndNodeTitleBar();

                    node.InputPins.ForEach(pin => {
                        ImNodes.BeginInputAttribute(pin.Id, ImNodesPinShape.CircleFilled);
                        ImGui.Text(pin.Name);
                        ImNodes.EndInputAttribute();

                    });

                    node.OutputPins.ForEach(pin => {
                        ImNodes.BeginOutputAttribute(pin.Id, ImNodesPinShape.CircleFilled);
                        ImGui.Text(pin.Name);
                        ImNodes.EndOutputAttribute();
                    });

                    ImGui.Text("stuff");
                    ImNodes.EndNode();
                });

                int linkId = 0;
                renderGraph.Nodes.ForEach(node => {
                    node.OutputPins.ForEach(pin => { 
                        if (pin.IsConnected()) {
                            ImNodes.Link(linkId, pin.Id, pin.GetConnectedPin().Id);
                        }
                    });
                });
                
                if (Input.IsMouseButtonJustDown(MouseButton.MOUSE2) )
                {
                    Logger.Info("Hit");
                }

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
