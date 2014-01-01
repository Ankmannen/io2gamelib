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

namespace io2GameLib.Screens
{
    public class SimpleSplashScreen : ScreenBase
    {
        SpriteFont _font;
        string _title;
        float _displayFor;
        ScreenBase _screenToFollow;
        string _soundToPlay;
        SoundEffect _soundEffect;

        /// <summary>
        /// Creates a simple splashscreen displaying a title for a given amount of time
        /// </summary>
        /// <param name="title">A title to display</param>
        /// <param name="displayFor">The number of milliseconds to display it for</param>
        /// <param name="screenToFollow">The screen that will show after the splash</param>
        public SimpleSplashScreen(string title, float displayFor, ScreenBase screenToFollow, string soundToPlay)
        {
            _title = title;
            _displayFor = displayFor;
            TransitionOnTime = TimeSpan.FromSeconds(0.5f);
            TransitionOffTime = TimeSpan.FromSeconds(0.5f);
            _screenToFollow = screenToFollow;
            _soundToPlay = soundToPlay;
        }

        public override void LoadContent(ContentManager content)
        {
            _font = content.Load<SpriteFont>("io2GameLib/GameLibHeaderFont");
            if (_soundToPlay.Length != 0)
            {
                _soundEffect = content.Load<SoundEffect>(_soundToPlay);
                _soundEffect.Play();
            }

            // Force the following screen to load content to avoid glitches
            _screenToFollow.ScreenManager = ScreenManager; // Hack set the screen manager to allow for content load
            _screenToFollow.Initialize();
            _screenToFollow.LoadContent(content);

            base.LoadContent(content);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {

            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            Vector2 position = new Vector2(100, 250);
            if (ScreenState == ScreenState.TransitionOn)
                position.X -= transitionOffset * 256;
            else
                position.X += transitionOffset * 512 * 1.5f;

            // Modify the alpha to fade text out during transitions.
            Color color = TransitionColor(Color.White);

            ScreenManager.SpriteBatch.DrawString(_font, _title, position, color);

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            _displayFor -= gameTime.ElapsedGameTime.Milliseconds;

            if (_displayFor < 0)
            {
                if(!IsExiting)
                    ScreenManager.AddScreen(_screenToFollow);

                ExitScreen();
            }

            if (ScreenManager.InputManager.IsBackButtonNewlyPressed())
            {
                Io2GameLibGame.Instance.Exit();
            }
            
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }
    }
}
