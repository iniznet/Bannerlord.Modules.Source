using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000004 RID: 4
	public class AutoHideZeroTextWidget : TextWidget
	{
		// Token: 0x06000009 RID: 9 RVA: 0x00002143 File Offset: 0x00000343
		public AutoHideZeroTextWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600000A RID: 10 RVA: 0x0000214C File Offset: 0x0000034C
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			base.IsVisible = base.IntText != 0;
		}
	}
}
