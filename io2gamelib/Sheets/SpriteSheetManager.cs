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

namespace io2GameLib.Sheets
{
    public class SpriteSheetManager
    {

        /// <summary>
        /// Stores all textures used
        /// </summary>
        Dictionary<string, SpriteSheet> sheets = new Dictionary<string, SpriteSheet>();

        public void AddSpriteSheet(string key, Texture2D texture, int frameWidth, int frameHeight)
        {
            if (sheets.ContainsKey(key))
                sheets.Remove(key);

            var sheet = new SpriteSheet(texture, frameWidth, frameHeight);
            sheets.Add(key, sheet);
        }

        public void AddSpriteSheet(string key, string textureAssetName, int frameWidth, int frameHeight)
        {
            var content = Io2GameLibGame.Instance.Content;
            var sheet = new SpriteSheet(content.Load<Texture2D>(textureAssetName), frameWidth, frameHeight);

            if (sheets.ContainsKey(key))
                sheets.Remove(key);

            sheets.Add(key, sheet);
        }

        public Rectangle GetSourceRectangle(string sheetKey, int frameNumber)
        {
            var sheet = GetSpriteSheet(sheetKey);
            return sheet.GetSourceRectangle(sheetKey, frameNumber);
        }

        public Rectangle GetSourceRectangle(string sheetKey, int x, int y)
        {
            var sheet = GetSpriteSheet(sheetKey);
            return sheet.GetSourceRectangle(sheetKey, x, y);
        }

        public SpriteSheet GetSpriteSheet(string sheetKey)
        {
            return sheets[sheetKey];
        }

       

    }
}
