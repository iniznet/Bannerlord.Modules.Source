using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.ClassLoadout;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Perks
{
	public class MultiplayerPerkContainerPanelWidget : Widget
	{
		public MultiplayerPerkContainerPanelWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnUpdate(float dt)
		{
			Widget latestMouseUpWidget = base.EventManager.LatestMouseUpWidget;
			if (this.TroopTupleBodyWidget != null)
			{
				MultiplayerClassLoadoutTroopSubclassButtonWidget troopTupleBodyWidget = this.TroopTupleBodyWidget;
				if (troopTupleBodyWidget == null || !troopTupleBodyWidget.IsSelected)
				{
					goto IL_5E;
				}
			}
			if (!base.CheckIsMyChildRecursive(latestMouseUpWidget) && (this.PopupWidgetFirst.IsVisible || this.PopupWidgetSecond.IsVisible || this.PopupWidgetThird.IsVisible))
			{
				this.ClosePanel();
			}
			IL_5E:
			MultiplayerClassLoadoutTroopSubclassButtonWidget troopTupleBodyWidget2 = this.TroopTupleBodyWidget;
			if ((troopTupleBodyWidget2 == null || !troopTupleBodyWidget2.IsSelected) && this._currentSelectedItem != null)
			{
				this._currentSelectedItem.IsSelected = false;
				this._currentSelectedItem = null;
			}
		}

		public void PerkSelected(MultiplayerPerkItemToggleWidget selectedItem)
		{
			if (selectedItem == this._currentSelectedItem || selectedItem == null)
			{
				this.ClosePanel();
				return;
			}
			if (selectedItem != null && selectedItem.ParentWidget != null)
			{
				if (this._currentSelectedItem != null)
				{
					this._currentSelectedItem.IsSelected = false;
				}
				int childIndex = selectedItem.ParentWidget.GetChildIndex(selectedItem);
				this.PopupWidgetFirst.IsVisible = childIndex == 0;
				this.PopupWidgetFirst.IsEnabled = childIndex == 0;
				this.PopupWidgetSecond.IsVisible = childIndex == 1;
				this.PopupWidgetSecond.IsEnabled = childIndex == 1;
				this.PopupWidgetThird.IsVisible = childIndex == 2;
				this.PopupWidgetThird.IsEnabled = childIndex == 2;
				this.PopupWidgetFirst.SetPopupPerksContainer(this);
				this.PopupWidgetSecond.SetPopupPerksContainer(this);
				this.PopupWidgetThird.SetPopupPerksContainer(this);
				this._currentSelectedItem = selectedItem;
				this._currentSelectedItem.IsSelected = true;
			}
		}

		private void ClosePanel()
		{
			if (this._currentSelectedItem != null)
			{
				this._currentSelectedItem.IsSelected = false;
			}
			this._currentSelectedItem = null;
			this.PopupWidgetFirst.IsVisible = false;
			this.PopupWidgetSecond.IsVisible = false;
			this.PopupWidgetThird.IsVisible = false;
		}

		public MultiplayerPerkPopupWidget PopupWidgetFirst
		{
			get
			{
				return this._popupWidgetFirst;
			}
			set
			{
				if (value != this._popupWidgetFirst)
				{
					this._popupWidgetFirst = value;
					base.OnPropertyChanged<MultiplayerPerkPopupWidget>(value, "PopupWidgetFirst");
				}
			}
		}

		public MultiplayerPerkPopupWidget PopupWidgetSecond
		{
			get
			{
				return this._popupWidgetSecond;
			}
			set
			{
				if (value != this._popupWidgetSecond)
				{
					this._popupWidgetSecond = value;
					base.OnPropertyChanged<MultiplayerPerkPopupWidget>(value, "PopupWidgetSecond");
				}
			}
		}

		public MultiplayerPerkPopupWidget PopupWidgetThird
		{
			get
			{
				return this._popupWidgetThird;
			}
			set
			{
				if (value != this._popupWidgetThird)
				{
					this._popupWidgetThird = value;
					base.OnPropertyChanged<MultiplayerPerkPopupWidget>(value, "PopupWidgetThird");
				}
			}
		}

		public MultiplayerClassLoadoutTroopSubclassButtonWidget TroopTupleBodyWidget
		{
			get
			{
				return this._troopTupleBodyWidget;
			}
			set
			{
				if (value != this._troopTupleBodyWidget)
				{
					this._troopTupleBodyWidget = value;
					base.OnPropertyChanged<MultiplayerClassLoadoutTroopSubclassButtonWidget>(value, "TroopTupleBodyWidget");
				}
			}
		}

		private MultiplayerPerkItemToggleWidget _currentSelectedItem;

		private MultiplayerPerkPopupWidget _popupWidgetFirst;

		private MultiplayerPerkPopupWidget _popupWidgetSecond;

		private MultiplayerPerkPopupWidget _popupWidgetThird;

		private MultiplayerClassLoadoutTroopSubclassButtonWidget _troopTupleBodyWidget;
	}
}
