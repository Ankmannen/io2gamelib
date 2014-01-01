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

namespace io2GameLib.GameLibUI
{
    /// <summary>
    /// UI button control
    /// </summary>
    public class GluiButton : GluiControl
    {
        #region Fields
        
        Texture2D darkButton;
        Texture2D lightButton;
        string text;

        #endregion

        public string LightBackground { get; set; }
        public string DarkBackground { get; set; }

        #region Initialization

        public GluiButton(string text)
        {
            this.text = text;
            LightBackground = "io2GameLib/glui/buttonBackDark";
            DarkBackground = "io2GameLib/glui/buttonBackLight";
        }

        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            darkButton = content.Load<Texture2D>(DarkBackground);
            lightButton = content.Load<Texture2D>(LightBackground);
          
            // For size issues
            Texture = lightButton;

            base.LoadContent(content);
        }

        #endregion

        #region Draw and update

        public override void Draw(GameTime gameTime, Vector2 offset, float scale, SpriteBatch spritebatch)
        {
            Texture = mouseOn ? lightButton : darkButton;
            Color color = Clicked ? Color.Green : Color.White;

            spritebatch.Draw(Texture, Position, null, color, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0);

            if (text.Length > 0)
            {
                Vector2 textPosition = Position;
                textPosition += new Vector2(lightButton.Width / 2, lightButton.Height / 2);
                textPosition -= (font.MeasureString(text) / 2);


                spritebatch.DrawString(font, text, textPosition, color);
            }
        }

        #endregion 
    }
}
