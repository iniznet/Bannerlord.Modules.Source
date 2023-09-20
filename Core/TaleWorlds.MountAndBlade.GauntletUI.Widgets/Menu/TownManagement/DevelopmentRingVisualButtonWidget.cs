using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Menu.TownManagement
{
	// Token: 0x020000EE RID: 238
	public class DevelopmentRingVisualButtonWidget : ButtonWidget
	{
		// Token: 0x06000C60 RID: 3168 RVA: 0x00022C33 File Offset: 0x00020E33
		public DevelopmentRingVisualButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000C61 RID: 3169 RVA: 0x00022C3C File Offset: 0x00020E3C
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!base.IsSelected)
			{
				this.SetState(base.ParentWidget.CurrentState);
				return;
			}
			this.SetState("Selected");
		}
	}
}
