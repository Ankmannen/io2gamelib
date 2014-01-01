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
using Microsoft.Xna.Framework.Input;

namespace io2GameLib.Screens
{
    public class MessagePopupScreen : PopupScreenBase
    {

        #region Fields

        string header;
        string message;

        #endregion

        public MessagePopupScreen(string header, string message)
        {
        
            this.header = header;
            this.message = message;
        }

        #region Draw and update

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Draw(gameTime);

            SpriteBatch spritebatch = ScreenManager.SpriteBatch;
            Rectangle popupRect = Bounds;
                
            // Draw header
            spritebatch.DrawString(ScreenManager.SharedHeaderFont, header, new Vector2(popupRect.X + 10, popupRect.Y + 10), Color.White);
            spritebatch.DrawString(ScreenManager.SharedSmallFont, message, new Vector2(popupRect.X + 20, popupRect.Y + 80), Color.White);

            spritebatch.DrawString(ScreenManager.SharedSmallFont, "Press ENTER to continue", new Vector2(popupRect.X + 10, popupRect.Y + popupRect.Height - 30), Color.White);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (ScreenManager.InputManager.IsNewKeyPressed(Keys.Escape) || ScreenManager.InputManager.IsNewKeyPressed(Keys.Enter))
                this.ExitScreen();
            
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        #endregion
    }
}
