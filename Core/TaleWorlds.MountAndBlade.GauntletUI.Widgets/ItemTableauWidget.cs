using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class ItemTableauWidget : TextureWidget
	{
		[Editor(false)]
		public string ItemModifierId
		{
			get
			{
				return this._itemModifierId;
			}
			set
			{
				if (value != this._itemModifierId)
				{
					this._itemModifierId = value;
					base.OnPropertyChanged<string>(value, "ItemModifierId");
					base.SetTextureProviderProperty("ItemModifierId", value);
				}
			}
		}

		[Editor(false)]
		public string StringId
		{
			get
			{
				return this._stringId;
			}
			set
			{
				if (value != this._stringId)
				{
					this._stringId = value;
					base.OnPropertyChanged<string>(value, "StringId");
					if (value != null)
					{
						base.SetTextureProviderProperty("StringId", value);
					}
				}
			}
		}

		[Editor(false)]
		public float InitialTiltRotation
		{
			get
			{
				return this._initialTiltRotation;
			}
			set
			{
				if (value != this._initialTiltRotation)
				{
					this._initialTiltRotation = value;
					base.OnPropertyChanged(value, "InitialTiltRotation");
					base.SetTextureProviderProperty("InitialTiltRotation", value);
				}
			}
		}

		[Editor(false)]
		public float InitialPanRotation
		{
			get
			{
				return this._initialPanRotation;
			}
			set
			{
				if (value != this._initialPanRotation)
				{
					this._initialPanRotation = value;
					base.OnPropertyChanged(value, "InitialPanRotation");
					base.SetTextureProviderProperty("InitialPanRotation", value);
				}
			}
		}

		[Editor(false)]
		public string BannerCode
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
					base.OnPropertyChanged<string>(value, "BannerCode");
					base.SetTextureProviderProperty("BannerCode", value);
				}
			}
		}

		public ItemTableauWidget(UIContext context)
			: base(context)
		{
			base.TextureProviderName = "ItemTableauTextureProvider";
		}

		protected override void OnMousePressed()
		{
			base.SetTextureProviderProperty("CurrentlyRotating", true);
		}

		protected override void OnRightStickMovement()
		{
			base.OnRightStickMovement();
			base.SetTextureProviderProperty("RotateItemVertical", base.EventManager.RightStickVerticalScrollAmount);
			base.SetTextureProviderProperty("RotateItemHorizontal", base.EventManager.RightStickHorizontalScrollAmount);
		}

		protected override void OnMouseReleased()
		{
			base.SetTextureProviderProperty("CurrentlyRotating", false);
		}

		protected override bool OnPreviewRightStickMovement()
		{
			return true;
		}

		private string _itemModifierId;

		private string _stringId;

		private float _initialTiltRotation;

		private float _initialPanRotation;

		private string _bannerCode;
	}
}
