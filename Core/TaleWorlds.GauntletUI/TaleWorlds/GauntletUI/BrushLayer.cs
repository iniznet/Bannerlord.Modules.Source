using System;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI
{
	public class BrushLayer : IBrushLayerData
	{
		public uint Version { get; private set; }

		[Editor(false)]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					uint version = this.Version;
					this.Version = version + 1U;
				}
			}
		}

		[Editor(false)]
		public Sprite Sprite
		{
			get
			{
				return this._sprite;
			}
			set
			{
				if (value != this._sprite)
				{
					this._sprite = value;
					uint version = this.Version;
					this.Version = version + 1U;
				}
			}
		}

		[Editor(false)]
		public Color Color
		{
			get
			{
				return this._color;
			}
			set
			{
				if (value != this._color)
				{
					this._color = value;
					uint version = this.Version;
					this.Version = version + 1U;
				}
			}
		}

		[Editor(false)]
		public float ColorFactor
		{
			get
			{
				return this._colorFactor;
			}
			set
			{
				if (value != this._colorFactor)
				{
					this._colorFactor = value;
					uint version = this.Version;
					this.Version = version + 1U;
				}
			}
		}

		[Editor(false)]
		public float AlphaFactor
		{
			get
			{
				return this._alphaFactor;
			}
			set
			{
				if (value != this._alphaFactor)
				{
					this._alphaFactor = value;
					uint version = this.Version;
					this.Version = version + 1U;
				}
			}
		}

		[Editor(false)]
		public float HueFactor
		{
			get
			{
				return this._hueFactor;
			}
			set
			{
				if (value != this._hueFactor)
				{
					this._hueFactor = value;
					uint version = this.Version;
					this.Version = version + 1U;
				}
			}
		}

		[Editor(false)]
		public float SaturationFactor
		{
			get
			{
				return this._saturationFactor;
			}
			set
			{
				if (value != this._saturationFactor)
				{
					this._saturationFactor = value;
					uint version = this.Version;
					this.Version = version + 1U;
				}
			}
		}

		[Editor(false)]
		public float ValueFactor
		{
			get
			{
				return this._valueFactor;
			}
			set
			{
				if (value != this._valueFactor)
				{
					this._valueFactor = value;
					uint version = this.Version;
					this.Version = version + 1U;
				}
			}
		}

		[Editor(false)]
		public bool IsHidden
		{
			get
			{
				return this._isHidden;
			}
			set
			{
				if (value != this._isHidden)
				{
					this._isHidden = value;
					uint version = this.Version;
					this.Version = version + 1U;
				}
			}
		}

		[Editor(false)]
		public bool UseOverlayAlphaAsMask
		{
			get
			{
				return this._useOverlayAlphaAsMask;
			}
			set
			{
				if (value != this._useOverlayAlphaAsMask)
				{
					this._useOverlayAlphaAsMask = value;
					uint version = this.Version;
					this.Version = version + 1U;
				}
			}
		}

		[Editor(false)]
		public float XOffset
		{
			get
			{
				return this._xOffset;
			}
			set
			{
				if (value != this._xOffset)
				{
					this._xOffset = value;
					uint version = this.Version;
					this.Version = version + 1U;
				}
			}
		}

		[Editor(false)]
		public float YOffset
		{
			get
			{
				return this._yOffset;
			}
			set
			{
				if (value != this._yOffset)
				{
					this._yOffset = value;
					uint version = this.Version;
					this.Version = version + 1U;
				}
			}
		}

		[Editor(false)]
		public float ExtendLeft
		{
			get
			{
				return this._extendLeft;
			}
			set
			{
				if (value != this._extendLeft)
				{
					this._extendLeft = value;
					uint version = this.Version;
					this.Version = version + 1U;
				}
			}
		}

		[Editor(false)]
		public float ExtendRight
		{
			get
			{
				return this._extendRight;
			}
			set
			{
				if (value != this._extendRight)
				{
					this._extendRight = value;
					uint version = this.Version;
					this.Version = version + 1U;
				}
			}
		}

		[Editor(false)]
		public float ExtendTop
		{
			get
			{
				return this._extendTop;
			}
			set
			{
				if (value != this._extendTop)
				{
					this._extendTop = value;
					uint version = this.Version;
					this.Version = version + 1U;
				}
			}
		}

		[Editor(false)]
		public float ExtendBottom
		{
			get
			{
				return this._extendBottom;
			}
			set
			{
				if (value != this._extendBottom)
				{
					this._extendBottom = value;
					uint version = this.Version;
					this.Version = version + 1U;
				}
			}
		}

		[Editor(false)]
		public float OverridenWidth
		{
			get
			{
				return this._overridenWidth;
			}
			set
			{
				if (value != this._overridenWidth)
				{
					this._overridenWidth = value;
					uint version = this.Version;
					this.Version = version + 1U;
				}
			}
		}

		[Editor(false)]
		public float OverridenHeight
		{
			get
			{
				return this._overridenHeight;
			}
			set
			{
				if (value != this._overridenHeight)
				{
					this._overridenHeight = value;
					uint version = this.Version;
					this.Version = version + 1U;
				}
			}
		}

		[Editor(false)]
		public BrushLayerSizePolicy WidthPolicy
		{
			get
			{
				return this._widthPolicy;
			}
			set
			{
				if (value != this._widthPolicy)
				{
					this._widthPolicy = value;
					uint version = this.Version;
					this.Version = version + 1U;
				}
			}
		}

		[Editor(false)]
		public BrushLayerSizePolicy HeightPolicy
		{
			get
			{
				return this._heightPolicy;
			}
			set
			{
				if (value != this._heightPolicy)
				{
					this._heightPolicy = value;
					uint version = this.Version;
					this.Version = version + 1U;
				}
			}
		}

		[Editor(false)]
		public bool HorizontalFlip
		{
			get
			{
				return this._horizontalFlip;
			}
			set
			{
				if (value != this._horizontalFlip)
				{
					this._horizontalFlip = value;
					uint version = this.Version;
					this.Version = version + 1U;
				}
			}
		}

		[Editor(false)]
		public bool VerticalFlip
		{
			get
			{
				return this._verticalFlip;
			}
			set
			{
				if (value != this._verticalFlip)
				{
					this._verticalFlip = value;
					uint version = this.Version;
					this.Version = version + 1U;
				}
			}
		}

		[Editor(false)]
		public BrushOverlayMethod OverlayMethod
		{
			get
			{
				return this._overlayMethod;
			}
			set
			{
				if (value != this._overlayMethod)
				{
					this._overlayMethod = value;
					uint version = this.Version;
					this.Version = version + 1U;
				}
			}
		}

		[Editor(false)]
		public Sprite OverlaySprite
		{
			get
			{
				return this._overlaySprite;
			}
			set
			{
				this._overlaySprite = value;
				uint version = this.Version;
				this.Version = version + 1U;
				if (this._overlaySprite != null)
				{
					if (this.OverlayMethod == BrushOverlayMethod.None)
					{
						this.OverlayMethod = BrushOverlayMethod.CoverWithTexture;
						return;
					}
				}
				else
				{
					this.OverlayMethod = BrushOverlayMethod.None;
				}
			}
		}

		[Editor(false)]
		public float OverlayXOffset
		{
			get
			{
				return this._overlayXOffset;
			}
			set
			{
				if (value != this._overlayXOffset)
				{
					this._overlayXOffset = value;
					uint version = this.Version;
					this.Version = version + 1U;
				}
			}
		}

		[Editor(false)]
		public float OverlayYOffset
		{
			get
			{
				return this._overlayYOffset;
			}
			set
			{
				if (value != this._overlayYOffset)
				{
					this._overlayYOffset = value;
					uint version = this.Version;
					this.Version = version + 1U;
				}
			}
		}

		[Editor(false)]
		public bool UseRandomBaseOverlayXOffset
		{
			get
			{
				return this._useRandomBaseOverlayXOffset;
			}
			set
			{
				if (value != this._useRandomBaseOverlayXOffset)
				{
					this._useRandomBaseOverlayXOffset = value;
					uint version = this.Version;
					this.Version = version + 1U;
				}
			}
		}

		[Editor(false)]
		public bool UseRandomBaseOverlayYOffset
		{
			get
			{
				return this._useRandomBaseOverlayYOffset;
			}
			set
			{
				if (value != this._useRandomBaseOverlayYOffset)
				{
					this._useRandomBaseOverlayYOffset = value;
					uint version = this.Version;
					this.Version = version + 1U;
				}
			}
		}

		public BrushLayer()
		{
			this.Color = new Color(1f, 1f, 1f, 1f);
			this.ColorFactor = 1f;
			this.AlphaFactor = 1f;
			this.HueFactor = 0f;
			this.SaturationFactor = 0f;
			this.ValueFactor = 0f;
			this.XOffset = 0f;
			this.YOffset = 0f;
			this.IsHidden = false;
			this.WidthPolicy = BrushLayerSizePolicy.StretchToTarget;
			this.HeightPolicy = BrushLayerSizePolicy.StretchToTarget;
			this.HorizontalFlip = false;
			this.VerticalFlip = false;
			this.OverlayMethod = BrushOverlayMethod.None;
			this.ExtendLeft = 0f;
			this.ExtendRight = 0f;
			this.ExtendTop = 0f;
			this.ExtendBottom = 0f;
			this.OverlayXOffset = 0f;
			this.OverlayYOffset = 0f;
			this.UseRandomBaseOverlayXOffset = false;
			this.UseRandomBaseOverlayYOffset = false;
			this.UseOverlayAlphaAsMask = false;
		}

		public void FillFrom(BrushLayer brushLayer)
		{
			this.Sprite = brushLayer.Sprite;
			this.Color = brushLayer.Color;
			this.ColorFactor = brushLayer.ColorFactor;
			this.AlphaFactor = brushLayer.AlphaFactor;
			this.HueFactor = brushLayer.HueFactor;
			this.SaturationFactor = brushLayer.SaturationFactor;
			this.ValueFactor = brushLayer.ValueFactor;
			this.XOffset = brushLayer.XOffset;
			this.YOffset = brushLayer.YOffset;
			this.Name = brushLayer.Name;
			this.IsHidden = brushLayer.IsHidden;
			this.WidthPolicy = brushLayer.WidthPolicy;
			this.HeightPolicy = brushLayer.HeightPolicy;
			this.OverridenWidth = brushLayer.OverridenWidth;
			this.OverridenHeight = brushLayer.OverridenHeight;
			this.HorizontalFlip = brushLayer.HorizontalFlip;
			this.VerticalFlip = brushLayer.VerticalFlip;
			this.OverlayMethod = brushLayer.OverlayMethod;
			this.OverlaySprite = brushLayer.OverlaySprite;
			this.ExtendLeft = brushLayer.ExtendLeft;
			this.ExtendRight = brushLayer.ExtendRight;
			this.ExtendTop = brushLayer.ExtendTop;
			this.ExtendBottom = brushLayer.ExtendBottom;
			this.OverlayXOffset = brushLayer.OverlayXOffset;
			this.OverlayYOffset = brushLayer.OverlayYOffset;
			this.UseRandomBaseOverlayXOffset = brushLayer.UseRandomBaseOverlayXOffset;
			this.UseRandomBaseOverlayYOffset = brushLayer.UseRandomBaseOverlayYOffset;
			this.UseOverlayAlphaAsMask = brushLayer.UseOverlayAlphaAsMask;
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
			Debug.FailedAssert("Invalid value type or property name for data source.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\BrushLayer.cs", "GetValueAsFloat", 669);
			return 0f;
		}

		public Color GetValueAsColor(BrushAnimationProperty.BrushAnimationPropertyType propertyType)
		{
			if (propertyType == BrushAnimationProperty.BrushAnimationPropertyType.Color)
			{
				return this.Color;
			}
			Debug.FailedAssert("Invalid value type or property name for data source.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\BrushLayer.cs", "GetValueAsColor", 683);
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
				Debug.FailedAssert("Invalid value type or property name for data source.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\BrushLayer.cs", "GetValueAsSprite", 700);
				return null;
			}
			return this.OverlaySprite;
		}

		public override string ToString()
		{
			if (!string.IsNullOrEmpty(this.Name))
			{
				return this.Name;
			}
			return base.ToString();
		}

		private string _name;

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
