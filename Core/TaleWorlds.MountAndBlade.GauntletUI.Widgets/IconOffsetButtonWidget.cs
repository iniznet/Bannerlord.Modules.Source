using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000021 RID: 33
	public class IconOffsetButtonWidget : ButtonWidget
	{
		// Token: 0x1700008A RID: 138
		// (get) Token: 0x0600019C RID: 412 RVA: 0x0000683B File Offset: 0x00004A3B
		// (set) Token: 0x0600019D RID: 413 RVA: 0x00006843 File Offset: 0x00004A43
		public int NormalXOffset { get; set; }

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x0600019E RID: 414 RVA: 0x0000684C File Offset: 0x00004A4C
		// (set) Token: 0x0600019F RID: 415 RVA: 0x00006854 File Offset: 0x00004A54
		public int NormalYOffset { get; set; }

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x060001A0 RID: 416 RVA: 0x0000685D File Offset: 0x00004A5D
		// (set) Token: 0x060001A1 RID: 417 RVA: 0x00006865 File Offset: 0x00004A65
		public int PressedXOffset { get; set; }

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x060001A2 RID: 418 RVA: 0x0000686E File Offset: 0x00004A6E
		// (set) Token: 0x060001A3 RID: 419 RVA: 0x00006876 File Offset: 0x00004A76
		public int PressedYOffset { get; set; }

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x060001A4 RID: 420 RVA: 0x0000687F File Offset: 0x00004A7F
		// (set) Token: 0x060001A5 RID: 421 RVA: 0x00006887 File Offset: 0x00004A87
		public Widget ButtonIcon { get; set; }

		// Token: 0x060001A6 RID: 422 RVA: 0x00006890 File Offset: 0x00004A90
		public IconOffsetButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x0000689C File Offset: 0x00004A9C
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (this.ButtonIcon != null)
			{
				if (base.IsPressed || base.IsSelected)
				{
					this.ButtonIcon.PositionYOffset = (float)this.PressedYOffset;
					this.ButtonIcon.PositionXOffset = (float)this.PressedXOffset;
					return;
				}
				this.ButtonIcon.PositionYOffset = (float)this.NormalYOffset;
				this.ButtonIcon.PositionXOffset = (float)this.NormalXOffset;
			}
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x00006914 File Offset: 0x00004B14
		protected override void RefreshState()
		{
			if (base.IsSelected)
			{
				this.SetState("Selected");
				return;
			}
			if (base.IsDisabled)
			{
				this.SetState("Disabled");
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
	}
}
