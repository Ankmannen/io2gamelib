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
using io2GameLib;
using io2GameLib.Screens;
using io2GameLib.Helpers;
using io2GameLib.Services;

namespace io2GameLib
{
    /// <summary>
    /// The main game class to inherit your game from.
    /// </summary>
    public class Io2GameLibGame : Game
    {
        #region Fields
        private static Io2GameLibGame instance;
        #endregion

        #region Properties
        
        /// <summary>
        /// The GraphicsDeviceManager created by this game
        /// </summary>
        public GraphicsDeviceManager Graphics { get; set; }

        /// <summary>
        /// The ScreenManager created by this game
        /// </summary>
        public ScreenManager ScreenManager { get; set; }


       

        /// <summary>
        /// Generic settings for the io2GameLib
        /// </summary>
        public GameLibSettings Settings { get; private set; }

        /// <summary>
        /// The evil singleton instance of the game
        /// </summary>
        public static Io2GameLibGame Instance
        {
            get
            {
                return instance;
            }
            protected set
            {
                instance = value;
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes io2GameLib
        /// </summary>
        public Io2GameLibGame()
        {
            // Regular initialization stuff
            Content.RootDirectory = "Content";
            Graphics = new GraphicsDeviceManager(this);
            Graphics.IsFullScreen = true;

            // Extend battery life under lock.
            // InactiveSleepTime = TimeSpan.FromSeconds(1); // CRASH on WIN8

            // Create the settings object
            Settings = new GameLibSettings();

            // Create the screenmanager
            ScreenManager = new ScreenManager(this);
            Components.Add(ScreenManager);

            // Add the default background screen
            ScreenManager.AddScreen(new BackgroundScreen());

            // Let the user get a change to initialize there own services
            InitializeServices();

            // Register the default services
            RegisterDefaultServices();

            // An old construct still alive
            Io2GameLibGame.instance = this;
        }

        /// <summary>
        /// Registers any library default services that are not
        /// already registered by the library user
        /// </summary>
        private void RegisterDefaultServices()
        {
            // Register the AudioService
            if (!ServiceLocator.Contains<IAudioService>())
            {
                ServiceLocator.Register<IAudioService>(new AudioService(Content));
            }
        }

        /// <summary>
        /// Overload this method to intercept the initialization of 
        /// default services, such as the IAudioService. If you
        /// register them first they will not be injected by the 
        /// library.
        /// </summary>
        /// <example>
        /// ServiceLocator.Register<!--<-->IAudioService<!-->-->(new MySuperAudioService());
        /// </example>
        public virtual void InitializeServices()
        {

        }

        #endregion


    }
}
