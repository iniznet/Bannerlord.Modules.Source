using System;
using System.Numerics;
using System.Text.RegularExpressions;

namespace TaleWorlds.TwoDimension
{
	public class EditableText : RichText
	{
		public int CursorPosition { get; private set; }

		public bool HighlightStart { get; set; }

		public bool HighlightEnd { get; set; }

		public int SelectedTextBegin { get; private set; }

		public int SelectedTextEnd { get; private set; }

		public float BlinkTimer { get; set; }

		public string VisibleText { get; set; }

		public EditableText(int width, int height, Font font, Func<int, Font> getUsableFontForCharacter)
			: base(width, height, font, getUsableFontForCharacter)
		{
			this._cursorVisible = false;
			this.CursorPosition = 0;
			this._visibleStart = 0;
			this.VisibleText = "";
			this.BlinkTimer = 0f;
			this.HighlightStart = false;
			this.HighlightEnd = true;
			this._selectionAnchor = 0;
			string text = "\\w+";
			this._nextWordRegex = new Regex(text);
		}

		public void SetCursorPosition(int position, bool visible)
		{
			if (this.CursorPosition != position || this._cursorVisible != visible)
			{
				this.CursorPosition = position;
				if (this._visibleStart > this.CursorPosition)
				{
					this._visibleStart = this.CursorPosition;
				}
				this._cursorVisible = visible;
				base.SetAllDirty();
			}
		}

		public void BlinkCursor()
		{
			this._cursorVisible = !this._cursorVisible;
		}

		public bool IsCursorVisible()
		{
			return this._cursorVisible;
		}

		public void ResetSelected()
		{
			this._visibleStart = 0;
			this._selectionAnchor = 0;
			this.SelectedTextBegin = 0;
			this.SelectedTextEnd = 0;
		}

		public void BeginSelection()
		{
			this._selectionAnchor = this.CursorPosition;
		}

		public bool IsAnySelected()
		{
			return this.SelectedTextEnd != this.SelectedTextBegin;
		}

		public Vector2 GetCursorPosition(Font font, float fontSize, float scale)
		{
			float num = fontSize / (float)font.Size;
			float num2 = (float)font.LineHeight * num * scale;
			string text = this.VisibleText.Substring(this._visibleStart, this.CursorPosition - this._visibleStart);
			float num3 = font.GetWordWidth(text, 0.5f) * num * scale;
			float num4 = font.GetWordWidth(this._realVisibleText, 0.5f) * num * scale;
			float num5 = 0f;
			if (base.HorizontalAlignment == TextHorizontalAlignment.Center)
			{
				num5 = ((float)base.Width - num4) * 0.5f;
			}
			else if (base.HorizontalAlignment == TextHorizontalAlignment.Right)
			{
				num5 = (float)base.Width - num4;
			}
			float num6 = 0f;
			if (base.VerticalAlignment == TextVerticalAlignment.Center)
			{
				num6 = ((float)base.Height - num2) * 0.5f;
			}
			else if (base.VerticalAlignment == TextVerticalAlignment.Bottom)
			{
				num6 = (float)base.Height - num2;
			}
			return new Vector2(num5 + num3, num6);
		}

		private void UpdateSelectedText(Vector2 mousePosition)
		{
			string text = this.VisibleText;
			this._visibleStart = Math.Min(this._visibleStart, this.CursorPosition);
			StyleFontContainer.FontData fontData = base.StyleFontContainer.GetFontData("Default");
			float num = fontData.FontSize / (float)fontData.Font.Size;
			int num2 = 10;
			int num3 = 0;
			while (num3 < this._visibleStart && text != "")
			{
				if (fontData.Font.GetWordWidth(text, 0f) * num <= (float)(base.Width - num2 - num2))
				{
					break;
				}
				text = text.Substring(1);
				num3++;
			}
			while (text.Length > this.CursorPosition - this._visibleStart && text != "")
			{
				if (fontData.Font.GetWordWidth(text, 0f) * num <= (float)(base.Width - num2 - num2))
				{
					break;
				}
				text = text.Substring(0, text.Length - 1);
			}
			while (text != "" && fontData.Font.GetWordWidth(text, 0f) * num > (float)(base.Width - num2 - num2))
			{
				text = text.Substring(1);
				num3++;
				this._visibleStart = Math.Min(this._visibleStart + 1, this.CursorPosition);
			}
			Vector2 vector = mousePosition;
			if (base.TextOutput != null && base.HorizontalAlignment != TextHorizontalAlignment.Left)
			{
				if (base.HorizontalAlignment == TextHorizontalAlignment.Center)
				{
					vector.X -= ((float)base.Width - base.TextOutput.GetLine(0).Width) * 0.5f;
				}
				else if (base.HorizontalAlignment == TextHorizontalAlignment.Right)
				{
					vector.X -= (float)base.Width - base.TextOutput.GetLine(0).Width;
				}
			}
			if (this.HighlightStart)
			{
				this.SelectedTextBegin = this.FindCharacterPosition(this.VisibleText, text, num, vector, num3);
				this.HighlightStart = false;
				this.SetCursor(this.SelectedTextBegin, true, false);
			}
			if (!this.HighlightEnd)
			{
				this.SelectedTextEnd = this.FindCharacterPosition(this.VisibleText, text, num, vector, num3);
				this.SetCursor(this.SelectedTextEnd, true, false);
			}
			else if (this.SelectedTextBegin > this.SelectedTextEnd)
			{
				int selectedTextBegin = this.SelectedTextBegin;
				this.SelectedTextBegin = this.SelectedTextEnd;
				this.SelectedTextEnd = selectedTextBegin;
			}
			int num4 = Math.Min(Math.Max(this.SelectedTextBegin - num3, 0), text.Length);
			int num5 = Math.Min(Math.Max(this.SelectedTextEnd - num3, 0), text.Length);
			if (num4 > num5)
			{
				int num6 = num4;
				num4 = num5;
				num5 = num6;
			}
			if (num4 > num5)
			{
				int num7 = num4;
				num4 = num5;
				num5 = num7;
			}
			string text2 = string.Concat(new string[]
			{
				text.Substring(0, num4),
				"<span style=\"Highlight\">",
				text.Substring(num4, num5 - num4),
				"</span>",
				text.Substring(num5, text.Length - num5)
			});
			this._realVisibleText = text.Substring(0, num4) + text.Substring(num4, num5 - num4) + text.Substring(num5, text.Length - num5);
			base.Value = text2;
		}

		public override void Update(SpriteData spriteData, Vector2 focusPosition, bool focus, bool isFixedWidth, bool isFixedHeight, float renderScale)
		{
			base.Update(spriteData, focusPosition, focus, isFixedWidth, isFixedHeight, renderScale);
			this.UpdateSelectedText(focusPosition);
		}

		public void SelectAll()
		{
			this.SelectedTextBegin = 0;
			this.SetCursor(this.VisibleText.Length, true, true);
		}

		public int FindNextWordPosition(int direction)
		{
			MatchCollection matchCollection = this._nextWordRegex.Matches(this.VisibleText);
			int num = 0;
			int num2 = this.VisibleText.Length;
			foreach (object obj in matchCollection)
			{
				int index = ((Match)obj).Index;
				if (index < this.CursorPosition)
				{
					num = index;
				}
				else if (index > this.CursorPosition)
				{
					num2 = index;
					break;
				}
			}
			if (direction <= 0)
			{
				return num;
			}
			return num2;
		}

		public void SetCursor(int position, bool visible = true, bool withSelection = false)
		{
			this.BlinkTimer = 0f;
			int num = Mathf.Clamp(position, 0, this.VisibleText.Length);
			this.SetCursorPosition(num, visible);
			if (withSelection)
			{
				this.SelectedTextBegin = Math.Min(num, this._selectionAnchor);
				this.SelectedTextEnd = Math.Max(num, this._selectionAnchor);
			}
		}

		private int FindCharacterPosition(string fullText, string text, float scale, Vector2 mousePosition, int omitCount)
		{
			int length = text.Length;
			int i = 0;
			if (mousePosition.X > (float)base.Width + 15f * scale)
			{
				return Math.Min(omitCount + length + 1, fullText.Length);
			}
			if (mousePosition.X < -15f * scale)
			{
				return Math.Max(omitCount - 1, 0);
			}
			StyleFontContainer.FontData fontData = base.StyleFontContainer.GetFontData("Default");
			while (i < length)
			{
				int num = i + omitCount;
				float num2 = fontData.Font.GetWordWidth(text.Substring(0, i + 1), 0f) * scale;
				if (num2 > mousePosition.X)
				{
					return num;
				}
				i++;
			}
			return i + omitCount;
		}

		private bool _cursorVisible;

		private int _visibleStart;

		private int _selectionAnchor;

		private string _realVisibleText;

		private Regex _nextWordRegex;
	}
}
