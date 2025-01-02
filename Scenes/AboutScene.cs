using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TANKBR.Logic;
using TANKBR.Controller;

namespace TANKBR.Scenes
{
    public class AboutScene
    {
        private SpriteFont font;
        private string aboutText = "Tank BR\n" +
                                   "Created by: Viren Pandya\n" +
                                   "Student Number: 8911979\n" +

                                   "Game Programming Project : Tank BR";

        public void LoadContent(SpriteFont font)
        {
            this.font = font;
        }

        public void Update(GameTime gameTime, ref GameState gameState)
        {
            // Return to menu on Escape
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                gameState = GameState.Menu;
            }
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
        {
            var viewport = graphics.GraphicsDevice.Viewport;

            // Draw about text
            var textPosition = new Vector2(50, viewport.Height / 3f);
            spriteBatch.DrawString(font, aboutText, textPosition, Color.White);

            // Draw back instruction
            var backInstruction = "Press Enter or Escape to return to Menu";
            var backPosition = new Vector2(50, viewport.Height - 100);
            spriteBatch.DrawString(font, backInstruction, backPosition, Color.Yellow);
        }
    }
}
