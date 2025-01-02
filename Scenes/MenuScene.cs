using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TANKBR.Logic;
using TANKBR.Controller;

namespace TANKBR.Scenes
{
    public class MenuScene
    {
        private SpriteFont font;
        private string title = "Tank Battle Royal";

        private int selectedIndex = 0; // Track which menu item is selected
        private string[] menuItems = { "Play", "Help(H)", "About(A)", "Exit(E)" };

        private MouseState currentMouseState;
        private MouseState previousMouseState;
        private Vector2 mousePosition;

        public void LoadContent(SpriteFont font)
        {
            this.font = font;
        }
        public void Update(GameTime gameTime, ref GameState gameState, GraphicsDeviceManager graphics)
        {
            // Handle input to switch to gameplay
            KeyboardState state = Keyboard.GetState();
            currentMouseState = Mouse.GetState();
            mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);

            // Select with Enter key
            if (state.IsKeyDown(Keys.Enter))
            {
                ExecuteMenuAction(ref gameState, selectedIndex);
            }

            // Check for hover effect and clicks
            var viewport = graphics.GraphicsDevice.Viewport;
            for (int i = 0; i < menuItems.Length; i++)
            {
                // Calculate bounding rectangle for each menu item
                var menuItemPosition = new Vector2(viewport.Width / 2f, viewport.Height / 2f + i * 30);
                var menuItemSize = font.MeasureString(menuItems[i]);
                var menuItemBounds = new Rectangle((int)(menuItemPosition.X - menuItemSize.X / 2), (int)menuItemPosition.Y, (int)menuItemSize.X, (int)menuItemSize.Y);

                // Check if mouse is hovering over the menu item
                if (menuItemBounds.Contains(currentMouseState.Position))
                {
                    selectedIndex = i; // Highlight hovered option

                    // If left mouse button is clicked
                    if (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
                    {
                        ExecuteMenuAction(ref gameState, selectedIndex);
                    }
                }

            }
            //store the current mouse state for the next frame
            previousMouseState = currentMouseState;
        }
        private void ExecuteMenuAction(ref GameState gameState, int menuIndex)
        {
            switch (menuIndex)
            {
                case 0: gameState = GameState.Play; break;
                case 1: gameState = GameState.Help; break;
                case 2: gameState = GameState.About; break;
                case 3: Environment.Exit(0); break;
            }
        }
        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
        {
            var viewport = graphics.GraphicsDevice.Viewport;
            Vector2 menuPosition = new Vector2(viewport.Width / 2f, viewport.Height / 2f);
            Vector2 titlePosition = new Vector2(viewport.Width / 2f, viewport.Height / 3f);

            // Draw title
            spriteBatch.DrawString(font, title, titlePosition - font.MeasureString(title) / 2, Color.White);

            // Draw menu items
            for (int i = 0; i < menuItems.Length; i++)
            {
                var color = i == selectedIndex ? Color.IndianRed : Color.White;
                var position = menuPosition + new Vector2(0, i * 30);
                spriteBatch.DrawString(font, menuItems[i], position - font.MeasureString(menuItems[i]) / 2, color);
            }
        }
    }
}
