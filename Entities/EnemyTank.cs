using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TANKBR.Entities
{
    public class EnemyTanks
    {
        public Vector2 position; // Position of the enemy tank
        private Texture2D _texture;
        public float _speed; // Movement speed

        public bool IsActive { get; set; } // Track if the enemy is active

        public EnemyTanks(Texture2D texture, Vector2 startPosition)
        {
            _texture = texture;
            position = startPosition;
            _speed = 120f; // Enemy movement speed
            IsActive = true; // Mark enemy as active
        }

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Move enemy downward
            position.Y += _speed * delta;

            // Deactivate the enemy if it moves off-screen
            if (position.Y > 818) // Assuming screen height is 818
            {
                IsActive = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, position, Color.White);
        }
    }
}
