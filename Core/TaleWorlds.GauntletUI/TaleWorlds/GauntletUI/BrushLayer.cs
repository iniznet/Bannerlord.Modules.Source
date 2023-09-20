using System;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI
{
	// Token: 0x0200000E RID: 14
	public class BrushLayer : IBrushLayerData
	{
		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060000B8 RID: 184 RVA: 0x00005F41 File Offset: 0x00004141
		// (set) Token: 0x060000B9 RID: 185 RVA: 0x00005F49 File Offset: 0x00004149
		public uint Version { get; private set; }

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060000BA RID: 186 RVA: 0x00005F52 File Offset: 0x00004152
		// (set) Token: 0x060000BB RID: 187 RVA: 0x00005F5C File Offset: 0x0000415C
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

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060000BC RID: 188 RVA: 0x00005F8E File Offset: 0x0000418E
		// (set) Token: 0x060000BD RID: 189 RVA: 0x00005F98 File Offset: 0x00004198
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

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060000BE RID: 190 RVA: 0x00005FC5 File Offset: 0x000041C5
		// (set) Token: 0x060000BF RID: 191 RVA: 0x00005FD0 File Offset: 0x000041D0
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

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060000C0 RID: 192 RVA: 0x00006002 File Offset: 0x00004202
		// (set) Token: 0x060000C1 RID: 193 RVA: 0x0000600C File Offset: 0x0000420C
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

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060000C2 RID: 194 RVA: 0x00006039 File Offset: 0x00004239
		// (set) Token: 0x060000C3 RID: 195 RVA: 0x00006044 File Offset: 0x00004244
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

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060000C4 RID: 196 RVA: 0x00006071 File Offset: 0x00004271
		// (set) Token: 0x060000C5 RID: 197 RVA: 0x0000607C File Offset: 0x0000427C
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

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060000C6 RID: 198 RVA: 0x000060A9 File Offset: 0x000042A9
		// (set) Token: 0x060000C7 RID: 199 RVA: 0x000060B4 File Offset: 0x000042B4
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

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060000C8 RID: 200 RVA: 0x000060E1 File Offset: 0x000042E1
		// (set) Token: 0x060000C9 RID: 201 RVA: 0x000060EC File Offset: 0x000042EC
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

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060000CA RID: 202 RVA: 0x00006119 File Offset: 0x00004319
		// (set) Token: 0x060000CB RID: 203 RVA: 0x00006124 File Offset: 0x00004324
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

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060000CC RID: 204 RVA: 0x00006151 File Offset: 0x00004351
		// (set) Token: 0x060000CD RID: 205 RVA: 0x0000615C File Offset: 0x0000435C
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

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060000CE RID: 206 RVA: 0x00006189 File Offset: 0x00004389
		// (set) Token: 0x060000CF RID: 207 RVA: 0x00006194 File Offset: 0x00004394
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

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060000D0 RID: 208 RVA: 0x000061C1 File Offset: 0x000043C1
		// (set) Token: 0x060000D1 RID: 209 RVA: 0x000061CC File Offset: 0x000043CC
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

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060000D2 RID: 210 RVA: 0x000061F9 File Offset: 0x000043F9
		// (set) Token: 0x060000D3 RID: 211 RVA: 0x00006204 File Offset: 0x00004404
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

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060000D4 RID: 212 RVA: 0x00006231 File Offset: 0x00004431
		// (set) Token: 0x060000D5 RID: 213 RVA: 0x0000623C File Offset: 0x0000443C
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

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060000D6 RID: 214 RVA: 0x00006269 File Offset: 0x00004469
		// (set) Token: 0x060000D7 RID: 215 RVA: 0x00006274 File Offset: 0x00004474
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

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060000D8 RID: 216 RVA: 0x000062A1 File Offset: 0x000044A1
		// (set) Token: 0x060000D9 RID: 217 RVA: 0x000062AC File Offset: 0x000044AC
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

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060000DA RID: 218 RVA: 0x000062D9 File Offset: 0x000044D9
		// (set) Token: 0x060000DB RID: 219 RVA: 0x000062E4 File Offset: 0x000044E4
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

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060000DC RID: 220 RVA: 0x00006311 File Offset: 0x00004511
		// (set) Token: 0x060000DD RID: 221 RVA: 0x0000631C File Offset: 0x0000451C
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

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060000DE RID: 222 RVA: 0x00006349 File Offset: 0x00004549
		// (set) Token: 0x060000DF RID: 223 RVA: 0x00006354 File Offset: 0x00004554
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

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060000E0 RID: 224 RVA: 0x00006381 File Offset: 0x00004581
		// (set) Token: 0x060000E1 RID: 225 RVA: 0x0000638C File Offset: 0x0000458C
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

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060000E2 RID: 226 RVA: 0x000063B9 File Offset: 0x000045B9
		// (set) Token: 0x060000E3 RID: 227 RVA: 0x000063C4 File Offset: 0x000045C4
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

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060000E4 RID: 228 RVA: 0x000063F1 File Offset: 0x000045F1
		// (set) Token: 0x060000E5 RID: 229 RVA: 0x000063FC File Offset: 0x000045FC
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

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060000E6 RID: 230 RVA: 0x00006429 File Offset: 0x00004629
		// (set) Token: 0x060000E7 RID: 231 RVA: 0x00006434 File Offset: 0x00004634
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

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060000E8 RID: 232 RVA: 0x00006461 File Offset: 0x00004661
		// (set) Token: 0x060000E9 RID: 233 RVA: 0x0000646C File Offset: 0x0000466C
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

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060000EA RID: 234 RVA: 0x000064AF File Offset: 0x000046AF
		// (set) Token: 0x060000EB RID: 235 RVA: 0x000064B8 File Offset: 0x000046B8
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

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x060000EC RID: 236 RVA: 0x000064E5 File Offset: 0x000046E5
		// (set) Token: 0x060000ED RID: 237 RVA: 0x000064F0 File Offset: 0x000046F0
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

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x060000EE RID: 238 RVA: 0x0000651D File Offset: 0x0000471D
		// (set) Token: 0x060000EF RID: 239 RVA: 0x00006528 File Offset: 0x00004728
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

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060000F0 RID: 240 RVA: 0x00006555 File Offset: 0x00004755
		// (set) Token: 0x060000F1 RID: 241 RVA: 0x00006560 File Offset: 0x00004760
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

		// Token: 0x060000F2 RID: 242 RVA: 0x00006590 File Offset: 0x00004790
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

		// Token: 0x060000F3 RID: 243 RVA: 0x00006690 File Offset: 0x00004890
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

		// Token: 0x060000F4 RID: 244 RVA: 0x000067F0 File Offset: 0x000049F0
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

		// Token: 0x060000F5 RID: 245 RVA: 0x000068F3 File Offset: 0x00004AF3
		public Color GetValueAsColor(BrushAnimationProperty.BrushAnimationPropertyType propertyType)
		{
			if (propertyType == BrushAnimationProperty.BrushAnimationPropertyType.Color)
			{
				return this.Color;
			}
			Debug.FailedAssert("Invalid value type or property name for data source.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\BrushLayer.cs", "GetValueAsColor", 683);
			return Color.Black;
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x0000691E File Offset: 0x00004B1E
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

		// Token: 0x060000F7 RID: 247 RVA: 0x00006954 File Offset: 0x00004B54
		public override string ToString()
		{
			if (!string.IsNullOrEmpty(this.Name))
			{
				return this.Name;
			}
			return base.ToString();
		}

		// Token: 0x04000048 RID: 72
		private string _name;

		// Token: 0x04000049 RID: 73
		private Sprite _sprite;

		// Token: 0x0400004A RID: 74
		private Color _color;

		// Token: 0x0400004B RID: 75
		private float _colorFactor;

		// Token: 0x0400004C RID: 76
		private float _alphaFactor;

		// Token: 0x0400004D RID: 77
		private float _hueFactor;

		// Token: 0x0400004E RID: 78
		private float _saturationFactor;

		// Token: 0x0400004F RID: 79
		private float _valueFactor;

		// Token: 0x04000050 RID: 80
		private bool _isHidden;

		// Token: 0x04000051 RID: 81
		private bool _useOverlayAlphaAsMask;

		// Token: 0x04000052 RID: 82
		private float _xOffset;

		// Token: 0x04000053 RID: 83
		private float _yOffset;

		// Token: 0x04000054 RID: 84
		private float _extendLeft;

		// Token: 0x04000055 RID: 85
		private float _extendRight;

		// Token: 0x04000056 RID: 86
		private float _extendTop;

		// Token: 0x04000057 RID: 87
		private float _extendBottom;

		// Token: 0x04000058 RID: 88
		private float _overridenWidth;

		// Token: 0x04000059 RID: 89
		private float _overridenHeight;

		// Token: 0x0400005A RID: 90
		private BrushLayerSizePolicy _widthPolicy;

		// Token: 0x0400005B RID: 91
		private BrushLayerSizePolicy _heightPolicy;

		// Token: 0x0400005C RID: 92
		private bool _horizontalFlip;

		// Token: 0x0400005D RID: 93
		private bool _verticalFlip;

		// Token: 0x0400005E RID: 94
		private BrushOverlayMethod _overlayMethod;

		// Token: 0x0400005F RID: 95
		private Sprite _overlaySprite;

		// Token: 0x04000060 RID: 96
		private float _overlayXOffset;

		// Token: 0x04000061 RID: 97
		private float _overlayYOffset;

		// Token: 0x04000062 RID: 98
		private bool _useRandomBaseOverlayXOffset;

		// Token: 0x04000063 RID: 99
		private bool _useRandomBaseOverlayYOffset;
	}
}
