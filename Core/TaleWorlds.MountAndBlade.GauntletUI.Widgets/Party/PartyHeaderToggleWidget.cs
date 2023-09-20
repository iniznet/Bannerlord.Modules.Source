using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Party
{
	public class PartyHeaderToggleWidget : ToggleButtonWidget
	{
		public bool AutoToggleTransferButtonState { get; set; } = true;

		public PartyHeaderToggleWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnClick(Widget widget)
		{
			if (!this.BlockInputsWhenDisabled || this._listPanel == null || this._listPanel.ChildCount > 0)
			{
				base.OnClick(widget);
				this.UpdateCollapseIndicator();
			}
		}

		private void OnListSizeChange(Widget widget)
		{
			this.UpdateSize();
		}

		private void OnListSizeChange(Widget parentWidget, Widget addedWidget)
		{
			this.UpdateSize();
		}

		public override void SetState(string stateName)
		{
			if (!this.BlockInputsWhenDisabled || this._listPanel == null || this._listPanel.ChildCount > 0)
			{
				base.SetState(stateName);
			}
		}

		private void UpdateSize()
		{
			if (this.TransferButtonWidget != null && this.AutoToggleTransferButtonState)
			{
				this.TransferButtonWidget.IsEnabled = this._listPanel.ChildCount > 0;
			}
			if (this.IsRelevant)
			{
				base.IsVisible = true;
				if (this._listPanel.ChildCount > 0)
				{
					this._listPanel.IsVisible = true;
				}
				if (this._listPanel.ChildCount > this._latestChildCount && !base.WidgetToClose.IsVisible)
				{
					this.OnClick();
				}
			}
			else
			{
				this._listPanel.IsVisible = false;
			}
			this._latestChildCount = this._listPanel.ChildCount;
			this.UpdateCollapseIndicator();
		}

		private void ListPanelUpdated()
		{
			if (this.TransferButtonWidget != null)
			{
				this.TransferButtonWidget.IsEnabled = false;
			}
			this._listPanel.ItemAfterRemoveEventHandlers.Add(new Action<Widget>(this.OnListSizeChange));
			this._listPanel.ItemAddEventHandlers.Add(new Action<Widget, Widget>(this.OnListSizeChange));
			this.UpdateSize();
		}

		private void TransferButtonUpdated()
		{
			this.TransferButtonWidget.IsEnabled = false;
		}

		private void CollapseIndicatorUpdated()
		{
			this.CollapseIndicator.AddState("Collapsed");
			this.CollapseIndicator.AddState("Expanded");
			this.UpdateCollapseIndicator();
		}

		private void UpdateCollapseIndicator()
		{
			if (base.WidgetToClose != null && this.CollapseIndicator != null)
			{
				if (base.WidgetToClose.IsVisible)
				{
					this.CollapseIndicator.SetState("Expanded");
					return;
				}
				this.CollapseIndicator.SetState("Collapsed");
			}
		}

		[Editor(false)]
		public ListPanel ListPanel
		{
			get
			{
				return this._listPanel;
			}
			set
			{
				if (this._listPanel != value)
				{
					this._listPanel = value;
					base.OnPropertyChanged<ListPanel>(value, "ListPanel");
					this.ListPanelUpdated();
				}
			}
		}

		[Editor(false)]
		public ButtonWidget TransferButtonWidget
		{
			get
			{
				return this._transferButtonWidget;
			}
			set
			{
				if (this._transferButtonWidget != value)
				{
					this._transferButtonWidget = value;
					base.OnPropertyChanged<ButtonWidget>(value, "TransferButtonWidget");
					this.TransferButtonUpdated();
				}
			}
		}

		[Editor(false)]
		public BrushWidget CollapseIndicator
		{
			get
			{
				return this._collapseIndicator;
			}
			set
			{
				if (this._collapseIndicator != value)
				{
					this._collapseIndicator = value;
					base.OnPropertyChanged<BrushWidget>(value, "CollapseIndicator");
					this.CollapseIndicatorUpdated();
				}
			}
		}

		[Editor(false)]
		public bool IsRelevant
		{
			get
			{
				return this._isRelevant;
			}
			set
			{
				if (this._isRelevant != value)
				{
					this._isRelevant = value;
					if (!this._isRelevant)
					{
						base.IsVisible = false;
					}
					this.UpdateSize();
					base.OnPropertyChanged(value, "IsRelevant");
				}
			}
		}

		[Editor(false)]
		public bool BlockInputsWhenDisabled
		{
			get
			{
				return this._blockInputsWhenDisabled;
			}
			set
			{
				if (this._blockInputsWhenDisabled != value)
				{
					this._blockInputsWhenDisabled = value;
					base.OnPropertyChanged(value, "BlockInputsWhenDisabled");
				}
			}
		}

		private int _latestChildCount;

		private ListPanel _listPanel;

		private ButtonWidget _transferButtonWidget;

		private BrushWidget _collapseIndicator;

		private bool _isRelevant = true;

		private bool _blockInputsWhenDisabled;
	}
}
