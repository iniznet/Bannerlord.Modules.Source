using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	// Token: 0x02000047 RID: 71
	public class MissionCrimeHandler : MissionLogic
	{
		// Token: 0x06000376 RID: 886 RVA: 0x00019370 File Offset: 0x00017570
		protected override void OnEndMission()
		{
			if (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.IsFortification)
			{
				IFaction mapFaction = Settlement.CurrentSettlement.MapFaction;
				if (!Hero.MainHero.IsPrisoner && !Campaign.Current.IsMainHeroDisguised && !mapFaction.IsBanditFaction && Campaign.Current.Models.CrimeModel.IsPlayerCrimeRatingSevere(mapFaction.MapFaction))
				{
					Campaign.Current.GameMenuManager.SetNextMenu("fortification_crime_rating");
				}
			}
		}
	}
}
