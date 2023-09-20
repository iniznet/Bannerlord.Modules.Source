using System;
using System.Numerics;
using TaleWorlds.GauntletUI.Layout;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	// Token: 0x0200006C RID: 108
	public class TextWidget : ImageWidget
	{
		// Token: 0x17000205 RID: 517
		// (get) Token: 0x06000705 RID: 1797 RVA: 0x0001EC91 File Offset: 0x0001CE91
		// (set) Token: 0x06000706 RID: 1798 RVA: 0x0001EC99 File Offset: 0x0001CE99
		public bool AutoHideIfEmpty { get; set; }

		// Token: 0x17000206 RID: 518
		// (get) Token: 0x06000707 RID: 1799 RVA: 0x0001ECA2 File Offset: 0x0001CEA2
		// (set) Token: 0x06000708 RID: 1800 RVA: 0x0001ECAF File Offset: 0x0001CEAF
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

		// Token: 0x17000207 RID: 519
		// (get) Token: 0x06000709 RID: 1801 RVA: 0x0001ECCC File Offset: 0x0001CECC
		// (set) Token: 0x0600070A RID: 1802 RVA: 0x0001ECF0 File Offset: 0x0001CEF0
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

		// Token: 0x17000208 RID: 520
		// (get) Token: 0x0600070B RID: 1803 RVA: 0x0001ED18 File Offset: 0x0001CF18
		// (set) Token: 0x0600070C RID: 1804 RVA: 0x0001ED40 File Offset: 0x0001CF40
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

		// Token: 0x0600070D RID: 1805 RVA: 0x0001ED68 File Offset: 0x0001CF68
		public TextWidget(UIContext context)
			: base(context)
		{
			FontFactory fontFactory = context.FontFactory;
			this._text = new Text((int)base.Size.X, (int)base.Size.Y, fontFactory.DefaultFont, new Func<int, Font>(fontFactory.GetUsableFontForCharacter));
			base.LayoutImp = new TextLayout(this._text);
		}

		// Token: 0x0600070E RID: 1806 RVA: 0x0001EDD0 File Offset: 0x0001CFD0
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

		// Token: 0x0600070F RID: 1807 RVA: 0x0001EE68 File Offset: 0x0001D068
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

		// Token: 0x06000710 RID: 1808 RVA: 0x0001EFD4 File Offset: 0x0001D1D4
		protected override void OnRender(TwoDimensionContext twoDimensionContext, TwoDimensionDrawContext drawContext)
		{
			base.OnRender(twoDimensionContext, drawContext);
			this.RefreshTextParameters();
			TextMaterial textMaterial = base.BrushRenderer.CreateTextMaterial(drawContext);
			textMaterial.AlphaFactor *= base.Context.ContextAlpha;
			Vector2 cachedGlobalPosition = this._cachedGlobalPosition;
			drawContext.Draw(this._text, textMaterial, cachedGlobalPosition.X, cachedGlobalPosition.Y, base.Size.X, base.Size.Y);
		}

		// Token: 0x0400034F RID: 847
		protected readonly Text _text;

		// Token: 0x04000351 RID: 849
		protected bool IsTextValueDirty = true;
	}
}
