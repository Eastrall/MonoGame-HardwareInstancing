using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*--------------------------------------------------------
 * Camera.cs
 * 
 * Version: 1.0
 * Author: Filipe
 * Created: 20/03/2016 20:03:22
 * 
 * Notes:
 * -------------------------------------------------------*/

namespace HardwareInstancing
{
    public class Camera : GameComponent
    {
        public const Single NEAR_PLAN = 0.5f;
        public const Single FAR_PLAN = 350f;

        #region FIELDS

        private Single thetaAngle;
        private Single phiAngle;

        public Single gameTime;

        private Vector3 verticalAxisVector;
        private Vector3 lateralAxisVector;

        private Vector3 targetVector;

        //private Vector3 translationVector;
        private Vector3 orientationVector;

        public MouseState lastMouseState;

        #endregion

        #region PROPRIETIES


        public Vector3 Position { get; set; }

        public Matrix View { get; private set; }

        public Matrix Projection { get; private set; }

        #endregion

        #region CONSTRUCTORS

        public Camera(Game game)
            : base(game)
        {
            this.Position = new Vector3(0, 3, 0);
            this.targetVector = new Vector3(3, 0, 3);
            this.orientationVector = new Vector3(0, 0, 0);
            this.lateralAxisVector = new Vector3(0, 0, 0);
            this.verticalAxisVector = new Vector3(0, 1, 0);

            float phiRadian = MathHelper.ToRadians(10f);
            float thetaRadian = MathHelper.ToRadians(0f);

            this.orientationVector.X = (float)(Math.Sin(phiRadian) * Math.Sin(thetaRadian));
            this.orientationVector.Z = (float)(Math.Sin(phiRadian));
            this.orientationVector.Y = (float)(Math.Cos(phiRadian) * Math.Sin(thetaRadian));
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Update the camera.
        /// </summary>
        /// <param name="gameTime">Game time</param>
        public override void Update(GameTime gameTime)
        {
            MouseState _currentMouseState = Mouse.GetState();

            this.gameTime = (Single)gameTime.TotalGameTime.TotalMilliseconds;

            this.CameraProcess(_currentMouseState);
            this.BuildMatrix();

            this.lastMouseState = _currentMouseState;
        }

        /// <summary>
        /// Handle the camera inputs.
        /// </summary>
        /// <param name="mouseState">Mouse state</param>
        private void CameraProcess(MouseState mouseState)
        {
            if (mouseState.RightButton == ButtonState.Pressed)
            {
                this.Orientation(mouseState);
            }
            this.lateralAxisVector = Vector3.Cross(this.verticalAxisVector, this.orientationVector);
            this.lateralAxisVector.Normalize();

            //Vector3 zoom = new Vector3(0, 0, (this.lastMouseState.ScrollWheelValue - mouseState.ScrollWheelValue) / 5000);

            this.Position -= this.lateralAxisVector * ((mouseState.ScrollWheelValue - this.lastMouseState.ScrollWheelValue) * 0.05f);
            this.targetVector = this.Position + this.orientationVector;
            this.Move(mouseState);
        }

        /// <summary>
        /// Handle camera orientation.
        /// </summary>
        /// <param name="mouseState">Mouse state</param>
        private void Orientation(MouseState mouseState)
        {
            this.thetaAngle += -(mouseState.X - this.lastMouseState.X) * 0.1f;
            this.phiAngle += -(mouseState.Y - this.lastMouseState.Y) * 0.1f;

            if (this.phiAngle > 89.0f)
            {
                this.phiAngle = 89.0f;
            }
            else if (this.phiAngle < -89.0f)
            {
                this.phiAngle = -89.0f;
            }

            if (this.verticalAxisVector.X == 1.0f)
            {
                this.orientationVector = new Vector3(
                    (Single)(Math.Sin(MathHelper.ToRadians(this.phiAngle))),
                    (Single)(Math.Cos(MathHelper.ToRadians(this.phiAngle)) * Math.Cos(MathHelper.ToRadians(this.thetaAngle))),
                    (Single)(Math.Cos(MathHelper.ToRadians(this.phiAngle)) * Math.Sin(MathHelper.ToRadians(this.thetaAngle))));
            }
            else if (this.verticalAxisVector.Y == 1.0f)
            {
                this.orientationVector.X = (Single)(Math.Cos(MathHelper.ToRadians(this.phiAngle)) * Math.Sin(MathHelper.ToRadians(this.thetaAngle)));
                this.orientationVector.Y = (Single)(Math.Sin(MathHelper.ToRadians(this.phiAngle)));
                this.orientationVector.Z = (Single)(Math.Cos(MathHelper.ToRadians(this.phiAngle)) * Math.Cos(MathHelper.ToRadians(this.thetaAngle)));
            }
            else
            {
                this.orientationVector = new Vector3(
                      (Single)(Math.Cos(MathHelper.ToRadians(this.phiAngle)) * Math.Cos(MathHelper.ToRadians(this.thetaAngle))),
                      (Single)(Math.Cos(MathHelper.ToRadians(this.phiAngle)) * Math.Sin(MathHelper.ToRadians(this.thetaAngle))),
                      (Single)(Math.Sin(MathHelper.ToRadians(this.phiAngle))));
            }
        }

        /// <summary>
        /// Move the camera.
        /// </summary>
        /// <param name="mouseState">Mouse state</param>
        private void Move(MouseState mouseState)
        {
            KeyboardState _ks = Keyboard.GetState();

            if (_ks.IsKeyDown(Keys.Up) == true)
            {
                this.Position = this.Position + this.orientationVector * 0.5f;
                this.targetVector = this.Position + this.orientationVector;
            }
            if (_ks.IsKeyDown(Keys.Down) == true)
            {
                this.Position = this.Position - this.orientationVector * 0.5f;
                this.targetVector = this.Position + this.orientationVector;
            }
            if (_ks.IsKeyDown(Keys.Left) == true)
            {
                this.Position = this.Position + this.lateralAxisVector * 0.5f;
                this.targetVector = this.Position + this.orientationVector;
            }
            if (_ks.IsKeyDown(Keys.Right) == true)
            {
                this.Position = this.Position - this.lateralAxisVector * 0.5f;
                this.targetVector = this.Position + this.orientationVector;
            }
        }

        /// <summary>
        /// Build camera matrices.
        /// </summary>
        private void BuildMatrix()
        {
            this.View = Matrix.CreateLookAt(this.Position, this.targetVector, this.verticalAxisVector);
            this.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, this.Game.GraphicsDevice.Viewport.AspectRatio, NEAR_PLAN, FAR_PLAN);
        }

        #endregion
    }
}
