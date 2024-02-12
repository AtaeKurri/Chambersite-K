using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K
{
    public struct SettingData
    {
        public SettingData() { }

        public bool IsFullscreen { get; set; } = false;
        public Vector2 WindowSize { get; set; } = new Vector2(1600, 900);
        public Vector2 ViewportSize { get; set; } = new Vector2(853, 480);
        public bool IsMouseVisible { get; set; } = true;
        public Keybindings Keybindings { get; set; } = new Keybindings();
    }

    public struct Keybindings
    {
        public Keybindings() { }

        public Keys Up { get; set; } = Keys.Up;
        public Keys Down { get; set; } = Keys.Down;
        public Keys Left { get; set; } = Keys.Left;
        public Keys Right { get; set; } = Keys.Right;
        public Keys Shoot { get; set; } = Keys.W;
        public Keys Bomb { get; set; } = Keys.X;
        public Keys Special { get; set; } = Keys.C;
    }

    public class Settings
    {
        public string FilePath { get; set; } = "userdata/";
        public SettingData SettingData { get; set; } = new SettingData();

        public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public Settings()
        {

        }

        public void LoadSettings()
        {
            GAME._graphics.IsFullScreen = SettingData.IsFullscreen;
            GAME._graphics.PreferredBackBufferWidth = (int)SettingData.WindowSize.X;
            GAME._graphics.PreferredBackBufferHeight = (int)SettingData.WindowSize.Y;
            GAME._graphics.ApplyChanges();
            Logger.Info("Settings Loaded and applied.");
        }

        public Matrix GetViewportScale()
        {
            float scaleX = SettingData.WindowSize.X / SettingData.ViewportSize.X;
            float scaleY = SettingData.WindowSize.Y / SettingData.ViewportSize.Y;
            Matrix scaleMatrix = Matrix.CreateScale(scaleX, scaleY, 1.0f);
            return scaleMatrix;
        }
    }
}
