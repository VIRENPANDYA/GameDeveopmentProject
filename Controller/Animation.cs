using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TANKBR.Logic
{
    public class Animation
    {
        public Texture2D spriteSheet; // The sprite sheet containing all frames
        public int frameWidth; // Width of each frame
        public int frameHeight; // Height of each frame
        public int totalFrames; // Total number of frames in the animation
        public float frameTime; // Duration of each frame in seconds
        public float timer; // Tracks time elapsed
        public int currentFrame; // Current frame index


        public bool IsComplete { get; private set; } // Check if animation is finished

        public Animation(Texture2D spriteSheet, int frameWidth, int frameHeight, int totalFrames, float frameTime)
        {
            this.spriteSheet = spriteSheet;
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
            this.totalFrames = totalFrames;
            this.frameTime = frameTime;
            timer = 0f;
            currentFrame = 0;
            IsComplete = false;
        }
        public void Update(GameTime gameTime)
        {
            if (IsComplete) return;

            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timer >= frameTime)
            {
                timer = 0f;
                currentFrame++;
                if (currentFrame >= totalFrames)
                {
                    currentFrame = 0;
                    IsComplete = true; // Mark as complete when all frames are played
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color)
        {
            if (IsComplete) return;

            Rectangle sourceRectangle = new Rectangle(
                currentFrame * frameWidth, 0,
                frameWidth, frameHeight
            );

            spriteBatch.Draw(spriteSheet, position, sourceRectangle, color);
        }
        public void Reset()
        {
            currentFrame = 0;
            timer = 0f;
            IsComplete = false;
        }
    }
}
