using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.GatherArmy
{
	// Token: 0x0200012E RID: 302
	public class GatherArmyTupleButtonWidget : ButtonWidget
	{
		// Token: 0x06000FF0 RID: 4080 RVA: 0x0002D292 File Offset: 0x0002B492
		public GatherArmyTupleButtonWidget(UIContext context)
			: base(context)
		{
			base.OverrideDefaultStateSwitchingEnabled = true;
		}

		// Token: 0x06000FF1 RID: 4081 RVA: 0x0002D2A2 File Offset: 0x0002B4A2
		protected override void HandleClick()
		{
			if (!this.IsTransferDisabled && (this.IsInCart || this.IsEligible))
			{
				base.HandleClick();
			}
		}

		// Token: 0x06000FF2 RID: 4082 RVA: 0x0002D2C4 File Offset: 0x0002B4C4
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

		// Token: 0x170005A0 RID: 1440
		// (get) Token: 0x06000FF3 RID: 4083 RVA: 0x0002D343 File Offset: 0x0002B543
		// (set) Token: 0x06000FF4 RID: 4084 RVA: 0x0002D34B File Offset: 0x0002B54B
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

		// Token: 0x170005A1 RID: 1441
		// (get) Token: 0x06000FF5 RID: 4085 RVA: 0x0002D369 File Offset: 0x0002B569
		// (set) Token: 0x06000FF6 RID: 4086 RVA: 0x0002D371 File Offset: 0x0002B571
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

		// Token: 0x170005A2 RID: 1442
		// (get) Token: 0x06000FF7 RID: 4087 RVA: 0x0002D38F File Offset: 0x0002B58F
		// (set) Token: 0x06000FF8 RID: 4088 RVA: 0x0002D397 File Offset: 0x0002B597
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

		// Token: 0x0400075D RID: 1885
		private bool _isInCart;

		// Token: 0x0400075E RID: 1886
		private bool _isEligible;

		// Token: 0x0400075F RID: 1887
		private bool _isTransferDisabled;
	}
}
