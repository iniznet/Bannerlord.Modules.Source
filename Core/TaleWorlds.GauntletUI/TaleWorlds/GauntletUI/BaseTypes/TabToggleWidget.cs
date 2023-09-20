using System;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	public class TabToggleWidget : ButtonWidget
	{
		public TabControl TabControlWidget { get; set; }

		public TabToggleWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnClick()
		{
			base.OnClick();
			if (this.TabControlWidget != null && !string.IsNullOrEmpty(this.TabName))
			{
				this.TabControlWidget.SetActiveTab(this.TabName);
			}
		}

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

		private bool DetermineIfIsSelected()
		{
			TabControl tabControlWidget = this.TabControlWidget;
			return ((tabControlWidget != null) ? tabControlWidget.ActiveTab : null) != null && !string.IsNullOrEmpty(this.TabName) && this.TabControlWidget.ActiveTab.Id == this.TabName && base.IsVisible;
		}

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

		private string _tabName;
	}
}
