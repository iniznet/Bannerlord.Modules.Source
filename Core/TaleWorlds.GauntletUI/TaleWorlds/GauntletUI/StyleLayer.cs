using System;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI
{
	// Token: 0x02000017 RID: 23
	public class StyleLayer : IBrushLayerData, IDataSource
	{
		// Token: 0x17000079 RID: 121
		// (get) Token: 0x06000182 RID: 386 RVA: 0x00009A20 File Offset: 0x00007C20
		// (set) Token: 0x06000183 RID: 387 RVA: 0x00009A28 File Offset: 0x00007C28
		public BrushLayer SourceLayer { get; private set; }

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000184 RID: 388 RVA: 0x00009A31 File Offset: 0x00007C31
		public uint Version
		{
			get
			{
				return this._localVersion + this.SourceLayer.Version;
			}
		}

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06000185 RID: 389 RVA: 0x00009A45 File Offset: 0x00007C45
		// (set) Token: 0x06000186 RID: 390 RVA: 0x00009A52 File Offset: 0x00007C52
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

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000187 RID: 391 RVA: 0x00009A54 File Offset: 0x00007C54
		// (set) Token: 0x06000188 RID: 392 RVA: 0x00009A70 File Offset: 0x00007C70
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

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000189 RID: 393 RVA: 0x00009AA7 File Offset: 0x00007CA7
		// (set) Token: 0x0600018A RID: 394 RVA: 0x00009AC3 File Offset: 0x00007CC3
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

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x0600018B RID: 395 RVA: 0x00009AFF File Offset: 0x00007CFF
		// (set) Token: 0x0600018C RID: 396 RVA: 0x00009B1B File Offset: 0x00007D1B
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

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x0600018D RID: 397 RVA: 0x00009B5A File Offset: 0x00007D5A
		// (set) Token: 0x0600018E RID: 398 RVA: 0x00009B76 File Offset: 0x00007D76
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

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x0600018F RID: 399 RVA: 0x00009BB5 File Offset: 0x00007DB5
		// (set) Token: 0x06000190 RID: 400 RVA: 0x00009BD1 File Offset: 0x00007DD1
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

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x06000191 RID: 401 RVA: 0x00009C10 File Offset: 0x00007E10
		// (set) Token: 0x06000192 RID: 402 RVA: 0x00009C2C File Offset: 0x00007E2C
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

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x06000193 RID: 403 RVA: 0x00009C6B File Offset: 0x00007E6B
		// (set) Token: 0x06000194 RID: 404 RVA: 0x00009C87 File Offset: 0x00007E87
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

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000195 RID: 405 RVA: 0x00009CC6 File Offset: 0x00007EC6
		// (set) Token: 0x06000196 RID: 406 RVA: 0x00009CE2 File Offset: 0x00007EE2
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

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000197 RID: 407 RVA: 0x00009D19 File Offset: 0x00007F19
		// (set) Token: 0x06000198 RID: 408 RVA: 0x00009D35 File Offset: 0x00007F35
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

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000199 RID: 409 RVA: 0x00009D6C File Offset: 0x00007F6C
		// (set) Token: 0x0600019A RID: 410 RVA: 0x00009D88 File Offset: 0x00007F88
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

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x0600019B RID: 411 RVA: 0x00009DC7 File Offset: 0x00007FC7
		// (set) Token: 0x0600019C RID: 412 RVA: 0x00009DE3 File Offset: 0x00007FE3
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

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x0600019D RID: 413 RVA: 0x00009E22 File Offset: 0x00008022
		// (set) Token: 0x0600019E RID: 414 RVA: 0x00009E3E File Offset: 0x0000803E
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

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x0600019F RID: 415 RVA: 0x00009E7D File Offset: 0x0000807D
		// (set) Token: 0x060001A0 RID: 416 RVA: 0x00009E99 File Offset: 0x00008099
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

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x060001A1 RID: 417 RVA: 0x00009ED8 File Offset: 0x000080D8
		// (set) Token: 0x060001A2 RID: 418 RVA: 0x00009EF4 File Offset: 0x000080F4
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

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x060001A3 RID: 419 RVA: 0x00009F33 File Offset: 0x00008133
		// (set) Token: 0x060001A4 RID: 420 RVA: 0x00009F4F File Offset: 0x0000814F
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

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x060001A5 RID: 421 RVA: 0x00009F8E File Offset: 0x0000818E
		// (set) Token: 0x060001A6 RID: 422 RVA: 0x00009FAA File Offset: 0x000081AA
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

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x060001A7 RID: 423 RVA: 0x00009FE9 File Offset: 0x000081E9
		// (set) Token: 0x060001A8 RID: 424 RVA: 0x0000A005 File Offset: 0x00008205
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

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x060001A9 RID: 425 RVA: 0x0000A044 File Offset: 0x00008244
		// (set) Token: 0x060001AA RID: 426 RVA: 0x0000A060 File Offset: 0x00008260
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

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x060001AB RID: 427 RVA: 0x0000A097 File Offset: 0x00008297
		// (set) Token: 0x060001AC RID: 428 RVA: 0x0000A0B3 File Offset: 0x000082B3
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

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x060001AD RID: 429 RVA: 0x0000A0EA File Offset: 0x000082EA
		// (set) Token: 0x060001AE RID: 430 RVA: 0x0000A106 File Offset: 0x00008306
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

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x060001AF RID: 431 RVA: 0x0000A13D File Offset: 0x0000833D
		// (set) Token: 0x060001B0 RID: 432 RVA: 0x0000A159 File Offset: 0x00008359
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

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x060001B1 RID: 433 RVA: 0x0000A190 File Offset: 0x00008390
		// (set) Token: 0x060001B2 RID: 434 RVA: 0x0000A1AC File Offset: 0x000083AC
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

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x060001B3 RID: 435 RVA: 0x0000A1E3 File Offset: 0x000083E3
		// (set) Token: 0x060001B4 RID: 436 RVA: 0x0000A1FF File Offset: 0x000083FF
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

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x060001B5 RID: 437 RVA: 0x0000A236 File Offset: 0x00008436
		// (set) Token: 0x060001B6 RID: 438 RVA: 0x0000A252 File Offset: 0x00008452
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

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x060001B7 RID: 439 RVA: 0x0000A291 File Offset: 0x00008491
		// (set) Token: 0x060001B8 RID: 440 RVA: 0x0000A2AD File Offset: 0x000084AD
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

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x060001B9 RID: 441 RVA: 0x0000A2EC File Offset: 0x000084EC
		// (set) Token: 0x060001BA RID: 442 RVA: 0x0000A308 File Offset: 0x00008508
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

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x060001BB RID: 443 RVA: 0x0000A33A File Offset: 0x0000853A
		// (set) Token: 0x060001BC RID: 444 RVA: 0x0000A356 File Offset: 0x00008556
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

		// Token: 0x060001BD RID: 445 RVA: 0x0000A388 File Offset: 0x00008588
		public StyleLayer(BrushLayer brushLayer)
		{
			this.SourceLayer = brushLayer;
		}

		// Token: 0x060001BE RID: 446 RVA: 0x0000A397 File Offset: 0x00008597
		public static StyleLayer CreateFrom(StyleLayer source)
		{
			StyleLayer styleLayer = new StyleLayer(source.SourceLayer);
			styleLayer.FillFrom(source);
			return styleLayer;
		}

		// Token: 0x060001BF RID: 447 RVA: 0x0000A3AC File Offset: 0x000085AC
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

		// Token: 0x060001C0 RID: 448 RVA: 0x0000A4F4 File Offset: 0x000086F4
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

		// Token: 0x060001C1 RID: 449 RVA: 0x0000A5F7 File Offset: 0x000087F7
		public Color GetValueAsColor(BrushAnimationProperty.BrushAnimationPropertyType propertyType)
		{
			if (propertyType == BrushAnimationProperty.BrushAnimationPropertyType.Color)
			{
				return this.Color;
			}
			Debug.FailedAssert("Invalid value type or property name for data source.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\StyleLayer.cs", "GetValueAsColor", 844);
			return Color.Black;
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x0000A622 File Offset: 0x00008822
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

		// Token: 0x060001C3 RID: 451 RVA: 0x0000A658 File Offset: 0x00008858
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

		// Token: 0x040000BF RID: 191
		private uint _localVersion;

		// Token: 0x040000C0 RID: 192
		private bool _isSpriteChanged;

		// Token: 0x040000C1 RID: 193
		private bool _isColorChanged;

		// Token: 0x040000C2 RID: 194
		private bool _isColorFactorChanged;

		// Token: 0x040000C3 RID: 195
		private bool _isAlphaFactorChanged;

		// Token: 0x040000C4 RID: 196
		private bool _isHueFactorChanged;

		// Token: 0x040000C5 RID: 197
		private bool _isSaturationFactorChanged;

		// Token: 0x040000C6 RID: 198
		private bool _isValueFactorChanged;

		// Token: 0x040000C7 RID: 199
		private bool _isIsHiddenChanged;

		// Token: 0x040000C8 RID: 200
		private bool _isXOffsetChanged;

		// Token: 0x040000C9 RID: 201
		private bool _isYOffsetChanged;

		// Token: 0x040000CA RID: 202
		private bool _isExtendLeftChanged;

		// Token: 0x040000CB RID: 203
		private bool _isExtendRightChanged;

		// Token: 0x040000CC RID: 204
		private bool _isExtendTopChanged;

		// Token: 0x040000CD RID: 205
		private bool _isExtendBottomChanged;

		// Token: 0x040000CE RID: 206
		private bool _isOverridenWidthChanged;

		// Token: 0x040000CF RID: 207
		private bool _isOverridenHeightChanged;

		// Token: 0x040000D0 RID: 208
		private bool _isWidthPolicyChanged;

		// Token: 0x040000D1 RID: 209
		private bool _isHeightPolicyChanged;

		// Token: 0x040000D2 RID: 210
		private bool _isHorizontalFlipChanged;

		// Token: 0x040000D3 RID: 211
		private bool _isVerticalFlipChanged;

		// Token: 0x040000D4 RID: 212
		private bool _isOverlayMethodChanged;

		// Token: 0x040000D5 RID: 213
		private bool _isOverlaySpriteChanged;

		// Token: 0x040000D6 RID: 214
		private bool _isUseOverlayAlphaAsMaskChanged;

		// Token: 0x040000D7 RID: 215
		private bool _isOverlayXOffsetChanged;

		// Token: 0x040000D8 RID: 216
		private bool _isOverlayYOffsetChanged;

		// Token: 0x040000D9 RID: 217
		private bool _isUseRandomBaseOverlayXOffset;

		// Token: 0x040000DA RID: 218
		private bool _isUseRandomBaseOverlayYOffset;

		// Token: 0x040000DB RID: 219
		private Sprite _sprite;

		// Token: 0x040000DC RID: 220
		private Color _color;

		// Token: 0x040000DD RID: 221
		private float _colorFactor;

		// Token: 0x040000DE RID: 222
		private float _alphaFactor;

		// Token: 0x040000DF RID: 223
		private float _hueFactor;

		// Token: 0x040000E0 RID: 224
		private float _saturationFactor;

		// Token: 0x040000E1 RID: 225
		private float _valueFactor;

		// Token: 0x040000E2 RID: 226
		private bool _isHidden;

		// Token: 0x040000E3 RID: 227
		private bool _useOverlayAlphaAsMask;

		// Token: 0x040000E4 RID: 228
		private float _xOffset;

		// Token: 0x040000E5 RID: 229
		private float _yOffset;

		// Token: 0x040000E6 RID: 230
		private float _extendLeft;

		// Token: 0x040000E7 RID: 231
		private float _extendRight;

		// Token: 0x040000E8 RID: 232
		private float _extendTop;

		// Token: 0x040000E9 RID: 233
		private float _extendBottom;

		// Token: 0x040000EA RID: 234
		private float _overridenWidth;

		// Token: 0x040000EB RID: 235
		private float _overridenHeight;

		// Token: 0x040000EC RID: 236
		private BrushLayerSizePolicy _widthPolicy;

		// Token: 0x040000ED RID: 237
		private BrushLayerSizePolicy _heightPolicy;

		// Token: 0x040000EE RID: 238
		private bool _horizontalFlip;

		// Token: 0x040000EF RID: 239
		private bool _verticalFlip;

		// Token: 0x040000F0 RID: 240
		private BrushOverlayMethod _overlayMethod;

		// Token: 0x040000F1 RID: 241
		private Sprite _overlaySprite;

		// Token: 0x040000F2 RID: 242
		private float _overlayXOffset;

		// Token: 0x040000F3 RID: 243
		private float _overlayYOffset;

		// Token: 0x040000F4 RID: 244
		private bool _useRandomBaseOverlayXOffset;

		// Token: 0x040000F5 RID: 245
		private bool _useRandomBaseOverlayYOffset;
	}
}
