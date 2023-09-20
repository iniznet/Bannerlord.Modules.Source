using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Encyclopedia
{
	public class EncyclopediaSearchBarBrushWidget : BrushWidget
	{
		public EncyclopediaSearchBarBrushWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			bool flag = base.EventManager.LatestMouseUpWidget == this || base.CheckIsMyChildRecursive(base.EventManager.LatestMouseUpWidget);
			bool flag2 = this.SearchResultPanel.VerticalScrollbar.CheckIsMyChildRecursive(base.EventManager.LatestMouseUpWidget);
			this.ShowResults = (flag || flag2) && this.SearchInputWidget.Text.Length >= this.MinCharAmountToShowResults;
			this.SearchResultPanel.IsVisible = this.ShowResults;
		}

		protected override void OnMousePressed()
		{
			base.OnMousePressed();
			base.EventFired("SearchBarClick", Array.Empty<object>());
		}

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

		private void OnSearchInputClick(Widget widget, string eventName, object[] arguments)
		{
			if (eventName == "MouseDown")
			{
				base.EventFired("SearchBarClick", Array.Empty<object>());
			}
		}

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

		private bool _showChat;

		private ScrollablePanel _searchResultPanel;

		private EditableTextWidget _searchInputWidget;

		private int _minCharAmountToShowResults;
	}
}
