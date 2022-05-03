using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolarSharp;

using SolarSharp.Rendering;
using SolarSharp.Assets;

namespace SolarEditor
{
    internal abstract class Window
    {
        protected bool show = true;
        public abstract void Show(EditorState editorState);

        public bool ShouldClose() => !show;
        public void Close() => show = false;
    }

    internal class ShaderEditorWindow : Window
    {
        private bool compileOnSave = false;

        public override void Show(EditorState editorState)
        {
            if (ImGui.Begin("Shader editor", ref show, (int)(ImGuiWindowFlags.HorizontalScrollbar | ImGuiWindowFlags.MenuBar)))
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
                        }
                        if (ImGui.MenuItem("Compile", "F5"))
                        {
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

                if (Input.IskeyJustDown(KeyCode.S) && Input.IsKeyDown(KeyCode.SHIFT_L))
                {
                    
                }
                
                ImGuiTextEditor.Render("Shader");
            }

            ImGui.End();
        }
    }


    internal class AssetSystemWindow : Window
    {
        public override void Show(EditorState editorState)
        {

            if (ImGui.Begin("Assets ", ref show))
            {
                if (ImGui.BeginTabBar("MyTabBar"))
                {
                    if (ImGui.BeginTabItem("Avocado"))
                    {
                        ImGui.Text("This is the Avocado tab!\nblah blah blah blah blah");
                        ImGui.EndTabItem();
                    }
                    if (ImGui.BeginTabItem("Broccoli"))
                    {
                        ImGui.Text("This is the Broccoli tab!\nblah blah blah blah blah");
                        ImGui.EndTabItem();
                    }
                    if (ImGui.BeginTabItem("Cucumber"))
                    {
                        ImGui.Text("This is the Cucumber tab!\nblah blah blah blah blah");
                        ImGui.EndTabItem();
                    }

                    ImGui.EndTabBar();
                }

                    
                
            }           

            ImGui.End();
        }
    }

}
