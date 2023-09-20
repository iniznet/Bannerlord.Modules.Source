using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x0200000B RID: 11
	public class CharacterTableauWidget : TextureWidget
	{
		// Token: 0x06000039 RID: 57 RVA: 0x000027BD File Offset: 0x000009BD
		public CharacterTableauWidget(UIContext context)
			: base(context)
		{
			base.TextureProviderName = "CharacterTableauTextureProvider";
		}

		// Token: 0x0600003A RID: 58 RVA: 0x000027D1 File Offset: 0x000009D1
		protected override void OnMousePressed()
		{
			base.SetTextureProviderProperty("CurrentlyRotating", true);
		}

		// Token: 0x0600003B RID: 59 RVA: 0x000027E4 File Offset: 0x000009E4
		protected override void OnMouseReleased()
		{
			base.SetTextureProviderProperty("CurrentlyRotating", false);
		}

		// Token: 0x0600003C RID: 60 RVA: 0x000027F7 File Offset: 0x000009F7
		private void OnSwapClick(Widget obj)
		{
			this._isCharacterMountSwapped = !this._isCharacterMountSwapped;
			base.SetTextureProviderProperty("TriggerCharacterMountPlacesSwap", this._isCharacterMountSwapped);
		}

		// Token: 0x0600003D RID: 61 RVA: 0x0000281E File Offset: 0x00000A1E
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00002828 File Offset: 0x00000A28
		protected override void OnRender(TwoDimensionContext twoDimensionContext, TwoDimensionDrawContext drawContext)
		{
			this._isRenderRequestedPreviousFrame = true;
			if (base.TextureProvider != null)
			{
				base.Texture = base.TextureProvider.GetTexture(twoDimensionContext, string.Empty);
				SimpleMaterial simpleMaterial = drawContext.CreateSimpleMaterial();
				Brush readOnlyBrush = base.ReadOnlyBrush;
				StyleLayer styleLayer;
				if (readOnlyBrush == null)
				{
					styleLayer = null;
				}
				else
				{
					List<StyleLayer> layers = readOnlyBrush.GetStyleOrDefault(base.CurrentState).Layers;
					styleLayer = ((layers != null) ? layers.FirstOrDefault<StyleLayer>() : null);
				}
				StyleLayer styleLayer2 = styleLayer ?? null;
				simpleMaterial.OverlayEnabled = false;
				simpleMaterial.CircularMaskingEnabled = false;
				simpleMaterial.Texture = base.Texture;
				simpleMaterial.AlphaFactor = ((styleLayer2 != null) ? styleLayer2.AlphaFactor : 1f) * base.ReadOnlyBrush.GlobalAlphaFactor * base.Context.ContextAlpha;
				simpleMaterial.ColorFactor = ((styleLayer2 != null) ? styleLayer2.ColorFactor : 1f) * base.ReadOnlyBrush.GlobalColorFactor;
				simpleMaterial.HueFactor = ((styleLayer2 != null) ? styleLayer2.HueFactor : 0f);
				simpleMaterial.SaturationFactor = ((styleLayer2 != null) ? styleLayer2.SaturationFactor : 0f);
				simpleMaterial.ValueFactor = ((styleLayer2 != null) ? styleLayer2.ValueFactor : 0f);
				simpleMaterial.Color = ((styleLayer2 != null) ? styleLayer2.Color : Color.White) * base.ReadOnlyBrush.GlobalColor;
				Vector2 globalPosition = base.GlobalPosition;
				float x = globalPosition.X;
				float y = globalPosition.Y;
				Vector2 size = base.Size;
				Vector2 size2 = base.Size;
				DrawObject2D drawObject2D = null;
				if (this._cachedQuad != null && this._cachedQuadSize == base.Size)
				{
					drawObject2D = this._cachedQuad;
				}
				if (drawObject2D == null)
				{
					drawObject2D = DrawObject2D.CreateQuad(base.Size);
					this._cachedQuad = drawObject2D;
					this._cachedQuadSize = base.Size;
				}
				if (drawContext.CircularMaskEnabled)
				{
					simpleMaterial.CircularMaskingEnabled = true;
					simpleMaterial.CircularMaskingCenter = drawContext.CircularMaskCenter;
					simpleMaterial.CircularMaskingRadius = drawContext.CircularMaskRadius;
					simpleMaterial.CircularMaskingSmoothingRadius = drawContext.CircularMaskSmoothingRadius;
				}
				drawContext.Draw(x, y, simpleMaterial, drawObject2D, base.Size.X, base.Size.Y);
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600003F RID: 63 RVA: 0x00002A26 File Offset: 0x00000C26
		// (set) Token: 0x06000040 RID: 64 RVA: 0x00002A2E File Offset: 0x00000C2E
		[Editor(false)]
		public string BannerCodeText
		{
			get
			{
				return this._bannerCode;
			}
			set
			{
				if (value != this._bannerCode)
				{
					this._bannerCode = value;
					base.OnPropertyChanged<string>(value, "BannerCodeText");
					base.SetTextureProviderProperty("BannerCodeText", value);
				}
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000041 RID: 65 RVA: 0x00002A5D File Offset: 0x00000C5D
		// (set) Token: 0x06000042 RID: 66 RVA: 0x00002A65 File Offset: 0x00000C65
		[Editor(false)]
		public ButtonWidget SwapPlacesButtonWidget
		{
			get
			{
				return this._swapPlacesButtonWidget;
			}
			set
			{
				if (value != this._swapPlacesButtonWidget)
				{
					this._swapPlacesButtonWidget = value;
					base.OnPropertyChanged<ButtonWidget>(value, "SwapPlacesButtonWidget");
					if (value != null)
					{
						this._swapPlacesButtonWidget.ClickEventHandlers.Add(new Action<Widget>(this.OnSwapClick));
					}
				}
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000043 RID: 67 RVA: 0x00002AA2 File Offset: 0x00000CA2
		// (set) Token: 0x06000044 RID: 68 RVA: 0x00002AAA File Offset: 0x00000CAA
		[Editor(false)]
		public string BodyProperties
		{
			get
			{
				return this._bodyProperties;
			}
			set
			{
				if (value != this._bodyProperties)
				{
					this._bodyProperties = value;
					base.OnPropertyChanged<string>(value, "BodyProperties");
					base.SetTextureProviderProperty("BodyProperties", value);
				}
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000045 RID: 69 RVA: 0x00002AD9 File Offset: 0x00000CD9
		// (set) Token: 0x06000046 RID: 70 RVA: 0x00002AE1 File Offset: 0x00000CE1
		[Editor(false)]
		public float CustomRenderScale
		{
			get
			{
				return this._customRenderScale;
			}
			set
			{
				if (value != this._customRenderScale)
				{
					this._customRenderScale = value;
					base.OnPropertyChanged(value, "CustomRenderScale");
					base.SetTextureProviderProperty("CustomRenderScale", value);
				}
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000047 RID: 71 RVA: 0x00002B10 File Offset: 0x00000D10
		// (set) Token: 0x06000048 RID: 72 RVA: 0x00002B18 File Offset: 0x00000D18
		[Editor(false)]
		public string CharStringId
		{
			get
			{
				return this._charStringId;
			}
			set
			{
				if (value != this._charStringId)
				{
					this._charStringId = value;
					base.OnPropertyChanged<string>(value, "CharStringId");
					base.SetTextureProviderProperty("CharStringId", value);
				}
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000049 RID: 73 RVA: 0x00002B47 File Offset: 0x00000D47
		// (set) Token: 0x0600004A RID: 74 RVA: 0x00002B4F File Offset: 0x00000D4F
		[Editor(false)]
		public int StanceIndex
		{
			get
			{
				return this._stanceIndex;
			}
			set
			{
				if (value != this._stanceIndex)
				{
					this._stanceIndex = value;
					base.OnPropertyChanged(value, "StanceIndex");
					base.SetTextureProviderProperty("StanceIndex", value);
				}
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600004B RID: 75 RVA: 0x00002B7E File Offset: 0x00000D7E
		// (set) Token: 0x0600004C RID: 76 RVA: 0x00002B86 File Offset: 0x00000D86
		[Editor(false)]
		public bool IsEquipmentAnimActive
		{
			get
			{
				return this._isEquipmentAnimActive;
			}
			set
			{
				if (value != this._isEquipmentAnimActive)
				{
					this._isEquipmentAnimActive = value;
					base.OnPropertyChanged(value, "IsEquipmentAnimActive");
					base.SetTextureProviderProperty("IsEquipmentAnimActive", value);
				}
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600004D RID: 77 RVA: 0x00002BB5 File Offset: 0x00000DB5
		// (set) Token: 0x0600004E RID: 78 RVA: 0x00002BBD File Offset: 0x00000DBD
		[Editor(false)]
		public bool IsFemale
		{
			get
			{
				return this._isFemale;
			}
			set
			{
				if (value != this._isFemale)
				{
					this._isFemale = value;
					base.OnPropertyChanged(value, "IsFemale");
					base.SetTextureProviderProperty("IsFemale", value);
				}
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600004F RID: 79 RVA: 0x00002BEC File Offset: 0x00000DEC
		// (set) Token: 0x06000050 RID: 80 RVA: 0x00002BF4 File Offset: 0x00000DF4
		[Editor(false)]
		public int Race
		{
			get
			{
				return this._race;
			}
			set
			{
				if (value != this._race)
				{
					this._race = value;
					base.OnPropertyChanged(value, "Race");
					base.SetTextureProviderProperty("Race", value);
				}
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000051 RID: 81 RVA: 0x00002C23 File Offset: 0x00000E23
		// (set) Token: 0x06000052 RID: 82 RVA: 0x00002C2B File Offset: 0x00000E2B
		[Editor(false)]
		public string EquipmentCode
		{
			get
			{
				return this._equipmentCode;
			}
			set
			{
				if (value != this._equipmentCode)
				{
					this._equipmentCode = value;
					base.OnPropertyChanged<string>(value, "EquipmentCode");
					base.SetTextureProviderProperty("EquipmentCode", value);
				}
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000053 RID: 83 RVA: 0x00002C5A File Offset: 0x00000E5A
		// (set) Token: 0x06000054 RID: 84 RVA: 0x00002C62 File Offset: 0x00000E62
		[Editor(false)]
		public string MountCreationKey
		{
			get
			{
				return this._mountCreationKey;
			}
			set
			{
				if (value != this._mountCreationKey)
				{
					this._mountCreationKey = value;
					base.OnPropertyChanged<string>(value, "MountCreationKey");
					base.SetTextureProviderProperty("MountCreationKey", value);
				}
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000055 RID: 85 RVA: 0x00002C91 File Offset: 0x00000E91
		// (set) Token: 0x06000056 RID: 86 RVA: 0x00002C99 File Offset: 0x00000E99
		[Editor(false)]
		public string IdleAction
		{
			get
			{
				return this._idleAction;
			}
			set
			{
				if (value != this._idleAction)
				{
					this._idleAction = value;
					base.OnPropertyChanged<string>(value, "IdleAction");
					base.SetTextureProviderProperty("IdleAction", value);
				}
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000057 RID: 87 RVA: 0x00002CC8 File Offset: 0x00000EC8
		// (set) Token: 0x06000058 RID: 88 RVA: 0x00002CD0 File Offset: 0x00000ED0
		[Editor(false)]
		public string IdleFaceAnim
		{
			get
			{
				return this._idleFaceAnim;
			}
			set
			{
				if (value != this._idleFaceAnim)
				{
					this._idleFaceAnim = value;
					base.OnPropertyChanged<string>(value, "IdleFaceAnim");
					base.SetTextureProviderProperty("IdleFaceAnim", value);
				}
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000059 RID: 89 RVA: 0x00002CFF File Offset: 0x00000EFF
		// (set) Token: 0x0600005A RID: 90 RVA: 0x00002D07 File Offset: 0x00000F07
		[Editor(false)]
		public uint ArmorColor1
		{
			get
			{
				return this._armorColor1;
			}
			set
			{
				if (value != this._armorColor1)
				{
					this._armorColor1 = value;
					base.OnPropertyChanged(value, "ArmorColor1");
					base.SetTextureProviderProperty("ArmorColor1", value);
				}
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x0600005B RID: 91 RVA: 0x00002D36 File Offset: 0x00000F36
		// (set) Token: 0x0600005C RID: 92 RVA: 0x00002D3E File Offset: 0x00000F3E
		[Editor(false)]
		public uint ArmorColor2
		{
			get
			{
				return this._armorColor2;
			}
			set
			{
				if (value != this._armorColor2)
				{
					this._armorColor2 = value;
					base.OnPropertyChanged(value, "ArmorColor2");
					base.SetTextureProviderProperty("ArmorColor2", value);
				}
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600005D RID: 93 RVA: 0x00002D6D File Offset: 0x00000F6D
		// (set) Token: 0x0600005E RID: 94 RVA: 0x00002D75 File Offset: 0x00000F75
		[Editor(false)]
		public bool IsBannerShownInBackground
		{
			get
			{
				return this._isBannerShownInBackground;
			}
			set
			{
				if (value != this._isBannerShownInBackground)
				{
					this._isBannerShownInBackground = value;
					base.OnPropertyChanged(value, "IsBannerShownInBackground");
					base.SetTextureProviderProperty("IsBannerShownInBackground", value);
				}
			}
		}

		// Token: 0x04000018 RID: 24
		private ButtonWidget _swapPlacesButtonWidget;

		// Token: 0x04000019 RID: 25
		private string _bannerCode;

		// Token: 0x0400001A RID: 26
		private string _bodyProperties;

		// Token: 0x0400001B RID: 27
		private string _charStringId;

		// Token: 0x0400001C RID: 28
		private string _equipmentCode;

		// Token: 0x0400001D RID: 29
		private string _mountCreationKey;

		// Token: 0x0400001E RID: 30
		private string _idleAction;

		// Token: 0x0400001F RID: 31
		private string _idleFaceAnim;

		// Token: 0x04000020 RID: 32
		private uint _armorColor1;

		// Token: 0x04000021 RID: 33
		private uint _armorColor2;

		// Token: 0x04000022 RID: 34
		private int _stanceIndex;

		// Token: 0x04000023 RID: 35
		private int _race;

		// Token: 0x04000024 RID: 36
		private bool _isEquipmentAnimActive;

		// Token: 0x04000025 RID: 37
		private bool _isFemale;

		// Token: 0x04000026 RID: 38
		private bool _isCharacterMountSwapped;

		// Token: 0x04000027 RID: 39
		private bool _isBannerShownInBackground;

		// Token: 0x04000028 RID: 40
		private float _customRenderScale;
	}
}
