using System;
using System.Numerics;
using System.Text.RegularExpressions;

namespace TaleWorlds.TwoDimension
{
	// Token: 0x02000005 RID: 5
	public class EditableText : RichText
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000003 RID: 3 RVA: 0x00002058 File Offset: 0x00000258
		// (set) Token: 0x06000004 RID: 4 RVA: 0x00002060 File Offset: 0x00000260
		public int CursorPosition { get; private set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000005 RID: 5 RVA: 0x00002069 File Offset: 0x00000269
		// (set) Token: 0x06000006 RID: 6 RVA: 0x00002071 File Offset: 0x00000271
		public bool HighlightStart { get; set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000007 RID: 7 RVA: 0x0000207A File Offset: 0x0000027A
		// (set) Token: 0x06000008 RID: 8 RVA: 0x00002082 File Offset: 0x00000282
		public bool HighlightEnd { get; set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000009 RID: 9 RVA: 0x0000208B File Offset: 0x0000028B
		// (set) Token: 0x0600000A RID: 10 RVA: 0x00002093 File Offset: 0x00000293
		public int SelectedTextBegin { get; private set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000B RID: 11 RVA: 0x0000209C File Offset: 0x0000029C
		// (set) Token: 0x0600000C RID: 12 RVA: 0x000020A4 File Offset: 0x000002A4
		public int SelectedTextEnd { get; private set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600000D RID: 13 RVA: 0x000020AD File Offset: 0x000002AD
		// (set) Token: 0x0600000E RID: 14 RVA: 0x000020B5 File Offset: 0x000002B5
		public float BlinkTimer { get; set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600000F RID: 15 RVA: 0x000020BE File Offset: 0x000002BE
		// (set) Token: 0x06000010 RID: 16 RVA: 0x000020C6 File Offset: 0x000002C6
		public string VisibleText { get; set; }

		// Token: 0x06000011 RID: 17 RVA: 0x000020D0 File Offset: 0x000002D0
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

		// Token: 0x06000012 RID: 18 RVA: 0x0000213C File Offset: 0x0000033C
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

		// Token: 0x06000013 RID: 19 RVA: 0x00002189 File Offset: 0x00000389
		public void BlinkCursor()
		{
			this._cursorVisible = !this._cursorVisible;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x0000219A File Offset: 0x0000039A
		public bool IsCursorVisible()
		{
			return this._cursorVisible;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x000021A2 File Offset: 0x000003A2
		public void ResetSelected()
		{
			this._visibleStart = 0;
			this._selectionAnchor = 0;
			this.SelectedTextBegin = 0;
			this.SelectedTextEnd = 0;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000021C0 File Offset: 0x000003C0
		public void BeginSelection()
		{
			this._selectionAnchor = this.CursorPosition;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000021CE File Offset: 0x000003CE
		public bool IsAnySelected()
		{
			return this.SelectedTextEnd != this.SelectedTextBegin;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000021E4 File Offset: 0x000003E4
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

		// Token: 0x06000019 RID: 25 RVA: 0x000022CC File Offset: 0x000004CC
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

		// Token: 0x0600001A RID: 26 RVA: 0x000025F2 File Offset: 0x000007F2
		public override void Update(SpriteData spriteData, Vector2 focusPosition, bool focus, bool isFixedWidth, bool isFixedHeight, float renderScale)
		{
			base.Update(spriteData, focusPosition, focus, isFixedWidth, isFixedHeight, renderScale);
			this.UpdateSelectedText(focusPosition);
		}

		// Token: 0x0600001B RID: 27 RVA: 0x0000260A File Offset: 0x0000080A
		public void SelectAll()
		{
			this.SelectedTextBegin = 0;
			this.SetCursor(this.VisibleText.Length, true, true);
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002628 File Offset: 0x00000828
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

		// Token: 0x0600001D RID: 29 RVA: 0x000026BC File Offset: 0x000008BC
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

		// Token: 0x0600001E RID: 30 RVA: 0x00002718 File Offset: 0x00000918
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

		// Token: 0x04000009 RID: 9
		private bool _cursorVisible;

		// Token: 0x0400000B RID: 11
		private int _visibleStart;

		// Token: 0x04000010 RID: 16
		private int _selectionAnchor;

		// Token: 0x04000013 RID: 19
		private string _realVisibleText;

		// Token: 0x04000014 RID: 20
		private Regex _nextWordRegex;
	}
}
