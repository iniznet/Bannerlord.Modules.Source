using System;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	// Token: 0x0200006A RID: 106
	public class TabToggleWidget : ButtonWidget
	{
		// Token: 0x170001FF RID: 511
		// (get) Token: 0x060006E9 RID: 1769 RVA: 0x0001E617 File Offset: 0x0001C817
		// (set) Token: 0x060006EA RID: 1770 RVA: 0x0001E61F File Offset: 0x0001C81F
		public TabControl TabControlWidget { get; set; }

		// Token: 0x060006EB RID: 1771 RVA: 0x0001E628 File Offset: 0x0001C828
		public TabToggleWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060006EC RID: 1772 RVA: 0x0001E631 File Offset: 0x0001C831
		protected override void OnClick()
		{
			base.OnClick();
			if (this.TabControlWidget != null && !string.IsNullOrEmpty(this.TabName))
			{
				this.TabControlWidget.SetActiveTab(this.TabName);
			}
		}

		// Token: 0x060006ED RID: 1773 RVA: 0x0001E660 File Offset: 0x0001C860
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			bool flag = false;
			if (this.TabControlWidget == null || string.IsNullOrEmpty(this.TabName))
			{
				flag = true;
			}
			else
			{
				Widget widget = this.TabControlWidget.FindChild(this.TabName);
				if (widget == null || widget.IsDisabled)
				{
					flag = true;
				}
			}
			base.IsDisabled = flag;
			base.IsSelected = this.DetermineIfIsSelected();
		}

		// Token: 0x060006EE RID: 1774 RVA: 0x0001E6C4 File Offset: 0x0001C8C4
		private bool DetermineIfIsSelected()
		{
			TabControl tabControlWidget = this.TabControlWidget;
			return ((tabControlWidget != null) ? tabControlWidget.ActiveTab : null) != null && !string.IsNullOrEmpty(this.TabName) && this.TabControlWidget.ActiveTab.Id == this.TabName && base.IsVisible;
		}

		// Token: 0x17000200 RID: 512
		// (get) Token: 0x060006EF RID: 1775 RVA: 0x0001E717 File Offset: 0x0001C917
		// (set) Token: 0x060006F0 RID: 1776 RVA: 0x0001E71F File Offset: 0x0001C91F
		[Editor(false)]
		public string TabName
		{
			get
			{
				return this._tabName;
			}
			set
			{
				if (this._tabName != value)
				{
					this._tabName = value;
					base.OnPropertyChanged<string>(value, "TabName");
				}
			}
		}

		// Token: 0x04000342 RID: 834
		private string _tabName;
	}
}
