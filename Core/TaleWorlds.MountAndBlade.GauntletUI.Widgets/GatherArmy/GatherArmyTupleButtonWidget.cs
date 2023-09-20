using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.GatherArmy
{
	public class GatherArmyTupleButtonWidget : ButtonWidget
	{
		public GatherArmyTupleButtonWidget(UIContext context)
			: base(context)
		{
			base.OverrideDefaultStateSwitchingEnabled = true;
		}

		protected override void HandleClick()
		{
			if (!this.IsTransferDisabled && (this.IsInCart || this.IsEligible))
			{
				base.HandleClick();
			}
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this.IsTransferDisabled || (!this.IsInCart && !this.IsEligible))
			{
				this.SetState("Disabled");
				return;
			}
			if (this.IsInCart)
			{
				this.SetState("Selected");
				return;
			}
			if (base.IsPressed)
			{
				this.SetState("Pressed");
				return;
			}
			if (base.IsHovered)
			{
				this.SetState("Hovered");
				return;
			}
			this.SetState("Default");
		}

		[Editor(false)]
		public bool IsInCart
		{
			get
			{
				return this._isInCart;
			}
			set
			{
				if (this._isInCart != value)
				{
					this._isInCart = value;
					base.OnPropertyChanged(value, "IsInCart");
				}
			}
		}

		[Editor(false)]
		public bool IsEligible
		{
			get
			{
				return this._isEligible;
			}
			set
			{
				if (this._isEligible != value)
				{
					this._isEligible = value;
					base.OnPropertyChanged(value, "IsEligible");
				}
			}
		}

		[Editor(false)]
		public bool IsTransferDisabled
		{
			get
			{
				return this._isTransferDisabled;
			}
			set
			{
				if (this._isTransferDisabled != value)
				{
					this._isTransferDisabled = value;
					base.OnPropertyChanged(value, "IsTransferDisabled");
				}
			}
		}

		private bool _isInCart;

		private bool _isEligible;

		private bool _isTransferDisabled;
	}
}
