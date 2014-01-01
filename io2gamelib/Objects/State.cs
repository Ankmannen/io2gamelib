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

namespace io2GameLib.Objects
{
    /// <summary>
    /// Keeps logic of an object to a given state.
    /// </summary>
    public abstract class State
    {
        #region Properties

        /// <summary>
        /// The object that owns this state
        /// </summary>
        public Object2D Owner { get; set; }

        /// <summary>
        /// Keeps track of the number of milliseconds the state has been active
        /// </summary>
        public float Duration { get; private set; }

        public State()
        {
        }

        #endregion

        /// <summary>
        /// Gets called when the state needs to return to it's initial state
        /// </summary>
        public virtual void Initialize()
        {
            Duration = 0.0f;
        }

        /// <summary>
        /// Gets called when the object enters this state
        /// </summary>
        public virtual void Enter()
        {
        }

        /// <summary>
        /// Gets called when the object exits this state
        /// </summary>
        public virtual void Exit()
        {
        }

        /// <summary>
        /// Gets called once each frame
        /// </summary>
        /// <param name="gameTime">The amount of time that passed since last call</param>
        public virtual void Update(GameTime gameTime)
        {
            Duration += gameTime.ElapsedGameTime.Milliseconds;
        }

        /// <summary>
        /// Changes the state to a new state
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void ChangeState<T>() where T : State, new() 
        {
            Owner.StateManager.CurrentState = Owner.StateManager.GetState<T>();
        }
    }
}
