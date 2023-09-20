﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TaleWorlds.InputSystem;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	public class FloatInputTextWidget : EditableTextWidget
	{
		public FloatInputTextWidget(UIContext context)
			: base(context)
		{
			base.PropertyChanged += this.IntegerInputTextWidget_PropertyChanged;
		}

		private void IntegerInputTextWidget_PropertyChanged(PropertyOwnerObject arg1, string arg2, object arg3)
		{
			float num;
			if (arg2 == "RealText" && (string)arg3 != this.FloatText.ToString() && float.TryParse((string)arg3, out num))
			{
				this.FloatText = num;
			}
		}

		public override void HandleInput(IReadOnlyList<int> lastKeysPressed)
		{
			int count = lastKeysPressed.Count;
			for (int i = 0; i < count; i++)
			{
				int num = lastKeysPressed[i];
				char c2 = Convert.ToChar(num);
				if (char.IsDigit(c2) || (c2 == '.' && this.GetNumberOfSeperatorsInText(base.RealText) == 0))
				{
					float num2;
					if (num != 60 && num != 62 && float.TryParse(this.GetAppendResult(num), out num2) && num2 <= this.MaxFloat && num2 >= this.MinFloat)
					{
						base.DeleteText(this._editableText.SelectedTextBegin, this._editableText.SelectedTextEnd);
						base.AppendCharacter(num);
					}
					this._cursorDirection = EditableTextWidget.CursorMovementDirection.None;
					this._isSelection = false;
				}
			}
			int tickCount = Environment.TickCount;
			bool flag = false;
			bool flag2 = false;
			if (Input.IsKeyPressed(InputKey.Left))
			{
				this._cursorDirection = EditableTextWidget.CursorMovementDirection.Left;
				flag = true;
			}
			else if (Input.IsKeyPressed(InputKey.Right))
			{
				this._cursorDirection = EditableTextWidget.CursorMovementDirection.Right;
				flag = true;
			}
			else if ((this._cursorDirection == EditableTextWidget.CursorMovementDirection.Left && !Input.IsKeyDown(InputKey.Left)) || (this._cursorDirection == EditableTextWidget.CursorMovementDirection.Right && !Input.IsKeyDown(InputKey.Right)))
			{
				this._cursorDirection = EditableTextWidget.CursorMovementDirection.None;
				if (!Input.IsKeyDown(InputKey.LeftShift))
				{
					this._isSelection = false;
				}
			}
			else if (Input.IsKeyReleased(InputKey.LeftShift))
			{
				this._isSelection = false;
			}
			else if (Input.IsKeyDown(InputKey.Home))
			{
				this._cursorDirection = EditableTextWidget.CursorMovementDirection.Left;
				flag2 = true;
			}
			else if (Input.IsKeyDown(InputKey.End))
			{
				this._cursorDirection = EditableTextWidget.CursorMovementDirection.Right;
				flag2 = true;
			}
			if (flag || flag2)
			{
				this._nextRepeatTime = tickCount + 500;
				if (Input.IsKeyDown(InputKey.LeftShift))
				{
					if (!this._editableText.IsAnySelected())
					{
						this._editableText.BeginSelection();
					}
					this._isSelection = true;
				}
			}
			if (this._cursorDirection != EditableTextWidget.CursorMovementDirection.None && (flag || flag2 || tickCount >= this._nextRepeatTime))
			{
				if (flag)
				{
					int num3 = (int)this._cursorDirection;
					if (Input.IsKeyDown(InputKey.LeftControl))
					{
						num3 = base.FindNextWordPosition(num3) - this._editableText.CursorPosition;
					}
					base.MoveCursor(num3, this._isSelection);
					if (tickCount >= this._nextRepeatTime)
					{
						this._nextRepeatTime = tickCount + 30;
					}
				}
				else if (flag2)
				{
					int num4 = ((this._cursorDirection == EditableTextWidget.CursorMovementDirection.Left) ? (-this._editableText.CursorPosition) : (this._editableText.VisibleText.Length - this._editableText.CursorPosition));
					base.MoveCursor(num4, this._isSelection);
					if (tickCount >= this._nextRepeatTime)
					{
						this._nextRepeatTime = tickCount + 30;
					}
				}
			}
			bool flag3 = false;
			if (Input.IsKeyPressed(InputKey.BackSpace))
			{
				flag3 = true;
				this._keyboardAction = EditableTextWidget.KeyboardAction.BackSpace;
				this._nextRepeatTime = tickCount + 500;
			}
			else if (Input.IsKeyPressed(InputKey.Delete))
			{
				flag3 = true;
				this._keyboardAction = EditableTextWidget.KeyboardAction.Delete;
				this._nextRepeatTime = tickCount + 500;
			}
			if ((this._keyboardAction == EditableTextWidget.KeyboardAction.BackSpace && !Input.IsKeyDown(InputKey.BackSpace)) || (this._keyboardAction == EditableTextWidget.KeyboardAction.Delete && !Input.IsKeyDown(InputKey.Delete)))
			{
				this._keyboardAction = EditableTextWidget.KeyboardAction.None;
			}
			if (Input.IsKeyReleased(InputKey.Enter))
			{
				base.EventFired("TextEntered", Array.Empty<object>());
				return;
			}
			if (this._keyboardAction == EditableTextWidget.KeyboardAction.BackSpace || this._keyboardAction == EditableTextWidget.KeyboardAction.Delete)
			{
				if (flag3 || tickCount >= this._nextRepeatTime)
				{
					if (this._editableText.IsAnySelected())
					{
						base.DeleteText(this._editableText.SelectedTextBegin, this._editableText.SelectedTextEnd);
					}
					else if (Input.IsKeyDown(InputKey.LeftControl))
					{
						int num5 = base.FindNextWordPosition(-1) - this._editableText.CursorPosition;
						base.DeleteText(this._editableText.CursorPosition + num5, this._editableText.CursorPosition);
					}
					else
					{
						base.DeleteChar(this._keyboardAction == EditableTextWidget.KeyboardAction.Delete);
					}
					if (tickCount >= this._nextRepeatTime)
					{
						this._nextRepeatTime = tickCount + 30;
						return;
					}
				}
			}
			else if (Input.IsKeyDown(InputKey.LeftControl))
			{
				if (Input.IsKeyPressed(InputKey.A))
				{
					this._editableText.SelectAll();
					return;
				}
				if (Input.IsKeyPressed(InputKey.C))
				{
					base.CopyText(this._editableText.SelectedTextBegin, this._editableText.SelectedTextEnd);
					return;
				}
				if (Input.IsKeyPressed(InputKey.X))
				{
					base.CopyText(this._editableText.SelectedTextBegin, this._editableText.SelectedTextEnd);
					base.DeleteText(this._editableText.SelectedTextBegin, this._editableText.SelectedTextEnd);
					return;
				}
				if (Input.IsKeyPressed(InputKey.V))
				{
					base.DeleteText(this._editableText.SelectedTextBegin, this._editableText.SelectedTextEnd);
					string text = Regex.Replace(Input.GetClipboardText(), "[<>]+", " ");
					text = new string(text.Where((char c) => char.IsDigit(c)).ToArray<char>());
					base.AppendText(text);
				}
			}
		}

		private int GetNumberOfSeperatorsInText(string realText)
		{
			return realText.Count((char c) => char.IsPunctuation(c));
		}

		private string GetAppendResult(int lastPressedKey)
		{
			if (base.MaxLength > -1 && base.Text.Length >= base.MaxLength)
			{
				return base.RealText;
			}
			string realText = base.RealText;
			if (this._editableText.SelectedTextBegin != this._editableText.SelectedTextEnd)
			{
				base.RealText.Substring(0, this._editableText.SelectedTextBegin) + base.RealText.Substring(this._editableText.SelectedTextEnd, base.RealText.Length - this._editableText.SelectedTextEnd);
			}
			int cursorPosition = this._editableText.CursorPosition;
			char c = Convert.ToChar(lastPressedKey);
			return base.RealText.Substring(0, cursorPosition) + c.ToString() + base.RealText.Substring(cursorPosition, base.RealText.Length - cursorPosition);
		}

		public override void SetAllText(string text)
		{
			base.DeleteText(0, base.RealText.Length);
			string text2 = Regex.Replace(text, "[<>]+", " ");
			text2 = new string(text2.Where((char c) => char.IsDigit(c)).ToArray<char>());
			base.AppendText(text2);
		}

		[Editor(false)]
		public float FloatText
		{
			get
			{
				return this._floatText;
			}
			set
			{
				if (this._floatText != value)
				{
					this._floatText = value;
					base.OnPropertyChanged(value, "FloatText");
					base.RealText = value.ToString();
					base.Text = value.ToString();
				}
			}
		}

		[Editor(false)]
		public float MaxFloat
		{
			get
			{
				return this._maxFloat;
			}
			set
			{
				if (this._maxFloat != value)
				{
					this._maxFloat = value;
				}
			}
		}

		[Editor(false)]
		public float MinFloat
		{
			get
			{
				return this._minFloat;
			}
			set
			{
				if (this._minFloat != value)
				{
					this._minFloat = value;
				}
			}
		}

		private float _floatText;

		private float _maxFloat = float.MaxValue;

		private float _minFloat = float.MinValue;
	}
}
