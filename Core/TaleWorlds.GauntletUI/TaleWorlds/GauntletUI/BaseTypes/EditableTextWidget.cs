using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;
using TaleWorlds.GauntletUI.Layout;
using TaleWorlds.InputSystem;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	// Token: 0x02000059 RID: 89
	public class EditableTextWidget : BrushWidget
	{
		// Token: 0x170001A1 RID: 417
		// (get) Token: 0x060005A7 RID: 1447 RVA: 0x00018710 File Offset: 0x00016910
		// (set) Token: 0x060005A8 RID: 1448 RVA: 0x00018718 File Offset: 0x00016918
		public int MaxLength { get; set; } = -1;

		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x060005A9 RID: 1449 RVA: 0x00018721 File Offset: 0x00016921
		// (set) Token: 0x060005AA RID: 1450 RVA: 0x00018729 File Offset: 0x00016929
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

		// Token: 0x170001A3 RID: 419
		// (get) Token: 0x060005AB RID: 1451 RVA: 0x00018744 File Offset: 0x00016944
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

		// Token: 0x170001A4 RID: 420
		// (get) Token: 0x060005AC RID: 1452 RVA: 0x00018785 File Offset: 0x00016985
		// (set) Token: 0x060005AD RID: 1453 RVA: 0x0001878D File Offset: 0x0001698D
		public string DefaultSearchText { get; set; }

		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x060005AE RID: 1454 RVA: 0x00018796 File Offset: 0x00016996
		// (set) Token: 0x060005AF RID: 1455 RVA: 0x000187A0 File Offset: 0x000169A0
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

		// Token: 0x170001A6 RID: 422
		// (get) Token: 0x060005B0 RID: 1456 RVA: 0x00018810 File Offset: 0x00016A10
		// (set) Token: 0x060005B1 RID: 1457 RVA: 0x00018818 File Offset: 0x00016A18
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

		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x060005B2 RID: 1458 RVA: 0x0001883B File Offset: 0x00016A3B
		// (set) Token: 0x060005B3 RID: 1459 RVA: 0x00018848 File Offset: 0x00016A48
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

		// Token: 0x060005B4 RID: 1460 RVA: 0x00018908 File Offset: 0x00016B08
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

		// Token: 0x060005B5 RID: 1461 RVA: 0x000189E0 File Offset: 0x00016BE0
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

		// Token: 0x060005B6 RID: 1462 RVA: 0x00018A84 File Offset: 0x00016C84
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

		// Token: 0x060005B7 RID: 1463 RVA: 0x00018B43 File Offset: 0x00016D43
		protected void BlinkCursor()
		{
			this._cursorVisible = !this._cursorVisible;
		}

		// Token: 0x060005B8 RID: 1464 RVA: 0x00018B54 File Offset: 0x00016D54
		protected void ResetSelected()
		{
			this._editableText.ResetSelected();
		}

		// Token: 0x060005B9 RID: 1465 RVA: 0x00018B64 File Offset: 0x00016D64
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

		// Token: 0x060005BA RID: 1466 RVA: 0x00018C3A File Offset: 0x00016E3A
		protected int FindNextWordPosition(int direction)
		{
			return this._editableText.FindNextWordPosition(direction);
		}

		// Token: 0x060005BB RID: 1467 RVA: 0x00018C48 File Offset: 0x00016E48
		protected void MoveCursor(int direction, bool withSelection = false)
		{
			if (!withSelection)
			{
				this.ResetSelected();
			}
			this._editableText.SetCursor(this._editableText.CursorPosition + direction, true, withSelection);
		}

		// Token: 0x060005BC RID: 1468 RVA: 0x00018C70 File Offset: 0x00016E70
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

		// Token: 0x060005BD RID: 1469 RVA: 0x00018CE8 File Offset: 0x00016EE8
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

		// Token: 0x060005BE RID: 1470 RVA: 0x00018D64 File Offset: 0x00016F64
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

		// Token: 0x060005BF RID: 1471 RVA: 0x00018E48 File Offset: 0x00017048
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

		// Token: 0x060005C0 RID: 1472 RVA: 0x00018EC4 File Offset: 0x000170C4
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

		// Token: 0x060005C1 RID: 1473 RVA: 0x00018F1C File Offset: 0x0001711C
		protected void PasteText()
		{
			string text = Regex.Replace(Input.GetClipboardText(), "[<>]+", " ");
			this.AppendText(text);
		}

		// Token: 0x060005C2 RID: 1474 RVA: 0x00018F48 File Offset: 0x00017148
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

		// Token: 0x060005C3 RID: 1475 RVA: 0x00019378 File Offset: 0x00017578
		protected internal override void OnGainFocus()
		{
			base.OnGainFocus();
			this._editableText.SetCursor(this.RealText.Length, true, false);
			if (string.IsNullOrEmpty(this.RealText) && !string.IsNullOrEmpty(this.DefaultSearchText))
			{
				this._editableText.VisibleText = "";
			}
		}

		// Token: 0x060005C4 RID: 1476 RVA: 0x000193D0 File Offset: 0x000175D0
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

		// Token: 0x060005C5 RID: 1477 RVA: 0x00019430 File Offset: 0x00017630
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

		// Token: 0x060005C6 RID: 1478 RVA: 0x000194C8 File Offset: 0x000176C8
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

		// Token: 0x060005C7 RID: 1479 RVA: 0x00019634 File Offset: 0x00017834
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

		// Token: 0x060005C8 RID: 1480 RVA: 0x000196AC File Offset: 0x000178AC
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

		// Token: 0x060005C9 RID: 1481 RVA: 0x00019784 File Offset: 0x00017984
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

		// Token: 0x060005CA RID: 1482 RVA: 0x00019B14 File Offset: 0x00017D14
		protected internal override void OnMousePressed()
		{
			base.OnMousePressed();
			this._mouseDownPosition = this.LocalMousePosition;
			this._mouseState = EditableTextWidget.MouseState.Down;
			this._editableText.HighlightStart = true;
			this._editableText.HighlightEnd = false;
		}

		// Token: 0x060005CB RID: 1483 RVA: 0x00019B47 File Offset: 0x00017D47
		protected internal override void OnMouseReleased()
		{
			base.OnMouseReleased();
			this._mouseState = EditableTextWidget.MouseState.Up;
			this._editableText.HighlightEnd = true;
		}

		// Token: 0x060005CC RID: 1484 RVA: 0x00019B62 File Offset: 0x00017D62
		private void OnObfuscationToggled(bool isEnabled)
		{
			if (isEnabled)
			{
				this.Text = this.ObfuscateText(this.RealText);
				return;
			}
			this.Text = this.RealText;
		}

		// Token: 0x060005CD RID: 1485 RVA: 0x00019B86 File Offset: 0x00017D86
		private string ObfuscateText(string stringToObfuscate)
		{
			return new string(this._obfuscationChar, stringToObfuscate.Length);
		}

		// Token: 0x060005CE RID: 1486 RVA: 0x00019B9C File Offset: 0x00017D9C
		public virtual void SetAllText(string text)
		{
			this.DeleteText(0, this.RealText.Length);
			string text2 = Regex.Replace(text, "[<>]+", " ");
			this.AppendText(text2);
		}

		// Token: 0x040002BD RID: 701
		protected EditableText _editableText;

		// Token: 0x040002BE RID: 702
		protected readonly char _obfuscationChar = '*';

		// Token: 0x040002BF RID: 703
		protected float _lastScale = -1f;

		// Token: 0x040002C0 RID: 704
		protected bool _isObfuscationEnabled;

		// Token: 0x040002C1 RID: 705
		protected string _lastLanguageCode;

		// Token: 0x040002C2 RID: 706
		protected Brush _lastFontBrush;

		// Token: 0x040002C3 RID: 707
		protected EditableTextWidget.MouseState _mouseState;

		// Token: 0x040002C4 RID: 708
		protected Vector2 _mouseDownPosition;

		// Token: 0x040002C5 RID: 709
		protected bool _cursorVisible;

		// Token: 0x040002C6 RID: 710
		protected int _textHeight;

		// Token: 0x040002C7 RID: 711
		protected EditableTextWidget.CursorMovementDirection _cursorDirection;

		// Token: 0x040002C8 RID: 712
		protected EditableTextWidget.KeyboardAction _keyboardAction;

		// Token: 0x040002C9 RID: 713
		protected int _nextRepeatTime;

		// Token: 0x040002CA RID: 714
		protected bool _isSelection;

		// Token: 0x040002CD RID: 717
		private string _realText = "";

		// Token: 0x040002CE RID: 718
		private string _keyboardInfoText = "";

		// Token: 0x02000088 RID: 136
		protected enum MouseState
		{
			// Token: 0x0400044E RID: 1102
			None,
			// Token: 0x0400044F RID: 1103
			Down,
			// Token: 0x04000450 RID: 1104
			Up
		}

		// Token: 0x02000089 RID: 137
		protected enum CursorMovementDirection
		{
			// Token: 0x04000452 RID: 1106
			None,
			// Token: 0x04000453 RID: 1107
			Left = -1,
			// Token: 0x04000454 RID: 1108
			Right = 1
		}

		// Token: 0x0200008A RID: 138
		protected enum KeyboardAction
		{
			// Token: 0x04000456 RID: 1110
			None,
			// Token: 0x04000457 RID: 1111
			BackSpace,
			// Token: 0x04000458 RID: 1112
			Delete
		}
	}
}
