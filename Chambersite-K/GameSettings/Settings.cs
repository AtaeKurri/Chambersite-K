using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.GameSettings
{
    public class Settings
    {
        public string FilePath { get; set; } = "userdata/";
        public List<(string, Keys)> Keybinds { get; set; } =
        [
            new ("Up", Keys.Up),
            new ("Down", Keys.Down),
            new ("Left", Keys.Left),
            new ("Right", Keys.Right),
            new ("Shoot", Keys.W),
            new ("Bomb", Keys.X),
            new ("Special", Keys.C),
            new ("Focus", Keys.LeftShift),
        ];

        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public virtual bool IsFullscreen { get; set; } = false;
        public virtual Vector2 WindowSize { get; set; } = new Vector2(1600, 900);
        public virtual Vector2 ViewportSize { get; set; } = new Vector2(853, 480);
        public virtual bool IsMouseVisible { get; set; } = true;
        public float BGMVolume { get; private set; } = .5f; // Always clamp from 0 to 100.
        public float SEVolume { get; private set; } = .5f; // Same, clamp from 0 to 100.
        public SamplerState SampleState { get; set; } = SamplerState.LinearWrap;

        public Settings()
        {

        }

        public void LoadSettings()
        {
            GAME._graphics.IsFullScreen = IsFullscreen;
            GAME._graphics.PreferredBackBufferWidth = (int)WindowSize.X;
            GAME._graphics.PreferredBackBufferHeight = (int)WindowSize.Y;

            GAME._graphics.ApplyChanges();
            Logger.Info("Settings Loaded and applied.");
        }

        public Matrix GetViewportScale()
        {
            float scaleX = WindowSize.X / ViewportSize.X;
            float scaleY = WindowSize.Y / ViewportSize.Y;
            Matrix translationMatrix = Matrix.CreateTranslation(ViewportSize.X / 2, ViewportSize.Y / 2, 1f);
            Matrix scaleMatrix = Matrix.CreateScale(scaleX, scaleY, 1f);
            return translationMatrix * scaleMatrix;
        }

        public void SetBGMVolume(float volume) => BGMVolume = MathHelper.Clamp(volume, 0f, 1f);
        public void SetSEVolume(float volume) => SEVolume = MathHelper.Clamp(volume, 0f, 1f);

        /// <summary>
        /// Try to get a <see cref="Keys"/> from a corresponding identifier.
        /// </summary>
        /// <param name="identifier">The string to identify a Key by its name.</param>
        /// <returns>A <see cref="Keys"/> code.</returns>
        /// <exception cref="KeyNotFoundException">Is thrown if the identifier doesn't not match any exisiting one.</exception>
        public Keys GetKey(string identifier)
        {
            (string, Keys) key = Keybinds.Find(x => x.Item1 == identifier);
            if (key == default) { throw new KeyNotFoundException("This Keybind does not exist, check case or any typos."); }
            return key.Item2;
        }
    }
}
