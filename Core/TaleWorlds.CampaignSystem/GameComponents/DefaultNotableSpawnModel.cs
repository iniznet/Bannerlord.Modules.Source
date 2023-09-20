using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x0200011F RID: 287
	public class DefaultNotableSpawnModel : NotableSpawnModel
	{
		// Token: 0x0600164D RID: 5709 RVA: 0x0006AD10 File Offset: 0x00068F10
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
