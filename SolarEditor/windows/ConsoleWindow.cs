using SolarSharp;
using SolarSharp.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEditor
{
    internal class ConsoleWindow : EditorWindow
    {
        private static bool AutoScroll = true;

        public override void Start()
        {
        }

        public override void Shutdown()
        {
        }

        public override void Show(EditorState editorState)
        {            
            if (ImGui.Begin("Console", ref show))
            {
                if (ImGui.Button("Clear")) {
                    Logger.ClearLogs();
                }

                ImGui.CheckBox("Auto Scroll", ref AutoScroll);

                ImGui.Separator();
                float footer = ImGui.GetStyleItemSpacingY() + ImGui.GetFrameHeightWithSpacing();
                if (ImGui.BeginChild("ScrollingRegion", 0, -footer, false, ImGuiWindowFlags.HorizontalScrollbar))
                {
                    ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, 4, 1);

                    List<string> logs = Logger.GetLogs();
                    foreach (string log in logs)
                    {
                        if (log.StartsWith("TRACE"))
                        {
                            ImGui.TextColored(0.7f, 0.7f, 0.7f, 0.7f, log + "\n");
                        }
                        else if (log.StartsWith("INFO"))
                        {
                            ImGui.TextColored(0.2f, 0.8f, 0.2f, 1.0f, log + "\n");
                        }
                        else if (log.StartsWith("WARN"))
                        {
                            ImGui.TextColored(0.8f, 0.7f, 0.2f, 1.0f, log + "\n");
                        }
                        else if (log.StartsWith("ERROR"))
                        {
                            ImGui.TextColored(0.8f, 0.2f, 0.2f, 1.0f, log + "\n");
                        }
                        else
                        {
                            ImGui.TextUnformatted(log + "\n");
                        }
                    }

                    if (AutoScroll && ImGui.GetScrollY() >= ImGui.GetScrollMaxY())
                    {
                        ImGui.SetScrollHereY(1.0f);
                    }

                    ImGui.PopStyleVar();
                }

                ImGui.EndChild();
                ImGui.Separator();
            }



            ImGui.End();
        }
    }
}
