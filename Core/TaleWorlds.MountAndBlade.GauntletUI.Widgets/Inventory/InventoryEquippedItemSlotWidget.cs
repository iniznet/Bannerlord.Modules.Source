using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Inventory
{
	// Token: 0x0200011D RID: 285
	public class InventoryEquippedItemSlotWidget : InventoryItemButtonWidget
	{
		// Token: 0x06000E86 RID: 3718 RVA: 0x00028592 File Offset: 0x00026792
		public InventoryEquippedItemSlotWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000E87 RID: 3719 RVA: 0x0002859C File Offset: 0x0002679C
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

		// Token: 0x06000E88 RID: 3720 RVA: 0x00028624 File Offset: 0x00026824
		private void ProcessSelectItem()
		{
			if (base.ScreenWidget != null)
			{
				base.IsSelected = true;
				this.SetState("Selected");
				base.ScreenWidget.SetCurrentTuple(this, true);
			}
		}

		// Token: 0x06000E89 RID: 3721 RVA: 0x0002864D File Offset: 0x0002684D
		protected override void OnMouseReleased()
		{
			base.OnMouseReleased();
			this.ProcessSelectItem();
		}

		// Token: 0x06000E8A RID: 3722 RVA: 0x0002865B File Offset: 0x0002685B
		private void ImageIdentifierOnPropertyChanged(PropertyOwnerObject owner, string propertyName, object value)
		{
			if (propertyName == "ImageId")
			{
				base.IsHidden = string.IsNullOrEmpty((string)value);
			}
		}

		// Token: 0x17000523 RID: 1315
		// (get) Token: 0x06000E8B RID: 3723 RVA: 0x0002867B File Offset: 0x0002687B
		// (set) Token: 0x06000E8C RID: 3724 RVA: 0x00028684 File Offset: 0x00026884
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

		// Token: 0x17000524 RID: 1316
		// (get) Token: 0x06000E8D RID: 3725 RVA: 0x000286EB File Offset: 0x000268EB
		// (set) Token: 0x06000E8E RID: 3726 RVA: 0x000286F3 File Offset: 0x000268F3
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

		// Token: 0x17000525 RID: 1317
		// (get) Token: 0x06000E8F RID: 3727 RVA: 0x00028721 File Offset: 0x00026921
		// (set) Token: 0x06000E90 RID: 3728 RVA: 0x00028729 File Offset: 0x00026929
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

		// Token: 0x040006A9 RID: 1705
		private ImageIdentifierWidget _imageIdentifier;

		// Token: 0x040006AA RID: 1706
		private Widget _background;

		// Token: 0x040006AB RID: 1707
		private int _targetEquipmentIndex;
	}
}
