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

namespace io2GameLib.Screens
{
    public class TextInputMenuEntry : MenuEntry
    {
        #region Fields

        string typedText;
        bool selectedLastTime;
        bool isPassword = false;

        #endregion

        #region Properties
        public bool IsPassword
        {
            get { return isPassword; }
            set { isPassword = value; }
        }

        public string Text 
        { 
            get { return typedText; } 
            set { typedText = value; } 
        }
        
        #endregion
        
        public TextInputMenuEntry(string header, string text) : base(header)
        {
            typedText = text;
        }

        public override void Update(MenuScreenBase screen, bool isSelected, Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(screen, isSelected, gameTime);


            var input = screen.ScreenManager.InputManager;

            if(isSelected && !selectedLastTime)
            {
                // Setup the input mnager to read text from keyboard
                input.TextInputBuffer = typedText;
                input.StartReadingKeysToTextBuffer();

            }

            if(!isSelected && selectedLastTime)
            {
                // Tear down input manager reading from keyboard if
                // the new selected entry isn't a TextInputMenuEntry 
                if(!(screen.SelectedEntry is TextInputMenuEntry))
                    input.StopReadingKeysToTextBuffer();
            }

            if (isSelected)
            {
                // Read the buffer to local variable
                typedText = input.TextInputBuffer;
            }

            // Remember the last state for setup and tear down of the input system
            selectedLastTime = isSelected;
        }

        public override void Draw(MenuScreenBase screen, Vector2 position, bool isSelected, GameTime gameTime)
        {

            // Draw the selected entry in yellow, otherwise white.
            Color color = isSelected ? Color.Yellow : Color.White;

            // Pulsate the size of the selected menu entry.
            double time = gameTime.TotalGameTime.TotalSeconds;

            float pulsate = (float)Math.Sin(time * 6) + 1;

            float scale = 1 + pulsate * 0.05f * selectionFade;

            // Modify the alpha to fade text out during transitions.
            color = screen.TransitionColor(color);

            var spriteBatch = screen.ScreenManager.SpriteBatch;
            var font = screen.ScreenManager.SharedSmallFont;

            Vector2 origin = new Vector2(0, font.LineSpacing / 2);

            string text = typedText;
            if (isPassword)
                text = "".PadRight(text.Length, '*');

            if (isSelected) text += "_";

            spriteBatch.DrawString(font, text, position + new Vector2(300, 0), color, 0.0f, origin, scale, SpriteEffects.None, 0);

            
            base.Draw(screen, position, isSelected, gameTime);
        }

    }
}
