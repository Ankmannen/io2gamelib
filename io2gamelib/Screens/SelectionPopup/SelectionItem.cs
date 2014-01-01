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

namespace io2GameLib.Screens.SelectionPopup
{

    public class SelectionItem
    {
        public string Text;
        float selectionFade;
        bool closeOnSelection;

        public SelectionItem(string text, bool closeOnSelection)
        {
            this.Text = text;
            this.closeOnSelection = closeOnSelection;
        }

        public delegate void EntrySelectedHandler(SelectionItem sender);
        public event EntrySelectedHandler EntrySelected;

        internal void RaiseSelectedEvent(SelectionPopupScreen screen)
        {
            if (closeOnSelection)
                screen.ExitScreen();

            if (EntrySelected != null)
                EntrySelected(this);
        }

        public void Update(SelectionPopupScreen screen, bool isSelected, GameTime gameTime)
        {
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (isSelected)
                selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
            else
                selectionFade = Math.Max(selectionFade - fadeSpeed, 0);
        }

        public void Draw(SelectionPopupScreen screen, Vector2 position, bool isSelected, GameTime gameTime)
        {

            var spritebatch = screen.ScreenManager.SpriteBatch;
            var font = screen.ScreenManager.SharedHeaderFont;

            // Draw the selected entry in yellow, otherwise white.
            Color color = isSelected ? Color.Yellow : Color.White;

            // Pulsate the size of the selected menu entry.
            double time = gameTime.TotalGameTime.TotalSeconds;

            float pulsate = (float)Math.Sin(time * 6) + 1;

            float scale = 1 + pulsate * 0.05f * selectionFade;
            Vector2 origin = new Vector2(0, font.LineSpacing / 2);

            //            spritebatch.DrawString(font, Text, position, color);
            spritebatch.DrawString(font, Text, position, color, 0.0f, origin, scale, SpriteEffects.None, 0);

        }


    }
}
