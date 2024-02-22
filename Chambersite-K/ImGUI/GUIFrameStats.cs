using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.ImGUI
{
    internal class GUIFrameStats : ImGuiLayout
    {
        public void Draw(FrameCounter drawFPS, FrameCounter updateFPS)
        {
            if (StartLayout("Frame Statistics", new Vector2(600, 600)))
            {
                if (ImGui.CollapsingHeader("Frame Time"))
                {
                    // Revoir tout ça, parce que ça fait pas très sens que ça soit tout le temps à 16ms
                    ImGui.Text(string.Format("Frame Per Seconds: {0:0.00}FPS", updateFPS.CurrentFramesPerSecond));
                    ImGui.Text(string.Format("Total Time: {0:0.000}ms",
                        drawFPS.GameTime.ElapsedGameTime.TotalMilliseconds + updateFPS.GameTime.ElapsedGameTime.TotalMilliseconds));
                    ImGui.Text(string.Format("Frame Time: {0:0.000}ms", updateFPS.GameTime.ElapsedGameTime.TotalMilliseconds));
                    ImGui.Text(string.Format("Render Time: {0:0.000}ms", drawFPS.GameTime.ElapsedGameTime.TotalMilliseconds));
                }
                if (ImGui.CollapsingHeader("Memory Usage"))
                {
                    //ImGui.Text($"Frame Compute Time: {GameTime.ElapsedGameTime.TotalMilliseconds}ms");
                    ImGui.Text($"Render Time: {0.0f}ms");
                }
                if (ImGui.CollapsingHeader("Objects"))
                {
                    //ImGui.Text($"Frame Compute Time: {GameTime.ElapsedGameTime.TotalMilliseconds}ms");
                    ImGui.Text($"Render Time: {0.0f}ms");
                }

                EndLayout();
            }
            
        }
    }
}
