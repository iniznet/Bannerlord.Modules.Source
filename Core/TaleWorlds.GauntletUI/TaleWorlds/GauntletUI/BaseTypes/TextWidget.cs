using System;
using System.Numerics;
using TaleWorlds.GauntletUI.Layout;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	public class TextWidget : ImageWidget
	{
		public bool AutoHideIfEmpty { get; set; }

		[Editor(false)]
		public string Text
		{
			get
			{
				return this._text.Value;
			}
			set
			{
				if (this._text.Value != value)
				{
					this.SetText(value);
				}
			}
		}

		[Editor(false)]
		public int IntText
		{
			get
			{
				int num;
				if (int.TryParse(this._text.Value, out num))
				{
					return num;
				}
				return -1;
			}
			set
			{
				if (this._text.Value != value.ToString())
				{
					this.SetText(value.ToString());
				}
			}
		}

		[Editor(false)]
		public float FloatText
		{
			get
			{
				float num;
				if (float.TryParse(this._text.Value, out num))
				{
					return num;
				}
				return -1f;
			}
			set
			{
				if (this._text.Value != value.ToString())
				{
					this.SetText(value.ToString());
				}
			}
		}

		public TextWidget(UIContext context)
			: base(context)
		{
			FontFactory fontFactory = context.FontFactory;
			this._text = new Text((int)base.Size.X, (int)base.Size.Y, fontFactory.DefaultFont, new Func<int, Font>(fontFactory.GetUsableFontForCharacter));
			base.LayoutImp = new TextLayout(this._text);
		}

		protected virtual void SetText(string value)
		{
			base.SetMeasureAndLayoutDirty();
			this._text.CurrentLanguage = base.Context.FontFactory.GetCurrentLanguage();
			this._text.Value = value;
			base.OnPropertyChanged(this.FloatText, "FloatText");
			base.OnPropertyChanged(this.IntText, "IntText");
			base.OnPropertyChanged<string>(this.Text, "Text");
			this.RefreshTextParameters();
			if (this.AutoHideIfEmpty)
			{
				base.IsVisible = !string.IsNullOrEmpty(this.Text);
			}
			this.IsTextValueDirty = true;
		}

		protected void RefreshTextParameters()
		{
			float num = (float)base.ReadOnlyBrush.FontSize * base._scaleToUse;
			this._text.HorizontalAlignment = base.ReadOnlyBrush.TextHorizontalAlignment;
			this._text.VerticalAlignment = base.ReadOnlyBrush.TextVerticalAlignment;
			this._text.FontSize = num;
			this._text.CurrentLanguage = base.Context.FontFactory.GetCurrentLanguage();
			if (base.ReadOnlyBrush.Font != null)
			{
				this._text.Font = base.Context.FontFactory.GetMappedFontForLocalization(base.ReadOnlyBrush.Font.Name);
			}
			else
			{
				this._text.Font = base.Context.FontFactory.DefaultFont;
			}
			if (this.IsTextValueDirty)
			{
				int i = 0;
				while (i < this._text.Value.Length)
				{
					if (char.IsLetter(this._text.Value[i]) && !this._text.Font.Characters.ContainsKey((int)this._text.Value[i]))
					{
						Font usableFontForCharacter = base.Context.FontFactory.GetUsableFontForCharacter((int)this._text.Value[i]);
						if (usableFontForCharacter != null)
						{
							this._text.Font = usableFontForCharacter;
							break;
						}
						break;
					}
					else
					{
						i++;
					}
				}
				this.IsTextValueDirty = false;
			}
		}

		protected override void OnRender(TwoDimensionContext twoDimensionContext, TwoDimensionDrawContext drawContext)
		{
			base.OnRender(twoDimensionContext, drawContext);
			this.RefreshTextParameters();
			TextMaterial textMaterial = base.BrushRenderer.CreateTextMaterial(drawContext);
			textMaterial.AlphaFactor *= base.Context.ContextAlpha;
			Vector2 cachedGlobalPosition = this._cachedGlobalPosition;
			drawContext.Draw(this._text, textMaterial, cachedGlobalPosition.X, cachedGlobalPosition.Y, base.Size.X, base.Size.Y);
		}

		protected readonly Text _text;

		protected bool IsTextValueDirty = true;
	}
}
