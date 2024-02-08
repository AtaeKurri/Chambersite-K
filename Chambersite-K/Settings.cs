﻿using Microsoft.Xna.Framework;
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
        public bool IsMouseVisible { get; set; } = true;
    }

    public class Settings
    {
        public string FilePath { get; set; } = "userdata/";
        public SettingData SettingData { get; set; } = new SettingData();

        public Settings()
        {

        }

        public void LoadSettings()
        {
            GAME._graphics.IsFullScreen = SettingData.IsFullscreen;
            GAME._graphics.PreferredBackBufferWidth = (int)SettingData.WindowSize.X;
            GAME._graphics.PreferredBackBufferHeight = (int)SettingData.WindowSize.Y;
            GAME._graphics.ApplyChanges();
        }
    }
}
