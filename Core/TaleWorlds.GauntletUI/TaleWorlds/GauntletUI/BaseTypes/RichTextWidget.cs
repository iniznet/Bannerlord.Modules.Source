using System;
using System.Collections.Generic;
using System.Numerics;
using TaleWorlds.GauntletUI.Layout;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	public class RichTextWidget : BrushWidget
	{
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

		protected override void OnParallelUpdate(float dt)
		{
			base.OnParallelUpdate(dt);
			this.SetRichTextParameters();
		}

		public override void OnBrushChanged()
		{
			base.OnBrushChanged();
			this.UpdateFontData();
		}

		protected virtual void SetText(string value)
		{
		}

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

		protected override void RefreshState()
		{
			base.RefreshState();
			this.UpdateText();
		}

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

		protected internal override void OnMousePressed()
		{
			if (this._mouseState == RichTextWidget.MouseState.None)
			{
				this._mouseDownPosition = this.LocalMousePosition;
				this._mouseState = RichTextWidget.MouseState.Down;
			}
		}

		protected internal override void OnMouseReleased()
		{
			if (this._mouseState == RichTextWidget.MouseState.Down)
			{
				this._mouseState = RichTextWidget.MouseState.Up;
			}
		}

		protected internal override void OnMouseAlternatePressed()
		{
			if (this._mouseState == RichTextWidget.MouseState.None)
			{
				this._mouseDownPosition = this.LocalMousePosition;
				this._mouseState = RichTextWidget.MouseState.AlternateDown;
			}
		}

		protected internal override void OnMouseAlternateReleased()
		{
			if (this._mouseState == RichTextWidget.MouseState.AlternateDown)
			{
				this._mouseState = RichTextWidget.MouseState.AlternateUp;
			}
		}

		protected readonly RichText _richText;

		private Brush _lastFontBrush;

		private string _lastLanguageCode;

		private float _lastContextScale;

		private FontFactory _fontFactory;

		private RichTextWidget.MouseState _mouseState;

		private Dictionary<Texture, SimpleMaterial> _textureMaterialDict;

		private Vector2 _mouseDownPosition;

		private int _textHeight;

		protected float _renderXOffset;

		private string _linkHoverCursorState;

		private enum MouseState
		{
			None,
			Down,
			Up,
			AlternateDown,
			AlternateUp
		}
	}
}
