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
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace io2GameLib.Objects
{
    public class MousePointer : Object2D 
    {

        public string Text = "";
        public Texture2D MiniImage;

        private int lastScrollWheelValue = 0;
        private SpriteFont smallFont;

        public MousePointer()
        {
            Scale = 0.5f;
            IgnoreOffset = true;
            IgnoreScale = true;
        }

        #region Properties
        
        public bool IsOnTopOfOtherObject
        {
            get
            {
                return isOnTopOfOtherObject;
            }
            set
            {
                isOnTopOfOtherObject = value;
            }
        }
        private bool isOnTopOfOtherObject;


        #endregion

        #region Events
        public delegate void ScrollWheelValueChangedHandler(int deltaValue);
        public event ScrollWheelValueChangedHandler ScrollWheelValueChanged;
#endregion

        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            Texture = content.Load<Texture2D>("io2GameLib/pointer");
            smallFont = content.Load<SpriteFont>("io2GameLib/GameLibSmallFont");

            base.LoadContent(content);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            MouseState state = Mouse.GetState();
            Position.X = state.X;
            Position.Y = state.Y;

            if (lastScrollWheelValue != state.ScrollWheelValue)
            {
                int delta = state.ScrollWheelValue - lastScrollWheelValue;
                lastScrollWheelValue = state.ScrollWheelValue;

                // Fire the event if someone is listening
                if (ScrollWheelValueChanged != null)
                    ScrollWheelValueChanged(delta);
            }
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime, Microsoft.Xna.Framework.Vector2 offset, float scale, SpriteBatch spritebatch)
        {
            // Draw the text if there is any
            if (Text.Trim() != String.Empty)
            {
                spritebatch.DrawString(smallFont, Text, Position + new Vector2(30, 0), Color.Black);
            }
            
            base.Draw(gameTime, offset, scale, spritebatch);
        }

    }
}
