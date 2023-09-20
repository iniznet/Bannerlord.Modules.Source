using System;
using System.Collections.Generic;
using System.Numerics;
using TaleWorlds.GauntletUI.Layout;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	// Token: 0x02000062 RID: 98
	public class RichTextWidget : BrushWidget
	{
		// Token: 0x170001C5 RID: 453
		// (get) Token: 0x06000628 RID: 1576 RVA: 0x0001B48C File Offset: 0x0001968C
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

		// Token: 0x170001C6 RID: 454
		// (get) Token: 0x06000629 RID: 1577 RVA: 0x0001B4CD File Offset: 0x000196CD
		// (set) Token: 0x0600062A RID: 1578 RVA: 0x0001B4D5 File Offset: 0x000196D5
		[Editor(false)]
		public string LinkHoverCursorState
		{
			get
			{
				return this._linkHoverCursorState;
			}
			set
			{
				if (this._linkHoverCursorState != value)
				{
					this._linkHoverCursorState = value;
				}
			}
		}

		// Token: 0x170001C7 RID: 455
		// (get) Token: 0x0600062B RID: 1579 RVA: 0x0001B4EC File Offset: 0x000196EC
		// (set) Token: 0x0600062C RID: 1580 RVA: 0x0001B4FC File Offset: 0x000196FC
		[Editor(false)]
		public string Text
		{
			get
			{
				return this._richText.Value;
			}
			set
			{
				if (this._richText.Value != value)
				{
					this._richText.CurrentLanguage = base.Context.FontFactory.GetCurrentLanguage();
					this._richText.Value = value;
					base.OnPropertyChanged<string>(value, "Text");
					base.SetMeasureAndLayoutDirty();
					this.SetText(this._richText.Value);
				}
			}
		}

		// Token: 0x0600062D RID: 1581 RVA: 0x0001B568 File Offset: 0x00019768
		public RichTextWidget(UIContext context)
			: base(context)
		{
			this._fontFactory = context.FontFactory;
			this._textHeight = -1;
			Font defaultFont = base.Context.FontFactory.DefaultFont;
			this._richText = new RichText((int)base.Size.X, (int)base.Size.Y, defaultFont, new Func<int, Font>(this._fontFactory.GetUsableFontForCharacter));
			this._textureMaterialDict = new Dictionary<Texture, SimpleMaterial>();
			this._lastFontBrush = null;
			base.LayoutImp = new TextLayout(this._richText);
			base.AddState("Pressed");
			base.AddState("Hovered");
			base.AddState("Disabled");
		}

		// Token: 0x0600062E RID: 1582 RVA: 0x0001B619 File Offset: 0x00019819
		protected override void OnParallelUpdate(float dt)
		{
			base.OnParallelUpdate(dt);
			this.SetRichTextParameters();
		}

		// Token: 0x0600062F RID: 1583 RVA: 0x0001B628 File Offset: 0x00019828
		public override void OnBrushChanged()
		{
			base.OnBrushChanged();
			this.UpdateFontData();
		}

		// Token: 0x06000630 RID: 1584 RVA: 0x0001B636 File Offset: 0x00019836
		protected virtual void SetText(string value)
		{
		}

		// Token: 0x06000631 RID: 1585 RVA: 0x0001B638 File Offset: 0x00019838
		private void SetRichTextParameters()
		{
			bool flag = false;
			this._richText.CurrentLanguage = base.Context.FontFactory.GetCurrentLanguage();
			this.UpdateFontData();
			if (this._richText.HorizontalAlignment != base.ReadOnlyBrush.TextHorizontalAlignment)
			{
				this._richText.HorizontalAlignment = base.ReadOnlyBrush.TextHorizontalAlignment;
				flag = true;
			}
			if (this._richText.VerticalAlignment != base.ReadOnlyBrush.TextVerticalAlignment)
			{
				this._richText.VerticalAlignment = base.ReadOnlyBrush.TextVerticalAlignment;
				flag = true;
			}
			if (this._richText.TextHeight != this._textHeight)
			{
				this._textHeight = this._richText.TextHeight;
				flag = true;
			}
			if (this._richText.CurrentStyle != base.CurrentState && !string.IsNullOrEmpty(base.CurrentState))
			{
				this._richText.CurrentStyle = base.CurrentState;
				flag = true;
			}
			if (flag)
			{
				base.SetMeasureAndLayoutDirty();
			}
		}

		// Token: 0x06000632 RID: 1586 RVA: 0x0001B72F File Offset: 0x0001992F
		protected override void RefreshState()
		{
			base.RefreshState();
			this.UpdateText();
		}

		// Token: 0x06000633 RID: 1587 RVA: 0x0001B740 File Offset: 0x00019940
		private void UpdateText()
		{
			if (base.IsDisabled)
			{
				this.SetState("Disabled");
				return;
			}
			if (base.IsPressed)
			{
				this.SetState("Pressed");
				return;
			}
			if (base.IsHovered)
			{
				this.SetState("Hovered");
				return;
			}
			this.SetState("Default");
		}

		// Token: 0x06000634 RID: 1588 RVA: 0x0001B794 File Offset: 0x00019994
		private void UpdateFontData()
		{
			if (this._lastFontBrush == base.ReadOnlyBrush && this._lastContextScale == base._scaleToUse && this._lastLanguageCode == base.Context.FontFactory.CurrentLangageID)
			{
				return;
			}
			this._richText.StyleFontContainer.ClearFonts();
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
				this._richText.StyleFontContainer.Add(style.Name, mappedFontForLocalization, (float)style.FontSize * base._scaleToUse);
			}
			this._lastFontBrush = base.ReadOnlyBrush;
			this._lastLanguageCode = base.Context.FontFactory.CurrentLangageID;
			this._lastContextScale = base._scaleToUse;
			this._richText.CurrentLanguage = base.Context.FontFactory.GetCurrentLanguage();
		}

		// Token: 0x06000635 RID: 1589 RVA: 0x0001B8FC File Offset: 0x00019AFC
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

		// Token: 0x06000636 RID: 1590 RVA: 0x0001B974 File Offset: 0x00019B74
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (base.Size.X > 0f && base.Size.Y > 0f)
			{
				Vector2 vector = this.LocalMousePosition;
				bool flag = this._mouseState == RichTextWidget.MouseState.Down || this._mouseState == RichTextWidget.MouseState.AlternateDown;
				bool flag2 = this._mouseState == RichTextWidget.MouseState.Up || this._mouseState == RichTextWidget.MouseState.AlternateUp;
				if (flag)
				{
					vector = this._mouseDownPosition;
				}
				RichTextLinkGroup focusedLinkGroup = this._richText.FocusedLinkGroup;
				this._richText.UpdateSize((int)base.Size.X, (int)base.Size.Y);
				if (focusedLinkGroup != null && this.LinkHoverCursorState != null)
				{
					base.Context.ActiveCursorOfContext = (UIContext.MouseCursors)Enum.Parse(typeof(UIContext.MouseCursors), this.LinkHoverCursorState);
				}
				bool flag3 = base.WidthSizePolicy != SizePolicy.CoverChildren || base.MaxWidth != 0f;
				bool flag4 = base.HeightSizePolicy != SizePolicy.CoverChildren || base.MaxHeight != 0f;
				this._richText.Update(base.Context.SpriteData, vector, flag, flag3, flag4, base._scaleToUse);
				if (flag2)
				{
					RichTextLinkGroup focusedLinkGroup2 = this._richText.FocusedLinkGroup;
					if (focusedLinkGroup != null && focusedLinkGroup == focusedLinkGroup2)
					{
						string text = focusedLinkGroup.Href;
						string[] array = text.Split(new char[] { ':' });
						if (array.Length == 2)
						{
							text = array[1];
						}
						if (this._mouseState == RichTextWidget.MouseState.Up)
						{
							base.EventFired("LinkClick", new object[] { text });
						}
						else if (this._mouseState == RichTextWidget.MouseState.AlternateUp)
						{
							base.EventFired("LinkAlternateClick", new object[] { text });
						}
					}
					this._mouseState = RichTextWidget.MouseState.None;
				}
			}
		}

		// Token: 0x06000637 RID: 1591 RVA: 0x0001BB34 File Offset: 0x00019D34
		protected override void OnRender(TwoDimensionContext twoDimensionContext, TwoDimensionDrawContext drawContext)
		{
			base.OnRender(twoDimensionContext, drawContext);
			if (!string.IsNullOrEmpty(this._richText.Value))
			{
				foreach (RichTextPart richTextPart in this._richText.GetParts())
				{
					DrawObject2D drawObject2D = richTextPart.DrawObject2D;
					if (drawObject2D != null)
					{
						Material material = null;
						Vector2 cachedGlobalPosition = this._cachedGlobalPosition;
						if (richTextPart.Type == RichTextPartType.Text)
						{
							Style styleOrDefault = base.ReadOnlyBrush.GetStyleOrDefault(richTextPart.Style);
							Font defaultFont = richTextPart.DefaultFont;
							float num = (float)styleOrDefault.FontSize * base._scaleToUse;
							TextMaterial textMaterial = styleOrDefault.CreateTextMaterial(drawContext);
							textMaterial.ColorFactor *= base.ReadOnlyBrush.GlobalColorFactor;
							textMaterial.AlphaFactor *= base.ReadOnlyBrush.GlobalAlphaFactor * base.Context.ContextAlpha;
							textMaterial.Color *= base.ReadOnlyBrush.GlobalColor;
							textMaterial.Texture = defaultFont.FontSprite.Texture;
							textMaterial.ScaleFactor = num;
							textMaterial.SmoothingConstant = defaultFont.SmoothingConstant;
							textMaterial.Smooth = defaultFont.Smooth;
							if (textMaterial.GlowRadius > 0f || textMaterial.Blur > 0f || textMaterial.OutlineAmount > 0f)
							{
								TextMaterial textMaterial2 = styleOrDefault.CreateTextMaterial(drawContext);
								textMaterial2.CopyFrom(textMaterial);
								drawContext.Draw(cachedGlobalPosition.X + this._renderXOffset, cachedGlobalPosition.Y, textMaterial2, drawObject2D, base.Size.X, base.Size.Y);
							}
							textMaterial.GlowRadius = 0f;
							textMaterial.Blur = 0f;
							textMaterial.OutlineAmount = 0f;
							material = textMaterial;
						}
						else if (richTextPart.Type == RichTextPartType.Sprite)
						{
							Sprite sprite = richTextPart.Sprite;
							if (((sprite != null) ? sprite.Texture : null) != null)
							{
								if (!this._textureMaterialDict.ContainsKey(sprite.Texture))
								{
									this._textureMaterialDict[sprite.Texture] = new SimpleMaterial(sprite.Texture);
								}
								SimpleMaterial simpleMaterial = this._textureMaterialDict[sprite.Texture];
								if (simpleMaterial.ColorFactor != base.ReadOnlyBrush.GlobalColorFactor)
								{
									simpleMaterial.ColorFactor = base.ReadOnlyBrush.GlobalColorFactor;
								}
								if (simpleMaterial.AlphaFactor != base.ReadOnlyBrush.GlobalAlphaFactor * base.Context.ContextAlpha)
								{
									simpleMaterial.AlphaFactor = base.ReadOnlyBrush.GlobalAlphaFactor * base.Context.ContextAlpha;
								}
								if (simpleMaterial.Color != base.ReadOnlyBrush.GlobalColor)
								{
									simpleMaterial.Color = base.ReadOnlyBrush.GlobalColor;
								}
								material = simpleMaterial;
							}
						}
						if (material != null)
						{
							drawContext.Draw(cachedGlobalPosition.X + this._renderXOffset, cachedGlobalPosition.Y, material, drawObject2D, base.Size.X, base.Size.Y);
						}
					}
				}
			}
		}

		// Token: 0x06000638 RID: 1592 RVA: 0x0001BE74 File Offset: 0x0001A074
		protected internal override void OnMousePressed()
		{
			if (this._mouseState == RichTextWidget.MouseState.None)
			{
				this._mouseDownPosition = this.LocalMousePosition;
				this._mouseState = RichTextWidget.MouseState.Down;
			}
		}

		// Token: 0x06000639 RID: 1593 RVA: 0x0001BE91 File Offset: 0x0001A091
		protected internal override void OnMouseReleased()
		{
			if (this._mouseState == RichTextWidget.MouseState.Down)
			{
				this._mouseState = RichTextWidget.MouseState.Up;
			}
		}

		// Token: 0x0600063A RID: 1594 RVA: 0x0001BEA3 File Offset: 0x0001A0A3
		protected internal override void OnMouseAlternatePressed()
		{
			if (this._mouseState == RichTextWidget.MouseState.None)
			{
				this._mouseDownPosition = this.LocalMousePosition;
				this._mouseState = RichTextWidget.MouseState.AlternateDown;
			}
		}

		// Token: 0x0600063B RID: 1595 RVA: 0x0001BEC0 File Offset: 0x0001A0C0
		protected internal override void OnMouseAlternateReleased()
		{
			if (this._mouseState == RichTextWidget.MouseState.AlternateDown)
			{
				this._mouseState = RichTextWidget.MouseState.AlternateUp;
			}
		}

		// Token: 0x040002EC RID: 748
		protected readonly RichText _richText;

		// Token: 0x040002ED RID: 749
		private Brush _lastFontBrush;

		// Token: 0x040002EE RID: 750
		private string _lastLanguageCode;

		// Token: 0x040002EF RID: 751
		private float _lastContextScale;

		// Token: 0x040002F0 RID: 752
		private FontFactory _fontFactory;

		// Token: 0x040002F1 RID: 753
		private RichTextWidget.MouseState _mouseState;

		// Token: 0x040002F2 RID: 754
		private Dictionary<Texture, SimpleMaterial> _textureMaterialDict;

		// Token: 0x040002F3 RID: 755
		private Vector2 _mouseDownPosition;

		// Token: 0x040002F4 RID: 756
		private int _textHeight;

		// Token: 0x040002F5 RID: 757
		protected float _renderXOffset;

		// Token: 0x040002F6 RID: 758
		private string _linkHoverCursorState;

		// Token: 0x0200008E RID: 142
		private enum MouseState
		{
			// Token: 0x04000465 RID: 1125
			None,
			// Token: 0x04000466 RID: 1126
			Down,
			// Token: 0x04000467 RID: 1127
			Up,
			// Token: 0x04000468 RID: 1128
			AlternateDown,
			// Token: 0x04000469 RID: 1129
			AlternateUp
		}
	}
}
