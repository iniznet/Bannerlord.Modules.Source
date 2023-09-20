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
	public class CharacterTableauWidget : TextureWidget
	{
		public CharacterTableauWidget(UIContext context)
			: base(context)
		{
			base.TextureProviderName = "CharacterTableauTextureProvider";
		}

		protected override void OnMousePressed()
		{
			base.SetTextureProviderProperty("CurrentlyRotating", true);
		}

		protected override void OnMouseReleased()
		{
			base.SetTextureProviderProperty("CurrentlyRotating", false);
		}

		private void OnSwapClick(Widget obj)
		{
			this._isCharacterMountSwapped = !this._isCharacterMountSwapped;
			base.SetTextureProviderProperty("TriggerCharacterMountPlacesSwap", this._isCharacterMountSwapped);
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
		}

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

		private ButtonWidget _swapPlacesButtonWidget;

		private string _bannerCode;

		private string _bodyProperties;

		private string _charStringId;

		private string _equipmentCode;

		private string _mountCreationKey;

		private string _idleAction;

		private string _idleFaceAnim;

		private uint _armorColor1;

		private uint _armorColor2;

		private int _stanceIndex;

		private int _race;

		private bool _isEquipmentAnimActive;

		private bool _isFemale;

		private bool _isCharacterMountSwapped;

		private bool _isBannerShownInBackground;

		private float _customRenderScale;
	}
}
