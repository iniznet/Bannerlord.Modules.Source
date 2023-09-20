using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Inventory
{
	public abstract class InventoryItemButtonWidget : ButtonWidget
	{
		protected InventoryItemButtonWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnDragBegin()
		{
			InventoryScreenWidget screenWidget = this.ScreenWidget;
			if (screenWidget != null)
			{
				screenWidget.ItemWidgetDragBegin(this);
			}
			base.OnDragBegin();
		}

		protected override bool OnDrop()
		{
			InventoryScreenWidget screenWidget = this.ScreenWidget;
			if (screenWidget != null)
			{
				screenWidget.ItemWidgetDrop(this);
			}
			return base.OnDrop();
		}

		public virtual void ResetIsSelected()
		{
			base.IsSelected = false;
		}

		public void PreviewItem()
		{
			base.EventFired("PreviewItem", Array.Empty<object>());
		}

		public void SellItem()
		{
			base.EventFired("SellItem", Array.Empty<object>());
		}

		public void EquipItem()
		{
			base.EventFired("EquipItem", Array.Empty<object>());
		}

		public void UnequipItem()
		{
			base.EventFired("UnequipItem", Array.Empty<object>());
		}

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

		private bool _isRightSide;

		private int _itemType;

		private int _equipmentIndex;

		private InventoryScreenWidget _screenWidget;
	}
}
