using SolarSharp;
using SolarSharp.Assets;
using SolarSharp.Rendering;
using SolarSharp.Rendering.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEditor
{
    internal class GameSceneWindow : EditorWindow
    {
        public override void Start()
        {
            EventSystem.AddListener(EventType.ON_SAVE, (type, context) => { return false; }, this);
        }

        public override void Shutdown()
        {
            EventSystem.RemoveListner(EventType.ON_SAVE, this);
        }

        public override void Show(EditorState editorState)
        {
            GameScene gameScene = editorState.displayScene;

            if (ImGui.Begin("Current Scene", ref show))
            {
                string name = gameScene.name;
                if (ImGui.InputText("Name", ref name)) {
                    gameScene.name = name;
                }

                string[] renderGraphAssets = AssetSystem.RenderGraphs.Select(x => x.Name).ToArray();
                int currentRenderGraphAsset = AssetSystem.RenderGraphs.FindIndex(x => x.Name == gameScene.RenderGraph?.Name);

                if (ImGui.Combo("Render graph", ref currentRenderGraphAsset, renderGraphAssets))
                {
                    if (gameScene.RenderGraph != null)
                    {
                        gameScene.RenderGraph.Shutdown();
                    }

                    gameScene.RenderGraph = AssetSystem.RenderGraphs[currentRenderGraphAsset];
                }

                ImGui.Text(nameof(editorState.airGame.PlaneModel) + " " + editorState.airGame.PlaneModel);

                if (ImGui.CollapsingHeader("Entities", 0))
                {
                    Entity[] entities = gameScene.GetAllEntities();

                    for (int i = 0; i < entities.Length; i++)
                    {
                        Entity entity = entities[i];

                        ImGui.PushId(i);
                        if (ImGui.Selectable(entity.Name))
                        {
                            editorState.AddWindow(new EntityWindow());
                            editorState.selection.Set(entity, true);
                        }
                        ImGui.PopId();

                    }
                }
            }

            ImGui.End();
        }
    }
}
