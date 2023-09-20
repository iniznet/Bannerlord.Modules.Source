using System;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x0200044D RID: 1101
	public static class MakePregnantAction
	{
		// Token: 0x06003F37 RID: 16183 RVA: 0x0012E78A File Offset: 0x0012C98A
		private static void ApplyInternal(Hero mother)
		{
			mother.IsPregnant = true;
			CampaignEventDispatcher.Instance.OnChildConceived(mother);
		}

		// Token: 0x06003F38 RID: 16184 RVA: 0x0012E79E File Offset: 0x0012C99E
		public static void Apply(Hero mother)
		{
			MakePregnantAction.ApplyInternal(mother);
		}
	}
}
