using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolarSharp;

using SolarSharp.Rendering;
using SolarSharp.Assets;
using SolarSharp.Rendering.Graph;

namespace SolarEditor
{
    internal abstract class EditorWindow
    {
        protected bool show = true;
        public abstract void Show(EditorState editorState);

        public bool ShouldClose() => !show;
        public void Close() => show = false;
    }

    internal class ShaderEditorWindow : EditorWindow
    {
        private bool compileOnSave = true;
        private ShaderAsset shaderAsset;
        public ShaderEditorWindow(ShaderAsset shaderAsset)
        {
            this.shaderAsset = shaderAsset;
            if (shaderAsset != null)
                ImGuiTextEditor.SetText(shaderAsset.Src);
        }

        public override void Show(EditorState editorState)
        {            
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

                    if (Input.IskeyJustDown(KeyCode.S) && Input.IsKeyDown(KeyCode.CTRL_L))
                    {
                        Save();
                    }

                    ImGuiTextEditor.Render("Shader");
                }
            }
            ImGui.End();
        }

        private void Compile()
        {
            Logger.Info($"Compiling {shaderAsset.Name}");
            RenderSystem.shader.Release().Create(shaderAsset);
        }

        private void Save()
        {
            try
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
            catch (Exception ex)
            {
                Logger.Error($"Could not save shader {shaderAsset.Path}. Error: {ex.Message}");
            }            
        }
    }

   

    

}
