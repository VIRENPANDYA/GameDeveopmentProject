using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using TANKBR.Logic;
using Microsoft.Xna.Framework.Audio;
using System;
using TANKBR.Entities;


namespace TANKBR.Entities
{
    public class PlayerTank
    {
        private SoundEffect bulletFireSound; // Add this field

        private Vector2 _position; // Backing field for position
        public Vector2 Position => _position;

        public Texture2D tankTexture;
        private float Speed;

        //private Texture2D bulletTexture;
        public List<Bullet> bullets;
        private Animation fireAnimation;
        //bullet offset
        private Vector2 barrelOffset;
        //adding animation
        private Animation impactAnimation;
        private bool isImpacting;
        private Vector2 impactAnimationPosition;
        private Vector2 impactOffset;




        public PlayerTank(Vector2 startPosition, Animation impactAnimation, Animation fireAnimation, SoundEffect bulletFireSound)
        {
            // Initial position and speed
            _position = startPosition; // Set initial position
            Speed = 200f; // Speed in pixels per second
            //adding animation
            this.impactAnimation = impactAnimation;
            this.fireAnimation = fireAnimation;
            this.bulletFireSound = bulletFireSound;
            isImpacting = false;
            //adding bullet ani
            bullets = new List<Bullet>();
            //default offset6
            impactOffset = new Vector2(12, -30);
            barrelOffset = new Vector2(25, -20);
        }

        public void LoadContent(Texture2D texture)
        {
            tankTexture = texture;

        }

        public void Update(GameTime gameTime, int windowWidth, int windowHeight)
        {
            KeyboardState state = Keyboard.GetState();
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Movement logic
            if (state.IsKeyDown(Keys.W) || state.IsKeyDown(Keys.Up)) _position.Y -= Speed * delta; // Move up
            if (state.IsKeyDown(Keys.S) || state.IsKeyDown(Keys.Down)) _position.Y += Speed * delta; // Move down
            if (state.IsKeyDown(Keys.A) || state.IsKeyDown(Keys.Left)) _position.X -= Speed * delta; // Move left
            if (state.IsKeyDown(Keys.D) || state.IsKeyDown(Keys.Right)) _position.X += Speed * delta; // Move right

            // Clamp the position within the game window boundaries
            _position.X = Math.Clamp(Position.X, 0, windowWidth - tankTexture.Width);
            _position.Y = Math.Clamp(Position.Y, 0, windowHeight - tankTexture.Height);
            // Shooting logic
            if (state.IsKeyDown(Keys.Space) && !isImpacting)
            {
                bulletFireSound.Play();
                isImpacting = true;
                impactAnimation.Reset();

                // Position animation around the tank
                impactAnimationPosition = new Vector2(
                    _position.X + (tankTexture.Width / 2) - (impactAnimation.frameWidth / 2) + impactOffset.X,
                    _position.Y + (tankTexture.Height / 2) - (impactAnimation.frameHeight / 2) + impactOffset.Y
                );

                bullets.Add(new Bullet(
                    new Vector2(_position.X + (tankTexture.Width / 2) - (fireAnimation.frameWidth / 2), _position.Y + barrelOffset.Y),
                    new Animation(fireAnimation.spriteSheet, fireAnimation.frameWidth, fireAnimation.frameHeight, fireAnimation.totalFrames, fireAnimation.frameTime)
                ));

            }
            // Update bullets
            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].Update(gameTime);
                if (!bullets[i].IsActive)
                {
                    bullets.RemoveAt(i);
                    i--; // Adjust index after removal
                }
            }

            if (isImpacting)
            {
                impactAnimation.Update(gameTime);

                if (impactAnimation.IsComplete)
                {
                    isImpacting = false;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (isImpacting)
            {
                impactAnimation.Draw(spriteBatch, impactAnimationPosition, Color.White);
            }

            // Draw bullets
            foreach (var bullet in bullets)
            {
                bullet.Draw(spriteBatch);
            }

            // Draw the player tank
            spriteBatch.Draw(tankTexture, _position, Color.White);

        }
    }
}
