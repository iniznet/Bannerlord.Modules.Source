using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.ClassLoadout;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Perks
{
	// Token: 0x0200008B RID: 139
	public class MultiplayerPerkContainerPanelWidget : Widget
	{
		// Token: 0x06000758 RID: 1880 RVA: 0x00015B31 File Offset: 0x00013D31
		public MultiplayerPerkContainerPanelWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000759 RID: 1881 RVA: 0x00015B3C File Offset: 0x00013D3C
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

		// Token: 0x0600075A RID: 1882 RVA: 0x00015BDC File Offset: 0x00013DDC
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

		// Token: 0x0600075B RID: 1883 RVA: 0x00015CC0 File Offset: 0x00013EC0
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

		// Token: 0x17000297 RID: 663
		// (get) Token: 0x0600075C RID: 1884 RVA: 0x00015D0C File Offset: 0x00013F0C
		// (set) Token: 0x0600075D RID: 1885 RVA: 0x00015D14 File Offset: 0x00013F14
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

		// Token: 0x17000298 RID: 664
		// (get) Token: 0x0600075E RID: 1886 RVA: 0x00015D32 File Offset: 0x00013F32
		// (set) Token: 0x0600075F RID: 1887 RVA: 0x00015D3A File Offset: 0x00013F3A
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

		// Token: 0x17000299 RID: 665
		// (get) Token: 0x06000760 RID: 1888 RVA: 0x00015D58 File Offset: 0x00013F58
		// (set) Token: 0x06000761 RID: 1889 RVA: 0x00015D60 File Offset: 0x00013F60
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

		// Token: 0x1700029A RID: 666
		// (get) Token: 0x06000762 RID: 1890 RVA: 0x00015D7E File Offset: 0x00013F7E
		// (set) Token: 0x06000763 RID: 1891 RVA: 0x00015D86 File Offset: 0x00013F86
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

		// Token: 0x04000350 RID: 848
		private MultiplayerPerkItemToggleWidget _currentSelectedItem;

		// Token: 0x04000351 RID: 849
		private MultiplayerPerkPopupWidget _popupWidgetFirst;

		// Token: 0x04000352 RID: 850
		private MultiplayerPerkPopupWidget _popupWidgetSecond;

		// Token: 0x04000353 RID: 851
		private MultiplayerPerkPopupWidget _popupWidgetThird;

		// Token: 0x04000354 RID: 852
		private MultiplayerClassLoadoutTroopSubclassButtonWidget _troopTupleBodyWidget;
	}
}
