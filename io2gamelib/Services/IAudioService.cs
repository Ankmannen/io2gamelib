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

namespace io2GameLib.Services
{
    public interface IAudioService
    {
        /// <summary>
        /// Returns an unused instance ID
        /// </summary>
        /// <returns></returns>
        int GetAvailableInstanceId();

        /// <summary>
        /// Loads a specific sound into memory
        /// </summary>
        /// <param name="assetName"></param>
        void PreloadSound(string assetName);

        /// <summary>
        /// Gives a specific sound effect and all it's instances to the evil garbage collector
        /// </summary>
        /// <param name="assetName">The name of the asset to release</param>
        void ReleaseSound(string assetName);

        /// <summary>
        /// Removes all effects and instances from memory.
        /// </summary>
        /// <remarks>DON'T do this during gameplay since it will cause the Garbage Collector to go nuts on your framerate.</remarks>
        void Flush();


        /// <summary>
        /// Plays a sound
        /// </summary>
        /// <param name="assetName">The name of the sound effect asset to play</param>
        void PlaySound(string assetName);

        /// <summary>
        /// Plays a sound
        /// </summary>
        /// <param name="assetName">The name of the sound effect asset to play</param>
        /// <param name="instanceId">The instanceId of your sound. Pass -1 if you don't want to stop it later on</param>
        void PlaySound(string assetName, int instanceId);

        /// <summary>
        /// Plays a sound
        /// </summary>
        /// <param name="assetName">The assetName of the sound</param>
        /// <param name="instanceId">The instanceId of your sound. Pass -1 if you don't want a specific instance created</param>
        /// <param name="volume">Volume, ranging from 0.0f (silence) to 1.0f (full volume). 1.0f is full volume relative to SoundEffect.MasterVolume.</param>
        /// <param name="pitch">Pitch adjustment, ranging from -1.0f (down one octave) to 1.0f (up one octave). 0.0f is unity (normal) pitch.</param>
        /// <param name="pan">Panning, ranging from -1.0f (full left) to 1.0f (full right). 0.0f is centered.</param>
        void PlaySound(string assetName, int instanceId, float volume, float pitch, float pan);

        /// <summary>
        /// Plays a sound in a loop.
        /// </summary>
        /// <param name="assetName">The assetName of the sound</param>
        /// <param name="instanceId">The instanceId of your sound. Used to keep track of which sound to stop later on. It cannot be -1.</param>
        void LoopSound(string assetName, int instanceId);

        /// <summary>
        /// Plays a sound in a loop
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="instanceId"></param>
        /// <param name="volume">Volume, ranging from 0.0f (silence) to 1.0f (full volume). 1.0f is full volume relative to SoundEffect.MasterVolume.</param>
        /// <param name="pitch">Pitch adjustment, ranging from -1.0f (down one octave) to 1.0f (up one octave). 0.0f is unity (normal) pitch.</param>
        /// <param name="pan">Panning, ranging from -1.0f (full left) to 1.0f (full right). 0.0f is centered.</param>
        void LoopSound(string assetName, int instanceId, float volume, float pitch, float pan);

        /// <summary>
        /// Stops playing of a sound
        /// </summary>
        /// <param name="assetName">The assesname to stop</param>
        /// <param name="instanceId"></param>
        void StopSound(string assetName, int instanceId);
    }
}
