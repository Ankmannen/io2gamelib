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
using System.Collections.Generic;

using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using io2GameLib.Objects;
using io2GameLib.Sheets;
using System.Linq;

namespace io2GameLib.Screens
{
    public abstract class ScreenBase
    {

        #region Fields
       
        ScreenManager _screenManager;
        ScreenState _screenState = ScreenState.TransitionOn;
        float _transitionPosition = 1;
        bool _otherScreenHasFocus;
        TimeSpan _transitionOnTime = TimeSpan.Zero;
        TimeSpan _transitionOffTime = TimeSpan.Zero;



#if(DEBUG)
        private Rectangle _screenshotRectangle = new Rectangle(0, 0, 160, 160);
#endif 

        bool _isExiting = false;
        bool _isPopup = false;
        bool _isInitialized = false;
        bool _isContentLoaded = false;
        int _packetsReceived = 0;

        // Temporary reference until something better is constructed (perhaps a service broker)
        private ObjectManager _objectManager = null;
        public SpriteSheetManager spriteSheetManager = null;

        #endregion

        #region Events

        public delegate void ExitScreenDelegate();
        public event ExitScreenDelegate OnExitScreen;

        #endregion

        #region Properties

        public ScreenManager ScreenManager
        {
            get { return _screenManager; }
            internal set { _screenManager = value; }
        }

        public ObjectManager ObjectManager
        {
            get
            {
                if (_objectManager == null)
                {
                    _objectManager = new ObjectManager(this);
                }
                return _objectManager;
            }
            set
            {
                _objectManager = value;
            }
        }

        public SpriteBatch SpriteBatch
        {
            get { return _screenManager.SpriteBatch; }
        }

        public int PacketsReceived
        {
            get { return _packetsReceived; }
        }

        public bool IsUpdateObjectManagerEnabled { get; set; }
        public bool IsDrawObjectManagerEnabled { get; set; }

        /// <summary>
        /// Returns the screen below this one in the stack.
        /// </summary>
        /// <remarks>
        /// If this screen isn't attached to a screenmanager, null is returned. 
        /// If this screen is the first in the screenmanager, null is returned.
        /// </remarks>
        public ScreenBase ScreenBelow
        {
            get
            {
                if (this.ScreenManager == null)
                    return null;

                var index = this.ScreenManager.IndexOf(this);
                if (index == -1) 
                    return null; // Not in collection

                if (index == 0)
                    return null; // Nothing below this screen

                // Get the screen before this one
                var screenToReturn = this.ScreenManager.GetScreen(index-1);
                if (screenToReturn == this)
                {
                    throw new Exception("This should not happen");
                }

                return screenToReturn;
            }
        }
    
        /// <summary>
        /// Gets the screens transition state
        /// </summary>
        public ScreenState ScreenState 
        {
            get { return _screenState; }
            protected set { _screenState = value; }
        }

        /// <summary>
        /// Gets set when initialize has been called
        /// </summary>
        public bool IsInitialized
        {
            get { return _isInitialized; }
            set { _isInitialized = value; }
        }

      


        /// <summary>
        /// The current position of the transition, ranging from 
        /// 0 (fully active, no transition) to 1 (transition off)
        /// </summary>
        public float TransitionPosition
        {
            get { return _transitionPosition; }
            protected set { _transitionPosition = value; }
        }

        /// <summary>
        /// Indicates how long the screen takes to
        /// transition on when it is activated.
        /// </summary>
        public TimeSpan TransitionOnTime
        {
            get { return _transitionOnTime; }
            protected set { _transitionOnTime = value; }
        }

        /// <summary>
        /// Indicates how long the screen takes to
        /// transition off when it is deactivated.
        /// </summary>
        public TimeSpan TransitionOffTime
        {
            get { return _transitionOffTime; }
            protected set { _transitionOffTime = value; }
        }

        /// <summary>
        /// Gets the current alpha of the screen transition, ranging
        /// from 255 (fully active, no transition) to 0 (transitioned
        /// fully off to nothing).
        /// </summary>
        public byte TransitionAlpha
        {
            get { return (byte)(255 - TransitionPosition * 255); }
        }

        /// <summary>
        /// Gets the current alpha of the screen transition and applies 
        /// it to the color passed in to the function.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public Color TransitionColor(Color color)
        {
            return Color.FromNonPremultiplied(color.R, color.G, color.B, TransitionAlpha);
        }

        /// <summary>
        /// There are two possible reasons why a screen might be transitioning
        /// off. It could be temporarily going away to make room for another
        /// screen that is on top of it, or it could be going away for good.
        /// This property indicates whether the screen is exiting for real:
        /// if set, the screen will automatically remove itself as soon as the
        /// transition finishes.
        /// </summary>
        public bool IsExiting
        {
            get { return _isExiting; }
            protected internal set { _isExiting = value; }
        }

        /// <summary>
        /// Normally when one screen is brought up over the top of another,
        /// the first screen will transition off to make room for the new
        /// one. This property indicates whether the screen is only a small
        /// popup, in which case screens underneath it do not need to bother
        /// transitioning off.
        /// </summary>
        public bool IsPopup
        {
            get { return _isPopup; }
            protected set { _isPopup = value; }
        }

        public bool IsContentLoaded
        {
            get { return _isContentLoaded; }
            protected set { _isContentLoaded = value; }

        }

    
        /// <summary>
        /// Checks whether this screen is active and can respond to user input.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return !_otherScreenHasFocus &&
                       (_screenState == ScreenState.TransitionOn ||
                        _screenState == ScreenState.Active);
            }
        }

        #endregion

        public ScreenBase()
        {
            this.IsUpdateObjectManagerEnabled = true;
            this.IsDrawObjectManagerEnabled = true;

        }

        #region Update and draw
        /// <summary>
        /// Allows the screen to run logic, such as updating the transition position.
        /// Unlike HandleInput, this method is called regardless of whether the screen
        /// is active, hidden, or in the middle of a transition.
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime, bool otherScreenHasFocus,
                                               bool coveredByOtherScreen)
        {
            if (this._objectManager != null && IsUpdateObjectManagerEnabled)
            {
                _objectManager.Update(gameTime);
            }

            this._otherScreenHasFocus = otherScreenHasFocus;

#if DEBUG
            // When running in debug-mode we want the possibility to take a screenshot
            if (this.ScreenManager.InputManager.TouchWithin(_screenshotRectangle))
            {
                Io2GameLibGame.Instance.Settings.TakeScreenShot = true;
            }
#endif

            if (_isExiting)
            {
                // If the screen is going away to die, it should transition off.
                _screenState = ScreenState.TransitionOff;

                if (!UpdateTransition(gameTime, _transitionOffTime, 1))
                {
                    // When the transition finishes, remove the screen.
                    UnloadScreen();
                }
            }
            else if (coveredByOtherScreen)
            {
                // If the screen is covered by another, it should transition off.
                if (UpdateTransition(gameTime, _transitionOffTime, 1))
                {
                    // Still busy transitioning.
                    _screenState = ScreenState.TransitionOff;
                }
                else
                {
                    // Transition finished!
                    _screenState = ScreenState.Hidden;
                }
            }
            else
            {
                // Otherwise the screen should transition on and become active.
                if (UpdateTransition(gameTime, _transitionOnTime, -1))
                {
                    // Still busy transitioning.
                    _screenState = ScreenState.TransitionOn;
                }
                else
                {
                    // Transition finished!
                    _screenState = ScreenState.Active;
                }
            }
        }

        /// <summary>
        /// Helper for updating the screen transition position.
        /// </summary>
        bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        {
            // How much should we move by?
            float transitionDelta;

            if (time == TimeSpan.Zero)
                transitionDelta = 1;
            else
                transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                          time.TotalMilliseconds);

            // Update the transition position.
            _transitionPosition += transitionDelta * direction;

            // Did we reach the end of the transition?
            if (((direction < 0) && (_transitionPosition <= 0)) ||
                ((direction > 0) && (_transitionPosition >= 1)))
            {
                _transitionPosition = MathHelper.Clamp(_transitionPosition, 0, 1);
                return false;
            }

            // Otherwise we are still busy transitioning.
            return true;
        }


        public virtual void Draw(GameTime gameTime) 
        {
            if (_objectManager != null && this.IsDrawObjectManagerEnabled)
            {
                _objectManager.Draw(gameTime, Vector2.Zero, 1.0f);
            }
        }

        /// <summary>
        /// Allows the screen to handle user input. Unlike Update, this method
        /// is only called when the screen is active, and not when some other
        /// screen has taken the focus.
        /// </summary>
        public virtual void HandleInput() { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Exits the screen and transitions back to the previous one
        /// </summary>
        public void ExitScreen()
        {
            if (TransitionOffTime == TimeSpan.Zero)
            {
                // If the screen has a zero transition time, remove it immediately.
                UnloadScreen();
            }
            else
            {
                // Otherwise flag that it should transition off and then exit.
                _isExiting = true;
            }
        }

        /// <summary>
        /// Exits all screens up to the searched screen type is found.
        /// </summary>
        /// <param name="typeToFind"></param>
        public ScreenBase ExitScreenUntil(Type typeToFind)
        {
            var screen = this;

            var exitList = new List<ScreenBase>();
            exitList.Add(screen);

            while (screen != null)
            {
                if (screen.ScreenBelow == null)
                    return screen;

                if (screen.ScreenBelow.GetType() == typeToFind)
                {
                    var screenToReturn = screen.ScreenBelow;

                    ExitScreensInList(exitList);
                    return screenToReturn;
                }

                exitList.Add(screen.ScreenBelow);
                screen = screen.ScreenBelow;
            }

            ExitScreensInList(exitList);

            return null;
        }

        private void ExitScreensInList(List<ScreenBase> screens)
        {
          //  screens.ForEach(e => e.ExitScreen());
            foreach (var screen in screens)
                screen.ExitScreen();
        }


        /// <summary>
        /// Resets flags and variables as if the screen was newly created.
        /// </summary>
        /// <remarks>
        /// To be more specific, the _isExiting and Transition related variables.
        /// </remarks>
        public void ResetState()
        {
            _isExiting = false;
            _screenState = ScreenState.TransitionOn;
            _transitionPosition = 1;
        }

        private void UnloadScreen()
        {
            ScreenManager.RemoveScreen(this);
            if (OnExitScreen != null)
                OnExitScreen();
        }

        

        public virtual void LoadContent(ContentManager content) { _isContentLoaded = true; }

        public virtual void UnloadContent() { }

        /// <summary>
        /// This method is called whenever this screen is the primary screen to be showed.
        /// </summary>
        public virtual void Surfaced() { }

        /// <summary>
        /// Called by the screen manager after the screen is added
        /// </summary>
        public virtual void Initialize() { _isInitialized = true; }

        #endregion

    }

    public enum ScreenState
    {
        Active,
        Hidden,
        TransitionOff,
        TransitionOn
    }
}
