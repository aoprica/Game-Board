// ----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------------
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
// ----------------------------------------------------------------------------------

#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using Microsoft.Devices.Sensors;
using GameStateManagement;
using Microsoft.Xna.Framework.GamerServices;
//using Microsoft.Xna.Framework.Input.Touch;
//using Microsoft.Devices;
using Microsoft.Xna.Framework.Audio;
#endregion

namespace MarbleMazeGame
{
    class GameplayScreen : GameScreen
    {
        #region Fields
        bool gameOver = false;
        bool startScreen = true;
        TimeSpan startScreenTime = TimeSpan.FromSeconds(4);
        Maze maze;
        Marble marble;
        Camera camera;
        LinkedListNode<Vector3> lastCheackpointNode;
        public new bool IsActive = true;
        readonly float angularVelocity = MathHelper.ToRadians(1.5f);
        KeyboardState oldKeyState;

        SpriteFont timeFont;

        TimeSpan gameTime;
        public Vector3 AccelerometerCalibrationData = Vector3.Zero;
        #endregion

        #region Initializations
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.0);
            TransitionOffTime = TimeSpan.FromSeconds(0.0);

            // Enable double tap gesture to calibrate the accelerometer
            //EnabledGestures = GestureType.Tap | GestureType.DoubleTap;
        }
        #endregion

        #region Loading
        /// <summary>
        /// Load the game content
        /// </summary>
        public override void LoadContent()
        {
            timeFont = ScreenManager.Game.Content.Load<SpriteFont>(@"Fonts\MenuFont");

            Accelerometer.Initialize();

            base.LoadContent();
        }

        /// <summary>
        /// Load all assets for the game
        /// </summary>
        public void LoadAssets()
        {
            InitializeCamera();
            InitializeMaze();
            InitializeMarble();
        }

        /// <summary>
        /// Initialize the camera
        /// </summary>
        private void InitializeCamera()
        {
            // Create the camera
            camera = new Camera(ScreenManager.Game, ScreenManager.GraphicsDevice);
            camera.Initialize();
        }

        /// <summary>
        /// Initialize maze
        /// </summary>
        private void InitializeMaze()
        {
            maze = new Maze(ScreenManager.Game)
            {
                Position = Vector3.Zero,
                Camera = camera
            };

            maze.Initialize();

            // Save the last checkpoint
            lastCheackpointNode = maze.Checkpoints.First;
        }

        /// <summary>
        /// Initialize the marble
        /// </summary>
        private void InitializeMarble()
        {
            marble = new Marble(ScreenManager.Game)
            {
                Position = maze.StartPoistion,
                Camera = camera,
                Maze = maze
            };

            marble.Initialize();
        }
        #endregion

        #region Update
        /// <summary>
        /// Handle all the input.
        /// </summary>
        /// <param name="input"></param>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (input.IsPauseGame(null))
            {
                if (!gameOver)
                    PauseCurrentGame();
                else
                    FinishCurrentGame();
            }

            var newKeyState = Keyboard.GetState();

            if (IsActive && !startScreen)
            {
                if (gameOver)
                {
                    if (newKeyState.IsKeyDown(Keys.Enter))
                    {
                        if (!oldKeyState.IsKeyDown(Keys.Enter))
                        {
                            FinishCurrentGame();
                        }
                    }
                }
                else
                {
                    var min = -MathHelper.ToRadians(30);
                    var max = MathHelper.ToRadians(30);

                    if (newKeyState.IsKeyDown(Keys.Left))
                    {
                        if (!oldKeyState.IsKeyDown(Keys.Left))
                        {
                            maze.Rotation.Z += -angularVelocity;

                            if (maze.Rotation.Z < min)
                                maze.Rotation.Z = min;
                        }
                    }
                    else if (newKeyState.IsKeyDown(Keys.Right))
                    {
                        if (!oldKeyState.IsKeyDown(Keys.Right))
                        {
                            maze.Rotation.Z += angularVelocity;

                            if (maze.Rotation.Z > max)
                                maze.Rotation.Z = max;
                        }
                    }
                    else if (newKeyState.IsKeyDown(Keys.Up))
                    {
                        if (!oldKeyState.IsKeyDown(Keys.Up))
                        {
                            maze.Rotation.X += -angularVelocity;
                            
                            if(maze.Rotation.X < min)
                                maze.Rotation.X = min;
                        }
                    }
                    else if (newKeyState.IsKeyDown(Keys.Down))
                    {
                        if (!oldKeyState.IsKeyDown(Keys.Down))
                        {
                            maze.Rotation.X += angularVelocity;

                            if (maze.Rotation.X > max)
                                maze.Rotation.X = max;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Update all the game component
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="otherScreenHasFocus"></param>
        /// <param name="coveredByOtherScreen"></param>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (IsActive && !gameOver)
            {
                if (!startScreen)
                {
                    // Calculate the time from the start of the game
                    this.gameTime += gameTime.ElapsedGameTime;

                    CheckFallInPit();
                    UpdateLastCheackpoint();
                }

                // Update all the component of the game
                maze.Update(gameTime);
                marble.Update(gameTime);
                camera.Update(gameTime);

                CheckGameFinish();

                base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            }
            if (startScreen)
            {
                if (startScreenTime.Ticks > 0)
                {
                    startScreenTime -= gameTime.ElapsedGameTime;
                }
                else
                {
                    startScreen = false;
                }
            }
        }
        #endregion

        #region Render
        /// <summary>
        /// Draw all the game component
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Color.Black);
            ScreenManager.SpriteBatch.Begin();
            if (startScreen)
            {
                DrawStartGame(gameTime);
            }
            if (IsActive)
            {
                // Draw the elapsed time
                ScreenManager.SpriteBatch.DrawString(timeFont,
                    String.Format("{0:00}:{1:00}", this.gameTime.Minutes,
                    this.gameTime.Seconds), new Vector2(20, 20), Color.YellowGreen);

                // Drawing sprites changes some render states around, which don't play
                // nicely with 3d models. 
                // In particular, we need to enable the depth buffer.
                DepthStencilState depthStensilState =
                    new DepthStencilState() { DepthBufferEnable = true };
                ScreenManager.GraphicsDevice.DepthStencilState = depthStensilState;

                // Draw all the game components
                maze.Draw(gameTime);
                marble.Draw(gameTime);
            }
            if (gameOver)
            {
                AudioManager.StopSounds();
                DrawEndGame(gameTime);
            }

            ScreenManager.SpriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawEndGame(GameTime gameTime)
        {
            string text = HighScoreScreen.IsInHighscores(this.gameTime) ? "    You got a High Score!" :
                                                                          "          Game Over";
            text += "\nTouch the screen to continue";
            Vector2 size = timeFont.MeasureString(text);
            Vector2 textPosition = (new Vector2(ScreenManager.GraphicsDevice.Viewport.Width, 
                ScreenManager.GraphicsDevice.Viewport.Height) - size) / 2f;

            ScreenManager.SpriteBatch.DrawString(timeFont, text,
                textPosition, Color.White);
        }

        private void DrawStartGame(GameTime gameTime)
        {
            string text = (startScreenTime.Seconds == 0) ? "Go!" : startScreenTime.Seconds.ToString();
            Vector2 size = timeFont.MeasureString(text);
            Vector2 textPosition = (new Vector2(ScreenManager.GraphicsDevice.Viewport.Width, 
                ScreenManager.GraphicsDevice.Viewport.Height) - size) / 2f;

            ScreenManager.SpriteBatch.DrawString(timeFont, text, textPosition, Color.White);
        }
        #endregion

        #region Private functions
        /// <summary>
        /// Update the last checkpoint to return to after falling in a pit.
        /// </summary>
        private void UpdateLastCheackpoint()
        {
            BoundingSphere marblePosition = marble.BoundingSphereTransformed;

            var tmp = lastCheackpointNode;
            while (tmp.Next != null)
            {
                // If the marble is close to a checkpoint save the checkpoint
                if (Math.Abs(Vector3.Distance(marblePosition.Center, tmp.Next.Value))
                    <= marblePosition.Radius * 3)
                {
                    AudioManager.PlaySound("checkpoint");
                    lastCheackpointNode = tmp.Next;
                    return;
                }
                tmp = tmp.Next;
            }

        }

        /// <summary>
        /// If marble falls in a pit, return the marble to the last checkpoint 
        /// the marble passed.
        /// </summary>
        private void CheckFallInPit()
        {
            if (marble.Position.Y < -150)
            {
                marble.Position = lastCheackpointNode.Value;
                maze.Rotation = Vector3.Zero;
                marble.Acceleration = Vector3.Zero;
                marble.Velocity = Vector3.Zero;
            }
        }

        /// <summary>
        /// Check if the game has ended.
        /// </summary>
        private void CheckGameFinish()
        {
            BoundingSphere marblePosition = marble.BoundingSphereTransformed;

            if (Math.Abs(Vector3.Distance(marblePosition.Center, maze.End)) <= marblePosition.Radius * 3)
            {
                gameOver = true;
                return;
            }
        }

        /// <summary>
        /// Finish the current game
        /// </summary>
        private void FinishCurrentGame()
        {
            IsActive = false;

            foreach (GameScreen screen in ScreenManager.GetScreens())
                screen.ExitScreen();

            if (HighScoreScreen.IsInHighscores(gameTime))
            {
                // Show the device's keyboard
                Guide.BeginShowKeyboardInput(PlayerIndex.One,
                    "Player Name", "Enter your name (max 15 characters)", "Player", (r) =>
                {
                    string playerName = Guide.EndShowKeyboardInput(r);

                    if (playerName != null && playerName.Length > 15)
                        playerName = playerName.Substring(0, 15);

                    HighScoreScreen.PutHighScore(playerName, gameTime);

                    ScreenManager.AddScreen(new BackgroundScreen(), null);
                    ScreenManager.AddScreen(new HighScoreScreen(), null);

                }, null);
                return;
            }

            ScreenManager.AddScreen(new BackgroundScreen(), null);
            ScreenManager.AddScreen(new HighScoreScreen(), null);
        }

        /// <summary>
        /// Pause the game.
        /// </summary>
        private void PauseCurrentGame()
        {
            IsActive = false;
            // Pause sounds
            AudioManager.PauseResumeSounds(false);

            ScreenManager.AddScreen(new BackgroundScreen(), null);
            ScreenManager.AddScreen(new PauseScreen(), null);
        }

        /// <summary>
        /// Launch calibration screen.
        /// </summary>
        private void CalibrateGame()
        {
            IsActive = false;
            // Pause the sounds
            AudioManager.PauseResumeSounds(false);

            ScreenManager.AddScreen(new BackgroundScreen(), null);
            //ScreenManager.AddScreen(new CalibrationScreen(this), null);
        }

        /// <summary>
        /// Restart the game.
        /// </summary>
        internal void Restart()
        {
            marble.Position = maze.StartPoistion;
            marble.Velocity = Vector3.Zero;
            marble.Acceleration = Vector3.Zero;
            maze.Rotation = Vector3.Zero;
            IsActive = true;
            gameOver = false;
            gameTime = TimeSpan.Zero;
            startScreen = true;
            startScreenTime = TimeSpan.FromSeconds(4);
            lastCheackpointNode = maze.Checkpoints.First;
        }
        #endregion
    }
}
