using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.Siege
{
	// Token: 0x020000FE RID: 254
	public class MapSiegeQueueIndexTextWidget : TextWidget
	{
		// Token: 0x06000D2A RID: 3370 RVA: 0x00024CF6 File Offset: 0x00022EF6
		public MapSiegeQueueIndexTextWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000D2B RID: 3371 RVA: 0x00024CFF File Offset: 0x00022EFF
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			base.IsVisible = base.IntText > 0;
		}
	}
}
