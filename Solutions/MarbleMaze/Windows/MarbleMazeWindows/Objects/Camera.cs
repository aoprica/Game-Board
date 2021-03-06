﻿// ----------------------------------------------------------------------------------
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
// Camera.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace MarbleMazeGame
{
    public class Camera : GameComponent
    {
        #region Fields
        Vector3 position = Vector3.Zero;
        Vector3 target = Vector3.Zero;
        GraphicsDevice graphicsDevice;

        public Vector3 ObjectToFollow { get; set; }
        public Matrix Projection { get; set; }
        public Matrix View { get; set; }

        public Vector3 CameraPositionOffset;
        public Vector3 CameraTargetOffset;

        readonly Vector3 defaultCameraPositionOffset = new Vector3(0, 450, 100);
        readonly Vector3 defaultTargetOffset = new Vector3(0, 0, -50);
        #endregion

        #region Initializtion
        public Camera(Game game, GraphicsDevice graphics)
            : base(game)
        {
            this.graphicsDevice = graphics;
        }

        /// <summary>
        /// Initialize the camera
        /// </summary>
        public override void Initialize()
        {
            // Create the projection matrix
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(75),
                graphicsDevice.Viewport.AspectRatio, 1, 10000);

            CameraPositionOffset = this.defaultCameraPositionOffset;

            CameraTargetOffset = this.defaultTargetOffset;

            base.Initialize();
        }
        #endregion

        #region Update
        /// <summary>
        /// Update the camera to follow the object it is set to follow.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Update(GameTime gameTime)
        {
            // Make the camera follow the object
            position = ObjectToFollow + CameraPositionOffset;

            target = ObjectToFollow + CameraTargetOffset;

            // Create the view matrix
            View = Matrix.CreateLookAt(position, target, Vector3.Up);

            base.Update(gameTime);
        }
        #endregion Update

        public void Zoom(int zoomValue)
        {
            this.CameraPositionOffset += new Vector3(0, zoomValue, 0);

            if (this.CameraPositionOffset.Y < this.defaultCameraPositionOffset.Y)
                this.CameraPositionOffset = this.defaultCameraPositionOffset;
            else if (this.CameraPositionOffset.Y > 2000)
                this.CameraPositionOffset.Y = 2000;
        }
    }
}
