using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Inventory;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Party
{
	// Token: 0x0200005E RID: 94
	public class PartyTroopTupleButtonWidget : ButtonWidget
	{
		// Token: 0x170001BF RID: 447
		// (get) Token: 0x060004F9 RID: 1273 RVA: 0x0000F4DE File Offset: 0x0000D6DE
		// (set) Token: 0x060004FA RID: 1274 RVA: 0x0000F4E6 File Offset: 0x0000D6E6
		public string CharacterID { get; set; }

		// Token: 0x060004FB RID: 1275 RVA: 0x0000F4EF File Offset: 0x0000D6EF
		public PartyTroopTupleButtonWidget(UIContext context)
			: base(context)
		{
			base.OverrideDefaultStateSwitchingEnabled = true;
			base.AddState("Selected");
		}

		// Token: 0x060004FC RID: 1276 RVA: 0x0000F50C File Offset: 0x0000D70C
		private void SetWidgetsState(string state)
		{
			this.SetState(state);
			string currentState = this._extendedControlsContainer.CurrentState;
			this._extendedControlsContainer.SetState(base.IsSelected ? "Selected" : "Default");
			this._main.SetState(state);
			if (currentState == "Default" && base.IsSelected)
			{
				base.EventFired("Opened", Array.Empty<object>());
				this.TransferSlider.IsExtended = true;
				this._extendedControlsContainer.IsExtended = true;
				return;
			}
			if (currentState == "Selected" && !base.IsSelected)
			{
				base.EventFired("Closed", Array.Empty<object>());
				this.TransferSlider.IsExtended = false;
				this._extendedControlsContainer.IsExtended = false;
			}
		}

		// Token: 0x060004FD RID: 1277 RVA: 0x0000F5D2 File Offset: 0x0000D7D2
		protected override void OnDisconnectedFromRoot()
		{
			base.OnDisconnectedFromRoot();
			if (this.ScreenWidget.CurrentMainTuple == this)
			{
				this.ScreenWidget.SetCurrentTuple(null, false);
			}
		}

		// Token: 0x060004FE RID: 1278 RVA: 0x0000F5F8 File Offset: 0x0000D7F8
		protected override void RefreshState()
		{
			base.RefreshState();
			this._extendedControlsContainer.IsEnabled = base.IsSelected;
			if (base.IsDisabled)
			{
				this.SetWidgetsState("Disabled");
				return;
			}
			if (base.IsPressed)
			{
				this.SetWidgetsState("Pressed");
				return;
			}
			if (base.IsHovered)
			{
				this.SetWidgetsState("Hovered");
				return;
			}
			if (base.IsSelected)
			{
				this.SetWidgetsState("Selected");
				return;
			}
			this.SetWidgetsState("Default");
		}

		// Token: 0x060004FF RID: 1279 RVA: 0x0000F678 File Offset: 0x0000D878
		private void AssignScreenWidget()
		{
			Widget widget = this;
			while (widget != base.EventManager.Root && this._screenWidget == null)
			{
				PartyScreenWidget partyScreenWidget;
				if ((partyScreenWidget = widget as PartyScreenWidget) != null)
				{
					this._screenWidget = partyScreenWidget;
				}
				else
				{
					widget = widget.ParentWidget;
				}
			}
		}

		// Token: 0x06000500 RID: 1280 RVA: 0x0000F6B9 File Offset: 0x0000D8B9
		protected override void OnMouseReleased()
		{
			base.OnMouseReleased();
			PartyScreenWidget screenWidget = this.ScreenWidget;
			if (screenWidget == null)
			{
				return;
			}
			screenWidget.SetCurrentTuple(this, this.IsTupleLeftSide);
		}

		// Token: 0x06000501 RID: 1281 RVA: 0x0000F6D8 File Offset: 0x0000D8D8
		public void ResetIsSelected()
		{
			base.IsSelected = false;
		}

		// Token: 0x06000502 RID: 1282 RVA: 0x0000F6E1 File Offset: 0x0000D8E1
		private void OnValueChanged(PropertyOwnerObject arg1, string arg2, int arg3)
		{
			if (arg2 == "ValueInt")
			{
				base.AcceptDrag = arg3 > 0;
			}
		}

		// Token: 0x170001C0 RID: 448
		// (get) Token: 0x06000503 RID: 1283 RVA: 0x0000F6FA File Offset: 0x0000D8FA
		public PartyScreenWidget ScreenWidget
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

		// Token: 0x170001C1 RID: 449
		// (get) Token: 0x06000504 RID: 1284 RVA: 0x0000F710 File Offset: 0x0000D910
		// (set) Token: 0x06000505 RID: 1285 RVA: 0x0000F718 File Offset: 0x0000D918
		[Editor(false)]
		public bool IsTupleLeftSide
		{
			get
			{
				return this._isTupleLeftSide;
			}
			set
			{
				if (this._isTupleLeftSide != value)
				{
					this._isTupleLeftSide = value;
					base.OnPropertyChanged(value, "IsTupleLeftSide");
				}
			}
		}

		// Token: 0x170001C2 RID: 450
		// (get) Token: 0x06000506 RID: 1286 RVA: 0x0000F736 File Offset: 0x0000D936
		// (set) Token: 0x06000507 RID: 1287 RVA: 0x0000F740 File Offset: 0x0000D940
		[Editor(false)]
		public InventoryTwoWaySliderWidget TransferSlider
		{
			get
			{
				return this._transferSlider;
			}
			set
			{
				if (this._transferSlider != value)
				{
					this._transferSlider = value;
					base.OnPropertyChanged<InventoryTwoWaySliderWidget>(value, "TransferSlider");
					value.intPropertyChanged += this.OnValueChanged;
					this._transferSlider.AddState("Selected");
					this._transferSlider.OverrideDefaultStateSwitchingEnabled = true;
				}
			}
		}

		// Token: 0x170001C3 RID: 451
		// (get) Token: 0x06000508 RID: 1288 RVA: 0x0000F797 File Offset: 0x0000D997
		// (set) Token: 0x06000509 RID: 1289 RVA: 0x0000F79F File Offset: 0x0000D99F
		[Editor(false)]
		public bool IsTransferable
		{
			get
			{
				return this._isTransferable;
			}
			set
			{
				if (this._isTransferable != value)
				{
					this._isTransferable = value;
					base.OnPropertyChanged(value, "IsTransferable");
				}
			}
		}

		// Token: 0x170001C4 RID: 452
		// (get) Token: 0x0600050A RID: 1290 RVA: 0x0000F7BD File Offset: 0x0000D9BD
		// (set) Token: 0x0600050B RID: 1291 RVA: 0x0000F7C5 File Offset: 0x0000D9C5
		[Editor(false)]
		public bool IsMainHero
		{
			get
			{
				return this._isMainHero;
			}
			set
			{
				if (this._isMainHero != value)
				{
					base.AcceptDrag = !value;
					this._isMainHero = value;
					base.OnPropertyChanged(value, "IsMainHero");
				}
			}
		}

		// Token: 0x170001C5 RID: 453
		// (get) Token: 0x0600050C RID: 1292 RVA: 0x0000F7ED File Offset: 0x0000D9ED
		// (set) Token: 0x0600050D RID: 1293 RVA: 0x0000F7F5 File Offset: 0x0000D9F5
		[Editor(false)]
		public bool IsPrisoner
		{
			get
			{
				return this._isPrisoner;
			}
			set
			{
				if (this._isPrisoner != value)
				{
					this._isPrisoner = value;
					base.OnPropertyChanged(value, "IsPrisoner");
				}
			}
		}

		// Token: 0x170001C6 RID: 454
		// (get) Token: 0x0600050E RID: 1294 RVA: 0x0000F813 File Offset: 0x0000DA13
		// (set) Token: 0x0600050F RID: 1295 RVA: 0x0000F81B File Offset: 0x0000DA1B
		[Editor(false)]
		public int TransferAmount
		{
			get
			{
				return this._transferAmount;
			}
			set
			{
				if (this._transferAmount != value)
				{
					this._transferAmount = value;
					base.OnPropertyChanged(value, "TransferAmount");
				}
			}
		}

		// Token: 0x170001C7 RID: 455
		// (get) Token: 0x06000510 RID: 1296 RVA: 0x0000F839 File Offset: 0x0000DA39
		// (set) Token: 0x06000511 RID: 1297 RVA: 0x0000F841 File Offset: 0x0000DA41
		[Editor(false)]
		public InventoryTupleExtensionControlsWidget ExtendedControlsContainer
		{
			get
			{
				return this._extendedControlsContainer;
			}
			set
			{
				if (this._extendedControlsContainer != value)
				{
					this._extendedControlsContainer = value;
					base.OnPropertyChanged<InventoryTupleExtensionControlsWidget>(value, "ExtendedControlsContainer");
				}
			}
		}

		// Token: 0x170001C8 RID: 456
		// (get) Token: 0x06000512 RID: 1298 RVA: 0x0000F85F File Offset: 0x0000DA5F
		// (set) Token: 0x06000513 RID: 1299 RVA: 0x0000F867 File Offset: 0x0000DA67
		[Editor(false)]
		public Widget Main
		{
			get
			{
				return this._main;
			}
			set
			{
				if (this._main != value)
				{
					this._main = value;
					base.OnPropertyChanged<Widget>(value, "Main");
				}
			}
		}

		// Token: 0x170001C9 RID: 457
		// (get) Token: 0x06000514 RID: 1300 RVA: 0x0000F885 File Offset: 0x0000DA85
		// (set) Token: 0x06000515 RID: 1301 RVA: 0x0000F88D File Offset: 0x0000DA8D
		[Editor(false)]
		public Widget UpgradesPanel
		{
			get
			{
				return this._upgradesPanel;
			}
			set
			{
				if (this._upgradesPanel != value)
				{
					this._upgradesPanel = value;
					base.OnPropertyChanged<Widget>(value, "UpgradesPanel");
				}
			}
		}

		// Token: 0x04000229 RID: 553
		private PartyScreenWidget _screenWidget;

		// Token: 0x0400022A RID: 554
		public InventoryTwoWaySliderWidget _transferSlider;

		// Token: 0x0400022B RID: 555
		private bool _isTupleLeftSide;

		// Token: 0x0400022C RID: 556
		private bool _isTransferable;

		// Token: 0x0400022D RID: 557
		private bool _isMainHero;

		// Token: 0x0400022E RID: 558
		private bool _isPrisoner;

		// Token: 0x0400022F RID: 559
		private int _transferAmount;

		// Token: 0x04000230 RID: 560
		private InventoryTupleExtensionControlsWidget _extendedControlsContainer;

		// Token: 0x04000231 RID: 561
		private Widget _main;

		// Token: 0x04000232 RID: 562
		private Widget _upgradesPanel;
	}
}
