using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TANKBR.Logic;

namespace TANKBR.Entities
{
    public class Bullet
    {
        public Vector2 Position { get; private set; }
        public bool IsActive { get; private set; }

        public Animation fireAnimation; // fire animation for bullet
        public float speed;

        public Bullet(Vector2 startPosition, Animation fireAnimation)
        {
            Position = startPosition;
            this.fireAnimation = fireAnimation;
            speed = 300f; // Speed of the bullet
            IsActive = true;
        }
        public void Update(GameTime gameTime)
        {
            // Move the bullet upward
            Position = new Vector2(Position.X, Position.Y - speed * (float)gameTime.ElapsedGameTime.TotalSeconds);

            // Deactivate the bullet if it goes off-screen
            if (Position.Y < -fireAnimation.frameHeight)
            {
                IsActive = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                fireAnimation.Draw(spriteBatch, Position, Color.White);
            }
        }

    }
}
