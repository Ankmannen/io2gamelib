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
using io2GameLib.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace io2GameLib.Input
{
	/// <summary>
	/// Helper for keeping track of input.
	/// </summary>
	public class InputManager
	{
		public KeyboardState LastKeyboardState;
		public KeyboardState CurrentKeyboardState;

        public TouchCollection LastTouchCollection;
        public TouchCollection CurrentTouchCollection;

		public InputManager()
		{
            TouchPanel.EnabledGestures = GestureType.FreeDrag | GestureType.Flick;
		}

		public void Update()
		{
			LastKeyboardState = CurrentKeyboardState;
			CurrentKeyboardState = Keyboard.GetState();

            LastTouchCollection = CurrentTouchCollection;
            CurrentTouchCollection = TouchPanel.GetState();

			if (readToTextInputBuffer)
			{
				foreach (var key in LastKeyboardState.GetPressedKeys())
				{
					if (CurrentKeyboardState.IsKeyUp(key))
					{
						// Key was released, add to buffer
						if (key == Keys.Back)
						{
							if (textInputBuffer.Length > 0)
								textInputBuffer = textInputBuffer.Substring(0, TextInputBuffer.Length - 1);
						}
						else
						{
							bool shift = CurrentKeyboardState.IsKeyDown(Keys.LeftShift) || CurrentKeyboardState.IsKeyDown(Keys.RightShift);
                            bool altgr = CurrentKeyboardState.IsKeyDown(Keys.RightAlt) || (CurrentKeyboardState.IsKeyDown(Keys.LeftAlt) && CurrentKeyboardState.IsKeyDown(Keys.LeftControl));
							
							char chr = TranslateChar(key, shift, false, false, altgr);
							if(chr!='\0')
								textInputBuffer += chr;
						}
					}
				}
			}

            // handle touch related stuff
            UpdateTouch();
		}

		/// <summary>
		/// Checks if the key was pressed for the first time on this frame or
		/// if it was pressed in an earlier frame.
		/// </summary>
		/// <param name="key">The key to check for</param>
		/// <returns></returns>
		public bool IsNewKeyPressed(Keys key)
		{
			return (CurrentKeyboardState.IsKeyDown(key) && LastKeyboardState.IsKeyUp(key));
		}

		#region Text input implementation
		bool readToTextInputBuffer = false;
		string textInputBuffer = String.Empty;

		public void ClearTextInputBuffer()
		{
			textInputBuffer = String.Empty;
		}

		public void StartReadingKeysToTextBuffer()
		{
			readToTextInputBuffer = true;
		}

		public void StopReadingKeysToTextBuffer()
		{
			readToTextInputBuffer = false;
		}

		public string TextInputBuffer
		{
			get { return textInputBuffer; }
			set { textInputBuffer = value; }
		}

		public static char TranslateChar(Keys key, bool shift, bool capsLock, bool numLock, bool altGr)
		{
			switch (key)
			{
				case Keys.A: return TranslateAlphabetic('a', shift, capsLock);
				case Keys.B: return TranslateAlphabetic('b', shift, capsLock);
				case Keys.C: return TranslateAlphabetic('c', shift, capsLock);
				case Keys.D: return TranslateAlphabetic('d', shift, capsLock);
				case Keys.E: return TranslateAlphabetic('e', shift, capsLock);
				case Keys.F: return TranslateAlphabetic('f', shift, capsLock);
				case Keys.G: return TranslateAlphabetic('g', shift, capsLock);
				case Keys.H: return TranslateAlphabetic('h', shift, capsLock);
				case Keys.I: return TranslateAlphabetic('i', shift, capsLock);
				case Keys.J: return TranslateAlphabetic('j', shift, capsLock);
				case Keys.K: return TranslateAlphabetic('k', shift, capsLock);
				case Keys.L: return TranslateAlphabetic('l', shift, capsLock);
				case Keys.M: return TranslateAlphabetic('m', shift, capsLock);
				case Keys.N: return TranslateAlphabetic('n', shift, capsLock);
				case Keys.O: return TranslateAlphabetic('o', shift, capsLock);
				case Keys.P: return TranslateAlphabetic('p', shift, capsLock);
				case Keys.Q: return TranslateAlphabetic('q', shift, capsLock);
				case Keys.R: return TranslateAlphabetic('r', shift, capsLock);
				case Keys.S: return TranslateAlphabetic('s', shift, capsLock);
				case Keys.T: return TranslateAlphabetic('t', shift, capsLock);
				case Keys.U: return TranslateAlphabetic('u', shift, capsLock);
				case Keys.V: return TranslateAlphabetic('v', shift, capsLock);
				case Keys.W: return TranslateAlphabetic('w', shift, capsLock);
				case Keys.X: return TranslateAlphabetic('x', shift, capsLock);
				case Keys.Y: return TranslateAlphabetic('y', shift, capsLock);
				case Keys.Z: return TranslateAlphabetic('z', shift, capsLock);

				case Keys.D0: return (shift) ? ')' : '0';
				case Keys.D1: return (shift) ? '!' : '1';
				case Keys.D2: return (shift || altGr) ? '@' : '2';
				case Keys.D3: return (shift) ? '#' : '3';
				case Keys.D4: return (shift) ? '$' : '4';
				case Keys.D5: return (shift) ? '%' : '5';
				case Keys.D6: return (shift) ? '^' : '6';
				case Keys.D7: return (shift) ? '&' : '7';
				case Keys.D8: return (shift) ? '*' : '8';
				case Keys.D9: return (shift) ? '(' : '9';

				//case Keys.Add: return '+';
				//case Keys.Divide: return '/';
				//case Keys.Multiply: return '*';
				//case Keys.Subtract: return '-';

				//case Keys.Space: return ' ';
				//case Keys.Tab: return '\t';

             

				case Keys.Decimal: if (numLock && !shift) return '.'; break;
				case Keys.NumPad0: if (numLock && !shift) return '0'; break;
				case Keys.NumPad1: if (numLock && !shift) return '1'; break;
                case Keys.NumPad2: if (numLock && !shift) return '2'; break;
				case Keys.NumPad3: if (numLock && !shift) return '3'; break;
				case Keys.NumPad4: if (numLock && !shift) return '4'; break;
				case Keys.NumPad5: if (numLock && !shift) return '5'; break;
				case Keys.NumPad6: if (numLock && !shift) return '6'; break;
				case Keys.NumPad7: if (numLock && !shift) return '7'; break;
				case Keys.NumPad8: if (numLock && !shift) return '8'; break;
				case Keys.NumPad9: if (numLock && !shift) return '9'; break;

				case Keys.OemPeriod: return '.';
				//case Keys.OemBackslash: return shift ? '|' : '\\';
				//case Keys.OemCloseBrackets: return shift ? '}' : ']';
				//case Keys.OemComma: return shift ? '<' : ',';
				//case Keys.OemMinus: return shift ? '_' : '-';
				//case Keys.OemOpenBrackets: return shift ? '{' : '[';
				//case Keys.OemPeriod: return shift ? '>' : '.';
				//case Keys.OemPipe: return shift ? '|' : '\\';
				//case Keys.OemPlus: return shift ? '+' : '=';
				//case Keys.OemQuestion: return shift ? '?' : '/';
				//case Keys.OemQuotes: return shift ? '"' : '\'';
				//case Keys.OemSemicolon: return shift ? ':' : ';';
				//case Keys.OemTilde: return shift ? '~' : '`';
			}

			return (char)0;
		}

		public static char TranslateAlphabetic(char baseChar, bool shift, bool capsLock)
		{
			return (capsLock ^ shift) ? char.ToUpper(baseChar) : baseChar;
		}

		#endregion

		// Move to inputManager???
		public bool HasKeyboardChanges()
		{
			if (this.CurrentKeyboardState.GetPressedKeys().Length != this.LastKeyboardState.GetPressedKeys().Length)
				return true;

			foreach (var key in CurrentKeyboardState.GetPressedKeys())
				if (LastKeyboardState.IsKeyUp(key))
					return true;

			return false;
		}

        public bool HasTouchChanges()
        {
            if (LastTouchCollection.Count != CurrentTouchCollection.Count)
                return true;

            return false;
        }

        /// <summary>
        /// No touch events registered this frame
        /// </summary>
        /// <returns></returns>
        public bool HasNoTouch()
        {
            return CurrentTouchCollection.Count == 0;
        }

        public bool HasNewTouch()
        {
            // This function can be blocked if ResetHasNewTouch() is called.
            if (_hasNewTouchBlocked)
                return false;

            if (CurrentTouchCollection.Count == 0)
                return false;

            if (LastTouchCollection.Count == 0 && CurrentTouchCollection.Count > 0)
                return true;

            return false;
        }

        /// <summary>
        /// Makes HasNewTouch return false for the rest of the frame.
        /// </summary>
        /// <remarks>
        /// Until InputManager.Update() is called again.
        /// </remarks>
        public void ResetHasNewTouch()
        {
            _hasNewTouchBlocked = true;
        }
        private bool _hasNewTouchBlocked = false;

        #region Touch stuff

        private float _horizontalDrag;
        private float _verticalDrag;

        /// <summary>
        /// Determines if there is a touch (Moved or Pressed) within
        /// the given rectangle. 
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        /// <remarks>
        /// Returns the momentary result. Does not keep track of last frame.
        /// </remarks>
        public bool TouchWithin(Microsoft.Xna.Framework.Rectangle rectangle)
        {
            foreach (var location in CurrentTouchCollection)
            {
                if (location.State == TouchLocationState.Pressed || location.State == TouchLocationState.Moved)
                {
                    if (rectangle.Contains(location.Position.ToPoint()))
                        return true;
                }
            }

            return false;
        }

        public bool IsTouched()
        {
            return CurrentTouchCollection.Count > 0;
        }

        public float GetHorizontalDrag()
        {
            return _horizontalDrag;
        }

        public float GetVerticalDrag()
        {
            return _verticalDrag;
        }

        private void UpdateTouch()
        {
            // Reset some states
            _horizontalDrag = 0;
            _verticalDrag = 0;
            _hasNewTouchBlocked = false;
           
            if (!TouchPanel.IsGestureAvailable)
                return;

            var gesture = TouchPanel.ReadGesture();

            if (gesture.GestureType == GestureType.FreeDrag)
            {
                _horizontalDrag = MathHelper.Clamp(gesture.Delta.X, -15, 15);
                _verticalDrag = MathHelper.Clamp(gesture.Delta.Y, -15, 15);
            }
        }

        private bool _wasBackButtonPressedLastFrame = false;
        public bool IsBackButtonNewlyPressed()
        {
            if (_wasBackButtonPressedLastFrame)
                return false;

            return internal_IsBackButtonPressed();
        }

        /// <summary>
        /// If the backbutton is checked by a new screen within the same frame, 
        /// mark it as handled to avoid it being handled twice.
        /// </summary>
        public void MarkBackButtonAsHandled()
        {
            _wasBackButtonPressedLastFrame = true;
        }

        private bool internal_IsBackButtonPressed()
        {
            return (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed);
        }

        public void PostLogicUpdate()
        {
            // Remember the state to the next frame
            _wasBackButtonPressedLastFrame = internal_IsBackButtonPressed();
             
        }

        #endregion
    }
}
