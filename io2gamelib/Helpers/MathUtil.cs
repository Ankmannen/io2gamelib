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

namespace io2GameLib.Helpers
{
    public static class MathUtil
    {
        /// <summary>
        /// Creates a normalized vector from an angle.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="angle"></param>
        /// <returns>A Vector2 object</returns>
        public static Vector2 AngleToVector2(float angle)
        {
            float x = (float)Math.Sin(angle);
            float y = (float)Math.Cos(angle);
            Vector2 ang = new Vector2(-x, y);
            ang.Normalize();
            return ang;
        }

        private static Random _rand = new Random();

        public static float RandomFloat()
        {
            return (float)_rand.NextDouble();
        }

        public static float RandomFloat(float minValue, float maxValue)
        {
            var delta = maxValue - minValue;
            return (RandomFloat() * delta) + minValue;
        }
    }
}
