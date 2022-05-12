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
    internal class AssetSystemWindow : EditorWindow
    {
        public override void Start()
        {            
        }
        public override void Shutdown()
        {
        }

        public override void Show(EditorState editorState)
        {
            if (ImGui.Begin("Assets ", ref show))
            {
                if (ImGui.BeginTabBar("MyTabBar"))
                {
                    if (ImGui.BeginTabItem("Models"))
                    {
                        ImGui.Columns(4, "modelColumns", true);
                        ImGui.Separator();

                        ImGui.Text("Name"); ImGui.NextColumn();
                        ImGui.Text("Actions"); ImGui.NextColumn();
                        ImGui.Text("GUID"); ImGui.NextColumn();
                        ImGui.Text("Path"); ImGui.NextColumn();

                        int idCounter = 0;

                        ModelAsset? reimportModel = null;
                        AssetSystem.GetSortedModelAssets().ForEach(x => {
                            ImGui.Separator();
                            ImGui.Text(x.name); ImGui.NextColumn();

                            ImGui.PushId(idCounter++);

                            if (ImGui.Button("View")) {
                                editorState.AddWindow(new ModelViewerWindow(x));
                            }
                            ImGui.SameLine();
                            if (ImGui.Button("Reimport")) {
                                reimportModel = x;
                            }

                            ImGui.PopId();
                            ImGui.NextColumn();

                            ImGui.Text(x.Guid.ToString()); ImGui.NextColumn();                      
                            ImGui.Text(x.path); ImGui.NextColumn();
                        });

                        if (reimportModel != null)
                        {
                            Task.Run(() => {
                                ModelImporter modelImporter = new ModelImporter(reimportModel.path);
                                if (modelImporter.Loaded) {
                                    ModelAsset modelAsset = modelImporter.LoadModel();
                                    modelAsset.Guid = reimportModel.Guid;

                                    AssetSystem.RemoveModelAsset(modelAsset.Guid);
                                    AssetSystem.AddModelAsset(modelAsset);

                                    RenderSystem.DeregisterModel(modelAsset);
                                    RenderSystem.RegisterModel(modelAsset);
                                }
                                else
                                {
                                    Logger.Error($"Could not reimport {reimportModel.name}");
                                }
                            });
                        }

                        ImGui.Columns(1);
                        ImGui.EndTabItem();
                    }
                    if (ImGui.BeginTabItem("Textures"))
                    {
                        ImGui.Columns(4, "textureColumns", true);
                        ImGui.Separator();

                        ImGui.Text("Name"); ImGui.NextColumn();
                        ImGui.Text("Actions"); ImGui.NextColumn();
                        ImGui.Text("GUID"); ImGui.NextColumn();
                        ImGui.Text("Path"); ImGui.NextColumn();

                        int idCounter = 0;
                        AssetSystem.GetSortedTextureAssets().ForEach(x => {
                            ImGui.Separator();
                            ImGui.Text(x.name); ImGui.NextColumn();

                            ImGui.PushId(idCounter++);
                            if (ImGui.Button("Edit"))
                            {

                            }
                            ImGui.PopId();
                            ImGui.NextColumn();

                            ImGui.Text(x.Guid.ToString()); ImGui.NextColumn();
                            ImGui.Text(x.path); ImGui.NextColumn();
                        });

                        ImGui.Columns(1);
                        ImGui.EndTabItem();
                    }
                    if (ImGui.BeginTabItem("Materials"))
                    {
                        ImGui.Columns(2, "materialColumns", true);
                        ImGui.Separator();

                        ImGui.Text("Name"); ImGui.NextColumn();
                        ImGui.Text("Actions"); ImGui.NextColumn();

                        int idCounter = 0;
                        AssetSystem.GetSortedMaterialAssets().ForEach(x => {
                            ImGui.Separator();
                            ImGui.Text(x.name); ImGui.NextColumn();

                            ImGui.PushId(idCounter++);
                            if (ImGui.Button("Edit"))
                            {

                            }
                            ImGui.PopId();
                            ImGui.NextColumn();
                        });

                        ImGui.Columns(1);
                        ImGui.EndTabItem();
                    }
                    if (ImGui.BeginTabItem("Shaders"))
                    {
                        ImGui.Columns(3, "shaderColumns", true);
                        ImGui.Separator();

                        ImGui.Text("Name"); ImGui.NextColumn();
                        ImGui.Text("Path"); ImGui.NextColumn();
                        ImGui.Text("Actions"); ImGui.NextColumn();

                        int idCounter = 0;
                        AssetSystem.ShaderAssets.ForEach(x => {
                            ImGui.Separator();
                            ImGui.Text(x.Name); ImGui.NextColumn();
                            ImGui.Text(x.Path); ImGui.NextColumn();
                            ImGui.PushId(idCounter++);
                            if (ImGui.Button("Edit"))
                            {
                                editorState.AddWindow(new ShaderEditorWindow(x));
                            }
                            ImGui.PopId();
                            ImGui.SameLine();
                            ImGui.PushId(idCounter++);
                            if (ImGui.Button("Delete"))
                            {

                            }
                            ImGui.PopId();
                            ImGui.NextColumn();
                        });

                        ImGui.Columns(1);
                        ImGui.Separator();

                        ImGui.EndTabItem();
                    }
                    if (ImGui.BeginTabItem("Scenes"))
                    {
                        ImGui.Columns(4, "gameScenesColumns", true);
                        ImGui.Separator();

                        ImGui.Text("Name"); ImGui.NextColumn();
                        ImGui.Text("Actions"); ImGui.NextColumn();
                        ImGui.Text("GUID"); ImGui.NextColumn();
                        ImGui.Text("Path"); ImGui.NextColumn();

                        int idCounter = 0;
                        AssetSystem.GetScenesAssets().ForEach(x => {
                            ImGui.Separator();
                            ImGui.Text(x.name); ImGui.NextColumn();

                            ImGui.PushId(idCounter++);
                            if (ImGui.Button("Open"))
                            {
                                //AssetSystem.LoadGameSceneAsset()
                            }
                            ImGui.PopId();
                            ImGui.NextColumn();

                            ImGui.Text(x.Guid.ToString()); ImGui.NextColumn();
                            ImGui.Text("Path"); ImGui.NextColumn();
                        });

                        ImGui.Columns(1);
                        ImGui.EndTabItem();
                    }
                    if (ImGui.BeginTabItem("Render Graphs"))
                    {
                        ImGui.Columns(3, "RenderColumns", true);
                        ImGui.Separator();

                        ImGui.Text("Name"); ImGui.NextColumn();
                        ImGui.Text("Path"); ImGui.NextColumn();
                        ImGui.Text("Actions"); ImGui.NextColumn();

                        int idCounter = 0;
                        AssetSystem.RenderGraphs.ForEach(x => {
                            ImGui.Separator();
                            ImGui.Text(x.Name); ImGui.NextColumn();
                            ImGui.Text(x.Path); ImGui.NextColumn();
                            ImGui.PushId(idCounter++);
                            if (ImGui.Button("Edit"))
                            {
                                editorState.AddWindow(new RenderGraphWindow()).RenderGraph = x;
                            }
                            ImGui.PopId();
                            ImGui.SameLine();
                            ImGui.PushId(idCounter++);
                            if (ImGui.Button("Delete"))
                            {

                            }
                            ImGui.PopId();
                            ImGui.NextColumn();
                        });

                        ImGui.Columns(1);
                        ImGui.Separator();

                        ImGui.EndTabItem();
                    }
                    ImGui.EndTabBar();
                }
            }

            ImGui.End();
        }
    }
}
