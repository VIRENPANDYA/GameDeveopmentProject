using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using TANKBR.Entities;
using Microsoft.Xna.Framework.Audio;

//adding scenes
using TANKBR.Scenes;
using TANKBR.Controller;
using TANKBR.Logic;

namespace TANKBR
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;


        //new level
        private int level = 1; // Track the current level
        private bool levelTransition = false; // Flag for showing level transition message
        private float transitionTimer = 0f; // Timer for level transition message
   
        private string playerName = ""; // Player's name
        private double lastKeyPressTime = 0;
        private const double KeyCooldown = 0.2;


        private Texture2D bgMapTexture; // Background map texture
        private PlayerTank playerTank; // Player tank instance

        //adding impact animation
        private Texture2D shootingSpriteSheet;
        private Animation shootingAnimation;

        //firing animation
        private Texture2D fireAnimationSheet;
        private Animation fireAnimation;

        //explosion animation
        private Texture2D explosionSpriteSheet; // Explosion animation sprite sheet
        private List<Explosion> explosions; // List of active explosions
        private Animation explosionAnimation;

        //on enemy now
        private Texture2D enemyTankTexture; // Enemy tank texture
        private Random random; // Random number generator
        private List<EnemyTanks> enemies; // List of enemies
        private float enemySpawnTimer; // Timer for spawning enemies
        private float spawnInterval = 1f; // Interval between spawns (in seconds)

        //lives counter
        private int lives = 3; // Player starts with 3 lives
        private SpriteFont font; // Font for displaying lives and game over
        private bool isGameOver = false; // Track game-over state

        //adding scenes
        private GameState currentState = GameState.Menu; // Start with menu scene
        private MenuScene menuScene; // Menu Scene instance
        private HelpScene helpScene;
        private AboutScene aboutScene;

        // Declare SoundEffect variables
        private SoundEffect bulletFireSound;
        private SoundEffect explosionSound;
        private SoundEffect backgroundMusic;
        private SoundEffectInstance backgroundMusicInstance;

        //fileds for store timer score and gameover state 
        private float gameTimeRemaining = 30f; // 30 seconds game timer
        private int score = 0; // Player's score
        private string gameResult = ""; // "You Won!" or "Game Lost"


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Set window size
            _graphics.PreferredBackBufferWidth = 740;
            _graphics.PreferredBackBufferHeight = 740;
            _graphics.ApplyChanges();

            // Initialize random and enemies list
            random = new Random();
            enemies = new List<EnemyTanks>();

            explosions = new List<Explosion>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load background texture
            bgMapTexture = Content.Load<Texture2D>("BGI");

            // Load and initialize the player tank
            Texture2D playerTankTexture = Content.Load<Texture2D>("PlayerTank");

            //adding animation content
            shootingSpriteSheet = Content.Load<Texture2D>("ImpactAnimation01");
            // CreatING shooting animation (6 frames, each 64x64, 0.1s per frame)
            shootingAnimation = new Animation(shootingSpriteSheet, 138, 165, 6, 0.1f);

            //adding explosion content
            explosionSpriteSheet = Content.Load<Texture2D>("ExplosionAnimation01");
            explosionAnimation = new Animation(explosionSpriteSheet, 106, 193, 6, 0.1f);

            //firing animation load content
            fireAnimationSheet = Content.Load<Texture2D>("FireingAnimation01");
            fireAnimation = new Animation(fireAnimationSheet, 138, 165, 6, 0.1f);
            bulletFireSound = Content.Load<SoundEffect>("bulletTankFiring");

            Vector2 playerSpawnPosition = new Vector2(
                _graphics.PreferredBackBufferWidth / 2 - 32, // Center horizontally (assuming 64x64 texture)
                _graphics.PreferredBackBufferHeight - 94 // Just above the bottom edge
            );
            playerTank = new PlayerTank(playerSpawnPosition, shootingAnimation, fireAnimation, bulletFireSound);
            playerTank.LoadContent(playerTankTexture);

            //work on enemy
            enemyTankTexture = Content.Load<Texture2D>("EnemyTank1");
            //enemyTankTexture = Content.Load<Texture2D>("EnemyTank2");
            font = Content.Load<SpriteFont>("DefaultFont");

            //adding scenes
            menuScene = new MenuScene();
            menuScene.LoadContent(font); // Pass the loaded font

            helpScene = new HelpScene();
            helpScene.LoadContent(font);

            aboutScene = new AboutScene();
            aboutScene.LoadContent(font);

            // Load sound effects
            explosionSound = Content.Load<SoundEffect>("Tankexplosion");
            backgroundMusic = Content.Load<SoundEffect>("BGM");

            // Create instance for background music
            backgroundMusicInstance = backgroundMusic.CreateInstance();
            backgroundMusicInstance.IsLooped = true; // Loop background music
            backgroundMusicInstance.Play();

        }

        protected override void Update(GameTime gameTime)
        {
            if (currentState == GameState.Play && Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                playerTank.Update(gameTime, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight); // Go back to Menu
            }

            //Handle scene updates
            switch (currentState)
            {
                case GameState.Menu:
                    menuScene.Update(gameTime, ref currentState, _graphics);
                    if (currentState == GameState.Play)
                    {
                        currentState = GameState.NameInput; // Transition to NameInput state
                    }
                    break;
                case GameState.NameInput:
                    KeyboardState keyState = Keyboard.GetState();
                    Keys[] pressedKeys = keyState.GetPressedKeys();

                    foreach (var key in pressedKeys)
                    {
                        if (gameTime.TotalGameTime.TotalSeconds - lastKeyPressTime > KeyCooldown)
                        {
                            if (key == Keys.Tab)
                            {
                                currentState = GameState.Play; // Start the game when Enter is pressed
                            }
                            else if (key == Keys.Back && playerName.Length > 0)
                            {
                                playerName = playerName.Substring(0, playerName.Length - 1); // Handle Backspace
                            }
                            else if (key != Keys.LeftShift && key != Keys.RightShift && playerName.Length < 10)
                            {
                                playerName += key.ToString(); // Add typed characters to the player's name
                            }
                            lastKeyPressTime = gameTime.TotalGameTime.TotalSeconds; // Update last key press time

                        }

                    }
                    break;
                case GameState.Play:
                    enemySpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    //enemy collision logic here
                    if (!isGameOver)
                    {
                        //Level 1
                        if (level == 1)
                        {
                            gameTimeRemaining -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                            if (gameTimeRemaining <= 0f)
                            {
                                // Transition to Level 2
                                level = 2;
                                levelTransition = true;
                                transitionTimer = 3f; // Display transition message for 3 seconds

                                // Adjust difficulty for Level 2
                                spawnInterval = 0.5f; // Faster spawning
                                foreach (var enemy in enemies)
                                {
                                    enemy._speed += 50f; // Increase enemy speed
                                }

                                // Reset timer for Level 2
                                gameTimeRemaining = 0f;
                            }
                        }
                        // Level 2: Count-up Timer
                        if (level == 2)
                        {
                            gameTimeRemaining += (float)gameTime.ElapsedGameTime.TotalSeconds;

                            if (gameTimeRemaining >= 30f)
                            {
                                Console.Write("Enter your name: ");
                                isGameOver = true; // Game ends after 60 seconds total
                                gameResult = score >= 10 ? "You Won!" : "Game Lost"; // Decide the game result
                            }
                        }

                        // Handle level transition message timer
                        if (levelTransition)
                        {
                            transitionTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                            if (transitionTimer <= 0f)
                            {
                                levelTransition = false; // Stop showing the transition message
                            }
                        }

                        // Update player tank movement
                        playerTank.Update(gameTime, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
                        // Update all enemies
                        for (int i = 0; i < enemies.Count; i++)
                        {
                            var enemy = enemies[i];
                            if (enemy != null)
                            {
                                enemy.Update(gameTime);
                                //lets add collision detection
                                Rectangle playerBounds = new Rectangle(
                                    (int)playerTank.Position.X, (int)playerTank.Position.Y,
                                    playerTank.tankTexture.Width, playerTank.tankTexture.Height
                                );

                                Rectangle enemyBounds = new Rectangle(
                                    (int)enemy.position.X, (int)enemy.position.Y,
                                    enemyTankTexture.Width, enemyTankTexture.Height
                                );
                                if (playerBounds.Intersects(enemyBounds))
                                {
                                    lives--; // Decrement lives
                                    enemies.RemoveAt(i); // Remove the collided enemy
                                    i--; // Adjust index after removal

                                    // Check if all lives are lost
                                    if (lives <= 0)
                                    {
                                        isGameOver = true;
                                        gameResult = "Game Lost";
                                        break;
                                    }
                                }
                                // Check for collision between bullets and enemy tanks
                                for (int j = 0; j < playerTank.bullets.Count; j++)
                                {
                                    var bullet = playerTank.bullets[j];
                                    if (bullet != null)
                                    {
                                        Rectangle bulletBounds = new Rectangle(
                                            (int)bullet.Position.X,
                                            (int)bullet.Position.Y,
                                            fireAnimation.frameWidth,
                                            fireAnimation.frameHeight
                                        );


                                        if (bulletBounds.Intersects(enemyBounds))
                                        {
                                            score++;
                                            // Play explosion sound
                                            explosionSound.Play();
                                            // Spawn explosion at enemy's position
                                            Vector2 explosionPosition = new Vector2(
                                                enemy.position.X + enemyTankTexture.Width / 2 - explosionAnimation.frameWidth / 2,
                                                enemy.position.Y + enemyTankTexture.Height / 2 - explosionAnimation.frameHeight / 2
                                            );
                                            // Spawn explosion at enemy's position
                                            explosions.Add(new Explosion(explosionPosition, explosionAnimation));

                                            // Remove bullet and enemy
                                            playerTank.bullets.RemoveAt(j);
                                            j--;
                                            enemies.RemoveAt(i);
                                            i--;
                                            break;
                                        }

                                    }

                                }

                            }

                        }

                        //bg sound should be over when game is over
                        if (isGameOver && backgroundMusicInstance.State == SoundState.Playing)
                        {
                            backgroundMusicInstance.Stop();
                        }
                        // Update explosions
                        foreach (var explosion in explosions)
                        {
                            if (explosion != null)
                            {
                                explosion.Update(gameTime);

                            }
                        }
                        explosions.RemoveAll(explosion => explosion == null || !explosion.IsActive);

                        // Update enemy spawn timer
                        if (enemySpawnTimer >= spawnInterval)
                        {
                            float spawnX = random.Next(0, _graphics.PreferredBackBufferWidth - 64); // Random X position
                            Vector2 spawnPosition = new Vector2(spawnX, -64); // Spawn above the screen

                            // Double the number of enemies for Level 2
                            int enemyCount = level == 2 ? 2 : 1; // Double enemies for Level 2
                            for (int e = 0; e < enemyCount; e++)
                            {
                                var newEnemy = new EnemyTanks(enemyTankTexture, spawnPosition);
                                if (level == 2) newEnemy._speed += 50f; // Increase speed for Level 2
                                enemies.Add(newEnemy);
                            }

                            enemySpawnTimer = 0f; // Reset the spawn timer
                        }

                    }
                    break;
                case GameState.Help:
                    helpScene.Update(gameTime, ref currentState);
                    break;
                case GameState.About:
                    aboutScene.Update(gameTime, ref currentState);
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            switch (currentState)
            {
                case GameState.Menu:
                    menuScene.Draw(_spriteBatch, _graphics);
                    break;
                case GameState.NameInput:
                    _spriteBatch.DrawString(font, "Enter Your Name:", new Vector2(300, 300), Color.DarkRed);
                    _spriteBatch.DrawString(font, playerName, new Vector2(300, 340), Color.DarkRed);
                    _spriteBatch.DrawString(font, "Press Tab to Start", new Vector2(300, 380), Color.DarkRed);
                    break;
                case GameState.Play:
                    // Draw background
                    _spriteBatch.Draw(bgMapTexture, Vector2.Zero, Color.White);

                    // Draw player tank
                    playerTank.Draw(_spriteBatch);

                    // Draw enemies
                    foreach (var enemy in enemies)
                    {
                        enemy.Draw(_spriteBatch);
                    }
                    //draw explosion
                    foreach (var explosion in explosions)
                    {
                        explosion.Draw(_spriteBatch);
                    }
                    //draw lives and score
                    _spriteBatch.DrawString(font, $"Level: {level}", new Vector2(10, 10), Color.DarkRed);
                    _spriteBatch.DrawString(font, $"Lives: {lives}", new Vector2(10, 40), Color.DarkRed);
                    _spriteBatch.DrawString(font, $"Score: {score}", new Vector2(10, 100), Color.DarkRed);

                    // Display level transition message
                    if (levelTransition)
                    {
                        _spriteBatch.DrawString(font, "You have entered Level 2!", new Vector2(300, 400), Color.IndianRed);
                    }


                    // Draw timer
                    _spriteBatch.DrawString(font, $"Time Left: {Math.Max(0, (int)gameTimeRemaining)}s", new Vector2(10, 70), Color.DarkRed);

                    if (isGameOver)
                    {
                        _spriteBatch.DrawString(font, gameResult, new Vector2(350, 400), Color.Red);
                    }
                    break;
                case GameState.Help:
                    helpScene.Draw(_spriteBatch, _graphics);
                    break;
                case GameState.About:
                    aboutScene.Draw(_spriteBatch, _graphics);
                    break;
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
