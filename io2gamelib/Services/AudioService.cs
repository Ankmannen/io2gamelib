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
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace io2GameLib.Services
{
    /// <summary>
    /// Concrete implementation of the IAudioService. Using the standard SoundEffect and SoundEffectInstance classes of
    /// the XNA Framework. No fancy pants here...
    /// </summary>
    public class AudioService : IAudioService
    {

        #region Fields

        /// <summary>
        /// A storage for all soundinstances
        /// </summary>
        private Dictionary<string, Dictionary<int, SoundEffectInstance>> _soundInstances;

        /// <summary>
        /// A storage for all sound effects (non instances)
        /// </summary>
        private Dictionary<string, SoundEffect> _soundEffects;      
   
        /// <summary>
        /// Keeps a local copy of the ContentManager needed to load sound resources
        /// </summary>
        private ContentManager _content = null;

        /// <summary>
        /// Keeps track of the next available instance ID
        /// </summary>
        private int _nextInstanceId = 0;

        #endregion

        #region Initialization

        public AudioService(ContentManager content)
        {
            _soundInstances = new Dictionary<string, Dictionary<int, SoundEffectInstance>>();
            _soundEffects = new Dictionary<string, SoundEffect>();
            _content = content;
        }

        #endregion

        #region IAudioService

        public int GetAvailableInstanceId()
        {
            _nextInstanceId++;
            return _nextInstanceId;
        }

        public void PlaySound(string assetName)
        {
            PlaySound(assetName, -1, 1.0f, 0.0f, 0.0f);
        }

        public void PlaySound(string assetName, int instanceId)
        {
            PlaySound(assetName, instanceId, 1.0f, 0.0f, 0.0f);

        }

        public void PlaySound(string assetName, int instanceId, float volume, float pitch, float pan)
        {
            PlaySound(assetName, instanceId, volume, pitch, pan, false);
        }


        public void LoopSound(string assetName, int instanceId)
        {
            PlaySound(assetName, instanceId, 1.0f, 0.0f, 0.0f, true);
        }

        public void LoopSound(string assetName, int instanceId, float volume, float pitch, float pan)
        {
            PlaySound(assetName, instanceId, volume, pitch, pan, true);
        }

        public void StopSound(string assetName, int instanceId)
        {
            var instance = GetSoundEffectInstance(assetName, instanceId);
            if (instance == null)
                return;

            instance.Stop();
        }

        public void PreloadSound(string assetName)
        {
            throw new NotImplementedException();
        }

        public void ReleaseSound(string assetName)
        {
            throw new NotImplementedException();
        }

        public void Flush()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private methods

        private void PlaySound(string assetName, int instanceId, float volume, float pitch, float pan, bool loop)
        {
            var soundEffect = GetSoundEffect(assetName);

            if (instanceId == -1)
            {
                soundEffect.Play(volume, pitch, pan);
            }

            Dictionary<int, SoundEffectInstance> instanceDictionary;

            // Check if the instance-specific part already exists
            if (!_soundInstances.ContainsKey(assetName))
            {
                instanceDictionary = new Dictionary<int, SoundEffectInstance>();
                _soundInstances.Add(assetName, instanceDictionary);
            }

            instanceDictionary = _soundInstances[assetName];

            // Check if there is an effect with the current instance ID
            SoundEffectInstance instance = null;
            if (!instanceDictionary.ContainsKey(instanceId))
            {
                instance = soundEffect.CreateInstance();
                instance.IsLooped = loop;
                instanceDictionary.Add(instanceId, instance);
            }
            else
            {
                instance = instanceDictionary[instanceId];
            }

            if (instance.State != SoundState.Stopped)
            {
                instance.Stop();
            }

            instance.Play();
        }

        private SoundEffect GetSoundEffect(string assetName)
        {
            if (!_soundEffects.ContainsKey(assetName))
            {
                _soundEffects.Add(assetName, _content.Load<SoundEffect>(assetName));
            }
            return _soundEffects[assetName];
        }

        private SoundEffectInstance GetSoundEffectInstance(string assetName, int instanceId)
        {
            if(!_soundInstances.ContainsKey(assetName))
                return null;

            var effect = _soundInstances[assetName];
            if (effect == null)
            {
                return null;
            }

            var instance = effect[instanceId];
            if (instance == null)
            {
                return null;
            }

            return instance;
        }

#endregion
       
    }
}
