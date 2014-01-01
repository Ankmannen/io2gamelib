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
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace io2GameLib.Sheets
{
    public class SpriteSheet
    {

        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }
        public Texture2D Texture { get; set; }

        public SpriteSheet(Texture2D texture, int frameWidth, int frameHeight)
        {
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
            Texture = texture;
        }

        public Rectangle GetSourceRectangle(string sheetKey, int frameNumber)
        {
            Point p = ResolveFrameNumber(sheetKey, frameNumber);
            return GetSourceRectangle(sheetKey, p.X, p.Y);
        }

        public Rectangle GetSourceRectangle(string sheetKey, int x, int y)
        {
            //TODO remove magic numbers
            return new Rectangle(x * FrameWidth, y * FrameHeight, FrameWidth, FrameHeight);
        }

        private Point ResolveFrameNumber(string sheetKey, int frameNumber)
        {
           
            int widthInFrames = Texture.Width / FrameWidth;
            int heightInFrames = Texture.Height / FrameHeight;

            int x = frameNumber % widthInFrames;
            int y = frameNumber / heightInFrames;

            return new Point(x, y);
        }
    }
}
