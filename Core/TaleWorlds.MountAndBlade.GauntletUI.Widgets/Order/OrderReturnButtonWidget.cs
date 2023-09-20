using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Order
{
	// Token: 0x02000064 RID: 100
	public class OrderReturnButtonWidget : OrderItemButtonWidget
	{
		// Token: 0x170001DD RID: 477
		// (get) Token: 0x06000547 RID: 1351 RVA: 0x0000FFA9 File Offset: 0x0000E1A9
		// (set) Token: 0x06000548 RID: 1352 RVA: 0x0000FFB1 File Offset: 0x0000E1B1
		public Widget InputVisualParent { get; set; }

		// Token: 0x06000549 RID: 1353 RVA: 0x0000FFBA File Offset: 0x0000E1BA
		public OrderReturnButtonWidget(UIContext context)
			: base(context)
		{
			Input.OnGamepadActiveStateChanged = (Action)Delegate.Combine(Input.OnGamepadActiveStateChanged, new Action(this.OnGamepadActiveStateChanged));
		}

		// Token: 0x0600054A RID: 1354 RVA: 0x0000FFE3 File Offset: 0x0000E1E3
		private void OnGamepadActiveStateChanged()
		{
			this.UpdateVisibility();
		}

		// Token: 0x0600054B RID: 1355 RVA: 0x0000FFEB File Offset: 0x0000E1EB
		private void UpdateVisibility()
		{
			base.IsVisible = this.IsDeployment && Input.IsGamepadActive && !this.IsHolding;
		}

		// Token: 0x0600054C RID: 1356 RVA: 0x0001000E File Offset: 0x0000E20E
		private void UpdateInputVisualVisibility()
		{
			if (this.InputVisualParent != null)
			{
				this.InputVisualParent.IsVisible = this.CanUseShortcuts && !this.IsAnyOrderSetActive;
			}
		}

		// Token: 0x0600054D RID: 1357 RVA: 0x00010037 File Offset: 0x0000E237
		protected override void OnDisconnectedFromRoot()
		{
			base.OnDisconnectedFromRoot();
			Input.OnGamepadActiveStateChanged = (Action)Delegate.Remove(Input.OnGamepadActiveStateChanged, new Action(this.OnGamepadActiveStateChanged));
		}

		// Token: 0x170001DE RID: 478
		// (get) Token: 0x0600054E RID: 1358 RVA: 0x0001005F File Offset: 0x0000E25F
		// (set) Token: 0x0600054F RID: 1359 RVA: 0x00010067 File Offset: 0x0000E267
		public bool IsHolding
		{
			get
			{
				return this._isHolding;
			}
			set
			{
				if (value != this._isHolding)
				{
					this._isHolding = value;
					base.OnPropertyChanged(value, "IsHolding");
					this.UpdateVisibility();
				}
			}
		}

		// Token: 0x170001DF RID: 479
		// (get) Token: 0x06000550 RID: 1360 RVA: 0x0001008B File Offset: 0x0000E28B
		// (set) Token: 0x06000551 RID: 1361 RVA: 0x00010093 File Offset: 0x0000E293
		public bool CanUseShortcuts
		{
			get
			{
				return this._canUseShortcuts;
			}
			set
			{
				if (value != this._canUseShortcuts)
				{
					this._canUseShortcuts = value;
					base.OnPropertyChanged(value, "CanUseShortcuts");
					this.UpdateInputVisualVisibility();
				}
			}
		}

		// Token: 0x170001E0 RID: 480
		// (get) Token: 0x06000552 RID: 1362 RVA: 0x000100B7 File Offset: 0x0000E2B7
		// (set) Token: 0x06000553 RID: 1363 RVA: 0x000100BF File Offset: 0x0000E2BF
		public bool IsAnyOrderSetActive
		{
			get
			{
				return this._isAnyOrderSetActive;
			}
			set
			{
				if (value != this._isAnyOrderSetActive)
				{
					this._isAnyOrderSetActive = value;
					base.OnPropertyChanged(value, "IsAnyOrderSetActive");
					this.UpdateInputVisualVisibility();
				}
			}
		}

		// Token: 0x170001E1 RID: 481
		// (get) Token: 0x06000554 RID: 1364 RVA: 0x000100E3 File Offset: 0x0000E2E3
		// (set) Token: 0x06000555 RID: 1365 RVA: 0x000100EB File Offset: 0x0000E2EB
		public bool IsDeployment
		{
			get
			{
				return this._isDeployment;
			}
			set
			{
				if (value != this._isDeployment)
				{
					this._isDeployment = value;
					base.OnPropertyChanged(value, "IsDeployment");
					this.UpdateVisibility();
				}
			}
		}

		// Token: 0x0400024A RID: 586
		private bool _isHolding;

		// Token: 0x0400024B RID: 587
		private bool _canUseShortcuts;

		// Token: 0x0400024C RID: 588
		private bool _isAnyOrderSetActive;

		// Token: 0x0400024D RID: 589
		private bool _isDeployment;
	}
}
