using System;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003E0 RID: 992
	public class VillageHealCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003C17 RID: 15383 RVA: 0x0011CCC0 File Offset: 0x0011AEC0
		public override void RegisterEvents()
		{
			CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, new Action<Settlement>(this.DailyTickSettlement));
		}

		// Token: 0x06003C18 RID: 15384 RVA: 0x0011CCDC File Offset: 0x0011AEDC
		private void DailyTickSettlement(Settlement settlement)
		{
			if ((settlement.IsVillage || settlement.IsTown) && settlement.SettlementHitPoints < 1f && settlement.Party.MapEvent == null && settlement.Party.SiegeEvent == null)
			{
				float num = (7000f - MathF.Min(7000f, MathF.Max(1000f, settlement.MapFaction.TotalStrength))) / 100000f;
				ExplainedNumber explainedNumber = new ExplainedNumber(0.06f + num, false, null);
				if (settlement.IsVillage)
				{
					Settlement tradeBound = settlement.Village.TradeBound;
					if (tradeBound != null && tradeBound.IsTown && PerkHelper.GetPerkValueForTown(DefaultPerks.Medicine.CleanInfrastructure, settlement.Village.TradeBound.Town))
					{
						explainedNumber.AddFactor(DefaultPerks.Medicine.CleanInfrastructure.SecondaryBonus, DefaultPerks.Medicine.CleanInfrastructure.Name);
					}
				}
				if (settlement.OwnerClan.Leader.GetPerkValue(DefaultPerks.Roguery.InBestLight))
				{
					explainedNumber.AddFactor(DefaultPerks.Roguery.InBestLight.SecondaryBonus, DefaultPerks.Roguery.InBestLight.Name);
				}
				IncreaseSettlementHealthAction.Apply(settlement, explainedNumber.ResultNumber);
			}
		}

		// Token: 0x06003C19 RID: 15385 RVA: 0x0011CDFF File Offset: 0x0011AFFF
		public override void SyncData(IDataStore dataStore)
		{
		}
	}
}
