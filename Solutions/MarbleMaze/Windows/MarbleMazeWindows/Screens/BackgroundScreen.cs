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
// BackgroundScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
	using Microsoft.Xna.Framework.Graphics;
using GameStateManagement; 
#endregion

namespace MarbleMazeGame
{
    class BackgroundScreen : GameScreen
    {
        #region Fields
        Texture2D background; 
        #endregion

        #region Initialization
        public BackgroundScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.0);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        } 
        #endregion

        #region Loading
        /// <summary>
        /// Load screen resources
        /// </summary>
        public override void LoadContent()
        {
            background = Load<Texture2D>(@"Images\titleScreen");
        } 
        #endregion

        #region Update
        /// <summary>
        /// Update the screen
        /// </summary>
        /// <param name="gameTime">The game time</param>
        /// <param name="otherScreenHasFocus"></param>
        /// <param name="coveredByOtherScreen"></param>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                            bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
        } 
        #endregion

        #region Render
        /// <summary>
        /// Renders the screen
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;            

            spriteBatch.Begin();
            
            // draw background to screen size;
            var screenWidth = spriteBatch.GraphicsDevice.PresentationParameters.BackBufferWidth;
            var screenHeight = spriteBatch.GraphicsDevice.PresentationParameters.BackBufferHeight;
            var drawRect = new Rectangle(0, 0, screenWidth, screenHeight);

            // Draw background
            spriteBatch.Draw(background, drawRect, /*new Vector2(0, 0),*/
                 Color.White * TransitionAlpha);

            spriteBatch.End();
        } 
        #endregion
    }
}
