using System;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI
{
	public class StyleLayer : IBrushLayerData, IDataSource
	{
		public BrushLayer SourceLayer { get; private set; }

		public uint Version
		{
			get
			{
				return this._localVersion + this.SourceLayer.Version;
			}
		}

		[Editor(false)]
		public string Name
		{
			get
			{
				return this.SourceLayer.Name;
			}
			set
			{
			}
		}

		[Editor(false)]
		public Sprite Sprite
		{
			get
			{
				if (this._isSpriteChanged)
				{
					return this._sprite;
				}
				return this.SourceLayer.Sprite;
			}
			set
			{
				if (this.Sprite != value)
				{
					this._isSpriteChanged = this.SourceLayer.Sprite != value;
					this._sprite = value;
					this._localVersion += 1U;
				}
			}
		}

		[Editor(false)]
		public Color Color
		{
			get
			{
				if (this._isColorChanged)
				{
					return this._color;
				}
				return this.SourceLayer.Color;
			}
			set
			{
				if (this.Color != value)
				{
					this._isColorChanged = this.SourceLayer.Color != value;
					this._color = value;
					this._localVersion += 1U;
				}
			}
		}

		[Editor(false)]
		public float ColorFactor
		{
			get
			{
				if (this._isColorFactorChanged)
				{
					return this._colorFactor;
				}
				return this.SourceLayer.ColorFactor;
			}
			set
			{
				if (this.ColorFactor != value)
				{
					this._isColorFactorChanged = MathF.Abs(this.SourceLayer.ColorFactor - value) > 1E-05f;
					this._colorFactor = value;
					this._localVersion += 1U;
				}
			}
		}

		[Editor(false)]
		public float AlphaFactor
		{
			get
			{
				if (this._isAlphaFactorChanged)
				{
					return this._alphaFactor;
				}
				return this.SourceLayer.AlphaFactor;
			}
			set
			{
				if (this.AlphaFactor != value)
				{
					this._isAlphaFactorChanged = MathF.Abs(this.SourceLayer.AlphaFactor - value) > 1E-05f;
					this._alphaFactor = value;
					this._localVersion += 1U;
				}
			}
		}

		[Editor(false)]
		public float HueFactor
		{
			get
			{
				if (this._isHueFactorChanged)
				{
					return this._hueFactor;
				}
				return this.SourceLayer.HueFactor;
			}
			set
			{
				if (this.HueFactor != value)
				{
					this._isHueFactorChanged = MathF.Abs(this.SourceLayer.HueFactor - value) > 1E-05f;
					this._hueFactor = value;
					this._localVersion += 1U;
				}
			}
		}

		[Editor(false)]
		public float SaturationFactor
		{
			get
			{
				if (this._isSaturationFactorChanged)
				{
					return this._saturationFactor;
				}
				return this.SourceLayer.SaturationFactor;
			}
			set
			{
				if (this.SaturationFactor != value)
				{
					this._isSaturationFactorChanged = MathF.Abs(this.SourceLayer.SaturationFactor - value) > 1E-05f;
					this._saturationFactor = value;
					this._localVersion += 1U;
				}
			}
		}

		[Editor(false)]
		public float ValueFactor
		{
			get
			{
				if (this._isValueFactorChanged)
				{
					return this._valueFactor;
				}
				return this.SourceLayer.ValueFactor;
			}
			set
			{
				if (this.ValueFactor != value)
				{
					this._isValueFactorChanged = MathF.Abs(this.SourceLayer.ValueFactor - value) > 1E-05f;
					this._valueFactor = value;
					this._localVersion += 1U;
				}
			}
		}

		[Editor(false)]
		public bool IsHidden
		{
			get
			{
				if (this._isIsHiddenChanged)
				{
					return this._isHidden;
				}
				return this.SourceLayer.IsHidden;
			}
			set
			{
				if (this.IsHidden != value)
				{
					this._isIsHiddenChanged = this.SourceLayer.IsHidden != value;
					this._isHidden = value;
					this._localVersion += 1U;
				}
			}
		}

		[Editor(false)]
		public bool UseOverlayAlphaAsMask
		{
			get
			{
				if (this._isUseOverlayAlphaAsMaskChanged)
				{
					return this._useOverlayAlphaAsMask;
				}
				return this.SourceLayer.UseOverlayAlphaAsMask;
			}
			set
			{
				if (this.UseOverlayAlphaAsMask != value)
				{
					this._isUseOverlayAlphaAsMaskChanged = this.SourceLayer.UseOverlayAlphaAsMask != value;
					this._useOverlayAlphaAsMask = value;
					this._localVersion += 1U;
				}
			}
		}

		[Editor(false)]
		public float XOffset
		{
			get
			{
				if (this._isXOffsetChanged)
				{
					return this._xOffset;
				}
				return this.SourceLayer.XOffset;
			}
			set
			{
				if (this.XOffset != value)
				{
					this._isXOffsetChanged = MathF.Abs(this.SourceLayer.XOffset - value) > 1E-05f;
					this._xOffset = value;
					this._localVersion += 1U;
				}
			}
		}

		[Editor(false)]
		public float YOffset
		{
			get
			{
				if (this._isYOffsetChanged)
				{
					return this._yOffset;
				}
				return this.SourceLayer.YOffset;
			}
			set
			{
				if (this.YOffset != value)
				{
					this._isYOffsetChanged = MathF.Abs(this.SourceLayer.YOffset - value) > 1E-05f;
					this._yOffset = value;
					this._localVersion += 1U;
				}
			}
		}

		[Editor(false)]
		public float ExtendLeft
		{
			get
			{
				if (this._isExtendLeftChanged)
				{
					return this._extendLeft;
				}
				return this.SourceLayer.ExtendLeft;
			}
			set
			{
				if (this.ExtendLeft != value)
				{
					this._isExtendLeftChanged = MathF.Abs(this.SourceLayer.ExtendLeft - value) > 1E-05f;
					this._extendLeft = value;
					this._localVersion += 1U;
				}
			}
		}

		[Editor(false)]
		public float ExtendRight
		{
			get
			{
				if (this._isExtendRightChanged)
				{
					return this._extendRight;
				}
				return this.SourceLayer.ExtendRight;
			}
			set
			{
				if (this.ExtendRight != value)
				{
					this._isExtendRightChanged = MathF.Abs(this.SourceLayer.ExtendRight - value) > 1E-05f;
					this._extendRight = value;
					this._localVersion += 1U;
				}
			}
		}

		[Editor(false)]
		public float ExtendTop
		{
			get
			{
				if (this._isExtendTopChanged)
				{
					return this._extendTop;
				}
				return this.SourceLayer.ExtendTop;
			}
			set
			{
				if (this.ExtendTop != value)
				{
					this._isExtendTopChanged = MathF.Abs(this.SourceLayer.ExtendTop - value) > 1E-05f;
					this._extendTop = value;
					this._localVersion += 1U;
				}
			}
		}

		[Editor(false)]
		public float ExtendBottom
		{
			get
			{
				if (this._isExtendBottomChanged)
				{
					return this._extendBottom;
				}
				return this.SourceLayer.ExtendBottom;
			}
			set
			{
				if (this.ExtendBottom != value)
				{
					this._isExtendBottomChanged = MathF.Abs(this.SourceLayer.ExtendBottom - value) > 1E-05f;
					this._extendBottom = value;
					this._localVersion += 1U;
				}
			}
		}

		[Editor(false)]
		public float OverridenWidth
		{
			get
			{
				if (this._isOverridenWidthChanged)
				{
					return this._overridenWidth;
				}
				return this.SourceLayer.OverridenWidth;
			}
			set
			{
				if (this.OverridenWidth != value)
				{
					this._isOverridenWidthChanged = MathF.Abs(this.SourceLayer.OverridenWidth - value) > 1E-05f;
					this._overridenWidth = value;
					this._localVersion += 1U;
				}
			}
		}

		[Editor(false)]
		public float OverridenHeight
		{
			get
			{
				if (this._isOverridenHeightChanged)
				{
					return this._overridenHeight;
				}
				return this.SourceLayer.OverridenHeight;
			}
			set
			{
				if (this.OverridenHeight != value)
				{
					this._isOverridenHeightChanged = MathF.Abs(this.SourceLayer.OverridenHeight - value) > 1E-05f;
					this._overridenHeight = value;
					this._localVersion += 1U;
				}
			}
		}

		[Editor(false)]
		public BrushLayerSizePolicy WidthPolicy
		{
			get
			{
				if (this._isWidthPolicyChanged)
				{
					return this._widthPolicy;
				}
				return this.SourceLayer.WidthPolicy;
			}
			set
			{
				if (this.WidthPolicy != value)
				{
					this._isWidthPolicyChanged = this.SourceLayer.WidthPolicy != value;
					this._widthPolicy = value;
					this._localVersion += 1U;
				}
			}
		}

		public BrushLayerSizePolicy HeightPolicy
		{
			get
			{
				if (this._isHeightPolicyChanged)
				{
					return this._heightPolicy;
				}
				return this.SourceLayer.HeightPolicy;
			}
			set
			{
				if (this.HeightPolicy != value)
				{
					this._isHeightPolicyChanged = this.SourceLayer.HeightPolicy != value;
					this._heightPolicy = value;
					this._localVersion += 1U;
				}
			}
		}

		[Editor(false)]
		public bool HorizontalFlip
		{
			get
			{
				if (this._isHorizontalFlipChanged)
				{
					return this._horizontalFlip;
				}
				return this.SourceLayer.HorizontalFlip;
			}
			set
			{
				if (this.HorizontalFlip != value)
				{
					this._isHorizontalFlipChanged = this.SourceLayer.HorizontalFlip != value;
					this._horizontalFlip = value;
					this._localVersion += 1U;
				}
			}
		}

		[Editor(false)]
		public bool VerticalFlip
		{
			get
			{
				if (this._isVerticalFlipChanged)
				{
					return this._verticalFlip;
				}
				return this.SourceLayer.VerticalFlip;
			}
			set
			{
				if (this.VerticalFlip != value)
				{
					this._isVerticalFlipChanged = this.SourceLayer.VerticalFlip != value;
					this._verticalFlip = value;
					this._localVersion += 1U;
				}
			}
		}

		[Editor(false)]
		public BrushOverlayMethod OverlayMethod
		{
			get
			{
				if (this._isOverlayMethodChanged)
				{
					return this._overlayMethod;
				}
				return this.SourceLayer.OverlayMethod;
			}
			set
			{
				if (this.OverlayMethod != value)
				{
					this._isOverlayMethodChanged = this.SourceLayer.OverlayMethod != value;
					this._overlayMethod = value;
					this._localVersion += 1U;
				}
			}
		}

		[Editor(false)]
		public Sprite OverlaySprite
		{
			get
			{
				if (this._isOverlaySpriteChanged)
				{
					return this._overlaySprite;
				}
				return this.SourceLayer.OverlaySprite;
			}
			set
			{
				if (this.OverlaySprite != value)
				{
					this._isOverlaySpriteChanged = this.SourceLayer.OverlaySprite != value;
					this._overlaySprite = value;
					this._localVersion += 1U;
				}
			}
		}

		[Editor(false)]
		public float OverlayXOffset
		{
			get
			{
				if (this._isOverlayXOffsetChanged)
				{
					return this._overlayXOffset;
				}
				return this.SourceLayer.OverlayXOffset;
			}
			set
			{
				if (this.OverlayXOffset != value)
				{
					this._isOverlayXOffsetChanged = MathF.Abs(this.SourceLayer.OverlayXOffset - value) > 1E-05f;
					this._overlayXOffset = value;
					this._localVersion += 1U;
				}
			}
		}

		[Editor(false)]
		public float OverlayYOffset
		{
			get
			{
				if (this._isOverlayYOffsetChanged)
				{
					return this._overlayYOffset;
				}
				return this.SourceLayer.OverlayYOffset;
			}
			set
			{
				if (this.OverlayYOffset != value)
				{
					this._isOverlayYOffsetChanged = MathF.Abs(this.SourceLayer.OverlayYOffset - value) > 1E-05f;
					this._overlayYOffset = value;
					this._localVersion += 1U;
				}
			}
		}

		[Editor(false)]
		public bool UseRandomBaseOverlayXOffset
		{
			get
			{
				if (this._isUseRandomBaseOverlayXOffset)
				{
					return this._useRandomBaseOverlayXOffset;
				}
				return this.SourceLayer.UseRandomBaseOverlayXOffset;
			}
			set
			{
				if (this.UseRandomBaseOverlayXOffset != value)
				{
					this._isUseRandomBaseOverlayXOffset = this._useRandomBaseOverlayXOffset != value;
					this._useRandomBaseOverlayXOffset = value;
					this._localVersion += 1U;
				}
			}
		}

		[Editor(false)]
		public bool UseRandomBaseOverlayYOffset
		{
			get
			{
				if (this._isUseRandomBaseOverlayYOffset)
				{
					return this._useRandomBaseOverlayYOffset;
				}
				return this.SourceLayer.UseRandomBaseOverlayYOffset;
			}
			set
			{
				if (this.UseRandomBaseOverlayYOffset != value)
				{
					this._isUseRandomBaseOverlayYOffset = this._useRandomBaseOverlayYOffset != value;
					this._useRandomBaseOverlayYOffset = value;
					this._localVersion += 1U;
				}
			}
		}

		public StyleLayer(BrushLayer brushLayer)
		{
			this.SourceLayer = brushLayer;
		}

		public static StyleLayer CreateFrom(StyleLayer source)
		{
			StyleLayer styleLayer = new StyleLayer(source.SourceLayer);
			styleLayer.FillFrom(source);
			return styleLayer;
		}

		public void FillFrom(StyleLayer source)
		{
			this.Sprite = source.Sprite;
			this.Color = source.Color;
			this.ColorFactor = source.ColorFactor;
			this.AlphaFactor = source.AlphaFactor;
			this.HueFactor = source.HueFactor;
			this.SaturationFactor = source.SaturationFactor;
			this.ValueFactor = source.ValueFactor;
			this.IsHidden = source.IsHidden;
			this.XOffset = source.XOffset;
			this.YOffset = source.YOffset;
			this.ExtendLeft = source.ExtendLeft;
			this.ExtendRight = source.ExtendRight;
			this.ExtendTop = source.ExtendTop;
			this.ExtendBottom = source.ExtendBottom;
			this.OverridenWidth = source.OverridenWidth;
			this.OverridenHeight = source.OverridenHeight;
			this.WidthPolicy = source.WidthPolicy;
			this.HeightPolicy = source.HeightPolicy;
			this.HorizontalFlip = source.HorizontalFlip;
			this.VerticalFlip = source.VerticalFlip;
			this.OverlayMethod = source.OverlayMethod;
			this.OverlaySprite = source.OverlaySprite;
			this.OverlayXOffset = source.OverlayXOffset;
			this.OverlayYOffset = source.OverlayYOffset;
			this.UseRandomBaseOverlayXOffset = source.UseRandomBaseOverlayXOffset;
			this.UseRandomBaseOverlayYOffset = source.UseRandomBaseOverlayYOffset;
		}

		public float GetValueAsFloat(BrushAnimationProperty.BrushAnimationPropertyType propertyType)
		{
			switch (propertyType)
			{
			case BrushAnimationProperty.BrushAnimationPropertyType.ColorFactor:
				return this.ColorFactor;
			case BrushAnimationProperty.BrushAnimationPropertyType.Color:
			case BrushAnimationProperty.BrushAnimationPropertyType.FontColor:
				break;
			case BrushAnimationProperty.BrushAnimationPropertyType.AlphaFactor:
				return this.AlphaFactor;
			case BrushAnimationProperty.BrushAnimationPropertyType.HueFactor:
				return this.HueFactor;
			case BrushAnimationProperty.BrushAnimationPropertyType.SaturationFactor:
				return this.SaturationFactor;
			case BrushAnimationProperty.BrushAnimationPropertyType.ValueFactor:
				return this.ValueFactor;
			case BrushAnimationProperty.BrushAnimationPropertyType.OverlayXOffset:
				return this.OverlayXOffset;
			case BrushAnimationProperty.BrushAnimationPropertyType.OverlayYOffset:
				return this.OverlayYOffset;
			default:
				switch (propertyType)
				{
				case BrushAnimationProperty.BrushAnimationPropertyType.XOffset:
					return this.XOffset;
				case BrushAnimationProperty.BrushAnimationPropertyType.YOffset:
					return this.YOffset;
				case BrushAnimationProperty.BrushAnimationPropertyType.OverridenWidth:
					return this.OverridenWidth;
				case BrushAnimationProperty.BrushAnimationPropertyType.OverridenHeight:
					return this.OverridenHeight;
				case BrushAnimationProperty.BrushAnimationPropertyType.ExtendLeft:
					return this.ExtendLeft;
				case BrushAnimationProperty.BrushAnimationPropertyType.ExtendRight:
					return this.ExtendRight;
				case BrushAnimationProperty.BrushAnimationPropertyType.ExtendTop:
					return this.ExtendTop;
				case BrushAnimationProperty.BrushAnimationPropertyType.ExtendBottom:
					return this.ExtendBottom;
				}
				break;
			}
			Debug.FailedAssert("Invalid value type or property name for data source.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\StyleLayer.cs", "GetValueAsFloat", 830);
			return 0f;
		}

		public Color GetValueAsColor(BrushAnimationProperty.BrushAnimationPropertyType propertyType)
		{
			if (propertyType == BrushAnimationProperty.BrushAnimationPropertyType.Color)
			{
				return this.Color;
			}
			Debug.FailedAssert("Invalid value type or property name for data source.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\StyleLayer.cs", "GetValueAsColor", 844);
			return Color.Black;
		}

		public Sprite GetValueAsSprite(BrushAnimationProperty.BrushAnimationPropertyType propertyType)
		{
			if (propertyType == BrushAnimationProperty.BrushAnimationPropertyType.Sprite)
			{
				return this.Sprite;
			}
			if (propertyType != BrushAnimationProperty.BrushAnimationPropertyType.OverlaySprite)
			{
				Debug.FailedAssert("Invalid value type or property name for data source.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\StyleLayer.cs", "GetValueAsSprite", 861);
				return null;
			}
			return this.OverlaySprite;
		}

		public bool GetIsValueChanged(BrushAnimationProperty.BrushAnimationPropertyType propertyType)
		{
			switch (propertyType)
			{
			case BrushAnimationProperty.BrushAnimationPropertyType.Name:
				return false;
			case BrushAnimationProperty.BrushAnimationPropertyType.ColorFactor:
				return this._isColorFactorChanged;
			case BrushAnimationProperty.BrushAnimationPropertyType.Color:
				return this._isSpriteChanged;
			case BrushAnimationProperty.BrushAnimationPropertyType.AlphaFactor:
				return this._isAlphaFactorChanged;
			case BrushAnimationProperty.BrushAnimationPropertyType.HueFactor:
				return this._isHueFactorChanged;
			case BrushAnimationProperty.BrushAnimationPropertyType.SaturationFactor:
				return this._isSaturationFactorChanged;
			case BrushAnimationProperty.BrushAnimationPropertyType.ValueFactor:
				return this._isValueFactorChanged;
			case BrushAnimationProperty.BrushAnimationPropertyType.OverlayXOffset:
				return this._isOverlayXOffsetChanged;
			case BrushAnimationProperty.BrushAnimationPropertyType.OverlayYOffset:
				return this._isOverlayYOffsetChanged;
			case BrushAnimationProperty.BrushAnimationPropertyType.Sprite:
				return this._isSpriteChanged;
			case BrushAnimationProperty.BrushAnimationPropertyType.IsHidden:
				return this._isIsHiddenChanged;
			case BrushAnimationProperty.BrushAnimationPropertyType.XOffset:
				return this._isXOffsetChanged;
			case BrushAnimationProperty.BrushAnimationPropertyType.YOffset:
				return this._isYOffsetChanged;
			case BrushAnimationProperty.BrushAnimationPropertyType.OverridenWidth:
				return this._isOverridenWidthChanged;
			case BrushAnimationProperty.BrushAnimationPropertyType.OverridenHeight:
				return this._isOverridenHeightChanged;
			case BrushAnimationProperty.BrushAnimationPropertyType.WidthPolicy:
				return this._isWidthPolicyChanged;
			case BrushAnimationProperty.BrushAnimationPropertyType.HeightPolicy:
				return this._isHeightPolicyChanged;
			case BrushAnimationProperty.BrushAnimationPropertyType.HorizontalFlip:
				return this._isHorizontalFlipChanged;
			case BrushAnimationProperty.BrushAnimationPropertyType.VerticalFlip:
				return this._isVerticalFlipChanged;
			case BrushAnimationProperty.BrushAnimationPropertyType.OverlayMethod:
				return this._isOverlayMethodChanged;
			case BrushAnimationProperty.BrushAnimationPropertyType.OverlaySprite:
				return this._isOverlaySpriteChanged;
			case BrushAnimationProperty.BrushAnimationPropertyType.ExtendLeft:
				return this._isExtendLeftChanged;
			case BrushAnimationProperty.BrushAnimationPropertyType.ExtendRight:
				return this._isExtendRightChanged;
			case BrushAnimationProperty.BrushAnimationPropertyType.ExtendTop:
				return this._isExtendTopChanged;
			case BrushAnimationProperty.BrushAnimationPropertyType.ExtendBottom:
				return this._isExtendBottomChanged;
			case BrushAnimationProperty.BrushAnimationPropertyType.UseRandomBaseOverlayXOffset:
				return this._isUseRandomBaseOverlayXOffset;
			case BrushAnimationProperty.BrushAnimationPropertyType.UseRandomBaseOverlayYOffset:
				return this._isUseRandomBaseOverlayYOffset;
			}
			return false;
		}

		private uint _localVersion;

		private bool _isSpriteChanged;

		private bool _isColorChanged;

		private bool _isColorFactorChanged;

		private bool _isAlphaFactorChanged;

		private bool _isHueFactorChanged;

		private bool _isSaturationFactorChanged;

		private bool _isValueFactorChanged;

		private bool _isIsHiddenChanged;

		private bool _isXOffsetChanged;

		private bool _isYOffsetChanged;

		private bool _isExtendLeftChanged;

		private bool _isExtendRightChanged;

		private bool _isExtendTopChanged;

		private bool _isExtendBottomChanged;

		private bool _isOverridenWidthChanged;

		private bool _isOverridenHeightChanged;

		private bool _isWidthPolicyChanged;

		private bool _isHeightPolicyChanged;

		private bool _isHorizontalFlipChanged;

		private bool _isVerticalFlipChanged;

		private bool _isOverlayMethodChanged;

		private bool _isOverlaySpriteChanged;

		private bool _isUseOverlayAlphaAsMaskChanged;

		private bool _isOverlayXOffsetChanged;

		private bool _isOverlayYOffsetChanged;

		private bool _isUseRandomBaseOverlayXOffset;

		private bool _isUseRandomBaseOverlayYOffset;

		private Sprite _sprite;

		private Color _color;

		private float _colorFactor;

		private float _alphaFactor;

		private float _hueFactor;

		private float _saturationFactor;

		private float _valueFactor;

		private bool _isHidden;

		private bool _useOverlayAlphaAsMask;

		private float _xOffset;

		private float _yOffset;

		private float _extendLeft;

		private float _extendRight;

		private float _extendTop;

		private float _extendBottom;

		private float _overridenWidth;

		private float _overridenHeight;

		private BrushLayerSizePolicy _widthPolicy;

		private BrushLayerSizePolicy _heightPolicy;

		private bool _horizontalFlip;

		private bool _verticalFlip;

		private BrushOverlayMethod _overlayMethod;

		private Sprite _overlaySprite;

		private float _overlayXOffset;

		private float _overlayYOffset;

		private bool _useRandomBaseOverlayXOffset;

		private bool _useRandomBaseOverlayYOffset;
	}
}
