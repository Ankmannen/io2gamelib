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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using io2GameLib.Helpers;
using Microsoft.Xna.Framework.Content;

namespace io2GameLib.Screens
{
    public class ImageSplashScreen : SimpleSplashScreen
    {
        #region Fields
        private Texture2D _texture;
        private Texture2D _whitePixel;
        private string _image;
        private Rectangle _screenRectangle;
        #endregion

        #region Initialization
        public ImageSplashScreen(string image, float displayFor, ScreenBase screenToFollow, string soundToPlay) 
            : base(String.Empty, displayFor, screenToFollow, soundToPlay)
        {
            _image = image;
            _screenRectangle = new Rectangle(0, 0, 800, 480);
        }

        public override void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>(_image);
            _whitePixel = content.Load<Texture2D>("io2GameLib/white32");
            base.LoadContent(content);
        }
        #endregion 

        #region Update and draw
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

            ScreenManager.SpriteBatch.Draw(_whitePixel, _screenRectangle, color);
            ScreenManager.SpriteBatch.Draw(_texture, position, color);

            base.Draw(gameTime);
        }
        #endregion 
    }
}
