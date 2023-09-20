using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003C3 RID: 963
	public class PlayerVariablesBehavior : CampaignBehaviorBase
	{
		// Token: 0x060039C4 RID: 14788 RVA: 0x00109784 File Offset: 0x00107984
		public override void RegisterEvents()
		{
			CampaignEvents.PlayerDesertedBattleEvent.AddNonSerializedListener(this, new Action<int>(this.OnPlayerDesertedBattle));
			CampaignEvents.VillageLooted.AddNonSerializedListener(this, new Action<Village>(this.OnVillageLooted));
			CampaignEvents.OnPlayerBattleEndEvent.AddNonSerializedListener(this, new Action<MapEvent>(this.OnPlayerBattleEnd));
		}

		// Token: 0x060039C5 RID: 14789 RVA: 0x001097D6 File Offset: 0x001079D6
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x060039C6 RID: 14790 RVA: 0x001097D8 File Offset: 0x001079D8
		private void OnPlayerDesertedBattle(int sacrificedMenCount)
		{
			SkillLevelingManager.OnTacticsUsed(MobileParty.MainParty, (float)(sacrificedMenCount * 50));
			TraitLevelingHelper.OnTroopsSacrificed();
		}

		// Token: 0x060039C7 RID: 14791 RVA: 0x001097EE File Offset: 0x001079EE
		private void OnVillageLooted(Village village)
		{
			if (PlayerEncounter.Current != null && PlayerEncounter.PlayerIsAttacker && PlayerEncounter.EncounterSettlement != null && PlayerEncounter.EncounterSettlement.Village == village)
			{
				TraitLevelingHelper.OnVillageRaided();
			}
		}

		// Token: 0x060039C8 RID: 14792 RVA: 0x00109818 File Offset: 0x00107A18
		private void OnPlayerBattleEnd(MapEvent mapEvent)
		{
			float playerPartyContributionRate = (mapEvent.AttackerSide.IsMainPartyAmongParties() ? mapEvent.AttackerSide : mapEvent.DefenderSide).GetPlayerPartyContributionRate();
			TraitLevelingHelper.OnBattleWon(mapEvent, playerPartyContributionRate);
		}
	}
}
