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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace io2GameLib.Screens.SelectionPopup
{
    /// <summary>
    /// Creates a screen from where the user can select one entry
    /// </summary>
    public class SelectionPopupScreen : PopupScreenBase 
    {
        List<SelectionItem> items = new List<SelectionItem>();
        int selectedItemIndex = 0;
        string header;

        public SelectionPopupScreen(string header)
        {
            this.header = header;
        }

        public void AddItem(SelectionItem item)
        {
            items.Add(item);
        }

        public void AddItem(string text, bool closeOnSelection, SelectionItem.EntrySelectedHandler selectionHandler)
        {
            var item = new SelectionItem(text, closeOnSelection);
            if(selectionHandler!=null)
                item.EntrySelected += selectionHandler;
            
            AddItem(item);
        }

        

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            // Draw the base first
            base.Draw(gameTime);

            // Get some needed objects
            SpriteBatch spritebatch = ScreenManager.SpriteBatch;
            Rectangle popupRect = Bounds;

            // Draw the header
            spritebatch.DrawString(ScreenManager.SharedHeaderFont, header, new Vector2(popupRect.X + 10, popupRect.Y + 10), Color.White);

            // Draw the items
            Vector2 position = new Vector2(popupRect.Left + 40, popupRect.Y + 100);

            foreach (var item in items)
            {
                bool isSelected = IsActive && (item == items[selectedItemIndex]);

                item.Draw(this, position, isSelected, gameTime);
                position.Y += ScreenManager.SharedHeaderFont.LineSpacing * 1.6f;
            }

        }

        public override void HandleInput()
        {
            // Do we have any entries to move between?
            if (items.Count == 0)
                return;

            // Get input manager
            var input = ScreenManager.InputManager;

            if (input.IsNewKeyPressed(Keys.Up))
            {
                selectedItemIndex--;
                if (selectedItemIndex < 0)
                    selectedItemIndex = items.Count - 1;
            }

            if (input.IsNewKeyPressed(Keys.Down))
            {
                selectedItemIndex++;
                if (selectedItemIndex >= items.Count)
                    selectedItemIndex = 0;
            }

            if (input.IsNewKeyPressed(Keys.Enter) || input.IsNewKeyPressed(Keys.Space))
            {
                var entry = items[selectedItemIndex];
                entry.RaiseSelectedEvent(this);
            }

            if (input.IsNewKeyPressed(Keys.Escape))
                this.ExitScreen();
            
            base.HandleInput();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            // Update each entry
            foreach (var entry in items)
            {
                bool isSelected = IsActive && (entry == items[selectedItemIndex]);
                entry.Update(this, isSelected, gameTime);
            }

            // Update base
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

    }

}
