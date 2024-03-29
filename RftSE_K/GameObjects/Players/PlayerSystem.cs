﻿using Chambersite_K;
using Chambersite_K.GameObjects;
using Chambersite_K.Graphics;
using Chambersite_K.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using RftSE_K.GameObjects;

namespace RftSE_K.GameObjects.Players
{
    public enum PlayerDeathState
    {
        Alive,
        Dead,
    }

    [InternalName("Player System")]
    public abstract class PlayerSystem : GameObject
    {
        public float Speed { get; set; } = 4.5f;
        public float FocusSpeed { get; set; } = 2.0f;
        public bool Focus { get; set; } = false;
        public bool IsLocked { get; set; } = false;

        public int ProtectTime { get; set; } = 0;
        public List<PlayerSupport> Supports { get; set; } = new List<PlayerSupport>();
        public GameObject? Target { get; set; } = null;

        private int Death { get; set; } = 0;
        public PlayerDeathState DeathState { get; set; } = PlayerDeathState.Alive;
        public bool SoftDeath { get; set; } = false;
        public bool IsInDialog { get; set; } = false;

        private int NextShoot { get; set; } = 0;
        private int NextBomb { get; set; } = 0;
        private int NextSpecial { get; set; } = 0;

        public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update()
        {
            FrameUpdateDeathState();
            FrameUpdateFocus();
            FrameControls();
            FrameMove();
            FrameItemCollect();
            FrameDeathMiss();
            FrameDeathReset();
            FrameUpdateVariables();
            FrameUpdateSupports();
            base.Update();
        }

        public void Shoot()
        {

        }

        public void Bomb()
        {

        }

        public void Special()
        {

        }

        #region Frame Steps

        private void FrameUpdateDeathState()
        {

        }

        private void FrameUpdateFocus()
        {
            if (DeathState != PlayerDeathState.Alive || Status != GameObjectStatus.Active)
                return;
            if (Keyboard.GetState().IsKeyDown(MainProcess.Settings.GetKey("Focus")))
                Focus = true;
            else
                Focus = false;
        }

        private void FrameControls()
        {
            if (DeathState != PlayerDeathState.Alive || Status != GameObjectStatus.Active)
                return;
            if (!IsInDialog)
            {
                if (Keyboard.GetState().IsKeyDown(MainProcess.Settings.GetKey("Shoot")) && NextShoot <= 0)
                    Shoot();
                if (Keyboard.GetState().IsKeyPressedOnce(MainProcess.Settings.GetKey("Bomb")) && NextBomb <= 0)
                    Bomb();
                if (Keyboard.GetState().IsKeyPressedOnce(MainProcess.Settings.GetKey("Special")) && NextSpecial <= 0)
                    Special();
            }
            else
            {
                NextShoot = 0;
                NextBomb = 0;
                NextSpecial = 0;
            }
        }

        private void FrameMove()
        {
            if (DeathState != PlayerDeathState.Alive || Status != GameObjectStatus.Active)
                return;
            Vector3 movementInfo = new Vector3(0, 0, Speed);
            if (Focus) movementInfo.Z = FocusSpeed;

            if (Keyboard.GetState().IsKeyDown(MainProcess.Settings.GetKey("Up")))
                movementInfo.Y -= 1;
            if (Keyboard.GetState().IsKeyDown(MainProcess.Settings.GetKey("Down")))
                movementInfo.Y += 1;
            if (Keyboard.GetState().IsKeyDown(MainProcess.Settings.GetKey("Left")))
                movementInfo.X -= 1;
            if (Keyboard.GetState().IsKeyDown(MainProcess.Settings.GetKey("Right")))
                movementInfo.X += 1;
            if (movementInfo.X * movementInfo.Y != 0)
                movementInfo.Z *= MathF.Sqrt(0.5f);

            movementInfo.X = movementInfo.Z * movementInfo.X;
            movementInfo.Y = movementInfo.Z * movementInfo.Y;

            Position += new Vector2(movementInfo.X, movementInfo.Y);
            Vector2 XOffset = new Vector2(ParentView.WorldBounds.WorldLeft + ParentView.WorldBounds.BoundsOffsets.X,
                ParentView.WorldBounds.WorldRight - ParentView.WorldBounds.BoundsOffsets.X);
            Vector2 YOffset = new Vector2(ParentView.WorldBounds.WorldTop + ParentView.WorldBounds.BoundsOffsets.Y,
                ParentView.WorldBounds.WorldBottom - ParentView.WorldBounds.BoundsOffsets.Y);
            Position = Vector2.Clamp(Position, new Vector2(XOffset.X, YOffset.X), new Vector2(XOffset.Y, YOffset.Y));
        }

        private void FrameItemCollect()
        {

        }

        private void FrameDeathMiss()
        {
            if (DeathState != PlayerDeathState.Dead)
                return;

            Miss();
            SoftDeath = VariableHolder.PlayerHealth != 0 ? true : false;
            if (SoftDeath)
            {
                // Do the deletion of bullets, and the player death animation
            }
            Hidden = true;

        }

        private void FrameDeathReset()
        {
            if (!SoftDeath)
            {
                Position = new Vector2(0f, -236);
                // Set the supports pos too
                Hidden = false;
            }
        }

        private void FrameUpdateVariables()
        {

        }

        private void FrameUpdateSupports()
        {

        }

        #endregion

        public void Miss()
        {

        }
    }
}
