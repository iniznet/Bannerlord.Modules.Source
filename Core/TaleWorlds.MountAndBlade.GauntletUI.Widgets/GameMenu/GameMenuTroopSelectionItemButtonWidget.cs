using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.GameMenu
{
	public class GameMenuTroopSelectionItemButtonWidget : ButtonWidget
	{
		public ButtonWidget AddButtonWidget { get; set; }

		public ButtonWidget RemoveButtonWidget { get; set; }

		public Widget CheckmarkVisualWidget { get; set; }

		public Widget AddRemoveControls { get; set; }

		public Widget HeroHealthParent { get; set; }

		public GameMenuTroopSelectionItemButtonWidget(UIContext context)
			: base(context)
		{
		}

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

		private void OnRemove(Widget obj)
		{
			base.EventFired("Remove", Array.Empty<object>());
			this.Refresh();
		}

		private void OnAdd(Widget obj)
		{
			base.EventFired("Add", Array.Empty<object>());
			this.Refresh();
		}

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

		private bool _initialized;

		private bool _isDirty = true;

		private int _maxAmount;

		private int _currentAmount;

		private bool _isRosterFull;

		private bool _isLocked;

		private bool _isTroopHero;
	}
}
