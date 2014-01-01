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

namespace io2GameLib.Objects
{
    public class Path
    {
        private int _currentPointIndex;
        private float _elapsedLengthOnSegment;
        
        public List<Vector2> Points { get; set; }
        public float Speed { get; set; }
        public Vector2 Offset { get; set; }
        public Vector2 CurrentPathPosition { get; private set; }
        public Vector2 CurrentDirection { get; private set; }
        public bool IsRunning { get; set; }

        public Path()
        {
            Points = new List<Vector2>();
        }

        public void Start()
        {
            _currentPointIndex = 0;
            if (Points.Count > 0)
                CurrentPathPosition = Points[0];
            IsRunning = true;
        }

        public void Update(GameTime gameTime)
        {
            if (!IsRunning)
                return;

            if (Points.Count < 2)
                return;

            // End of path reached
            if (_currentPointIndex == (Points.Count-1))
                return;

            Vector2 pointA = Points[_currentPointIndex];
            Vector2 pointB = Points[_currentPointIndex + 1];

            // Move forward
            _elapsedLengthOnSegment += Speed * gameTime.ElapsedGameTime.Milliseconds;

            // Check result of movement
            var length = (pointA - pointB).Length();

            while (_elapsedLengthOnSegment > length)
            {
                _elapsedLengthOnSegment = _elapsedLengthOnSegment - length;
                _currentPointIndex++;
                if (_currentPointIndex == (Points.Count - 1))
                {
                    // Last point reached
                    CurrentPathPosition = Points[Points.Count - 1];
                    IsRunning = false;
                    return;
                }

                // Recheck the length so that we havn't overrun another segment
                pointA = Points[_currentPointIndex];
                pointB = Points[_currentPointIndex + 1];
                length = (pointA - pointB).Length();
            }

            // Still in bounds
            var delta = (pointB - pointA);
            CurrentPathPosition = Offset + Vector2.Lerp(pointA, pointB, _elapsedLengthOnSegment / delta.Length());

            // Expose the direction
            CurrentDirection = delta;
            CurrentDirection.Normalize();
    
        }

        public void Pause()
        {
            IsRunning = false;
        }

        public void Resume()
        {
            IsRunning = true;
        }
    }
}
