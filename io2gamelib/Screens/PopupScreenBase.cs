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
using io2GameLib.Screens;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using io2GameLib.Helpers;
using Microsoft.Xna.Framework.Content;

namespace io2GameLib.Screens
{
    public abstract class PopupScreenBase : ScreenBase
    {
        Texture2D popupBackground;
        Rectangle bounds = new Rectangle();

        /// <summary>
        /// Gets or sets the bounds of the popup.
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                return bounds;
            }

            set
            {
                bounds = value;
            }

        }

        public PopupScreenBase()
        {
            IsPopup = true;
        }

        public override void LoadContent(ContentManager content)
        {
            popupBackground = content.Load<Texture2D>("io2GameLib/glui/popupback");

            // Setup bounds
            Rectangle vpRect = GraphicsHelper.ViewPortToRectangle(ScreenManager.Game.GraphicsDevice.Viewport);
            bounds = new Rectangle(0, 0, 500, 399);
            bounds.X = (vpRect.Width / 2) - (bounds.Width / 2);
            bounds.Y = (vpRect.Height / 2) - (bounds.Height / 2);

            base.LoadContent(content);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            SpriteBatch spritebatch = ScreenManager.SpriteBatch;

            //Rectangle vpRect = GraphicsHelper.ViewPortToRectangle(ScreenManager.Game.GraphicsDevice.Viewport);

            //Rectangle popupRect = new Rectangle(0, 0, 500, 399);
            //popupRect.X = (vpRect.Width / 2) - (popupRect.Width / 2);
            //popupRect.Y = (vpRect.Height / 2) - (popupRect.Height / 2);

            spritebatch.Draw(popupBackground, bounds, Color.White);
        }
    }
}
