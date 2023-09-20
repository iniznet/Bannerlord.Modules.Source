using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Order
{
	public class OrderReturnButtonWidget : OrderItemButtonWidget
	{
		public Widget InputVisualParent { get; set; }

		public OrderReturnButtonWidget(UIContext context)
			: base(context)
		{
			Input.OnGamepadActiveStateChanged = (Action)Delegate.Combine(Input.OnGamepadActiveStateChanged, new Action(this.OnGamepadActiveStateChanged));
		}

		private void OnGamepadActiveStateChanged()
		{
			this.UpdateVisibility();
		}

		private void UpdateVisibility()
		{
			base.IsVisible = this.IsDeployment && Input.IsGamepadActive && !this.IsHolding;
		}

		private void UpdateInputVisualVisibility()
		{
			if (this.InputVisualParent != null)
			{
				this.InputVisualParent.IsVisible = this.CanUseShortcuts && !this.IsAnyOrderSetActive;
			}
		}

		protected override void OnDisconnectedFromRoot()
		{
			base.OnDisconnectedFromRoot();
			Input.OnGamepadActiveStateChanged = (Action)Delegate.Remove(Input.OnGamepadActiveStateChanged, new Action(this.OnGamepadActiveStateChanged));
		}

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

		private bool _isHolding;

		private bool _canUseShortcuts;

		private bool _isAnyOrderSetActive;

		private bool _isDeployment;
	}
}
