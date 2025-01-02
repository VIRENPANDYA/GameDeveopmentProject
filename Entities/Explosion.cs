using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TANKBR.Logic;

namespace TANKBR.Entities
{
    public class Explosion
    {
        private Animation explosionAnimation;
        private Vector2 explosionOffset;
        public Vector2 Position { get; private set; }
        public bool IsActive { get; private set; }

        public Explosion(Vector2 position, Animation explosionAnimation)
        {
            this.Position = position;
            this.explosionAnimation = explosionAnimation;
            this.explosionAnimation.Reset();
            IsActive = true;
            explosionOffset = new Vector2(12, -30);

        }

        public void Update(GameTime gameTime)
        {
            if (IsActive)
            {
                explosionAnimation.Update(gameTime);

                if (explosionAnimation.IsComplete)
                {
                    IsActive = false; // Deactivate when animation finishes
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                explosionAnimation.Draw(spriteBatch, Position, Color.White);
            }
        }
    }
}
