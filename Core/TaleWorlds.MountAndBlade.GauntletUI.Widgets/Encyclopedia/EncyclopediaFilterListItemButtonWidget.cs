using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Encyclopedia
{
	// Token: 0x02000137 RID: 311
	public class EncyclopediaFilterListItemButtonWidget : ButtonWidget
	{
		// Token: 0x06001074 RID: 4212 RVA: 0x0002E2FD File Offset: 0x0002C4FD
		public EncyclopediaFilterListItemButtonWidget(UIContext context)
			: base(context)
		{
			base.OverrideDefaultStateSwitchingEnabled = true;
		}

		// Token: 0x06001075 RID: 4213 RVA: 0x0002E310 File Offset: 0x0002C510
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (base.IsDisabled)
			{
				this.SetState("Disabled");
				return;
			}
			if (base.IsHovered)
			{
				this.SetState("Hovered");
				return;
			}
			if (base.IsSelected)
			{
				this.SetState("Selected");
				return;
			}
			if (base.IsPressed)
			{
				this.SetState("Pressed");
				return;
			}
			this.SetState("Default");
		}
	}
}
