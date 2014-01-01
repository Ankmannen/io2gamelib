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

namespace io2GameLib.Helpers
{
    /// <summary>
    /// Simple servicelocator that stores one instance of each registered type.
    /// </summary>
    public static class ServiceLocator
    {
        private static Dictionary<Type, object> _services = new Dictionary<Type, object>();

        /// <summary>
        /// Returns a service of type T. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The requested service if it is registered. Otherwise an exception is raised.</returns>
        /// <remarks>
        /// The exception when resolving types that are not registered is to avoid later NULL-reference
        /// exceptions that are harder to track.
        /// </remarks>
        public static T Resolve<T>() where T : class
        {
            Type t = typeof(T);
            T service = (T)_services[t];

            if (service == null)
            {
                throw new Exception(String.Format("The service for type '{0}' cannot be found'", t.ToString()));
            }

            return service;
        }

        /// <summary>
        /// Register a service
        /// </summary>
        /// <typeparam name="T">The type of the service, useful if you would like to register an interface instead of a concrete object</typeparam>
        /// <param name="service">An instance of the service to register</param>
        /// <remarks>
        /// The type T cannot be registered before. If you want to reregister a service, remove it first with
        /// RemoveService&lt;T&gt;
        /// </remarks>
        public static void Register<T>(T service) where T : class
        {
            Type t = typeof(T);
            internalRegister(service, t);
        }


        /// <summary>
        /// Registers an instance based on its concrete class.
        /// </summary>
        /// <param name="service">The instance</param>
        public static void Register(object service)
        {
            internalRegister(service, service.GetType());
        }

        /// <summary>
        /// Registeres an instance based on its concrete class and removes 
        /// any already existing mappings for this type.
        /// </summary>
        /// <param name="service">The instance</param>
        public static void Reregister(object service)
        {
            _services.Remove(service.GetType());
            Register(service);
        }

        private static void internalRegister(object service, Type t) 
        {
            if (_services.ContainsKey(t))
                throw new Exception(String.Format("The service for type '{0}' is already registered", t.ToString()));

            _services.Add(t, service);
        }

        /// <summary>
        /// Removes an instance of the service.
        /// </summary>
        /// <typeparam name="T">The type to remove</typeparam>
        /// <remarks>
        /// If the service isn't found to be removed, nothing is done.
        /// </remarks>
        public static void Remove<T>() where T : class
        {
            Type t = typeof(T);
            _services.Remove(t);
        }

        /// <summary>
        /// Registers a service like Register but it removes an
        /// already existing instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service"></param>
        public static void Reregister<T>(T service) where T : class
        {
            Remove<T>();
            Register<T>(service);
        }

        /// <summary>
        /// Checks if the ServiceLocator already contains an instance of the type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool Contains<T>() where T : class
        {
            Type t = typeof(T);
            return _services.ContainsKey(t);
        }
    }
}
