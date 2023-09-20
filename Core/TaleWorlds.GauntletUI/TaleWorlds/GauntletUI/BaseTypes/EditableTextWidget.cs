using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;
using TaleWorlds.GauntletUI.Layout;
using TaleWorlds.InputSystem;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	public class EditableTextWidget : BrushWidget
	{
		public int MaxLength { get; set; } = -1;

		public bool IsObfuscationEnabled
		{
			get
			{
				return this._isObfuscationEnabled;
			}
			set
			{
				if (value != this._isObfuscationEnabled)
				{
					this._isObfuscationEnabled = value;
					this.OnObfuscationToggled(value);
				}
			}
		}

		private Vector2 LocalMousePosition
		{
			get
			{
				Vector2 mousePosition = base.EventManager.MousePosition;
				Vector2 globalPosition = base.GlobalPosition;
				float num = mousePosition.X - globalPosition.X;
				float num2 = mousePosition.Y - globalPosition.Y;
				return new Vector2(num, num2);
			}
		}

		public string DefaultSearchText { get; set; }

		[Editor(false)]
		public string RealText
		{
			get
			{
				return this._realText;
			}
			set
			{
				if (this._realText != value)
				{
					this._editableText.CurrentLanguage = base.Context.FontFactory.GetCurrentLanguage();
					if (string.IsNullOrEmpty(value))
					{
						value = "";
					}
					this.Text = (this.IsObfuscationEnabled ? this.ObfuscateText(value) : value);
					this._realText = value;
					base.OnPropertyChanged<string>(value, "RealText");
				}
			}
		}

		[Editor(false)]
		public string KeyboardInfoText
		{
			get
			{
				return this._keyboardInfoText;
			}
			set
			{
				if (this._keyboardInfoText != value)
				{
					this._keyboardInfoText = value;
					base.OnPropertyChanged<string>(value, "KeyboardInfoText");
				}
			}
		}

		[Editor(false)]
		public string Text
		{
			get
			{
				return this._editableText.VisibleText;
			}
			set
			{
				if (this._editableText.VisibleText != value)
				{
					this._editableText.CurrentLanguage = base.Context.FontFactory.GetCurrentLanguage();
					this._editableText.VisibleText = value;
					if (!string.IsNullOrEmpty(this._editableText.VisibleText))
					{
						this._editableText.SetCursor(this._editableText.VisibleText.Length, true, false);
					}
					if (string.IsNullOrEmpty(value))
					{
						this._editableText.VisibleText = "";
						this._editableText.SetCursor(0, base.IsFocused, false);
					}
					this.RealText = value;
					base.OnPropertyChanged<string>(value, "Text");
					base.SetMeasureAndLayoutDirty();
				}
			}
		}

		public EditableTextWidget(UIContext context)
			: base(context)
		{
			FontFactory fontFactory = context.FontFactory;
			this._editableText = new EditableText((int)base.Size.X, (int)base.Size.Y, fontFactory.DefaultFont, new Func<int, Font>(fontFactory.GetUsableFontForCharacter));
			base.LayoutImp = new TextLayout(this._editableText);
			this._realText = "";
			this._textHeight = -1;
			this._cursorVisible = false;
			this._lastFontBrush = null;
			this._cursorDirection = EditableTextWidget.CursorMovementDirection.None;
			this._keyboardAction = EditableTextWidget.KeyboardAction.None;
			this._nextRepeatTime = int.MinValue;
			this._isSelection = false;
			base.IsFocusable = true;
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			this.UpdateText();
			if (base.IsFocused && base.IsEnabled)
			{
				this._editableText.BlinkTimer += dt;
				if (this._editableText.BlinkTimer > 0.5f)
				{
					this._editableText.BlinkCursor();
					this._editableText.BlinkTimer = 0f;
				}
				if (base.ContainsState("Selected"))
				{
					this.SetState("Selected");
				}
			}
			else if (this._editableText.IsCursorVisible())
			{
				this._editableText.BlinkCursor();
			}
			this.SetEditTextParameters();
		}

		private void SetEditTextParameters()
		{
			bool flag = false;
			this._editableText.CurrentLanguage = base.Context.FontFactory.GetCurrentLanguage();
			this.UpdateFontData();
			if (this._editableText.HorizontalAlignment != base.ReadOnlyBrush.TextHorizontalAlignment)
			{
				this._editableText.HorizontalAlignment = base.ReadOnlyBrush.TextHorizontalAlignment;
				flag = true;
			}
			if (this._editableText.VerticalAlignment != base.ReadOnlyBrush.TextVerticalAlignment)
			{
				this._editableText.VerticalAlignment = base.ReadOnlyBrush.TextVerticalAlignment;
				flag = true;
			}
			if (this._editableText.TextHeight != this._textHeight)
			{
				this._textHeight = this._editableText.TextHeight;
				flag = true;
			}
			if (flag)
			{
				base.SetMeasureAndLayoutDirty();
			}
		}

		protected void BlinkCursor()
		{
			this._cursorVisible = !this._cursorVisible;
		}

		protected void ResetSelected()
		{
			this._editableText.ResetSelected();
		}

		protected void DeleteChar(bool nextChar = false)
		{
			int num = this._editableText.CursorPosition;
			if (nextChar)
			{
				num++;
			}
			if (num == 0 || num > this.Text.Length)
			{
				return;
			}
			if (this.IsObfuscationEnabled)
			{
				this.RealText = this.RealText.Substring(0, num - 1) + this.RealText.Substring(num, this.RealText.Length - num);
				this.Text = this.ObfuscateText(this.RealText);
			}
			else
			{
				this.Text = this.Text.Substring(0, num - 1) + this.Text.Substring(num, this.Text.Length - num);
				this.RealText = this.Text;
			}
			this._editableText.SetCursor(num - 1, true, false);
			this.ResetSelected();
		}

		protected int FindNextWordPosition(int direction)
		{
			return this._editableText.FindNextWordPosition(direction);
		}

		protected void MoveCursor(int direction, bool withSelection = false)
		{
			if (!withSelection)
			{
				this.ResetSelected();
			}
			this._editableText.SetCursor(this._editableText.CursorPosition + direction, true, withSelection);
		}

		protected string GetAppendCharacterResult(int charCode)
		{
			if (this.MaxLength > -1 && this.Text.Length >= this.MaxLength)
			{
				return this.RealText;
			}
			int cursorPosition = this._editableText.CursorPosition;
			char c = Convert.ToChar(charCode);
			return this.RealText.Substring(0, cursorPosition) + c.ToString() + this.RealText.Substring(cursorPosition, this.RealText.Length - cursorPosition);
		}

		protected void AppendCharacter(int charCode)
		{
			if (this.MaxLength > -1 && this.Text.Length >= this.MaxLength)
			{
				return;
			}
			int cursorPosition = this._editableText.CursorPosition;
			this.RealText = this.GetAppendCharacterResult(charCode);
			this.Text = (this.IsObfuscationEnabled ? this.ObfuscateText(this.RealText) : this.RealText);
			this._editableText.SetCursor(cursorPosition + 1, true, false);
			this.ResetSelected();
		}

		protected void AppendText(string text)
		{
			if (this.MaxLength > -1 && this.Text.Length >= this.MaxLength)
			{
				return;
			}
			if (this.MaxLength > -1 && this.Text.Length + text.Length >= this.MaxLength)
			{
				text = text.Substring(0, this.MaxLength - this.Text.Length);
			}
			int cursorPosition = this._editableText.CursorPosition;
			this.RealText = this.RealText.Substring(0, cursorPosition) + text + this.RealText.Substring(cursorPosition, this.RealText.Length - cursorPosition);
			this.Text = (this.IsObfuscationEnabled ? this.ObfuscateText(this.RealText) : this.RealText);
			this._editableText.SetCursor(cursorPosition + text.Length, true, false);
			this.ResetSelected();
		}

		protected void DeleteText(int beginIndex, int endIndex)
		{
			if (beginIndex == endIndex)
			{
				return;
			}
			this.RealText = this.RealText.Substring(0, beginIndex) + this.RealText.Substring(endIndex, this.RealText.Length - endIndex);
			this.Text = (this.IsObfuscationEnabled ? this.ObfuscateText(this.RealText) : this.RealText);
			this._editableText.SetCursor(beginIndex, true, false);
			this.ResetSelected();
		}

		protected void CopyText(int beginIndex, int endIndex)
		{
			if (beginIndex == endIndex)
			{
				return;
			}
			int num = Math.Min(beginIndex, endIndex);
			int num2 = Math.Max(beginIndex, endIndex);
			if (num < 0)
			{
				num = 0;
			}
			if (num2 > this.RealText.Length)
			{
				num2 = this.RealText.Length;
			}
			Input.SetClipboardText(this.RealText.Substring(num, num2 - num));
		}

		protected void PasteText()
		{
			string text = Regex.Replace(Input.GetClipboardText(), "[<>]+", " ");
			this.AppendText(text);
		}

		public override void HandleInput(IReadOnlyList<int> lastKeysPressed)
		{
			if (base.IsDisabled)
			{
				return;
			}
			int count = lastKeysPressed.Count;
			for (int i = 0; i < count; i++)
			{
				int num = lastKeysPressed[i];
				if (num >= 32 && (num < 127 || num >= 160))
				{
					if (num != 60 && num != 62)
					{
						this.DeleteText(this._editableText.SelectedTextBegin, this._editableText.SelectedTextEnd);
						this.AppendCharacter(num);
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
				if (flag)
				{
					this._nextRepeatTime = tickCount + 500;
				}
				if (Input.IsKeyDown(InputKey.LeftShift))
				{
					if (!this._editableText.IsAnySelected())
					{
						this._editableText.BeginSelection();
					}
					this._isSelection = true;
				}
			}
			if (this._cursorDirection != EditableTextWidget.CursorMovementDirection.None)
			{
				if (flag || tickCount >= this._nextRepeatTime)
				{
					int num2 = (int)this._cursorDirection;
					if (Input.IsKeyDown(InputKey.LeftControl))
					{
						num2 = this.FindNextWordPosition(num2) - this._editableText.CursorPosition;
					}
					this.MoveCursor(num2, this._isSelection);
					if (tickCount >= this._nextRepeatTime)
					{
						this._nextRepeatTime = tickCount + 30;
					}
				}
				else if (flag2)
				{
					int num3 = ((this._cursorDirection == EditableTextWidget.CursorMovementDirection.Left) ? (-this._editableText.CursorPosition) : (this._editableText.VisibleText.Length - this._editableText.CursorPosition));
					this.MoveCursor(num3, this._isSelection);
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
						this.DeleteText(this._editableText.SelectedTextBegin, this._editableText.SelectedTextEnd);
					}
					else if (Input.IsKeyDown(InputKey.LeftControl))
					{
						int num4 = this.FindNextWordPosition(-1) - this._editableText.CursorPosition;
						this.DeleteText(this._editableText.CursorPosition + num4, this._editableText.CursorPosition);
					}
					else
					{
						this.DeleteChar(this._keyboardAction == EditableTextWidget.KeyboardAction.Delete);
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
					this.CopyText(this._editableText.SelectedTextBegin, this._editableText.SelectedTextEnd);
					return;
				}
				if (Input.IsKeyPressed(InputKey.X))
				{
					this.CopyText(this._editableText.SelectedTextBegin, this._editableText.SelectedTextEnd);
					this.DeleteText(this._editableText.SelectedTextBegin, this._editableText.SelectedTextEnd);
					return;
				}
				if (Input.IsKeyPressed(InputKey.V))
				{
					this.DeleteText(this._editableText.SelectedTextBegin, this._editableText.SelectedTextEnd);
					this.PasteText();
				}
			}
		}

		protected internal override void OnGainFocus()
		{
			base.OnGainFocus();
			this._editableText.SetCursor(this.RealText.Length, true, false);
			if (string.IsNullOrEmpty(this.RealText) && !string.IsNullOrEmpty(this.DefaultSearchText))
			{
				this._editableText.VisibleText = "";
			}
		}

		protected internal override void OnLoseFocus()
		{
			base.OnLoseFocus();
			this._editableText.ResetSelected();
			this._isSelection = false;
			this._editableText.SetCursor(0, false, false);
			if (string.IsNullOrEmpty(this.RealText) && !string.IsNullOrEmpty(this.DefaultSearchText))
			{
				this._editableText.VisibleText = this.DefaultSearchText;
			}
		}

		private void UpdateText()
		{
			if (base.IsDisabled)
			{
				this.SetState("Disabled");
			}
			else if (base.IsPressed)
			{
				this.SetState("Pressed");
			}
			else if (base.IsHovered)
			{
				this.SetState("Hovered");
			}
			else
			{
				this.SetState("Default");
			}
			if (string.IsNullOrEmpty(this.Text) && !string.IsNullOrEmpty(this.DefaultSearchText) && this._mouseState == EditableTextWidget.MouseState.None && base.EventManager.FocusedWidget != this)
			{
				this._editableText.VisibleText = this.DefaultSearchText;
			}
		}

		private void UpdateFontData()
		{
			if (this._lastFontBrush == base.ReadOnlyBrush && this._lastScale == base._scaleToUse && this._lastLanguageCode == base.Context.FontFactory.CurrentLangageID)
			{
				return;
			}
			this._editableText.StyleFontContainer.ClearFonts();
			foreach (Style style in base.ReadOnlyBrush.Styles)
			{
				Font font;
				if (style.Font != null)
				{
					font = style.Font;
				}
				else if (base.ReadOnlyBrush.Font != null)
				{
					font = base.ReadOnlyBrush.Font;
				}
				else
				{
					font = base.Context.FontFactory.DefaultFont;
				}
				Font mappedFontForLocalization = base.Context.FontFactory.GetMappedFontForLocalization(font.Name);
				this._editableText.StyleFontContainer.Add(style.Name, mappedFontForLocalization, (float)base.ReadOnlyBrush.FontSize * base._scaleToUse);
			}
			this._lastFontBrush = base.ReadOnlyBrush;
			this._lastScale = base._scaleToUse;
			this._lastLanguageCode = base.Context.FontFactory.CurrentLangageID;
			this._editableText.CurrentLanguage = base.Context.FontFactory.GetCurrentLanguage();
		}

		private Font GetFont(Style style = null)
		{
			if (((style != null) ? style.Font : null) != null)
			{
				return base.Context.FontFactory.GetMappedFontForLocalization(style.Font.Name);
			}
			if (base.ReadOnlyBrush.Font != null)
			{
				return base.Context.FontFactory.GetMappedFontForLocalization(base.ReadOnlyBrush.Font.Name);
			}
			return base.Context.FontFactory.DefaultFont;
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (base.Size.X > 0f && base.Size.Y > 0f)
			{
				Vector2 localMousePosition = this.LocalMousePosition;
				bool flag = this._mouseState == EditableTextWidget.MouseState.Down;
				this._editableText.UpdateSize((int)base.Size.X, (int)base.Size.Y);
				this.SetEditTextParameters();
				this.UpdateFontData();
				bool flag2 = base.WidthSizePolicy != SizePolicy.CoverChildren || base.MaxWidth != 0f;
				bool flag3 = base.HeightSizePolicy != SizePolicy.CoverChildren || base.MaxHeight != 0f;
				this._editableText.Update(base.Context.SpriteData, localMousePosition, flag, flag2, flag3, base._scaleToUse);
			}
		}

		protected override void OnRender(TwoDimensionContext twoDimensionContext, TwoDimensionDrawContext drawContext)
		{
			base.OnRender(twoDimensionContext, drawContext);
			if (!string.IsNullOrEmpty(this._editableText.Value))
			{
				Vector2 globalPosition = base.GlobalPosition;
				foreach (RichTextPart richTextPart in this._editableText.GetParts())
				{
					DrawObject2D drawObject2D = richTextPart.DrawObject2D;
					Style styleOrDefault = base.ReadOnlyBrush.GetStyleOrDefault(richTextPart.Style);
					Font font = this.GetFont(styleOrDefault);
					int fontSize = styleOrDefault.FontSize;
					float num = (float)fontSize * base._scaleToUse;
					float num2 = (float)fontSize / (float)font.Size;
					float num3 = (float)font.LineHeight * num2 * base._scaleToUse;
					TextMaterial textMaterial = styleOrDefault.CreateTextMaterial(drawContext);
					textMaterial.ColorFactor *= base.ReadOnlyBrush.GlobalColorFactor;
					textMaterial.AlphaFactor *= base.ReadOnlyBrush.GlobalAlphaFactor;
					textMaterial.Color *= base.ReadOnlyBrush.GlobalColor;
					textMaterial.Texture = font.FontSprite.Texture;
					textMaterial.ScaleFactor = num;
					textMaterial.Smooth = font.Smooth;
					textMaterial.SmoothingConstant = font.SmoothingConstant;
					if (textMaterial.GlowRadius > 0f || textMaterial.Blur > 0f || textMaterial.OutlineAmount > 0f)
					{
						TextMaterial textMaterial2 = styleOrDefault.CreateTextMaterial(drawContext);
						textMaterial2.CopyFrom(textMaterial);
						drawContext.Draw(globalPosition.X, globalPosition.Y, textMaterial2, drawObject2D, base.Size.X, base.Size.Y);
					}
					textMaterial.GlowRadius = 0f;
					textMaterial.Blur = 0f;
					textMaterial.OutlineAmount = 0f;
					Material material = textMaterial;
					if (richTextPart.Style == "Highlight")
					{
						SpriteData spriteData = base.Context.SpriteData;
						string text = "warm_overlay";
						Sprite sprite = spriteData.GetSprite(text);
						SimpleMaterial simpleMaterial = drawContext.CreateSimpleMaterial();
						simpleMaterial.Reset((sprite != null) ? sprite.Texture : null);
						drawContext.DrawSprite(sprite, simpleMaterial, globalPosition.X + richTextPart.PartPosition.X, globalPosition.Y + richTextPart.PartPosition.Y, 1f, richTextPart.WordWidth, num3, false, false);
					}
					drawContext.Draw(globalPosition.X, globalPosition.Y, material, drawObject2D, base.Size.X, base.Size.Y);
				}
				if (this._editableText.IsCursorVisible())
				{
					Style styleOrDefault2 = base.ReadOnlyBrush.GetStyleOrDefault("Default");
					Font font2 = this.GetFont(styleOrDefault2);
					int fontSize2 = styleOrDefault2.FontSize;
					float num4 = (float)fontSize2 / (float)font2.Size;
					float num5 = (float)font2.LineHeight * num4 * base._scaleToUse;
					SpriteData spriteData2 = base.Context.SpriteData;
					string text2 = "BlankWhiteSquare_9";
					Sprite sprite2 = spriteData2.GetSprite(text2);
					SimpleMaterial simpleMaterial2 = drawContext.CreateSimpleMaterial();
					simpleMaterial2.Reset((sprite2 != null) ? sprite2.Texture : null);
					Vector2 cursorPosition = this._editableText.GetCursorPosition(font2, (float)fontSize2, base._scaleToUse);
					drawContext.DrawSprite(sprite2, simpleMaterial2, (float)((int)(globalPosition.X + cursorPosition.X)), globalPosition.Y + cursorPosition.Y, 1f, 1f, num5, false, false);
				}
			}
		}

		protected internal override void OnMousePressed()
		{
			base.OnMousePressed();
			this._mouseDownPosition = this.LocalMousePosition;
			this._mouseState = EditableTextWidget.MouseState.Down;
			this._editableText.HighlightStart = true;
			this._editableText.HighlightEnd = false;
		}

		protected internal override void OnMouseReleased()
		{
			base.OnMouseReleased();
			this._mouseState = EditableTextWidget.MouseState.Up;
			this._editableText.HighlightEnd = true;
		}

		private void OnObfuscationToggled(bool isEnabled)
		{
			if (isEnabled)
			{
				this.Text = this.ObfuscateText(this.RealText);
				return;
			}
			this.Text = this.RealText;
		}

		private string ObfuscateText(string stringToObfuscate)
		{
			return new string(this._obfuscationChar, stringToObfuscate.Length);
		}

		public virtual void SetAllText(string text)
		{
			this.DeleteText(0, this.RealText.Length);
			string text2 = Regex.Replace(text, "[<>]+", " ");
			this.AppendText(text2);
		}

		protected EditableText _editableText;

		protected readonly char _obfuscationChar = '*';

		protected float _lastScale = -1f;

		protected bool _isObfuscationEnabled;

		protected string _lastLanguageCode;

		protected Brush _lastFontBrush;

		protected EditableTextWidget.MouseState _mouseState;

		protected Vector2 _mouseDownPosition;

		protected bool _cursorVisible;

		protected int _textHeight;

		protected EditableTextWidget.CursorMovementDirection _cursorDirection;

		protected EditableTextWidget.KeyboardAction _keyboardAction;

		protected int _nextRepeatTime;

		protected bool _isSelection;

		private string _realText = "";

		private string _keyboardInfoText = "";

		protected enum MouseState
		{
			None,
			Down,
			Up
		}

		protected enum CursorMovementDirection
		{
			None,
			Left = -1,
			Right = 1
		}

		protected enum KeyboardAction
		{
			None,
			BackSpace,
			Delete
		}
	}
}
