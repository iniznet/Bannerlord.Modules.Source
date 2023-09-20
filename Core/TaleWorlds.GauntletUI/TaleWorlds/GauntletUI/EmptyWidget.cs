using System;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.GauntletUI
{
	// Token: 0x02000034 RID: 52
	internal class EmptyWidget : Widget
	{
		// Token: 0x0600036B RID: 875 RVA: 0x0000ED27 File Offset: 0x0000CF27
		public EmptyWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600036C RID: 876 RVA: 0x0000ED30 File Offset: 0x0000CF30
		protected override void OnUpdate(float dt)
		{
		}

		// Token: 0x0600036D RID: 877 RVA: 0x0000ED32 File Offset: 0x0000CF32
		protected override void OnParallelUpdate(float dt)
		{
		}

		// Token: 0x0600036E RID: 878 RVA: 0x0000ED34 File Offset: 0x0000CF34
		protected override void OnLateUpdate(float dt)
		{
		}

		// Token: 0x0600036F RID: 879 RVA: 0x0000ED36 File Offset: 0x0000CF36
		public override void UpdateBrushes(float dt)
		{
		}
	}
}
