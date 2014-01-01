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
using Microsoft.Xna.Framework.Graphics;

namespace io2GameLib.GameLibUI
{
    /// <summary>
    /// Represents a label
    /// </summary>
    public class GluiLabel : GluiControl
    {
        #region Fields

        private string _text;
        private Color _foreColor = Color.Black;
        private bool _useLargeFont = false;

        #endregion

        #region Properties

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public Color ForeColor 
        {
            get { return _foreColor; }
            set { _foreColor = value; }
        }

        public bool UseLargeFont
        {
            get { return _useLargeFont; }
            set { _useLargeFont = value; }
        }

        #endregion

        #region Initialization

        public GluiLabel(string text)
        {
            this._text = text;
        }
        public GluiLabel(string text, Vector2 position)
        {
            this._text = text;
            this.Position = position;
        }

        #endregion

        #region Draw and update

        public override void Draw(GameTime gameTime, Vector2 offset, float scale, SpriteBatch spritebatch)
        {
            if (!IsVisible)
                return;

            spritebatch.DrawString((_useLargeFont ? largeFont : font), _text, Position, ForeColor, 0f, Vector2.Zero, this.Scale, SpriteEffects.None, 0f);
        }

        #endregion 

    }
}
