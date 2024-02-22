using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using ImGuiNET;

namespace Chambersite_K.ImGUI
{
    public abstract class ImGuiLayout
    {
        private bool showWindow = false;
        public virtual bool ShowWindow
        {
            get => showWindow; set => showWindow = value;
        }

        public void SetBaseBehaviour(bool showByDefault)
        {
            ShowWindow = showByDefault;
        }

        public bool StartLayout(string windowName, Vector2 windowSize)
        {
            if (!showWindow) return false;
            ImGui.SetNextWindowSize(windowSize, ImGuiCond.Appearing);
            if (ImGui.Begin(windowName, ref showWindow))
            {
                return true;
            }
            return false;
        }

        public void EndLayout()
        {
            ImGui.End();
        }
    }
}
