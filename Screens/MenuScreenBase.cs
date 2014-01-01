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
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using io2GameLib.Screens;

namespace io2GameLib.Screens
{
    public abstract class MenuScreenBase : ScreenBase
    {
        string _menuTitle;
        SpriteFont _headerFont;
        List<MenuEntry> _entries = new List<MenuEntry>();
        int _selectedEntry;
        float _keydownRepeatDuration;
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="menuTitle"></param>
        /// <param name="screenToFollow">The screen that will show after the splash</param>
        public MenuScreenBase(string menuTitle)
        {
            _menuTitle = menuTitle;
            TransitionOnTime = TimeSpan.FromSeconds(0.5f);
            TransitionOffTime = TimeSpan.FromSeconds(0.5f);
        }

        public void AddEntry(MenuEntry entry)
        {
            _entries.Add(entry);
        }

        public override void LoadContent(ContentManager content)
        {
            _headerFont = content.Load<SpriteFont>("io2GameLib/GameLibHeaderFont");
            base.LoadContent(content);
        }

        public MenuEntry SelectedEntry
        {
            get
            {
                if (_entries.Count == 0)
                    return null;

                return _entries[_selectedEntry];
            }
          
        }

        public override void HandleInput()
        {
            // Wait to handle input until this timer reaches zero
            if (_keydownRepeatDuration > 0)
                return;

            // Do we have any entries to move between?
            if (_entries.Count == 0)
                return;
            
            // Replace with a helper
            var input = ScreenManager.InputManager;


            if (input.IsNewKeyPressed(Keys.Up))
            {
                _selectedEntry--;
                if (_selectedEntry < 0)
                    _selectedEntry = _entries.Count - 1;
                _keydownRepeatDuration = 200;
            }

            if (input.IsNewKeyPressed(Keys.Down))
            {
                _selectedEntry++;
                if (_selectedEntry >= _entries.Count)
                    _selectedEntry = 0;
                _keydownRepeatDuration = 200;
            }

            if (input.IsNewKeyPressed(Keys.Enter) || input.IsNewKeyPressed(Keys.Space))
            {
                MenuEntry entry = _entries[_selectedEntry];
                entry.RaiseSelectedEvent();
            }
            
            base.HandleInput();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            // Wait 400 ms before making the keyboard active
            if (this.ScreenState != ScreenState.Active)
                _keydownRepeatDuration = 0;

            // Update each entry
            foreach (var entry in _entries)
            {
                bool isSelected = IsActive && (entry == _entries[_selectedEntry]);

                entry.Update(this, isSelected, gameTime);
            }

            // Count down the repeat duration used to block the keyboard
            // between calls.
            if (_keydownRepeatDuration > 0)
                _keydownRepeatDuration -= gameTime.ElapsedGameTime.Milliseconds;

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            Vector2 position = new Vector2(100, 250);
            if (ScreenState == ScreenState.TransitionOn)
                position.X -= transitionOffset * 256;
            else
                position.X += transitionOffset * 512 * 1.5f;

            // Modify the alpha to fade text out during transitions.
            Color color = TransitionColor(Color.White);
       
            ScreenManager.SpriteBatch.DrawString(_headerFont, _menuTitle, position, color);

            // Draw each entry
            position.Y += _headerFont.LineSpacing * 1.5f;
            foreach (var entry in _entries)
            {
                bool isSelected = IsActive && (entry == _entries [_selectedEntry]);

                entry.Draw(this, position, isSelected, gameTime);
                position.Y += _headerFont.LineSpacing;
            }

            base.Draw(gameTime);
        }
    }
}
