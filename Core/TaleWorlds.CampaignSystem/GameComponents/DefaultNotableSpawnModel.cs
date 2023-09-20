using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultNotableSpawnModel : NotableSpawnModel
	{
		public override int GetTargetNotableCountForSettlement(Settlement settlement, Occupation occupation)
		{
			int num = 0;
			if (settlement.IsTown)
			{
				if (occupation == Occupation.Merchant)
				{
					num = 2;
				}
				else if (occupation == Occupation.GangLeader)
				{
					num = 2;
				}
				else if (occupation == Occupation.Artisan)
				{
					num = 1;
				}
				else
				{
					num = 0;
				}
			}
			else if (settlement.IsVillage)
			{
				if (occupation == Occupation.Headman)
				{
					num = 1;
				}
				else if (occupation == Occupation.RuralNotable)
				{
					num = 2;
				}
			}
			return num;
		}
	}
}
