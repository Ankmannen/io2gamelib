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

using System.Collections.Generic;
using System.IO;
//using System.IO.IsolatedStorage;
using io2GameLib.Helpers;
using io2GameLib.Input;
using io2GameLib.Screens.SelectionPopup;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace io2GameLib.Screens
{
    /// <summary>
    /// Handles screens
    /// </summary>
    public class ScreenManager : DrawableGameComponent
    {

        #region Fields

        private readonly List<ScreenBase> _screens = new List<ScreenBase>();
        private readonly List<ScreenBase> _screensToUpdate = new List<ScreenBase>();
        private bool _isInitialized;
        private SpriteFont _sharedHeaderFont;
        private SpriteFont _sharedSmallFont;
        private SpriteBatch _spriteBatch;
        private InputManager _inputManager;
        private Vector2 _adPosition = new Vector2(320, 0);
        private long _frameNumber;
        
        #endregion
        
        #region Properties

        /// <summary>
        /// Returns the current framenumber
        /// </summary>
        public long FrameNumber 
        { 
            get { return _frameNumber; } 
        }

        /// <summary>
        /// This string will be displayed at the bottom left of the display. Useful for trial modes.
        /// </summary>
        public string FooterText { get; set; }

        /// <summary>
        /// A reference to the SpriteBatch object that this ScreenManager uses to draw stuff.
        /// </summary>
        public SpriteBatch SpriteBatch
        {
            get { return _spriteBatch; }
        }

        /// <summary>
        /// Sets or gets the shared font that can be used by screens.
        /// </summary>
        public SpriteFont SharedHeaderFont
        {
            get { return _sharedHeaderFont; }
            set { _sharedHeaderFont = value; }
        }

        /// <summary>
        /// Sets or gets the shared small font that can be used by screens.
        /// </summary>
        public SpriteFont SharedSmallFont
        {
            get { return _sharedSmallFont; }
            set { _sharedSmallFont = value; }
        }

        /// <summary>
        /// Returns the Io2GameLibGame object that owns this ScreenManager
        /// </summary>
        public Io2GameLibGame GameLibGame
        {
            get { return (Io2GameLibGame) Game; }
        }

        /// <summary>
        /// Returns the InputManager associated with this ScreenManager
        /// </summary>
        public InputManager InputManager
        {
            get { return _inputManager; }
        }

        public Color BackgroundColor { get; set; }

        #endregion

        #region Initialization

        public ScreenManager(Io2GameLibGame game) : base(game)
        {
            FooterText = string.Empty;
            BackgroundColor = Color.Black;
            
        }

        public override void Initialize()
        {
            _inputManager = new InputManager();
            

            _isInitialized = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        
            _sharedHeaderFont = Game.Content.Load<SpriteFont>("io2GameLib/GameLibHeaderFont");
            _sharedSmallFont = Game.Content.Load<SpriteFont>("io2GameLib/GameLibSmallFont");

            foreach (ScreenBase screen in _screens)
            {
                screen.LoadContent(Game.Content);
            }
       
        }

        protected override void UnloadContent()
        {
            foreach (ScreenBase screen in _screens)
            {
                screen.UnloadContent();
            }

            base.UnloadContent();
        }

        #endregion

        #region Public methods

        public void AddScreen(ScreenBase screen)
        {
            screen.ScreenManager = this;
            
            // Initialize is called only when adding the screen to the manager
            screen.Initialize();

            if (_isInitialized)
                screen.LoadContent(this.Game.Content);

            // Surfaced is also called every time the screen obtains focus, for example
            // if another screen is covering this one and then removed, Surfaced() is called
            // but not Initialize()
            screen.Surfaced();

            _screens.Add(screen);
        }

        /// <summary>
        /// Removes a screen from the screen manager. You should normally
        /// use GameScreen.ExitScreen instead of calling this directly, so
        /// the screen can gradually transition off rather than just being
        /// instantly removed.
        /// </summary>
        public void RemoveScreen(ScreenBase screen)
        {
            var topmostScreen = GetTopScreen();

            // If we have a graphics device, tell the screen to unload content.
            if (_isInitialized)
            {
                screen.UnloadContent();
            }

            _screens.Remove(screen);
            _screensToUpdate.Remove(screen);

            // Reset the state if we want to add it again later on
            screen.ResetState();

            // Get topmost if present and tell it to reinitialize
            if (topmostScreen != GetTopScreen())
            {
                GetTopScreen().Surfaced();
            }
        }

        /// <summary>
        /// Gets a screen on a specific index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ScreenBase GetScreen(int index)
        {
            return _screens[index];
        }

        /// <summary>
        /// Gets the screen currently showing
        /// </summary>
        /// <returns></returns>
        public ScreenBase GetTopScreen()
        {
            if (_screens.Count == 0)
                return null;

            return _screens[_screens.Count-1];
        }

        public void CheckForQuitters()
        {
            if (InputManager.IsNewKeyPressed(Keys.Q))
            {
                var selection = new SelectionPopupScreen("Wanna quit?");
                selection.AddItem("Yup", true, ExitGame);
                selection.AddItem("Nope", true, null);
                AddScreen(selection);
            }
        }

        public bool Contains(ScreenBase screen)
        {
            return _screens.Contains(screen);
        }

        #endregion

        #region Update and draw

        public override void Draw(GameTime gameTime)
        {
#if(DEBUG)
            // ScreenShot?
            RenderTarget2D rt = null;
            if (Io2GameLibGame.Instance.Settings.TakeScreenShot)
            {
                rt = new RenderTarget2D(this.GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth,
                                                       GraphicsDevice.PresentationParameters.BackBufferHeight);
                this.GraphicsDevice.SetRenderTarget(rt);
            }
#endif

            // Clear the sky
            this.GraphicsDevice.Clear(BackgroundColor);

            // Only draw visible screens
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            foreach (ScreenBase screen in _screens)
            {
                if (!screen.IsInitialized)
                    continue;

                if (screen.ScreenState == ScreenState.Hidden)
                    continue;

                screen.Draw(gameTime);
            }

            // Draw PoC text
            if (FooterText.Trim() != string.Empty)
            {
                Vector2 viewportVector = GraphicsHelper.ViewPortToVector2(GraphicsDevice.Viewport);
                SpriteBatch.DrawString(SharedSmallFont,
                                       FooterText,
                                       new Vector2(20, viewportVector.Y - 20), Color.White);
            }

            SpriteBatch.End();

            base.Draw(gameTime);

#if(DEBUG)
            if (Io2GameLibGame.Instance.Settings.TakeScreenShot)
            {
                Io2GameLibGame.Instance.Settings.TakeScreenShot = false;
                this.GraphicsDevice.SetRenderTarget(null);
                if (rt != null)
                {
                    var stream = GetPngStream();
                    rt.SaveAsPng(stream, 800, 480);
                    //stream.Close();
                    stream.Dispose();
                }
            }
#endif
        }

#if(DEBUG)
        private int _screenshotCounter = 0;
        protected Stream GetPngStream()
        {
            //_screenshotCounter++;

            //var file = IsolatedStorageFile.GetUserStoreForApplication();
            //var filename = String.Format("screenshot_{0:yyyy-MM-dd_hh-mm-ss}.png", DateTime.Now);
            //var stream = file.OpenFile(filename, FileMode.Create);
            //return stream;
            return null;
        }
#endif

        public override void Update(GameTime gameTime)
        {
            // Increment framenumber
            _frameNumber++;

            // Read the keyboard and gamepad.
            _inputManager.Update();

            // Make a copy of the master screen list, to avoid confusion if
            // the process of updating one screen adds or removes others.
            _screensToUpdate.Clear();

            foreach (ScreenBase screen in _screens)
                _screensToUpdate.Add(screen);

            bool otherScreenHasFocus = !Game.IsActive;
            bool coveredByOtherScreen = false;

            // Loop as long as there are screens waiting to be updated.
            while (_screensToUpdate.Count > 0)
            {
                // Pop the topmost screen off the waiting list.
                ScreenBase screen = _screensToUpdate[_screensToUpdate.Count - 1];

                _screensToUpdate.RemoveAt(_screensToUpdate.Count - 1);

                // Update the screen.
                if (!screen.IsInitialized)
                    continue;

                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if (screen.ScreenState == ScreenState.TransitionOn ||
                    screen.ScreenState == ScreenState.Active)
                {
                    // If this is the first active screen we came across,
                    // give it a chance to handle input.
                    if (!otherScreenHasFocus)
                    {
                        screen.HandleInput(); //input);

                        otherScreenHasFocus = true;
                    }

                    // If this is an active non-popup, inform any subsequent
                    // screens that they are covered by it.
                    if (!screen.IsPopup)
                        coveredByOtherScreen = true;
                }
            }

            _inputManager.PostLogicUpdate();
           
            base.Update(gameTime);
        }

        #endregion

        #region Private methods
        
        private void ExitGame(object sender)
        {
            this.Game.Exit();
        }

        #endregion

        /// <summary>
        /// Determines if the screen is the top most
        /// </summary>
        /// <param name="screen"></param>
        /// <returns></returns>
        public bool IsInFocus(ScreenBase screen)
        {
            return GetTopScreen() == screen;
        }

        internal int IndexOf(ScreenBase screenBase)
        {
            return _screens.IndexOf(screenBase);
        }

        public int ScreenCount { get { return _screens.Count; } }

        /// <summary>
        /// Finds the first occurance of a screen of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T FindScreen<T>() where T : ScreenBase 
        {
            foreach (var screen in _screens)
            {
                if (screen is T)
                    return screen as T;
            }

            return null;
        }
    }
}