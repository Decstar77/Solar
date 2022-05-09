using SolarSharp;
using SolarSharp.Assets;
using SolarSharp.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEditor
{
    internal class ShaderEditorWindow : EditorWindow
    {
        private bool compileOnSave = true;
        private bool isFocused = false;

        private ShaderAsset shaderAsset;
        public ShaderEditorWindow(ShaderAsset shaderAsset)
        {
            this.shaderAsset = shaderAsset;
            if (shaderAsset != null)
                ImGuiTextEditor.SetText(shaderAsset.Src);
        }

        public override void Start()
        {
            EventSystem.AddListener(EventType.ON_SAVE, (type, context) => {
                if (isFocused) {
                    Save();
                    return true;
                }
                return false; 
            }, this);
        }

        public override void Shutdown()
        {
            EventSystem.RemoveListner(EventType.ON_SAVE, this);
        }

        public override void Show(EditorState editorState)
        {
            isFocused = false;
            if (ImGui.Begin("Shader editor", ref show, (int)(ImGuiWindowFlags.HorizontalScrollbar | ImGuiWindowFlags.MenuBar)))
            {
                if (shaderAsset != null)
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
                            if (ImGui.MenuItem("Compile", "F5"))
                            {
                                Save();
                                Compile();
                            }

                            ImGui.EndMenu();
                        }

                        if (ImGui.BeginMenu("Edit"))
                        {
                            bool ro = ImGuiTextEditor.IsReadOnly();
                            if (ImGui.MenuItem("Read-only mode", "", ro))
                            {
                                ImGuiTextEditor.SetReadOnly(!ro);
                            }

                            bool ws = ImGuiTextEditor.IsShowingWhitespaces();
                            if (ImGui.MenuItem("Show white space", "", ws))
                            {
                                ImGuiTextEditor.SetShowWhitespaces(!ws);
                            }

                            if (ImGui.MenuItem("Compile on save", "", compileOnSave))
                            {
                                compileOnSave = !compileOnSave;
                            }

                            ImGui.EndMenu();
                        }

                        ImGui.EndMenuBar();
                    }

                    ImGui.Text(shaderAsset.Path);
                    ImGuiTextEditor.Render("Shader");

                    isFocused = ImGui.IsWindowFocused( ImGuiFocusedFlags.RootAndChildWindows  );
                }
            }
            ImGui.End();
        }

        private void Compile()
        {
            Logger.Info($"Compiling {shaderAsset.Name}");
            RenderSystem.shader.Release().Create(RenderSystem.device, shaderAsset);
        }

        private void Save()
        {
            try
            {
                if (shaderAsset != null)
                {
                    shaderAsset.Src = ImGuiTextEditor.GetText();

                    StreamWriter writer = new StreamWriter(shaderAsset.Path);
                    writer.Write(shaderAsset.Src);
                    writer.Close();

                    Logger.Info($"Saved {shaderAsset.Path}");

                    if (compileOnSave)
                    {
                        Compile();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Could not save shader {shaderAsset.Path}. Error: {ex.Message}");
            }
        }
    }
}
