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
    public class HelpScene
    {
        private SpriteFont font;
        private string helpText = "Controls:\n" +
                                  "W/A/S/D or Arrow Keys: Move Tank\n" +
                                  "Space: Fire Bullets\n" +
                                  "Tab: Start the Game (Enter Name)\n" +
                                  "H: Open Help Scene\n" +
                                  "A: Open About Scene\n" +
                                  "E: Exit Game\n" +
                                  "Escape: Return to Menu from Play, Help, or About\n";

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

            // Draw help text
            var textPosition = new Vector2(50, viewport.Height / 4f);
            spriteBatch.DrawString(font, helpText, textPosition, Color.White);

            // Draw back instruction
            var backInstruction = "Press Escape to return to Menu";
            var backPosition = new Vector2(50, viewport.Height - 100);
            spriteBatch.DrawString(font, backInstruction, backPosition, Color.Yellow);
        }

    }
}
