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
using Microsoft.Xna.Framework.Content;

namespace io2GameLib.Objects
{
    public class ObjectBroker
    {
        private ContentManager _content;

        public ObjectBroker(ContentManager content)
        {
            _content = content;
        }

        private Dictionary<Type, Stack<Object2D>> _containers = new Dictionary<Type,Stack<Object2D>>();

        public T GetObject<T>() where T : Object2D, new()
        {
            Stack<Object2D> stack;

            if (_containers.ContainsKey(typeof(T)))
            {
                // Get the container
                stack = _containers[typeof(T)];
            }
            else
            {
                // No container exists, just return the newly created object
                return CreateObject<T>();
            }

            if (stack.Count > 0)
            {
                T obj = (T)stack.Pop();

                // Reset the initialization flag
                obj.IsInitialized = false; 

                // Initialize the object
                obj.Initialize();
                
                // Assert that the object got initialized correctly
                VerifyInitialization(obj);

                return obj;
            }
            else
                return CreateObject<T>();
        }

        private void VerifyInitialization(Object2D obj)
        {
            if (!obj.IsInitialized)
                throw new Exception("The object is not initialized, did you remember to call the base.Initialize() or base.LoadContent() in the overrides?");
        }

        private T CreateObject<T>() where T : Object2D, new()
        {
            var obj = new T();
            obj.LoadContent(_content);

            VerifyInitialization(obj);

            return obj;
        }
          
        /// <summary>
        /// Returns the object to its container making it available at the next GetObject call.
        /// </summary>
        /// <param name="obj"></param>
        public void ReturnObject(Object2D obj)
        {
            // Create the container if it doesn't exist
            if (!_containers.ContainsKey(obj.GetType()))
            {
                _containers.Add(obj.GetType(), new Stack<Object2D>());
            }
            
            // Add object to container
            _containers[obj.GetType()].Push(obj);
        }
    }
}
