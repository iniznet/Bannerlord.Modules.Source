using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI
{
	// Token: 0x02000015 RID: 21
	public class Style : IDataSource
	{
		// Token: 0x17000060 RID: 96
		// (get) Token: 0x0600014A RID: 330 RVA: 0x00008F8D File Offset: 0x0000718D
		// (set) Token: 0x0600014B RID: 331 RVA: 0x00008F95 File Offset: 0x00007195
		public Style DefaultStyle { get; set; }

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x0600014C RID: 332 RVA: 0x00008F9E File Offset: 0x0000719E
		// (set) Token: 0x0600014D RID: 333 RVA: 0x00008FA6 File Offset: 0x000071A6
		[Editor(false)]
		public string Name { get; set; }

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x0600014E RID: 334 RVA: 0x00008FAF File Offset: 0x000071AF
		private uint DefaultStyleVersion
		{
			get
			{
				if (this.DefaultStyle == null)
				{
					return 0U;
				}
				return (uint)((long)this.DefaultStyle._localVersion % (long)((ulong)(-1)));
			}
		}

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x0600014F RID: 335 RVA: 0x00008FCC File Offset: 0x000071CC
		public long Version
		{
			get
			{
				uint num = 0U;
				for (int i = 0; i < this._layersWithIndex.Count; i++)
				{
					num += this._layersWithIndex[i].Version;
				}
				return (((long)this._localVersion << 32) | (long)((ulong)num)) + (long)((ulong)this.DefaultStyleVersion);
			}
		}

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x06000150 RID: 336 RVA: 0x0000901A File Offset: 0x0000721A
		// (set) Token: 0x06000151 RID: 337 RVA: 0x00009022 File Offset: 0x00007222
		[Editor(false)]
		public string AnimationToPlayOnBegin
		{
			get
			{
				return this._animationToPlayOnBegin;
			}
			set
			{
				this._animationToPlayOnBegin = value;
				this.AnimationMode = StyleAnimationMode.Animation;
			}
		}

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x06000152 RID: 338 RVA: 0x00009032 File Offset: 0x00007232
		public int LayerCount
		{
			get
			{
				return this._layers.Count;
			}
		}

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06000153 RID: 339 RVA: 0x0000903F File Offset: 0x0000723F
		public StyleLayer DefaultLayer
		{
			get
			{
				return this._layers["Default"];
			}
		}

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x06000154 RID: 340 RVA: 0x00009051 File Offset: 0x00007251
		[Editor(false)]
		public List<StyleLayer> Layers
		{
			get
			{
				return this._layersWithIndex;
			}
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x06000155 RID: 341 RVA: 0x00009059 File Offset: 0x00007259
		// (set) Token: 0x06000156 RID: 342 RVA: 0x00009061 File Offset: 0x00007261
		[Editor(false)]
		public StyleAnimationMode AnimationMode { get; set; }

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000157 RID: 343 RVA: 0x0000906A File Offset: 0x0000726A
		// (set) Token: 0x06000158 RID: 344 RVA: 0x00009086 File Offset: 0x00007286
		[Editor(false)]
		public Color FontColor
		{
			get
			{
				if (this._isFontColorChanged)
				{
					return this._fontColor;
				}
				return this.DefaultStyle.FontColor;
			}
			set
			{
				if (this.FontColor != value)
				{
					this._isFontColorChanged = true;
					this._fontColor = value;
					this._localVersion++;
				}
			}
		}

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x06000159 RID: 345 RVA: 0x000090B2 File Offset: 0x000072B2
		// (set) Token: 0x0600015A RID: 346 RVA: 0x000090CE File Offset: 0x000072CE
		[Editor(false)]
		public Color TextGlowColor
		{
			get
			{
				if (this._isTextGlowColorChanged)
				{
					return this._textGlowColor;
				}
				return this.DefaultStyle.TextGlowColor;
			}
			set
			{
				if (this.TextGlowColor != value)
				{
					this._isTextGlowColorChanged = true;
					this._textGlowColor = value;
					this._localVersion++;
				}
			}
		}

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x0600015B RID: 347 RVA: 0x000090FA File Offset: 0x000072FA
		// (set) Token: 0x0600015C RID: 348 RVA: 0x00009116 File Offset: 0x00007316
		[Editor(false)]
		public Color TextOutlineColor
		{
			get
			{
				if (this._isTextOutlineColorChanged)
				{
					return this._textOutlineColor;
				}
				return this.DefaultStyle.TextOutlineColor;
			}
			set
			{
				if (this.TextOutlineColor != value)
				{
					this._isTextOutlineColorChanged = true;
					this._textOutlineColor = value;
					this._localVersion++;
				}
			}
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x0600015D RID: 349 RVA: 0x00009142 File Offset: 0x00007342
		// (set) Token: 0x0600015E RID: 350 RVA: 0x0000915E File Offset: 0x0000735E
		[Editor(false)]
		public float TextOutlineAmount
		{
			get
			{
				if (this._isTextOutlineAmountChanged)
				{
					return this._textOutlineAmount;
				}
				return this.DefaultStyle.TextOutlineAmount;
			}
			set
			{
				if (this.TextOutlineAmount != value)
				{
					this._isTextOutlineAmountChanged = true;
					this._textOutlineAmount = value;
					this._localVersion++;
				}
			}
		}

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x0600015F RID: 351 RVA: 0x00009185 File Offset: 0x00007385
		// (set) Token: 0x06000160 RID: 352 RVA: 0x000091A1 File Offset: 0x000073A1
		[Editor(false)]
		public float TextGlowRadius
		{
			get
			{
				if (this._isTextGlowRadiusChanged)
				{
					return this._textGlowRadius;
				}
				return this.DefaultStyle.TextGlowRadius;
			}
			set
			{
				if (this.TextGlowRadius != value)
				{
					this._isTextGlowRadiusChanged = true;
					this._textGlowRadius = value;
					this._localVersion++;
				}
			}
		}

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x06000161 RID: 353 RVA: 0x000091C8 File Offset: 0x000073C8
		// (set) Token: 0x06000162 RID: 354 RVA: 0x000091E4 File Offset: 0x000073E4
		[Editor(false)]
		public float TextBlur
		{
			get
			{
				if (this._isTextBlurChanged)
				{
					return this._textBlur;
				}
				return this.DefaultStyle.TextBlur;
			}
			set
			{
				if (this.TextBlur != value)
				{
					this._isTextBlurChanged = true;
					this._textBlur = value;
					this._localVersion++;
				}
			}
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000163 RID: 355 RVA: 0x0000920B File Offset: 0x0000740B
		// (set) Token: 0x06000164 RID: 356 RVA: 0x00009227 File Offset: 0x00007427
		[Editor(false)]
		public float TextShadowOffset
		{
			get
			{
				if (this._isTextShadowOffsetChanged)
				{
					return this._textShadowOffset;
				}
				return this.DefaultStyle.TextShadowOffset;
			}
			set
			{
				if (this.TextShadowOffset != value)
				{
					this._isTextShadowOffsetChanged = true;
					this._textShadowOffset = value;
					this._localVersion++;
				}
			}
		}

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x06000165 RID: 357 RVA: 0x0000924E File Offset: 0x0000744E
		// (set) Token: 0x06000166 RID: 358 RVA: 0x0000926A File Offset: 0x0000746A
		[Editor(false)]
		public float TextShadowAngle
		{
			get
			{
				if (this._isTextShadowAngleChanged)
				{
					return this._textShadowAngle;
				}
				return this.DefaultStyle.TextShadowAngle;
			}
			set
			{
				if (this.TextShadowAngle != value)
				{
					this._isTextShadowAngleChanged = true;
					this._textShadowAngle = value;
					this._localVersion++;
				}
			}
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x06000167 RID: 359 RVA: 0x00009291 File Offset: 0x00007491
		// (set) Token: 0x06000168 RID: 360 RVA: 0x000092AD File Offset: 0x000074AD
		[Editor(false)]
		public float TextColorFactor
		{
			get
			{
				if (this._isTextColorFactorChanged)
				{
					return this._textColorFactor;
				}
				return this.DefaultStyle.TextColorFactor;
			}
			set
			{
				if (this.TextColorFactor != value)
				{
					this._isTextColorFactorChanged = true;
					this._textColorFactor = value;
					this._localVersion++;
				}
			}
		}

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x06000169 RID: 361 RVA: 0x000092D4 File Offset: 0x000074D4
		// (set) Token: 0x0600016A RID: 362 RVA: 0x000092F0 File Offset: 0x000074F0
		[Editor(false)]
		public float TextAlphaFactor
		{
			get
			{
				if (this._isTextAlphaFactorChanged)
				{
					return this._textAlphaFactor;
				}
				return this.DefaultStyle.TextAlphaFactor;
			}
			set
			{
				if (this.TextAlphaFactor != value)
				{
					this._isTextAlphaFactorChanged = true;
					this._textAlphaFactor = value;
					this._localVersion++;
				}
			}
		}

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x0600016B RID: 363 RVA: 0x00009317 File Offset: 0x00007517
		// (set) Token: 0x0600016C RID: 364 RVA: 0x00009333 File Offset: 0x00007533
		[Editor(false)]
		public float TextHueFactor
		{
			get
			{
				if (this._isTextHueFactorChanged)
				{
					return this._textHueFactor;
				}
				return this.DefaultStyle.TextHueFactor;
			}
			set
			{
				if (this.TextHueFactor != value)
				{
					this._isTextHueFactorChanged = true;
					this._textHueFactor = value;
					this._localVersion++;
				}
			}
		}

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x0600016D RID: 365 RVA: 0x0000935A File Offset: 0x0000755A
		// (set) Token: 0x0600016E RID: 366 RVA: 0x00009376 File Offset: 0x00007576
		[Editor(false)]
		public float TextSaturationFactor
		{
			get
			{
				if (this._isTextSaturationFactorChanged)
				{
					return this._textSaturationFactor;
				}
				return this.DefaultStyle.TextSaturationFactor;
			}
			set
			{
				if (this.TextSaturationFactor != value)
				{
					this._isTextSaturationFactorChanged = true;
					this._textSaturationFactor = value;
					this._localVersion++;
				}
			}
		}

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x0600016F RID: 367 RVA: 0x0000939D File Offset: 0x0000759D
		// (set) Token: 0x06000170 RID: 368 RVA: 0x000093B9 File Offset: 0x000075B9
		[Editor(false)]
		public float TextValueFactor
		{
			get
			{
				if (this._isTextValueFactorChanged)
				{
					return this._textValueFactor;
				}
				return this.DefaultStyle.TextValueFactor;
			}
			set
			{
				if (this.TextValueFactor != value)
				{
					this._isTextValueFactorChanged = true;
					this._textValueFactor = value;
					this._localVersion++;
				}
			}
		}

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x06000171 RID: 369 RVA: 0x000093E0 File Offset: 0x000075E0
		// (set) Token: 0x06000172 RID: 370 RVA: 0x000093FC File Offset: 0x000075FC
		[Editor(false)]
		public Font Font
		{
			get
			{
				if (this._isFontChanged)
				{
					return this._font;
				}
				return this.DefaultStyle.Font;
			}
			set
			{
				if (this.Font != value)
				{
					this._isFontChanged = true;
					this._font = value;
					this._localVersion++;
				}
			}
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000173 RID: 371 RVA: 0x00009423 File Offset: 0x00007623
		// (set) Token: 0x06000174 RID: 372 RVA: 0x0000943F File Offset: 0x0000763F
		[Editor(false)]
		public FontStyle FontStyle
		{
			get
			{
				if (this._isFontStyleChanged)
				{
					return this._fontStyle;
				}
				return this.DefaultStyle.FontStyle;
			}
			set
			{
				if (this.FontStyle != value)
				{
					this._isFontStyleChanged = true;
					this._fontStyle = value;
					this._localVersion++;
				}
			}
		}

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x06000175 RID: 373 RVA: 0x00009466 File Offset: 0x00007666
		// (set) Token: 0x06000176 RID: 374 RVA: 0x00009482 File Offset: 0x00007682
		[Editor(false)]
		public int FontSize
		{
			get
			{
				if (this._isFontSizeChanged)
				{
					return this._fontSize;
				}
				return this.DefaultStyle.FontSize;
			}
			set
			{
				if (this.FontSize != value)
				{
					this._isFontSizeChanged = true;
					this._fontSize = value;
					this._localVersion++;
				}
			}
		}

		// Token: 0x06000177 RID: 375 RVA: 0x000094AC File Offset: 0x000076AC
		public Style(IEnumerable<BrushLayer> layers)
		{
			this.AnimationMode = StyleAnimationMode.BasicTransition;
			this._layers = new Dictionary<string, StyleLayer>();
			this._layersWithIndex = new List<StyleLayer>();
			this._fontColor = new Color(0f, 0f, 0f, 1f);
			this._textGlowColor = new Color(0f, 0f, 0f, 1f);
			this._textOutlineColor = new Color(0f, 0f, 0f, 1f);
			this._textOutlineAmount = 0f;
			this._textGlowRadius = 0.2f;
			this._textBlur = 0.8f;
			this._textShadowOffset = 0.5f;
			this._textShadowAngle = 45f;
			this._textColorFactor = 1f;
			this._textAlphaFactor = 1f;
			this._textHueFactor = 0f;
			this._textSaturationFactor = 0f;
			this._textValueFactor = 0f;
			this._fontSize = 30;
			foreach (BrushLayer brushLayer in layers)
			{
				StyleLayer styleLayer = new StyleLayer(brushLayer);
				this.AddLayer(styleLayer);
			}
		}

		// Token: 0x06000178 RID: 376 RVA: 0x000095F0 File Offset: 0x000077F0
		public void FillFrom(Style style)
		{
			this.Name = style.Name;
			this.FontColor = style.FontColor;
			this.TextGlowColor = style.TextGlowColor;
			this.TextOutlineColor = style.TextOutlineColor;
			this.TextOutlineAmount = style.TextOutlineAmount;
			this.TextGlowRadius = style.TextGlowRadius;
			this.TextBlur = style.TextBlur;
			this.TextShadowOffset = style.TextShadowOffset;
			this.TextShadowAngle = style.TextShadowAngle;
			this.TextColorFactor = style.TextColorFactor;
			this.TextAlphaFactor = style.TextAlphaFactor;
			this.TextHueFactor = style.TextHueFactor;
			this.TextSaturationFactor = style.TextSaturationFactor;
			this.TextValueFactor = style.TextValueFactor;
			this.Font = style.Font;
			this.FontStyle = style.FontStyle;
			this.FontSize = style.FontSize;
			this.AnimationToPlayOnBegin = style.AnimationToPlayOnBegin;
			this.AnimationMode = style.AnimationMode;
			foreach (StyleLayer styleLayer in style._layers.Values)
			{
				this._layers[styleLayer.Name].FillFrom(styleLayer);
			}
		}

		// Token: 0x06000179 RID: 377 RVA: 0x0000973C File Offset: 0x0000793C
		public void AddLayer(StyleLayer layer)
		{
			this._layers.Add(layer.Name, layer);
			this._layersWithIndex.Add(layer);
			this._localVersion++;
		}

		// Token: 0x0600017A RID: 378 RVA: 0x0000976A File Offset: 0x0000796A
		public void RemoveLayer(string layerName)
		{
			this._layersWithIndex.Remove(this._layers[layerName]);
			this._layers.Remove(layerName);
			this._localVersion++;
		}

		// Token: 0x0600017B RID: 379 RVA: 0x0000979F File Offset: 0x0000799F
		public StyleLayer GetLayer(int index)
		{
			return this._layersWithIndex[index];
		}

		// Token: 0x0600017C RID: 380 RVA: 0x000097AD File Offset: 0x000079AD
		public StyleLayer GetLayer(string name)
		{
			if (this._layers.ContainsKey(name))
			{
				return this._layers[name];
			}
			return null;
		}

		// Token: 0x0600017D RID: 381 RVA: 0x000097CC File Offset: 0x000079CC
		public TextMaterial CreateTextMaterial(TwoDimensionDrawContext drawContext)
		{
			TextMaterial textMaterial = drawContext.CreateTextMaterial();
			textMaterial.Color = this.FontColor;
			textMaterial.GlowColor = this.TextGlowColor;
			textMaterial.OutlineColor = this.TextOutlineColor;
			textMaterial.OutlineAmount = this.TextOutlineAmount;
			textMaterial.GlowRadius = this.TextGlowRadius;
			textMaterial.Blur = this.TextBlur;
			textMaterial.ShadowOffset = this.TextShadowOffset;
			textMaterial.ShadowAngle = this.TextShadowAngle;
			textMaterial.ColorFactor = this.TextColorFactor;
			textMaterial.AlphaFactor = this.TextAlphaFactor;
			textMaterial.HueFactor = this.TextHueFactor;
			textMaterial.SaturationFactor = this.TextSaturationFactor;
			textMaterial.ValueFactor = this.TextValueFactor;
			return textMaterial;
		}

		// Token: 0x0600017E RID: 382 RVA: 0x0000987C File Offset: 0x00007A7C
		public float GetValueAsFloat(BrushAnimationProperty.BrushAnimationPropertyType propertyType)
		{
			switch (propertyType)
			{
			case BrushAnimationProperty.BrushAnimationPropertyType.TextOutlineAmount:
				return this.TextOutlineAmount;
			case BrushAnimationProperty.BrushAnimationPropertyType.TextGlowRadius:
				return this.TextGlowRadius;
			case BrushAnimationProperty.BrushAnimationPropertyType.TextBlur:
				return this.TextBlur;
			case BrushAnimationProperty.BrushAnimationPropertyType.TextShadowOffset:
				return this.TextShadowOffset;
			case BrushAnimationProperty.BrushAnimationPropertyType.TextShadowAngle:
				return this.TextShadowAngle;
			case BrushAnimationProperty.BrushAnimationPropertyType.TextColorFactor:
				return this.TextColorFactor;
			case BrushAnimationProperty.BrushAnimationPropertyType.TextAlphaFactor:
				return this.TextAlphaFactor;
			case BrushAnimationProperty.BrushAnimationPropertyType.TextHueFactor:
				return this.TextHueFactor;
			case BrushAnimationProperty.BrushAnimationPropertyType.TextSaturationFactor:
				return this.TextSaturationFactor;
			case BrushAnimationProperty.BrushAnimationPropertyType.TextValueFactor:
				return this.TextValueFactor;
			default:
				Debug.FailedAssert("Invalid value type or property name for data source.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\Style.cs", "GetValueAsFloat", 613);
				return 0f;
			}
		}

		// Token: 0x0600017F RID: 383 RVA: 0x00009920 File Offset: 0x00007B20
		public Color GetValueAsColor(BrushAnimationProperty.BrushAnimationPropertyType propertyType)
		{
			switch (propertyType)
			{
			case BrushAnimationProperty.BrushAnimationPropertyType.FontColor:
				return this.FontColor;
			case BrushAnimationProperty.BrushAnimationPropertyType.TextGlowColor:
				return this.TextGlowColor;
			case BrushAnimationProperty.BrushAnimationPropertyType.TextOutlineColor:
				return this.TextOutlineColor;
			}
			Debug.FailedAssert("Invalid value type or property name for data source.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\Style.cs", "GetValueAsColor", 633);
			return Color.Black;
		}

		// Token: 0x06000180 RID: 384 RVA: 0x0000997E File Offset: 0x00007B7E
		public Sprite GetValueAsSprite(BrushAnimationProperty.BrushAnimationPropertyType propertyType)
		{
			Debug.FailedAssert("Invalid value type or property name for data source.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\Style.cs", "GetValueAsSprite", 641);
			return null;
		}

		// Token: 0x06000181 RID: 385 RVA: 0x0000999C File Offset: 0x00007B9C
		public void SetAsDefaultStyle()
		{
			this._isFontColorChanged = true;
			this._isTextGlowColorChanged = true;
			this._isTextOutlineColorChanged = true;
			this._isTextOutlineAmountChanged = true;
			this._isTextGlowRadiusChanged = true;
			this._isTextBlurChanged = true;
			this._isTextShadowOffsetChanged = true;
			this._isTextShadowAngleChanged = true;
			this._isTextColorFactorChanged = true;
			this._isTextAlphaFactorChanged = true;
			this._isTextHueFactorChanged = true;
			this._isTextSaturationFactorChanged = true;
			this._isTextValueFactorChanged = true;
			this._isFontChanged = true;
			this._isFontStyleChanged = true;
			this._isFontSizeChanged = true;
			this.DefaultStyle = null;
		}

		// Token: 0x04000095 RID: 149
		private int _localVersion;

		// Token: 0x04000096 RID: 150
		private bool _isFontColorChanged;

		// Token: 0x04000097 RID: 151
		private bool _isTextGlowColorChanged;

		// Token: 0x04000098 RID: 152
		private bool _isTextOutlineColorChanged;

		// Token: 0x04000099 RID: 153
		private bool _isTextOutlineAmountChanged;

		// Token: 0x0400009A RID: 154
		private bool _isTextGlowRadiusChanged;

		// Token: 0x0400009B RID: 155
		private bool _isTextBlurChanged;

		// Token: 0x0400009C RID: 156
		private bool _isTextShadowOffsetChanged;

		// Token: 0x0400009D RID: 157
		private bool _isTextShadowAngleChanged;

		// Token: 0x0400009E RID: 158
		private bool _isTextColorFactorChanged;

		// Token: 0x0400009F RID: 159
		private bool _isTextAlphaFactorChanged;

		// Token: 0x040000A0 RID: 160
		private bool _isTextHueFactorChanged;

		// Token: 0x040000A1 RID: 161
		private bool _isTextSaturationFactorChanged;

		// Token: 0x040000A2 RID: 162
		private bool _isTextValueFactorChanged;

		// Token: 0x040000A3 RID: 163
		private bool _isFontChanged;

		// Token: 0x040000A4 RID: 164
		private bool _isFontStyleChanged;

		// Token: 0x040000A5 RID: 165
		private bool _isFontSizeChanged;

		// Token: 0x040000A6 RID: 166
		private Color _fontColor;

		// Token: 0x040000A7 RID: 167
		private Color _textGlowColor;

		// Token: 0x040000A8 RID: 168
		private Color _textOutlineColor;

		// Token: 0x040000A9 RID: 169
		private float _textOutlineAmount;

		// Token: 0x040000AA RID: 170
		private float _textGlowRadius;

		// Token: 0x040000AB RID: 171
		private float _textBlur;

		// Token: 0x040000AC RID: 172
		private float _textShadowOffset;

		// Token: 0x040000AD RID: 173
		private float _textShadowAngle;

		// Token: 0x040000AE RID: 174
		private float _textColorFactor;

		// Token: 0x040000AF RID: 175
		private float _textAlphaFactor;

		// Token: 0x040000B0 RID: 176
		private float _textHueFactor;

		// Token: 0x040000B1 RID: 177
		private float _textSaturationFactor;

		// Token: 0x040000B2 RID: 178
		private float _textValueFactor;

		// Token: 0x040000B3 RID: 179
		private Font _font;

		// Token: 0x040000B4 RID: 180
		private FontStyle _fontStyle;

		// Token: 0x040000B5 RID: 181
		private int _fontSize;

		// Token: 0x040000B6 RID: 182
		private string _animationToPlayOnBegin;

		// Token: 0x040000B7 RID: 183
		private Dictionary<string, StyleLayer> _layers;

		// Token: 0x040000B8 RID: 184
		private List<StyleLayer> _layersWithIndex;
	}
}
