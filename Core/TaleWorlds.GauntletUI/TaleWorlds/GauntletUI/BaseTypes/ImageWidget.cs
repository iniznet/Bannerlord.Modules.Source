using System;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	// Token: 0x0200005C RID: 92
	public class ImageWidget : BrushWidget
	{
		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x060005EC RID: 1516 RVA: 0x0001A427 File Offset: 0x00018627
		// (set) Token: 0x060005ED RID: 1517 RVA: 0x0001A42F File Offset: 0x0001862F
		public bool OverrideDefaultStateSwitchingEnabled { get; set; }

		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x060005EE RID: 1518 RVA: 0x0001A438 File Offset: 0x00018638
		// (set) Token: 0x060005EF RID: 1519 RVA: 0x0001A443 File Offset: 0x00018643
		public bool OverrideDefaultStateSwitchingDisabled
		{
			get
			{
				return !this.OverrideDefaultStateSwitchingEnabled;
			}
			set
			{
				if (value != !this.OverrideDefaultStateSwitchingEnabled)
				{
					this.OverrideDefaultStateSwitchingEnabled = !value;
				}
			}
		}

		// Token: 0x060005F0 RID: 1520 RVA: 0x0001A45B File Offset: 0x0001865B
		public ImageWidget(UIContext context)
			: base(context)
		{
			base.AddState("Pressed");
			base.AddState("Hovered");
			base.AddState("Disabled");
		}

		// Token: 0x060005F1 RID: 1521 RVA: 0x0001A488 File Offset: 0x00018688
		protected override void RefreshState()
		{
			if (!this.OverrideDefaultStateSwitchingEnabled)
			{
				if (base.IsDisabled)
				{
					this.SetState("Disabled");
				}
				else if (base.IsPressed)
				{
					this.SetState("Pressed");
				}
				else if (base.IsHovered)
				{
					this.SetState("Hovered");
				}
				else
				{
					this.SetState("Default");
				}
			}
			base.RefreshState();
		}
	}
}
