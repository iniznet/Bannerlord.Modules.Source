using System;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x0200013E RID: 318
	public class DefaultSiegeAftermathModel : SiegeAftermathModel
	{
		// Token: 0x0600178D RID: 6029 RVA: 0x00074978 File Offset: 0x00072B78
		public override int GetSiegeAftermathTraitXpChangeForPlayer(TraitObject trait, Settlement devastatedSettlement, SiegeAftermathAction.SiegeAftermath aftermathType)
		{
			int num = 0;
			if (trait == DefaultTraits.Mercy)
			{
				if (aftermathType == SiegeAftermathAction.SiegeAftermath.Devastate)
				{
					if (devastatedSettlement.IsTown)
					{
						num = -50;
					}
					else
					{
						num = -30;
					}
				}
				else if (aftermathType == SiegeAftermathAction.SiegeAftermath.ShowMercy)
				{
					if (devastatedSettlement.IsTown)
					{
						num = 20;
					}
					else
					{
						num = 10;
					}
				}
			}
			return num;
		}
	}
}
