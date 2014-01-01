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

namespace io2GameLib.Helpers
{
    public class GraphicsHelper
    {

        public static Rectangle ViewPortToRectangle(Viewport viewPort)
        {

            return new Rectangle(viewPort.X, viewPort.Y, viewPort.Width, viewPort.Height);
        }

        public static Vector2 ViewPortToVector2(Viewport viewPort)
        {
            return new Vector2(viewPort.Width - viewPort.X, viewPort.Height - viewPort.Y);
        }

        /// <summary>
        /// Converts world coordinates into block coordinates.
        /// </summary>
        /// <param name="worldCoordinates"></param>
        /// <remarks>Takes care of a little mathematical rounding issue when any coordinates goes negative</remarks>
        /// <returns></returns>
        public static Point ToBlockCoordinates(Vector2 worldCoordinates, int wrapAroundBlockCount)
        {
            while (worldCoordinates.X < 0)
            {
                worldCoordinates.X += wrapAroundBlockCount * 32f;
            }

            int x = (int)(worldCoordinates.X / 32f);
            int y = (int)(worldCoordinates.Y / 32f);

            return new Point(x, y);
        }

        public static Point ToBlockCoordinates(float x, float y, int wrapAroundBlockCount)
        {
            return ToBlockCoordinates(new Vector2(x, y), wrapAroundBlockCount);
        }

        /// <summary>
        ///  Calculates the vector from an angle
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Vector2 VectorFromAngle(float angle)
        {
            return new Vector2((float)Math.Sin(angle), -(float)Math.Cos(angle));
        }
       
    }
}
