using System;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class SettlementVariablesBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.HourlyTickSettlementEvent.AddNonSerializedListener(this, new Action<Settlement>(this.HourlyTickSettlement));
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

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
