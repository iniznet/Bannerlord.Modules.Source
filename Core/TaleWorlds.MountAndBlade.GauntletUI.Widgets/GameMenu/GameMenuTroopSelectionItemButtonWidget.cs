using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.GameMenu
{
	// Token: 0x02000131 RID: 305
	public class GameMenuTroopSelectionItemButtonWidget : ButtonWidget
	{
		// Token: 0x170005A9 RID: 1449
		// (get) Token: 0x06001009 RID: 4105 RVA: 0x0002D549 File Offset: 0x0002B749
		// (set) Token: 0x0600100A RID: 4106 RVA: 0x0002D551 File Offset: 0x0002B751
		public ButtonWidget AddButtonWidget { get; set; }

		// Token: 0x170005AA RID: 1450
		// (get) Token: 0x0600100B RID: 4107 RVA: 0x0002D55A File Offset: 0x0002B75A
		// (set) Token: 0x0600100C RID: 4108 RVA: 0x0002D562 File Offset: 0x0002B762
		public ButtonWidget RemoveButtonWidget { get; set; }

		// Token: 0x170005AB RID: 1451
		// (get) Token: 0x0600100D RID: 4109 RVA: 0x0002D56B File Offset: 0x0002B76B
		// (set) Token: 0x0600100E RID: 4110 RVA: 0x0002D573 File Offset: 0x0002B773
		public Widget CheckmarkVisualWidget { get; set; }

		// Token: 0x170005AC RID: 1452
		// (get) Token: 0x0600100F RID: 4111 RVA: 0x0002D57C File Offset: 0x0002B77C
		// (set) Token: 0x06001010 RID: 4112 RVA: 0x0002D584 File Offset: 0x0002B784
		public Widget AddRemoveControls { get; set; }

		// Token: 0x170005AD RID: 1453
		// (get) Token: 0x06001011 RID: 4113 RVA: 0x0002D58D File Offset: 0x0002B78D
		// (set) Token: 0x06001012 RID: 4114 RVA: 0x0002D595 File Offset: 0x0002B795
		public Widget HeroHealthParent { get; set; }

		// Token: 0x06001013 RID: 4115 RVA: 0x0002D59E File Offset: 0x0002B79E
		public GameMenuTroopSelectionItemButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06001014 RID: 4116 RVA: 0x0002D5B0 File Offset: 0x0002B7B0
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._initialized)
			{
				this.AddButtonWidget.ClickEventHandlers.Add(new Action<Widget>(this.OnAdd));
				this.RemoveButtonWidget.ClickEventHandlers.Add(new Action<Widget>(this.OnRemove));
				this._initialized = true;
			}
			if (this._isDirty)
			{
				this.Refresh();
			}
		}

		// Token: 0x06001015 RID: 4117 RVA: 0x0002D619 File Offset: 0x0002B819
		private void OnRemove(Widget obj)
		{
			base.EventFired("Remove", Array.Empty<object>());
			this.Refresh();
		}

		// Token: 0x06001016 RID: 4118 RVA: 0x0002D631 File Offset: 0x0002B831
		private void OnAdd(Widget obj)
		{
			base.EventFired("Add", Array.Empty<object>());
			this.Refresh();
		}

		// Token: 0x06001017 RID: 4119 RVA: 0x0002D649 File Offset: 0x0002B849
		protected override void HandleClick()
		{
			base.HandleClick();
			if (this.CurrentAmount == 0)
			{
				base.EventFired("Add", Array.Empty<object>());
			}
			else
			{
				base.EventFired("Remove", Array.Empty<object>());
			}
			this.Refresh();
		}

		// Token: 0x06001018 RID: 4120 RVA: 0x0002D684 File Offset: 0x0002B884
		private void Refresh()
		{
			if (this.CheckmarkVisualWidget == null || this.AddRemoveControls == null || this.AddButtonWidget == null || this.RemoveButtonWidget == null)
			{
				return;
			}
			if (this.MaxAmount == 0)
			{
				base.DoNotAcceptEvents = false;
				base.DoNotPassEventsToChildren = true;
				this.CheckmarkVisualWidget.IsHidden = this.CurrentAmount == 0;
				this.AddRemoveControls.IsHidden = true;
				this.AddButtonWidget.IsHidden = true;
				this.RemoveButtonWidget.IsHidden = true;
				base.IsDisabled = true;
				base.UpdateChildrenStates = true;
				base.DominantSelectedState = this.IsLocked;
				this.HeroHealthParent.IsHidden = !this.IsTroopHero;
				if (this.IsLocked)
				{
					base.IsDisabled = this.CurrentAmount <= 0;
					base.DoNotPassEventsToChildren = true;
					base.DoNotAcceptEvents = true;
				}
			}
			else if (this.MaxAmount == 1)
			{
				base.DoNotAcceptEvents = false;
				base.DoNotPassEventsToChildren = true;
				this.CheckmarkVisualWidget.IsHidden = this.CurrentAmount == 0;
				this.AddRemoveControls.IsHidden = true;
				this.AddButtonWidget.IsHidden = true;
				this.RemoveButtonWidget.IsHidden = true;
				base.IsDisabled = (this.IsRosterFull && this.CurrentAmount <= 0) || this.IsLocked;
				base.UpdateChildrenStates = true;
				base.DominantSelectedState = this.IsLocked;
				this.HeroHealthParent.IsHidden = !this.IsTroopHero;
				if (this.IsLocked)
				{
					base.IsDisabled = this.CurrentAmount <= 0;
					base.DoNotPassEventsToChildren = true;
					base.DoNotAcceptEvents = true;
				}
			}
			else
			{
				base.DoNotAcceptEvents = true;
				base.DoNotPassEventsToChildren = false;
				this.CheckmarkVisualWidget.IsHidden = true;
				this.AddRemoveControls.IsHidden = false;
				this.HeroHealthParent.IsHidden = true;
				this.AddButtonWidget.IsHidden = false;
				this.RemoveButtonWidget.IsHidden = false;
				this.AddButtonWidget.IsDisabled = this.IsRosterFull || this.CurrentAmount >= this.MaxAmount;
				this.RemoveButtonWidget.IsDisabled = this.CurrentAmount <= 0;
				base.UpdateChildrenStates = false;
				if (this.IsLocked)
				{
					base.IsDisabled = false;
					base.DoNotPassEventsToChildren = true;
					base.DoNotAcceptEvents = true;
				}
			}
			base.GamepadNavigationIndex = (this.AddRemoveControls.IsVisible ? (-1) : 0);
		}

		// Token: 0x170005AE RID: 1454
		// (get) Token: 0x06001019 RID: 4121 RVA: 0x0002D8ED File Offset: 0x0002BAED
		// (set) Token: 0x0600101A RID: 4122 RVA: 0x0002D8F5 File Offset: 0x0002BAF5
		public bool IsRosterFull
		{
			get
			{
				return this._isRosterFull;
			}
			set
			{
				if (this._isRosterFull != value)
				{
					this._isRosterFull = value;
					base.OnPropertyChanged(value, "IsRosterFull");
					this._isDirty = true;
				}
			}
		}

		// Token: 0x170005AF RID: 1455
		// (get) Token: 0x0600101B RID: 4123 RVA: 0x0002D91A File Offset: 0x0002BB1A
		// (set) Token: 0x0600101C RID: 4124 RVA: 0x0002D922 File Offset: 0x0002BB22
		public bool IsLocked
		{
			get
			{
				return this._isLocked;
			}
			set
			{
				if (this._isLocked != value)
				{
					this._isLocked = value;
					base.OnPropertyChanged(value, "IsLocked");
					this._isDirty = true;
				}
			}
		}

		// Token: 0x170005B0 RID: 1456
		// (get) Token: 0x0600101D RID: 4125 RVA: 0x0002D947 File Offset: 0x0002BB47
		// (set) Token: 0x0600101E RID: 4126 RVA: 0x0002D94F File Offset: 0x0002BB4F
		public bool IsTroopHero
		{
			get
			{
				return this._isTroopHero;
			}
			set
			{
				if (this._isTroopHero != value)
				{
					this._isTroopHero = value;
					base.OnPropertyChanged(value, "IsTroopHero");
					this._isDirty = true;
				}
			}
		}

		// Token: 0x170005B1 RID: 1457
		// (get) Token: 0x0600101F RID: 4127 RVA: 0x0002D974 File Offset: 0x0002BB74
		// (set) Token: 0x06001020 RID: 4128 RVA: 0x0002D97C File Offset: 0x0002BB7C
		public int CurrentAmount
		{
			get
			{
				return this._currentAmount;
			}
			set
			{
				if (this._currentAmount != value)
				{
					this._currentAmount = value;
					base.OnPropertyChanged(value, "CurrentAmount");
					this._isDirty = true;
				}
			}
		}

		// Token: 0x170005B2 RID: 1458
		// (get) Token: 0x06001021 RID: 4129 RVA: 0x0002D9A1 File Offset: 0x0002BBA1
		// (set) Token: 0x06001022 RID: 4130 RVA: 0x0002D9A9 File Offset: 0x0002BBA9
		public int MaxAmount
		{
			get
			{
				return this._maxAmount;
			}
			set
			{
				if (this._maxAmount != value)
				{
					this._maxAmount = value;
					base.OnPropertyChanged(value, "MaxAmount");
					this._isDirty = true;
				}
			}
		}

		// Token: 0x0400076B RID: 1899
		private bool _initialized;

		// Token: 0x0400076C RID: 1900
		private bool _isDirty = true;

		// Token: 0x0400076D RID: 1901
		private int _maxAmount;

		// Token: 0x0400076E RID: 1902
		private int _currentAmount;

		// Token: 0x0400076F RID: 1903
		private bool _isRosterFull;

		// Token: 0x04000770 RID: 1904
		private bool _isLocked;

		// Token: 0x04000771 RID: 1905
		private bool _isTroopHero;
	}
}
