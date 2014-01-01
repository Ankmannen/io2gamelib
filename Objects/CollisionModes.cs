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

namespace io2GameLib.Objects
{
    public enum CollisionModes
    {
        /// <summary>
        /// The object is not involved in collisions
        /// </summary>
        NoCollision = 0,

        /// <summary>
        /// Simple box calculated at startup. Does not rotate with the object
        /// </summary>
        FixedBox = 1,

        /// <summary>
        /// A box containing all points of the rotated vertices that make up the Bounding Box.
        /// </summary>
        BoundingBox = 2,

        /// <summary>
        /// Separation Of Axis theorem is used to determine collision. Both objects involved in the collision 
        /// must have this value set. Otherwise it falls back on BoundingBox or FixedBox collision testing.
        /// </summary>
        SatBox
    }
}
