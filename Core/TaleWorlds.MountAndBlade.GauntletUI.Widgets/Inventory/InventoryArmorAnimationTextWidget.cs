using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Inventory
{
	public class InventoryArmorAnimationTextWidget : TextWidget
	{
		public InventoryArmorAnimationTextWidget(UIContext context)
			: base(context)
		{
			base.FloatText = 0f;
		}

		private void HandleAnimation(float oldValue, float newValue)
		{
			if (oldValue > newValue)
			{
				this.SetState("Decrease");
				return;
			}
			if (oldValue < newValue)
			{
				this.SetState("Increase");
				return;
			}
			this.SetState("Default");
		}

		[Editor(false)]
		public float FloatAmount
		{
			get
			{
				return this._floatAmount;
			}
			set
			{
				if (this._floatAmount != value)
				{
					this.HandleAnimation(this._floatAmount, value);
					this._floatAmount = value;
					base.FloatText = this._floatAmount;
					base.OnPropertyChanged(value, "FloatAmount");
				}
			}
		}

		private float _floatAmount;
	}
}
