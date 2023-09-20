using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Inventory
{
	public class InventoryEquippedItemSlotWidget : InventoryItemButtonWidget
	{
		public InventoryEquippedItemSlotWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (base.ScreenWidget == null || this.Background == null)
			{
				return;
			}
			bool flag = base.ScreenWidget.TargetEquipmentIndex == this.TargetEquipmentIndex;
			bool flag2 = this.TargetEquipmentIndex == 0 && base.ScreenWidget.TargetEquipmentIndex >= 0 && base.ScreenWidget.TargetEquipmentIndex <= 3;
			if (flag || flag2)
			{
				this.Background.SetState("Selected");
				return;
			}
			this.Background.SetState("Default");
		}

		private void ProcessSelectItem()
		{
			if (base.ScreenWidget != null)
			{
				base.IsSelected = true;
				this.SetState("Selected");
				base.ScreenWidget.SetCurrentTuple(this, true);
			}
		}

		protected override void OnMouseReleased()
		{
			base.OnMouseReleased();
			this.ProcessSelectItem();
		}

		private void ImageIdentifierOnPropertyChanged(PropertyOwnerObject owner, string propertyName, object value)
		{
			if (propertyName == "ImageId")
			{
				base.IsHidden = string.IsNullOrEmpty((string)value);
			}
		}

		[Editor(false)]
		public ImageIdentifierWidget ImageIdentifier
		{
			get
			{
				return this._imageIdentifier;
			}
			set
			{
				if (this._imageIdentifier != value)
				{
					if (this._imageIdentifier != null)
					{
						this._imageIdentifier.PropertyChanged -= this.ImageIdentifierOnPropertyChanged;
					}
					this._imageIdentifier = value;
					if (this._imageIdentifier != null)
					{
						this._imageIdentifier.PropertyChanged += this.ImageIdentifierOnPropertyChanged;
					}
					base.OnPropertyChanged<ImageIdentifierWidget>(value, "ImageIdentifier");
				}
			}
		}

		[Editor(false)]
		public Widget Background
		{
			get
			{
				return this._background;
			}
			set
			{
				if (this._background != value)
				{
					this._background = value;
					this._background.AddState("Selected");
					base.OnPropertyChanged<Widget>(value, "Background");
				}
			}
		}

		[Editor(false)]
		public int TargetEquipmentIndex
		{
			get
			{
				return this._targetEquipmentIndex;
			}
			set
			{
				if (this._targetEquipmentIndex != value)
				{
					this._targetEquipmentIndex = value;
					base.OnPropertyChanged(value, "TargetEquipmentIndex");
				}
			}
		}

		private ImageIdentifierWidget _imageIdentifier;

		private Widget _background;

		private int _targetEquipmentIndex;
	}
}
