using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Inventory
{
	// Token: 0x0200011F RID: 287
	public abstract class InventoryItemButtonWidget : ButtonWidget
	{
		// Token: 0x06000E93 RID: 3731 RVA: 0x00028759 File Offset: 0x00026959
		protected InventoryItemButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000E94 RID: 3732 RVA: 0x00028762 File Offset: 0x00026962
		protected override void OnDragBegin()
		{
			InventoryScreenWidget screenWidget = this.ScreenWidget;
			if (screenWidget != null)
			{
				screenWidget.ItemWidgetDragBegin(this);
			}
			base.OnDragBegin();
		}

		// Token: 0x06000E95 RID: 3733 RVA: 0x0002877C File Offset: 0x0002697C
		protected override bool OnDrop()
		{
			InventoryScreenWidget screenWidget = this.ScreenWidget;
			if (screenWidget != null)
			{
				screenWidget.ItemWidgetDrop(this);
			}
			return base.OnDrop();
		}

		// Token: 0x06000E96 RID: 3734 RVA: 0x00028796 File Offset: 0x00026996
		public virtual void ResetIsSelected()
		{
			base.IsSelected = false;
		}

		// Token: 0x06000E97 RID: 3735 RVA: 0x0002879F File Offset: 0x0002699F
		public void PreviewItem()
		{
			base.EventFired("PreviewItem", Array.Empty<object>());
		}

		// Token: 0x06000E98 RID: 3736 RVA: 0x000287B1 File Offset: 0x000269B1
		public void SellItem()
		{
			base.EventFired("SellItem", Array.Empty<object>());
		}

		// Token: 0x06000E99 RID: 3737 RVA: 0x000287C3 File Offset: 0x000269C3
		public void EquipItem()
		{
			base.EventFired("EquipItem", Array.Empty<object>());
		}

		// Token: 0x06000E9A RID: 3738 RVA: 0x000287D5 File Offset: 0x000269D5
		public void UnequipItem()
		{
			base.EventFired("UnequipItem", Array.Empty<object>());
		}

		// Token: 0x06000E9B RID: 3739 RVA: 0x000287E8 File Offset: 0x000269E8
		private void AssignScreenWidget()
		{
			Widget widget = this;
			while (widget != base.EventManager.Root && this._screenWidget == null)
			{
				if (widget is InventoryScreenWidget)
				{
					this._screenWidget = (InventoryScreenWidget)widget;
				}
				else
				{
					widget = widget.ParentWidget;
				}
			}
		}

		// Token: 0x06000E9C RID: 3740 RVA: 0x0002882C File Offset: 0x00026A2C
		private void ItemTypeUpdated()
		{
			AudioProperty audioProperty = base.Brush.SoundProperties.GetEventAudioProperty("DragEnd");
			if (audioProperty == null)
			{
				audioProperty = new AudioProperty();
				base.Brush.SoundProperties.AddEventSound("DragEnd", audioProperty);
			}
			audioProperty.AudioName = this.GetSound(this.ItemType);
		}

		// Token: 0x06000E9D RID: 3741 RVA: 0x00028880 File Offset: 0x00026A80
		private string GetSound(int typeID)
		{
			switch (typeID)
			{
			case 1:
				return "inventory/horse";
			case 2:
				return "inventory/onehanded";
			case 3:
				return "inventory/twohanded";
			case 4:
				return "inventory/polearm";
			case 5:
			case 6:
				return "inventory/quiver";
			case 7:
				return "inventory/shield";
			case 8:
				return "inventory/bow";
			case 9:
				return "inventory/crossbow";
			case 10:
				return "inventory/throwing";
			case 11:
				return "inventory/sack";
			case 12:
				return "inventory/helmet";
			case 13:
				return "inventory/leather";
			case 14:
			case 15:
				return "inventory/leather_lite";
			case 19:
				return "inventory/animal";
			case 20:
				return "inventory/book";
			case 21:
			case 22:
				return "inventory/leather";
			case 23:
				return "inventory/horsearmor";
			case 24:
				return "inventory/perk";
			case 25:
				return "inventory/leather";
			case 26:
				return "inventory/siegeweapon";
			}
			return "inventory/leather";
		}

		// Token: 0x17000526 RID: 1318
		// (get) Token: 0x06000E9E RID: 3742 RVA: 0x0002897C File Offset: 0x00026B7C
		// (set) Token: 0x06000E9F RID: 3743 RVA: 0x00028984 File Offset: 0x00026B84
		[Editor(false)]
		public bool IsRightSide
		{
			get
			{
				return this._isRightSide;
			}
			set
			{
				if (this._isRightSide != value)
				{
					this._isRightSide = value;
					base.OnPropertyChanged(value, "IsRightSide");
				}
			}
		}

		// Token: 0x17000527 RID: 1319
		// (get) Token: 0x06000EA0 RID: 3744 RVA: 0x000289A2 File Offset: 0x00026BA2
		// (set) Token: 0x06000EA1 RID: 3745 RVA: 0x000289AA File Offset: 0x00026BAA
		[Editor(false)]
		public int ItemType
		{
			get
			{
				return this._itemType;
			}
			set
			{
				if (this._itemType != value)
				{
					this._itemType = value;
					base.OnPropertyChanged(value, "ItemType");
					this.ItemTypeUpdated();
				}
			}
		}

		// Token: 0x17000528 RID: 1320
		// (get) Token: 0x06000EA2 RID: 3746 RVA: 0x000289CE File Offset: 0x00026BCE
		// (set) Token: 0x06000EA3 RID: 3747 RVA: 0x000289D6 File Offset: 0x00026BD6
		[Editor(false)]
		public int EquipmentIndex
		{
			get
			{
				return this._equipmentIndex;
			}
			set
			{
				if (this._equipmentIndex != value)
				{
					this._equipmentIndex = value;
					base.OnPropertyChanged(value, "EquipmentIndex");
				}
			}
		}

		// Token: 0x17000529 RID: 1321
		// (get) Token: 0x06000EA4 RID: 3748 RVA: 0x000289F4 File Offset: 0x00026BF4
		public InventoryScreenWidget ScreenWidget
		{
			get
			{
				if (this._screenWidget == null)
				{
					this.AssignScreenWidget();
				}
				return this._screenWidget;
			}
		}

		// Token: 0x040006AC RID: 1708
		private bool _isRightSide;

		// Token: 0x040006AD RID: 1709
		private int _itemType;

		// Token: 0x040006AE RID: 1710
		private int _equipmentIndex;

		// Token: 0x040006AF RID: 1711
		private InventoryScreenWidget _screenWidget;
	}
}
