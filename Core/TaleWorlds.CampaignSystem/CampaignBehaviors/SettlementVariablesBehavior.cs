using System;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003D2 RID: 978
	public class SettlementVariablesBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003AE9 RID: 15081 RVA: 0x00113E70 File Offset: 0x00112070
		public override void RegisterEvents()
		{
			CampaignEvents.HourlyTickSettlementEvent.AddNonSerializedListener(this, new Action<Settlement>(this.HourlyTickSettlement));
		}

		// Token: 0x06003AEA RID: 15082 RVA: 0x00113E89 File Offset: 0x00112089
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06003AEB RID: 15083 RVA: 0x00113E8C File Offset: 0x0011208C
		private void HourlyTickSettlement(Settlement settlement)
		{
			if ((settlement.IsVillage || settlement.IsFortification) && settlement.LastAttackerParty != null)
			{
				if (settlement.IsVillage)
				{
					if (settlement.Party.MapEvent == null)
					{
						settlement.PassedHoursAfterLastThreat--;
					}
				}
				else if (settlement.IsFortification && settlement.Party.MapEvent == null && !settlement.IsUnderSiege)
				{
					settlement.PassedHoursAfterLastThreat--;
				}
				if (settlement.PassedHoursAfterLastThreat == 0)
				{
					settlement.LastAttackerParty = null;
				}
			}
		}
	}
}
