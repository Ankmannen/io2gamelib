/* Copyright (C) 2012 io2GameLabs (www.io2gamelabs.se)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do 
 * so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
 * PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
 * OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using io2GameLib.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace io2GameLib.GameLibUI
{
    /// <summary>
    /// Baseclass for all control in Glui-control library
    /// </summary>
    public abstract class GluiControl : Object2D
    {
        #region Fields

        protected bool mouseOn = false;
        protected SpriteFont font;
        protected SpriteFont largeFont;
        protected int scrollWheelDelta;
        bool leftMouseDown;
        bool touchDown;
        int lastScrollWheelValue;
        Random _random = new Random();
        private Vector2 _originalPosition = Vector2.Zero;

        #endregion

        #region Events

        public delegate void ClickHandler(GluiControl sender);
        public event ClickHandler OnClick;

        #endregion

        #region Properties
        public bool Clicked { get { return leftMouseDown; } }
       
        #endregion 

        #region Initialization
        public GluiControl()
        {
            IgnoreOffset = true;
            IgnoreScale = true;
        }

        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            font = content.Load<SpriteFont>("io2GameLib/glui/gluiNormalFont");
            largeFont = content.Load<SpriteFont>("io2GameLib/glui/gluiLargeFont");

            base.LoadContent(content);
        }

        #endregion 

        #region Draw and update

        public override void Update(GameTime gameTime)
        {
            UpdateAnimation(gameTime);

            if (!IsEnabled)
                return;

            MouseState state = Mouse.GetState();
            Point mousePoint = new Point(state.X, state.Y);

            // Check if the mouse pointer or the mouse is entering to click
            if (mouseOn && state.LeftButton == ButtonState.Pressed && (leftMouseDown == false) && CoveringRectangle(Vector2.Zero).Contains(mousePoint))
            {
                // We only want to fire the event once for every click
                leftMouseDown = true;
            }

            // Reset the mousedown watchdog
            if (leftMouseDown)
            {
                if (state.LeftButton == ButtonState.Released && CoveringRectangle(Vector2.Zero).Contains(mousePoint))
                {
                    leftMouseDown = false;
                    MouseOff();

                    if (OnClick != null)
                        OnClick(this);
                }
            }

            if (CoveringRectangle(Vector2.Zero).Contains(mousePoint))
            {
                // call mouse one only once per entry
                if(!mouseOn)
                    MouseOn();
            }
            else
            {
                // Call mouse off only once per "offing"
                if(mouseOn)
                    MouseOff();
            }

            // Update the scrollwheeldelta if needed
            if (mouseOn)
            {
                scrollWheelDelta = state.ScrollWheelValue - lastScrollWheelValue;
            }
            lastScrollWheelValue = state.ScrollWheelValue;

            base.Update(gameTime);
        }

        private void UpdateAnimation(GameTime gameTime)
        {
            // Animation (concept)
            if (_isAnimating)
            {
                _currentAnimationTime += gameTime.ElapsedGameTime.Milliseconds;
                var currentAnimationPosition = _currentAnimationTime / _animationTime;
                Position.X = MathHelper.SmoothStep(_originalPosition.X, _animateTo.X, currentAnimationPosition);
                Position.Y = MathHelper.SmoothStep(_originalPosition.Y, _animateTo.Y, currentAnimationPosition);
                if (currentAnimationPosition > 1)
                    _isAnimating = false;
            }
        }

        #endregion 
      
        #region Public method

        public void Hide()
        {
            Hide(0);
        }

        public void Hide(float transitionOffDuration)
        {
            if (transitionOffDuration == 0)
            {
                this.IsVisible = false;
                this.IsEnabled = false;
                return;
            }

            // Save the current position
            _originalPosition = this.Position;
            var randomVector = GetRandomOffScreenPosition();

            AnimateTo(randomVector, transitionOffDuration);

        }

        private Vector2 GetRandomOffScreenPosition()
        {
            var x = _random.NextDouble();
            _random.NextDouble();
            var y = _random.NextDouble();

            return new Vector2(x < 0.5 ? -500 : 500,
                                            y < 0.5 ? -500 : 500);
        }

        public void Show()
        {
            Show(0);
        }

        public void Show(int transitionOnDuration)
        {
            if (transitionOnDuration == 0)
            {
                this.Position = _originalPosition;
                this.IsVisible = true;
                this.IsEnabled = true;
                return;
            }

            if (_originalPosition == Vector2.Zero)
                _originalPosition = Position;

            Position = GetRandomOffScreenPosition();
            AnimateTo(_originalPosition, transitionOnDuration);
        }
        
        public virtual void MouseOn()
        {
            // TODO Create a helper for mousestates

            mouseOn = true;

            // Reset the scrollwheelvalue so that we get the delta for
            // this object only since the wheel might have been used outside
            // of here.
            lastScrollWheelValue = Mouse.GetState().ScrollWheelValue;
        }

        public virtual void MouseOff()
        {
            mouseOn = false;
            scrollWheelDelta = 0;
        }

        #endregion 

        #region Animation


        // Concept Code
        private bool _isAnimating = false;
        private Vector2 _animateTo;
        private float _animationTime;
        private float _currentAnimationTime;
        public Object2D AnimateTo(Vector2 position, float animationTime)
        {
            _isAnimating = true;
            _animateTo = position;
            _originalPosition = Position;
            _animationTime = animationTime;
            _currentAnimationTime = 0;

            return this;
        }

      

        #endregion
    }
}
