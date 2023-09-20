using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TaleWorlds.InputSystem;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	// Token: 0x0200005D RID: 93
	public class IntegerInputTextWidget : EditableTextWidget
	{
		// Token: 0x170001B5 RID: 437
		// (get) Token: 0x060005F2 RID: 1522 RVA: 0x0001A4ED File Offset: 0x000186ED
		// (set) Token: 0x060005F3 RID: 1523 RVA: 0x0001A4F5 File Offset: 0x000186F5
		public bool EnableClamp { get; set; }

		// Token: 0x060005F4 RID: 1524 RVA: 0x0001A4FE File Offset: 0x000186FE
		public IntegerInputTextWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060005F5 RID: 1525 RVA: 0x0001A524 File Offset: 0x00018724
		public override void HandleInput(IReadOnlyList<int> lastKeysPressed)
		{
			int count = lastKeysPressed.Count;
			for (int i = 0; i < count; i++)
			{
				int num = lastKeysPressed[i];
				if (char.IsDigit(Convert.ToChar(num)))
				{
					if (num != 60 && num != 62)
					{
						this.HandleInput(num);
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
					int num2 = (int)this._cursorDirection;
					if (Input.IsKeyDown(InputKey.LeftControl))
					{
						num2 = base.FindNextWordPosition(num2) - this._editableText.CursorPosition;
					}
					base.MoveCursor(num2, this._isSelection);
					if (tickCount >= this._nextRepeatTime)
					{
						this._nextRepeatTime = tickCount + 30;
					}
				}
				else if (flag2)
				{
					int num3 = ((this._cursorDirection == EditableTextWidget.CursorMovementDirection.Left) ? (-this._editableText.CursorPosition) : (this._editableText.VisibleText.Length - this._editableText.CursorPosition));
					base.MoveCursor(num3, this._isSelection);
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
						this.TrySetStringAsInteger(base.RealText);
					}
					else if (Input.IsKeyDown(InputKey.LeftControl))
					{
						int num4 = base.FindNextWordPosition(-1) - this._editableText.CursorPosition;
						base.DeleteText(this._editableText.CursorPosition + num4, this._editableText.CursorPosition);
						this.TrySetStringAsInteger(base.RealText);
					}
					else
					{
						base.DeleteChar(this._keyboardAction == EditableTextWidget.KeyboardAction.Delete);
						this.TrySetStringAsInteger(base.RealText);
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
					this.TrySetStringAsInteger(base.RealText);
					return;
				}
				if (Input.IsKeyPressed(InputKey.V))
				{
					base.DeleteText(this._editableText.SelectedTextBegin, this._editableText.SelectedTextEnd);
					string text = Regex.Replace(Input.GetClipboardText(), "[<>]+", " ");
					text = new string(text.Where((char c) => char.IsDigit(c)).ToArray<char>());
					base.AppendText(text);
					this.TrySetStringAsInteger(base.RealText);
				}
			}
		}

		// Token: 0x060005F6 RID: 1526 RVA: 0x0001A9C4 File Offset: 0x00018BC4
		private void HandleInput(int lastPressedKey)
		{
			string text = null;
			bool flag = false;
			if (base.MaxLength > -1 && base.Text.Length >= base.MaxLength)
			{
				text = base.RealText;
			}
			if (text == null)
			{
				string text2 = base.RealText;
				if (this._editableText.SelectedTextBegin != this._editableText.SelectedTextEnd)
				{
					if (this._editableText.SelectedTextEnd > base.RealText.Length)
					{
						text = Convert.ToChar(lastPressedKey).ToString();
						flag = true;
					}
					else
					{
						text2 = base.RealText.Substring(0, this._editableText.SelectedTextBegin) + base.RealText.Substring(this._editableText.SelectedTextEnd, base.RealText.Length - this._editableText.SelectedTextEnd);
						if (this._editableText.SelectedTextEnd - this._editableText.SelectedTextBegin >= base.RealText.Length)
						{
							this._editableText.SetCursorPosition(0, true);
							this._editableText.ResetSelected();
							flag = true;
						}
						else
						{
							this._editableText.SetCursorPosition(this._editableText.SelectedTextBegin, true);
						}
						int cursorPosition = this._editableText.CursorPosition;
						char c = Convert.ToChar(lastPressedKey);
						text = text2.Substring(0, cursorPosition) + c.ToString() + text2.Substring(cursorPosition, text2.Length - cursorPosition);
					}
					this._editableText.ResetSelected();
				}
				else
				{
					if (this._editableText.CursorPosition == base.RealText.Length)
					{
						flag = true;
					}
					int cursorPosition2 = this._editableText.CursorPosition;
					char c2 = Convert.ToChar(lastPressedKey);
					text = text2.Substring(0, cursorPosition2) + c2.ToString() + text2.Substring(cursorPosition2, text2.Length - cursorPosition2);
					if (!flag)
					{
						this._editableText.SetCursor(cursorPosition2 + 1, true, false);
					}
				}
			}
			this.TrySetStringAsInteger(text);
			if (flag)
			{
				this._editableText.SetCursorPosition(base.RealText.Length, true);
			}
		}

		// Token: 0x060005F7 RID: 1527 RVA: 0x0001ABC4 File Offset: 0x00018DC4
		private void SetInteger(int newInteger)
		{
			if (newInteger != this.IntText)
			{
				if (newInteger <= this.MaxInt && newInteger >= this.MinInt)
				{
					this.IntText = newInteger;
					return;
				}
				if (this.EnableClamp)
				{
					newInteger = ((newInteger > this.MaxInt) ? this.MaxInt : this.MinInt);
					this.IntText = newInteger;
					if (this.IntText.ToString() != base.RealText)
					{
						base.RealText = this.IntText.ToString();
						base.Text = this.IntText.ToString();
					}
					base.ResetSelected();
				}
			}
		}

		// Token: 0x060005F8 RID: 1528 RVA: 0x0001AC68 File Offset: 0x00018E68
		private bool TrySetStringAsInteger(string str)
		{
			int num;
			if (int.TryParse(str, out num))
			{
				this.SetInteger(num);
				if (this._editableText.SelectedTextEnd - this._editableText.SelectedTextBegin >= base.RealText.Length)
				{
					this._editableText.SetCursorPosition(0, true);
					this._editableText.ResetSelected();
				}
				else if (this._editableText.SelectedTextBegin != 0 || this._editableText.SelectedTextEnd != 0)
				{
					this._editableText.SetCursorPosition(this._editableText.SelectedTextBegin, true);
				}
				if (this._editableText.CursorPosition > base.RealText.Length)
				{
					this._editableText.SetCursorPosition(base.RealText.Length, true);
				}
				return true;
			}
			return false;
		}

		// Token: 0x060005F9 RID: 1529 RVA: 0x0001AD2C File Offset: 0x00018F2C
		public override void SetAllText(string text)
		{
			base.DeleteText(0, base.RealText.Length);
			string text2 = Regex.Replace(text, "[<>]+", " ");
			text2 = new string(text2.Where((char c) => char.IsDigit(c)).ToArray<char>());
			this.TrySetStringAsInteger(text2);
		}

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x060005FA RID: 1530 RVA: 0x0001AD94 File Offset: 0x00018F94
		// (set) Token: 0x060005FB RID: 1531 RVA: 0x0001AD9C File Offset: 0x00018F9C
		[Editor(false)]
		public int IntText
		{
			get
			{
				return this._intText;
			}
			set
			{
				if (this._intText != value)
				{
					this._intText = value;
					base.OnPropertyChanged(value, "IntText");
					base.RealText = value.ToString();
					base.Text = value.ToString();
				}
			}
		}

		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x060005FC RID: 1532 RVA: 0x0001ADD4 File Offset: 0x00018FD4
		// (set) Token: 0x060005FD RID: 1533 RVA: 0x0001ADDC File Offset: 0x00018FDC
		[Editor(false)]
		public int MaxInt
		{
			get
			{
				return this._maxInt;
			}
			set
			{
				if (this._maxInt != value)
				{
					this._maxInt = value;
				}
			}
		}

		// Token: 0x170001B8 RID: 440
		// (get) Token: 0x060005FE RID: 1534 RVA: 0x0001ADEE File Offset: 0x00018FEE
		// (set) Token: 0x060005FF RID: 1535 RVA: 0x0001ADF6 File Offset: 0x00018FF6
		[Editor(false)]
		public int MinInt
		{
			get
			{
				return this._minInt;
			}
			set
			{
				if (this._minInt != value)
				{
					this._minInt = value;
				}
			}
		}

		// Token: 0x040002D9 RID: 729
		private int _intText = -1;

		// Token: 0x040002DA RID: 730
		private int _maxInt = int.MaxValue;

		// Token: 0x040002DB RID: 731
		private int _minInt = int.MinValue;
	}
}
