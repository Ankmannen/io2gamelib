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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace io2GameLib.Objects
{
    public class GameLibBoundingBox
    {
        #region Fields

        /// <summary>
        /// Keeps track of the current angle of all calculated boxes so we don't have to recalculate every frame
        /// </summary>
        private float _currentAngle;

        /// <summary>
        /// Axis A of the bounding box
        /// </summary>
        private Vector2 _axisA;

        /// <summary>
        /// Axis B of the bounding box
        /// </summary>
        private Vector2 _axisB;

        /// <summary>
        /// The vertices that make up the bounding box
        /// </summary>
        private Vector2[] _vertices;
        private int _identifier;
        private Rectangle _rectangle;
        private Vector2 _origin;
        private float _angle;
        private Vector2 _translation;
        private Texture2D _texture;
        private float _boundingRadius;

        #endregion

        #region Properties
        public Vector2 AxisA { get { return _axisA; } }
        public Vector2 AxisB { get { return _axisB; } }
        public Vector2[] Vertices { get { return _vertices; } }

        /// <summary>
        /// Returns the position of the boundingbox. Used for Lo-Fi testing
        /// in pair with the BoundingRadius property.
        /// </summary>
        public Vector2 Position { get { return _translation; } }

        /// <summary>
        /// Returns the bounding radius for the bounding box
        /// </summary>
        public float BoundingRadius { get { return _boundingRadius; } }

        internal Vector2 CalculatedCenter { get { return _translation - _origin; } }

        /// <summary>
        /// Represents a rectangle that is used for LO-FI collision testing.
        /// </summary>
        /// <remarks>
        /// Updates every time the angle of the object changes
        /// </remarks>
        internal Rectangle BoundingRectangle { get; set; }


        public bool SimpleBox { get; set; }

        #endregion 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rect">The rectangle of the object when scale is set to 1.0f</param>
        /// <param name="origin">The center of rotation when scale is set to 1.0f</param>
        /// <param name="identifier">Identifier of boundingbox</param>
        /// <param name="angle">The angle of the boundingbox relative to its center</param>
        /// <remarks>
        /// The bounding box is a representation of a hit area when the object is at scale 1.0f. The ObjectManager
        /// updates the boundingboxes and passes scale into the Update-method in this class. A good tip is to
        /// visualize the bounding box.
        /// </remarks>
        public GameLibBoundingBox(Rectangle rect, Vector2 origin, int identifier, float angle)
        {
            _identifier = identifier;
            _rectangle = rect;
            _origin = origin;
            _angle = angle;

            _vertices = new Vector2[4];
            _vertices[0] = new Vector2(rect.Left, rect.Top);
            _vertices[1] = new Vector2(rect.Right, rect.Top);
            _vertices[2] = new Vector2(rect.Right, rect.Bottom);
            _vertices[3] = new Vector2(rect.Left, rect.Bottom);

            _currentAngle = 0;
            _translation = Vector2.Zero;

            CalculateRadius();
        }

        public void Update(float angle, Vector2 position, float scale)
        {
            // No need to recalculate if the current angle and position is the same as the new one
            if (_currentAngle == angle && position == _translation)
                return;

            // Store the angle and position for later use
            _currentAngle = angle;
            _translation = position;

            // Create a vertex buffer from the rectangle
            GetVerticesFromRect(_rectangle, ref _vertices);

            // Rotate and translate
            Matrix mat = Matrix.CreateTranslation(new Vector3(-_origin, 0));
            
            if (MathHelper.WrapAngle(angle) != 0)
            {
                // Only need to rotate if we have an angle
                mat *= Matrix.CreateRotationZ(_currentAngle);
            }

            mat *= Matrix.CreateTranslation(new Vector3(position, 0));
          
            Vector2.Transform(_vertices, ref mat, _vertices);

            // Calculate the lo-fi bounding box on translated vertices
            CalculateLoFiBoundingBox();
        }

        private void CalculateLoFiBoundingBox()
        {

            // Find bounds and expand the box by one to 
            // make sure we dont't miss a collision because
            // of fragements of a decimal.
            var minX = (int)FindMinX()-1;
            var maxX = (int)FindMaxX()-1;
            var minY = (int)FindMinY()+1;
            var maxY = (int)FindMaxY()+1;

            BoundingRectangle = new Rectangle(
                 minX, minY, (maxX-minX), (maxY - minY));
        }

        /// <summary>
        /// Returns the bounding box that covers the entire object
        /// </summary>
        /// <remarks>
        /// If an object contains multiple bounding boxes or are rotated
        /// the lo-fi bounding box will cover them all. 
        /// </remarks>
        public Rectangle LoFiBoundingBox
        {
            get { return BoundingRectangle; }
        }


        private float FindMinX()
        {
            var value = _vertices[0].X;

            foreach (var vertex in _vertices)
            {
                if (vertex.X < value)
                    value = vertex.X;
            }

            return value;
        }


        private float FindMaxX()
        {
            var value = _vertices[0].X;

            foreach (var vertex in _vertices)
            {
                if (vertex.X > value)
                    value = vertex.X;
            }

            return value;
        }

        private float FindMinY()
        {
            var value = _vertices[0].Y;

            foreach (var vertex in _vertices)
            {
                if (vertex.Y < value)
                    value = vertex.Y;
            }

            return value;
        }


        private float FindMaxY()
        {
            var value = _vertices[0].Y;

            foreach (var vertex in _vertices)
            {
                if (vertex.Y > value)
                    value = vertex.Y;
            }

            return value;
        }

        private void GetVerticesFromRect(Rectangle rect, ref Vector2[] buffer)
        {
            buffer[0] = new Vector2(rect.Left, rect.Top);
            buffer[1] = new Vector2(rect.Right, rect.Top);
            buffer[2] = new Vector2(rect.Right, rect.Bottom);
            buffer[3] = new Vector2(rect.Left, rect.Bottom);
        }

        public void Draw(SpriteBatch batch)
        {
            if (_texture == null)
                _texture = Io2GameLibGame.Instance.Content.Load<Texture2D>("io2GameLib/square");

            foreach (var item in _vertices)
                batch.Draw(_texture, item, new Rectangle(0, 0, 2, 2), Color.White); 
        }

        public void CalculatedAxis()
        {

            _axisA = _vertices[1] - _vertices[0];
            _axisA.Normalize();

            _axisB = _vertices[2] - _vertices[1];
            _axisB.Normalize();
        }

        /// <summary>
        /// Determines if two GameLibBoundingBox-object are intersecting.
        /// </summary>
        /// <param name="boxA"></param>
        /// <param name="boxB"></param>
        /// <returns></returns>
        public static bool Test(GameLibBoundingBox boxA, GameLibBoundingBox boxB, bool onlyLoFiTest)
        {
            // Test "lo-fi" by bounding sphere
            var lofiCheck = LoFiTest(boxA, boxB); 
            
            if (!lofiCheck)
                return false;

            // We only want to test the bounding boxes and not the rotated ones.
            if (onlyLoFiTest)
                return true;

            // Test "hi-fi" by SAT
            boxA.CalculatedAxis();  // Update the axis
            boxB.CalculatedAxis();

            if (!TestAxis(boxA.AxisA, boxA.Vertices, boxB.Vertices))
                return false;

            if (!TestAxis(boxA.AxisB, boxA.Vertices, boxB.Vertices))
                return false;

            if (!TestAxis(boxB.AxisA, boxA.Vertices, boxB.Vertices))
                return false;

            if (!TestAxis(boxB.AxisB, boxA.Vertices, boxB.Vertices))
                return false;

            return true;
        }

        private static bool LoFiTest(GameLibBoundingBox boxA, GameLibBoundingBox boxB)
        {
            return (boxA.BoundingRectangle.Intersects(boxB.BoundingRectangle));
        }

        /// <summary>
        /// Calculates the MEC for the boundingbox
        /// </summary>
        private void CalculateRadius()
        {
            float radius = 0f;

            var vertices = new Vector2[_vertices.Length];
            GetVerticesFromRect(_rectangle, ref vertices);

            // We need to move the vertices to the origin to account for rotation
            Matrix mat = Matrix.CreateTranslation(new Vector3(-_origin, 0));
            Vector2.Transform(vertices, ref mat, vertices);

            // Find the longest radius
            foreach (var vertex in vertices)
            {
                var length = vertex.Length();
                if (radius < length)
                    radius = length;
            }

            _boundingRadius = radius;
        }

        public static bool Test(List<GameLibBoundingBox> boxesA, List<GameLibBoundingBox> boxesB, bool onlyLoFiTest)
        {
            foreach (var boxA in boxesA)
            {
                foreach (var boxB in boxesB)
                {
                    if (Test(boxA, boxB, onlyLoFiTest))
                        return true;
                }
            }

            return false;
        }


        public static bool Test(Object2D objA, Object2D objB, bool onlyLoFiTest)
        {
            return Test(objA.BoundingBoxes, objB.BoundingBoxes, onlyLoFiTest);
        }

        public static bool TestWithInfo(Object2D objA, Object2D objB, out GameLibBoundingBox sourceBoundingBox, out GameLibBoundingBox targetBoundingBox)
        {
            bool useSat = (objA.CollisionMode == CollisionModes.SatBox && objB.CollisionMode == CollisionModes.SatBox);

            foreach (var boxA in objA.BoundingBoxes)
            {
                foreach (var boxB in objB.BoundingBoxes)
                {
                    if (Test(boxA, boxB, !useSat))
                    {
                        sourceBoundingBox = boxA;
                        targetBoundingBox = boxB;
                        return true;
                    }
                }
            }

            sourceBoundingBox = null;
            targetBoundingBox = null;

            return false;
        }

        /// <summary>
        /// Tests to set of vertices projection onto the axis
        /// intersects.
        /// </summary>
        /// <param name="axis">The axis to project the vertices to</param>
        /// <param name="verticesA">The first set of vertices</param>
        /// <param name="verticesB">The second set of vertices</param>
        /// <returns>True if the vertices intersects</returns>
        private static bool TestAxis(Vector2 axis, Vector2[] verticesA, Vector2[] verticesB)
        {
            // Project A & B onto axis and find min and max
            float minA, maxA;
            float minB, maxB;
            
            FindMinMax(axis, verticesA, out minA, out maxA);
            FindMinMax(axis, verticesB, out minB, out maxB);

            // Check for intersection
            return !(minB > maxA || maxB < minA);
        }

        /// <summary>
        /// Finds the min and max values from the projection of the
        /// given vertices onto the axis.
        /// </summary>
        /// <param name="axis">A normal vector representing the axis</param>
        /// <param name="vertices">The vertices to project onto the axis</param>
        /// <param name="min">The min value of the projection</param>
        /// <param name="max">The max value of the projection</param>
        private static void FindMinMax(Vector2 axis, Vector2[] vertices, out float min, out float max)
        {
            // TODO Check if this generates garbage
            bool firstIteration = true;
            float minA = 0;
            float maxA = 0;
            
            foreach (var vertex in vertices)
            {
                var dp = Vector2.Dot(vertex, axis);
                var t = new Vector2(dp * axis.X, dp * axis.Y);  // Project
                var length = t.LengthSquared() * (dp > 0 ? 1 : -1);    // Resolve if the length is negative or positive, use the squared length to avoid extra math

                if (firstIteration || minA > length)
                    minA = length;

                if (firstIteration || maxA < length)
                    maxA = length;

                firstIteration = false;
            }

            // Assign values
            min = minA;
            max = maxA;
        }
    }
}