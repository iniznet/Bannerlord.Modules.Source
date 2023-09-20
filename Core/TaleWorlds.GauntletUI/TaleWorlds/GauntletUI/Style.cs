using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI
{
	public class Style : IDataSource
	{
		public Style DefaultStyle { get; set; }

		[Editor(false)]
		public string Name { get; set; }

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

		public int LayerCount
		{
			get
			{
				return this._layers.Count;
			}
		}

		public StyleLayer DefaultLayer
		{
			get
			{
				return this._layers["Default"];
			}
		}

		[Editor(false)]
		public List<StyleLayer> Layers
		{
			get
			{
				return this._layersWithIndex;
			}
		}

		[Editor(false)]
		public StyleAnimationMode AnimationMode { get; set; }

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

		public void AddLayer(StyleLayer layer)
		{
			this._layers.Add(layer.Name, layer);
			this._layersWithIndex.Add(layer);
			this._localVersion++;
		}

		public void RemoveLayer(string layerName)
		{
			this._layersWithIndex.Remove(this._layers[layerName]);
			this._layers.Remove(layerName);
			this._localVersion++;
		}

		public StyleLayer GetLayer(int index)
		{
			return this._layersWithIndex[index];
		}

		public StyleLayer GetLayer(string name)
		{
			if (this._layers.ContainsKey(name))
			{
				return this._layers[name];
			}
			return null;
		}

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

		public Sprite GetValueAsSprite(BrushAnimationProperty.BrushAnimationPropertyType propertyType)
		{
			Debug.FailedAssert("Invalid value type or property name for data source.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\Style.cs", "GetValueAsSprite", 641);
			return null;
		}

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

		private int _localVersion;

		private bool _isFontColorChanged;

		private bool _isTextGlowColorChanged;

		private bool _isTextOutlineColorChanged;

		private bool _isTextOutlineAmountChanged;

		private bool _isTextGlowRadiusChanged;

		private bool _isTextBlurChanged;

		private bool _isTextShadowOffsetChanged;

		private bool _isTextShadowAngleChanged;

		private bool _isTextColorFactorChanged;

		private bool _isTextAlphaFactorChanged;

		private bool _isTextHueFactorChanged;

		private bool _isTextSaturationFactorChanged;

		private bool _isTextValueFactorChanged;

		private bool _isFontChanged;

		private bool _isFontStyleChanged;

		private bool _isFontSizeChanged;

		private Color _fontColor;

		private Color _textGlowColor;

		private Color _textOutlineColor;

		private float _textOutlineAmount;

		private float _textGlowRadius;

		private float _textBlur;

		private float _textShadowOffset;

		private float _textShadowAngle;

		private float _textColorFactor;

		private float _textAlphaFactor;

		private float _textHueFactor;

		private float _textSaturationFactor;

		private float _textValueFactor;

		private Font _font;

		private FontStyle _fontStyle;

		private int _fontSize;

		private string _animationToPlayOnBegin;

		private Dictionary<string, StyleLayer> _layers;

		private List<StyleLayer> _layersWithIndex;
	}
}
