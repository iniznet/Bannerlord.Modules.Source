using System;
using TaleWorlds.Library;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	// Token: 0x02000069 RID: 105
	public class TabControl : Widget
	{
		// Token: 0x14000010 RID: 16
		// (add) Token: 0x060006DD RID: 1757 RVA: 0x0001E3D8 File Offset: 0x0001C5D8
		// (remove) Token: 0x060006DE RID: 1758 RVA: 0x0001E410 File Offset: 0x0001C610
		public event OnActiveTabChangeEvent OnActiveTabChange;

		// Token: 0x060006DF RID: 1759 RVA: 0x0001E445 File Offset: 0x0001C645
		public TabControl(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060006E0 RID: 1760 RVA: 0x0001E44E File Offset: 0x0001C64E
		protected override void OnChildRemoved(Widget child)
		{
			base.OnChildRemoved(child);
			if (child == this.ActiveTab)
			{
				this.ActiveTab = null;
			}
		}

		// Token: 0x170001FD RID: 509
		// (get) Token: 0x060006E1 RID: 1761 RVA: 0x0001E467 File Offset: 0x0001C667
		// (set) Token: 0x060006E2 RID: 1762 RVA: 0x0001E46F File Offset: 0x0001C66F
		[Editor(false)]
		public Widget ActiveTab
		{
			get
			{
				return this._activeTab;
			}
			private set
			{
				if (this._activeTab != value)
				{
					this._activeTab = value;
					OnActiveTabChangeEvent onActiveTabChange = this.OnActiveTabChange;
					if (onActiveTabChange == null)
					{
						return;
					}
					onActiveTabChange();
				}
			}
		}

		// Token: 0x060006E3 RID: 1763 RVA: 0x0001E494 File Offset: 0x0001C694
		private void SetActiveTab(int index)
		{
			Widget child = base.GetChild(index);
			this.SetActiveTab(child);
		}

		// Token: 0x060006E4 RID: 1764 RVA: 0x0001E4B0 File Offset: 0x0001C6B0
		public void SetActiveTab(string tabName)
		{
			Widget widget = base.FindChild(tabName);
			this.SetActiveTab(widget);
		}

		// Token: 0x060006E5 RID: 1765 RVA: 0x0001E4CC File Offset: 0x0001C6CC
		private void SetActiveTab(Widget newTab)
		{
			if (this.ActiveTab != newTab && newTab != null)
			{
				if (this.ActiveTab != null)
				{
					this.ActiveTab.IsVisible = false;
				}
				this.ActiveTab = newTab;
				this.ActiveTab.IsVisible = true;
				this.SelectedIndex = base.GetChildIndex(this.ActiveTab);
			}
		}

		// Token: 0x060006E6 RID: 1766 RVA: 0x0001E520 File Offset: 0x0001C720
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (this.ActiveTab != null && this.ActiveTab.ParentWidget == null)
			{
				this.ActiveTab = null;
			}
			if (this.ActiveTab == null || this.ActiveTab.IsDisabled)
			{
				for (int i = 0; i < base.ChildCount; i++)
				{
					Widget child = base.GetChild(i);
					if (child.IsEnabled && !string.IsNullOrEmpty(child.Id))
					{
						this.ActiveTab = child;
						break;
					}
				}
			}
			for (int j = 0; j < base.ChildCount; j++)
			{
				Widget child2 = base.GetChild(j);
				if (this.ActiveTab != child2 && (child2.IsEnabled || child2.IsVisible))
				{
					child2.IsVisible = false;
				}
				if (this.ActiveTab == child2)
				{
					child2.IsVisible = true;
				}
			}
		}

		// Token: 0x170001FE RID: 510
		// (get) Token: 0x060006E7 RID: 1767 RVA: 0x0001E5E5 File Offset: 0x0001C7E5
		// (set) Token: 0x060006E8 RID: 1768 RVA: 0x0001E5ED File Offset: 0x0001C7ED
		[DataSourceProperty]
		public int SelectedIndex
		{
			get
			{
				return this._selectedIndex;
			}
			set
			{
				if (this._selectedIndex != value)
				{
					this._selectedIndex = value;
					this.SetActiveTab(this._selectedIndex);
					base.OnPropertyChanged(value, "SelectedIndex");
				}
			}
		}

		// Token: 0x0400033F RID: 831
		private Widget _activeTab;

		// Token: 0x04000340 RID: 832
		private int _selectedIndex;
	}
}
