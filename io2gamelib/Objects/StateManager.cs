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
    public class StateManager
    {
        public StateManager(Object2D owner)
        {
            this.Owner = owner;
        }
        
        /// <summary>
        /// The object that owns this state manager
        /// </summary>
        public Object2D Owner { get; set; }
        
        /// <summary>
        /// Keeps an internal dictionary of states
        /// </summary>
        private Dictionary<Type, object> _states = new Dictionary<Type,object>();

        private State _currentState = null;

        /// <summary>
        /// The current state. 
        /// </summary>
        /// <remarks>Can be null, but that's bad practice</remarks>
        public State CurrentState
        {
            get { return _currentState; }
            set
            {
                if (_currentState != null)
                {
                    _currentState.Exit();
                }

                _currentState = value;
                if (_currentState != null)
                {
                    _currentState.Enter();
                }
            }

        }

        /// <summary>
        /// Gets a state of the type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>A newly initialized state</returns>
        /// <remarks>Each state exists as a singleton, that is, each Type has one
        /// instance. The StateManager will construct the state if it's not already
        /// initialized. There is no way to remove the state and it will be reused
        /// if the object2d that owns the StateManager is cached in the ObjectBroker.</remarks>
        public T GetState<T>() where T : State, new()
        {
            var t = typeof(T);
            if (_states.ContainsKey(t))
            {
                return (T)_states[t];
            }

            T instance = new T();
            instance.Owner = this.Owner;
            instance.Initialize();

            return instance;
        }
    }
}
