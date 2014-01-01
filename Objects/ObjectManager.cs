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
using io2GameLib.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using io2GameLib.Input;
using Microsoft.Xna.Framework.Input;
using io2GameLib.Sheets;

namespace io2GameLib.Objects
{
    public class ObjectManager
    {
        #region Fields

        /// <summary>
        /// If this number is less than the ScreenManager FrameNumber then
        /// this ObjectManager has not handled the update call for this frame yet.
        /// </summary>
        long _handledUpdateFrameNumber;

        /// <summary>
        /// If this number is less than the ScreenManager FrameNumber then
        /// this ObjectManager has not handled the draw call for this frame yet.
        /// </summary>
        long _handledDrawFrameNumber;

        List<Object2D> _list = new List<Object2D>(500);
        List<Object2D> _deleteList = new List<Object2D>(100);
        List<Object2D> _addList = new List<Object2D>(100);
        ScreenBase screen;

        Texture2D _boundingBoxTexture = null;

        // This list is used to keep track of objects that are already 
        // drawn. It is reset in Update. The purpose is for isometric games
        // when tiles that have references to objects order them to get drawn
        // before we call ObjectManager.Draw
        List<Object2D> objectsToDraw = new List<Object2D>(500);

        /// <summary>
        /// Gets the BaseScreen that owns this MapManager
        /// </summary>
        public ScreenBase BaseScreen
        {
            get { return screen; }
        }

        #endregion

        #region Properties

        public float Gravity { get; set; }
        public float Friction { get; set; }
        public SpriteSheetManager SpriteSheetManager { get; set; }
        public int Count { get { return _list.Count; } }

        public bool DrawBoundingBoxes { get; set; }

        /// <summary>
        /// Gets the items collection.
        /// </summary>
        /// <remarks>
        /// You should never add or remove items to this list while enumerating it.
        /// </remarks>
        public List<Object2D> Items { get { return _list; } }

        public ObjectBroker ObjectBroker;

        #endregion

        #region Initialization

        public ObjectManager(ScreenBase screen)
        {
            this.screen = screen;
            this.Gravity = 0.8f;
            this.Friction = 0.02f;
            this.DrawBoundingBoxes = false;

            // This feels like a hack...
            var content = Io2GameLibGame.Instance.Content;
            this._boundingBoxTexture = content.Load<Texture2D>("io2GameLib/square");
            this.ObjectBroker = new ObjectBroker(content);
        }

        #endregion

        #region Draw and update

        public void Draw(GameTime gameTime, Vector2 offset, float scale)
        {
            // Check if this frame is already handled
            if (this.screen.ScreenManager.FrameNumber <= this._handledDrawFrameNumber)
            {
                return;
            }

            // Set the local framenumber to avoid double handling of the manager
            this._handledDrawFrameNumber = this.screen.ScreenManager.FrameNumber;

            SpriteBatch spriteBatch = this.screen!=null ? this.screen.ScreenManager.SpriteBatch : null;

            foreach (Object2D obj in _list)
            {
                if (obj.MarkForDeletion)
                    continue;

                var calculatedOffset = -offset;

                if (obj.IsVisible)
                {
                    obj.Draw(gameTime, calculatedOffset, scale, spriteBatch);

                    if (this.DrawBoundingBoxes)
                    {
                        //var rect = obj.BoundingBox;
                        //var c = Color.FromNonPremultiplied(255, 255, 255, 100);
                        //spriteBatch.Draw(_boundingBoxTexture, rect, c);
                    }
                }
            }
        }


        public void Update(GameTime gameTime)
        {
            // Check if this frame is already handled
            if(this.screen.ScreenManager.FrameNumber <= this._handledUpdateFrameNumber)
            {
                return;
            }

            // Set the local framenumber to avoid double handling of the manager
            this._handledUpdateFrameNumber = this.screen.ScreenManager.FrameNumber;

            foreach (Object2D obj in _addList)
            {
                internal_addObject(obj);
            }
            _addList.Clear();

            foreach (Object2D obj in _list)
            {
                if (obj.MarkForDeletion)
                {
                    _deleteList.Add(obj);
                    continue;
                }

                if (obj.IsEnabled)
                {
                    // Check keys using the local keyboard state
                    obj.Update(gameTime);
                }

                objectsToDraw.Add(obj);
            }

            foreach (var obj in _deleteList)
                internal_removeObject(obj);

            _deleteList.Clear();

            foreach (Object2D obj in _list)
            {
                if (obj.CollisionMode != CollisionModes.NoCollision)
                {
                    foreach (var boundingBox in obj.BoundingBoxes)
                    {
                        boundingBox.Update(obj.Angle, obj.Position, obj.Scale);
                    }
                }
            }
            CheckCollisions(gameTime);
        }

        private void CheckCollisions(GameTime gameTime)
        {
            // Create a new list of objects including children of objects
            for (int i = 0; i < _list.Count; i++)
            {
                Object2D objA = _list[i];
                if (objA.CollisionMode == CollisionModes.NoCollision)
                    continue;

                for (int y = 0; y < _list.Count; y++)
                {
                    if (i == y)
                        continue;

                    Object2D objB = _list[y];
                    if (objB.CollisionMode == CollisionModes.NoCollision)
                        continue;

                    // Determine if we have a category match
                    if ((objA.CollisionCategory & objB.CollisionCategory) == 0)
                        continue;

                    // Determine if the objects are of the same time and if they should not collide
                    if ((objA.CollisionCategory & CollisionCategory.Category_NotSelf) > 0 &&
                        objA.ObjectTypeId == objB.ObjectTypeId)
                        continue;

                    GameLibBoundingBox sourceBoundingBox = null;
                    GameLibBoundingBox targetBoundingBox = null;
                    
                    if(GameLibBoundingBox.TestWithInfo(objA, objB, out sourceBoundingBox, out targetBoundingBox))
                    {
                        objA.OnObjectCollision(objB, sourceBoundingBox, targetBoundingBox);
                    }
                }
            }
        }


        #endregion

        #region Public methods

        public Object2D FindObjectById(long id)
        {
            foreach (var obj in _list)
                if (obj.ObjectId == id)
                    return obj;

            return null;
        }

        public T FindFirstObject<T>(Vector2 position, float radius) where T : Object2D
        {
            foreach (var obj in _list)
            {
                if (obj is T)
                {
                    var distance = Math.Abs((obj.Position - position).Length());
                    if (distance < radius)
                        return obj as T;
                }
            }

            return null;
        }

        public List<T> FindObjects<T>(Vector2 position, float radius) where T : Object2D
        {
            var results = new List<T>();

            foreach (var obj in _list)
            {
                if (obj is T)
                {
                    var distance = Math.Abs((obj.Position - position).Length());
                    if (distance < radius)
                        results.Add(obj as T);
                }
            }

            return results;
        }

        public void AddObject(Object2D obj)
        {
            _addList.Add(obj);
        }

        /// <summary>
        /// Adds an object to the manager to be drawn and updated
        /// </summary>
        /// <param name="obj">The object to add</param>
        private void internal_addObject(Object2D obj)
        {
            if (_list.Contains(obj))
                return;

            obj.objectManager = this;

            if (this.screen != null && this.screen.ScreenManager != null && obj.IsContentLoaded == false)
            {
                obj.LoadContent(this.screen.ScreenManager.Game.Content);
            }
      
            _list.Add(obj);

            obj.OnAddToObjectManager(this);
        }

        /// <summary>
        /// Marks the object for deletion on the next frame
        /// </summary>
        /// <remarks>
        /// The objects Update or Draw method will not be called after this method is called.
        /// </remarks>
        /// <param name="obj"></param>
        public void RemoveObject(Object2D obj)
        {
            obj.MarkForDeletion = true;
        }

        private void internal_removeObject(Object2D obj)
        {
            if (obj == null)
                return;

            obj.CleanUp();
            ObjectBroker.ReturnObject(obj);
            _list.Remove(obj);
        }

        #endregion
    }
}
