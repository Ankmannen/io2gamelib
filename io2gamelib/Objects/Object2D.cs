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
using io2GameLib.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace io2GameLib.Objects
{
    /// <summary>
    /// Base class for a 2D object
    /// </summary>
    public abstract class Object2D
    {
        #region Fields

        /// <summary>
        /// This will be initialized at first call to the public property StateManager
        /// </summary>
        private StateManager _stateManager = null;

        /// <summary>
        /// Unique ID of the object. Set to the hashcode at creation. Can be reset to another value
        /// when needed, as in the case of a multiplayer environment.
        /// </summary>
        public long ObjectId;

        /// <summary>
        /// A unique ID from the type of the object that is created at construction time. Can be used
        /// to compare if objects are of the same type.
        /// </summary>
        public long ObjectTypeId;
        public Vector2 Position;
        public Vector2 Momentum = Vector2.Zero;
        public float Speed = 0.2f;
        public PathManager Path;
        public Vector2 Origin = Vector2.Zero;
        public float Angle = 0f;

        public CollisionCategory CollisionCategory { get; set; }
        public CollisionModes CollisionMode { get; set; }

        public Texture2D Texture;

        public int Width = 0;
        public int Height = 0;
        public float Scale = 1.0f;
        public bool IgnoreOffset;
        public bool IgnoreScale;
        public bool EnableTileInteraction = true;
        public bool IsEnabled = true;
        public bool IsVisible = true;
        public bool MarkForDeletion = false;
        public bool IsInitialized = false;
        public bool IsContentLoaded = false;

        /// <summary>
        /// Contains boundingboxes. To keep track of which boundingbox that is hit, keep local
        /// references in derived classes to compare against.
        /// </summary>
        /// <remarks>
        /// If this collection is empty at Initialization-time, a default box will be created
        /// for you. If you do not want collisions, set CanCollide = false.
        /// </remarks>
        public List<GameLibBoundingBox> BoundingBoxes { get; set; }

        /// <summary>
        /// Keeps a list of currently colliding objects. The list is reset at the end of each
        /// update cycle
        /// </summary>
        protected List<Object2D> collidingObjects = new List<Object2D>(30);

        /// <summary>
        /// Reference to the manager that handles this object if its attached to one
        /// </summary>
        internal ObjectManager objectManager;


        #endregion

        #region Properties

        public Color DrawColor { get; set; }

        public StateManager StateManager
        {
            get
            {
                if (_stateManager == null)
                {
                    _stateManager = new StateManager(this);
                }

                return _stateManager;
            }
        }

        public ObjectManager ObjectManager
        {
            get
            {

                var om = objectManager;
                if (om != null)
                    return om;

                // TODO Quick hack, this needs to be traversed up to the root object
                return null; // this.Parent.objectManager;
            }
        }

        public InputManager InputManager
        {
            get
            {
                if (this.ObjectManager == null)
                {
                    throw new Exception("ObjectManager is null, the object must be added to an ObjectManager");
                }

                return ObjectManager.BaseScreen.ScreenManager.InputManager;
            }
        }

        /// <summary>
        /// Returns a list of objects that this object is colliding with.
        /// </summary>
        /// <remarks>
        /// The list is reset at each object update and must be checked before that
        /// </remarks>
        public List<Object2D> CollidingObjects
        {
            get { return collidingObjects; }
        }


        #endregion

        #region Events
        public delegate void OnDrawHandler(Object2D obj);
        public event OnDrawHandler OnDraw;
        #endregion

        #region Initialization

        public Object2D()
        {
            ObjectId = GetHashCode();
            ObjectTypeId = GetType().GetHashCode();

            DrawColor = Color.White;
            Path = new PathManager();
            BoundingBoxes = new List<GameLibBoundingBox>();

            // This is the default value
            CollisionMode = CollisionModes.NoCollision;
        }

        /// <summary>
        /// Override this method to load resources. DO NOT initialize any variables in this method, use Initialize() for that.
        /// </summary>
        /// <param name="content">The current ContextManager</param>
        public virtual void LoadContent(ContentManager content)
        {
            if (Texture != null)
            {
                if (Width == 0)
                {
                    Width = (int)(Texture.Width);
                }
                if (Height == 0)
                {
                    Height = (int)(Texture.Height);
                }
            }

            if (BoundingBoxes.Count == 0)
            {
                // Create a default boundingbox
                SetupDefaultBoundingBox();
            }

            Initialize();
            IsContentLoaded = true;
        }

        public void SetupDefaultBoundingBox()
        {
            if (Texture == null)
            {
                throw new Exception("Texture has to be set to create a boundingbox");
            }

            BoundingBoxes.Clear();

            var box = new GameLibBoundingBox(new Rectangle(0, 0, Width, Height), Origin, 0, Angle);
            BoundingBoxes.Add(box);
        }

        #endregion

        #region Draw and update
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="offset">The offset to use when drawing this object</param>
        /// <param name="scale">The scale to draw the object with, not to be confused with Object2D.Scale</param>
        /// <param name="spritebatch"></param>
        public virtual void Draw(GameTime gameTime, Vector2 offset, float worldScale, SpriteBatch spritebatch)
        {
            if (!IsInitialized)
                return;

            if (!IsVisible)
                return;

            if (Texture == null)
                return;

            Vector2 placement = TranslatePosition(offset, worldScale);


            float size = this.Scale;
            if (!IgnoreScale)
                size *= worldScale;

            spritebatch.Draw(Texture, placement, null, DrawColor, Angle, Origin,
                size, SpriteEffects.None, 0);

            foreach (var bb in BoundingBoxes)
                bb.Draw(spritebatch);

            // Fire the event to anyone who is listening
            if (OnDraw != null)
                OnDraw(this);
        }

        public float TranslateSize(float worldScale)
        {
            float size = this.Scale;
            if (!IgnoreScale)
                size *= worldScale;

            return size;
        }

        public Vector2 TranslatePosition(Vector2 offset, float worldScale)
        {
            Vector2 placement = Position * worldScale;

            if (!IgnoreOffset)
                placement += offset;

            return placement;
        }



        public virtual void Update(GameTime gameTime)
        {
            // Update the every day physics
            Position += Momentum;

            // Check if we are following a path
            if (Path.CurrentPath != null && Path.CurrentPath.IsRunning)
            {
                Path.CurrentPath.Update(gameTime);
                Position = Path.CurrentPath.CurrentPathPosition;
            }

            // Check if we have a state manager
            if (_stateManager != null)
            {
                if (_stateManager.CurrentState != null)
                {
                    _stateManager.CurrentState.Update(gameTime);
                }
            }

            // Reset colliding list
            collidingObjects.Clear();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Returns the rectangle that this object covers on screen.
        /// <param name="offset">the estimated offset</param>
        /// </summary>
        public Rectangle CoveringRectangle(Vector2 offset)
        {
            var rect = new Rectangle();

            Vector2 placement = Position;
            if (!IgnoreOffset)
                placement += offset;

            float scale = 1.0f;
            if (!IgnoreScale)
                scale = Scale;

            rect.X = (int)placement.X;
            rect.Y = (int)placement.Y;
            rect.Width = (int)(Width * Scale);
            rect.Height = (int)(Height * Scale);

            return rect;
        }



        /// <summary>
        /// Override this method to perform initialization of variables.
        /// </summary>
        /// <remarks>
        /// Objects are often reused by using the ObjectBroker. Remember to 
        /// reset the object to its default state in this method. Do not
        /// set default state in the constructor of the object since it's
        /// only called once.
        /// </remarks>
        public virtual void Initialize()
        {
            this.MarkForDeletion = false;
            this.IsEnabled = true;
            this.IsVisible = true;
            this.IsInitialized = true;
        }

        //public virtual void OnObjectCollision(Object2D collidingObject) { collidingObjects.Add(collidingObject); }
        public virtual void OnObjectCollision(Object2D collidingObject, GameLibBoundingBox sourceBoundingBox, GameLibBoundingBox targetBoundingBox) { collidingObjects.Add(collidingObject); }

        /// <summary>
        /// Override this method to hook in to the lifecycle of an object as its added to
        /// and ObjectManager.
        /// </summary>
        /// <param name="manager"></param>
        public virtual void OnAddToObjectManager(ObjectManager manager) { }

        /// <summary>
        /// Called by the framework just before removal from the ObjectManager
        /// </summary>
        public virtual void CleanUp()
        {
            this.CollidingObjects.Clear();
            this.objectManager = null;
        }

        /// <summary>
        /// Sets the origin to half-size of the Texture property
        /// </summary>
        /// <remarks>
        /// The origin is the local point to which rotation is applied using the Angle-property
        /// </remarks>
        public void CenterOriginToTexture()
        {
            CenterOriginToTexture(Texture);
        }

        /// <summary>
        /// Sets the origin to half-size of the passed texture.
        /// </summary>
        /// <param name="texture">The texture to extract size from</param>
        /// <remarks>
        /// The origin is the local point to which rotation is applied using the Angle-property
        /// </remarks>
        public void CenterOriginToTexture(Texture2D texture)
        {
            if (texture == null)
            {
                throw new ArgumentException("The texture is null");
            }

            Origin = new Vector2(texture.Height / 2, texture.Width / 2);
        }

        #endregion


    }
}
