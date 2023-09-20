using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Encyclopedia
{
	// Token: 0x0200013B RID: 315
	public class EncyclopediaSearchBarBrushWidget : BrushWidget
	{
		// Token: 0x06001095 RID: 4245 RVA: 0x0002E786 File Offset: 0x0002C986
		public EncyclopediaSearchBarBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06001096 RID: 4246 RVA: 0x0002E790 File Offset: 0x0002C990
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			bool flag = base.EventManager.LatestMouseUpWidget == this || base.CheckIsMyChildRecursive(base.EventManager.LatestMouseUpWidget);
			bool flag2 = this.SearchResultPanel.VerticalScrollbar.CheckIsMyChildRecursive(base.EventManager.LatestMouseUpWidget);
			this.ShowResults = (flag || flag2) && this.SearchInputWidget.Text.Length >= this.MinCharAmountToShowResults;
			this.SearchResultPanel.IsVisible = this.ShowResults;
		}

		// Token: 0x06001097 RID: 4247 RVA: 0x0002E81D File Offset: 0x0002CA1D
		protected override void OnMousePressed()
		{
			base.OnMousePressed();
			base.EventFired("SearchBarClick", Array.Empty<object>());
		}

		// Token: 0x170005DA RID: 1498
		// (get) Token: 0x06001098 RID: 4248 RVA: 0x0002E835 File Offset: 0x0002CA35
		// (set) Token: 0x06001099 RID: 4249 RVA: 0x0002E83D File Offset: 0x0002CA3D
		public bool ShowResults
		{
			get
			{
				return this._showChat;
			}
			set
			{
				if (value != this._showChat)
				{
					this._showChat = value;
					base.OnPropertyChanged(value, "ShowResults");
				}
			}
		}

		// Token: 0x170005DB RID: 1499
		// (get) Token: 0x0600109A RID: 4250 RVA: 0x0002E85B File Offset: 0x0002CA5B
		// (set) Token: 0x0600109B RID: 4251 RVA: 0x0002E864 File Offset: 0x0002CA64
		public EditableTextWidget SearchInputWidget
		{
			get
			{
				return this._searchInputWidget;
			}
			set
			{
				if (value != this._searchInputWidget)
				{
					if (this._searchInputWidget != null)
					{
						this._searchInputWidget.EventFire -= this.OnSearchInputClick;
					}
					this._searchInputWidget = value;
					base.OnPropertyChanged<EditableTextWidget>(value, "SearchInputWidget");
					if (this._searchInputWidget != null)
					{
						this._searchInputWidget.EventFire += this.OnSearchInputClick;
					}
				}
			}
		}

		// Token: 0x0600109C RID: 4252 RVA: 0x0002E8CB File Offset: 0x0002CACB
		private void OnSearchInputClick(Widget widget, string eventName, object[] arguments)
		{
			if (eventName == "MouseDown")
			{
				base.EventFired("SearchBarClick", Array.Empty<object>());
			}
		}

		// Token: 0x170005DC RID: 1500
		// (get) Token: 0x0600109D RID: 4253 RVA: 0x0002E8EA File Offset: 0x0002CAEA
		// (set) Token: 0x0600109E RID: 4254 RVA: 0x0002E8F2 File Offset: 0x0002CAF2
		public ScrollablePanel SearchResultPanel
		{
			get
			{
				return this._searchResultPanel;
			}
			set
			{
				if (value != this._searchResultPanel)
				{
					this._searchResultPanel = value;
					base.OnPropertyChanged<ScrollablePanel>(value, "SearchResultPanel");
				}
			}
		}

		// Token: 0x170005DD RID: 1501
		// (get) Token: 0x0600109F RID: 4255 RVA: 0x0002E910 File Offset: 0x0002CB10
		// (set) Token: 0x060010A0 RID: 4256 RVA: 0x0002E918 File Offset: 0x0002CB18
		public int MinCharAmountToShowResults
		{
			get
			{
				return this._minCharAmountToShowResults;
			}
			set
			{
				if (value != this._minCharAmountToShowResults)
				{
					this._minCharAmountToShowResults = value;
					base.OnPropertyChanged(value, "MinCharAmountToShowResults");
				}
			}
		}

		// Token: 0x0400079F RID: 1951
		private bool _showChat;

		// Token: 0x040007A0 RID: 1952
		private ScrollablePanel _searchResultPanel;

		// Token: 0x040007A1 RID: 1953
		private EditableTextWidget _searchInputWidget;

		// Token: 0x040007A2 RID: 1954
		private int _minCharAmountToShowResults;
	}
}
