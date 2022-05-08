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
        public override void Show(EditorState editorState)
        {
            GameScene gameScene = GameSystem.CurrentScene;

            if (ImGui.Begin("Current Scene", ref show))
            {
                string name = gameScene.Name;
                ImGui.InputText("Name", ref name);
                gameScene.Name = name;

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

                

                //ImGui.Combo("RenderGraph", )
            }

            ImGui.End();
        }
    }
}
