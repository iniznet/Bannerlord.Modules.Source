using System;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultSiegeAftermathModel : SiegeAftermathModel
	{
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
