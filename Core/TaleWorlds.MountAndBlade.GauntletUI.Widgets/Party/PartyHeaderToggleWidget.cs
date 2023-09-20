using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Party
{
	// Token: 0x02000058 RID: 88
	public class PartyHeaderToggleWidget : ToggleButtonWidget
	{
		// Token: 0x1700019A RID: 410
		// (get) Token: 0x0600048D RID: 1165 RVA: 0x0000E1E8 File Offset: 0x0000C3E8
		// (set) Token: 0x0600048E RID: 1166 RVA: 0x0000E1F0 File Offset: 0x0000C3F0
		public bool AutoToggleTransferButtonState { get; set; } = true;

		// Token: 0x0600048F RID: 1167 RVA: 0x0000E1F9 File Offset: 0x0000C3F9
		public PartyHeaderToggleWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000490 RID: 1168 RVA: 0x0000E210 File Offset: 0x0000C410
		protected override void OnClick(Widget widget)
		{
			if (!this.BlockInputsWhenDisabled || this._listPanel == null || this._listPanel.ChildCount > 0)
			{
				base.OnClick(widget);
				this.UpdateCollapseIndicator();
			}
		}

		// Token: 0x06000491 RID: 1169 RVA: 0x0000E23D File Offset: 0x0000C43D
		private void OnListSizeChange(Widget widget)
		{
			this.UpdateSize();
		}

		// Token: 0x06000492 RID: 1170 RVA: 0x0000E245 File Offset: 0x0000C445
		private void OnListSizeChange(Widget parentWidget, Widget addedWidget)
		{
			this.UpdateSize();
		}

		// Token: 0x06000493 RID: 1171 RVA: 0x0000E24D File Offset: 0x0000C44D
		public override void SetState(string stateName)
		{
			if (!this.BlockInputsWhenDisabled || this._listPanel == null || this._listPanel.ChildCount > 0)
			{
				base.SetState(stateName);
			}
		}

		// Token: 0x06000494 RID: 1172 RVA: 0x0000E274 File Offset: 0x0000C474
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

		// Token: 0x06000495 RID: 1173 RVA: 0x0000E320 File Offset: 0x0000C520
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

		// Token: 0x06000496 RID: 1174 RVA: 0x0000E37F File Offset: 0x0000C57F
		private void TransferButtonUpdated()
		{
			this.TransferButtonWidget.IsEnabled = false;
		}

		// Token: 0x06000497 RID: 1175 RVA: 0x0000E38D File Offset: 0x0000C58D
		private void CollapseIndicatorUpdated()
		{
			this.CollapseIndicator.AddState("Collapsed");
			this.CollapseIndicator.AddState("Expanded");
			this.UpdateCollapseIndicator();
		}

		// Token: 0x06000498 RID: 1176 RVA: 0x0000E3B5 File Offset: 0x0000C5B5
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

		// Token: 0x1700019B RID: 411
		// (get) Token: 0x06000499 RID: 1177 RVA: 0x0000E3F5 File Offset: 0x0000C5F5
		// (set) Token: 0x0600049A RID: 1178 RVA: 0x0000E3FD File Offset: 0x0000C5FD
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

		// Token: 0x1700019C RID: 412
		// (get) Token: 0x0600049B RID: 1179 RVA: 0x0000E421 File Offset: 0x0000C621
		// (set) Token: 0x0600049C RID: 1180 RVA: 0x0000E429 File Offset: 0x0000C629
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

		// Token: 0x1700019D RID: 413
		// (get) Token: 0x0600049D RID: 1181 RVA: 0x0000E44D File Offset: 0x0000C64D
		// (set) Token: 0x0600049E RID: 1182 RVA: 0x0000E455 File Offset: 0x0000C655
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

		// Token: 0x1700019E RID: 414
		// (get) Token: 0x0600049F RID: 1183 RVA: 0x0000E479 File Offset: 0x0000C679
		// (set) Token: 0x060004A0 RID: 1184 RVA: 0x0000E481 File Offset: 0x0000C681
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

		// Token: 0x1700019F RID: 415
		// (get) Token: 0x060004A1 RID: 1185 RVA: 0x0000E4B4 File Offset: 0x0000C6B4
		// (set) Token: 0x060004A2 RID: 1186 RVA: 0x0000E4BC File Offset: 0x0000C6BC
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

		// Token: 0x040001FA RID: 506
		private int _latestChildCount;

		// Token: 0x040001FC RID: 508
		private ListPanel _listPanel;

		// Token: 0x040001FD RID: 509
		private ButtonWidget _transferButtonWidget;

		// Token: 0x040001FE RID: 510
		private BrushWidget _collapseIndicator;

		// Token: 0x040001FF RID: 511
		private bool _isRelevant = true;

		// Token: 0x04000200 RID: 512
		private bool _blockInputsWhenDisabled;
	}
}
