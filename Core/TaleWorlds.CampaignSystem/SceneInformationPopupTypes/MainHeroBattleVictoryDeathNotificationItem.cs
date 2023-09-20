using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	// Token: 0x020000BE RID: 190
	public class MainHeroBattleVictoryDeathNotificationItem : SceneNotificationData
	{
		// Token: 0x17000524 RID: 1316
		// (get) Token: 0x0600122E RID: 4654 RVA: 0x000530CB File Offset: 0x000512CB
		public Hero DeadHero { get; }

		// Token: 0x17000525 RID: 1317
		// (get) Token: 0x0600122F RID: 4655 RVA: 0x000530D3 File Offset: 0x000512D3
		public List<CharacterObject> EncounterAllyCharacters { get; }

		// Token: 0x17000526 RID: 1318
		// (get) Token: 0x06001230 RID: 4656 RVA: 0x000530DB File Offset: 0x000512DB
		public override string SceneID
		{
			get
			{
				return "scn_cutscene_main_hero_battle_victory_death";
			}
		}

		// Token: 0x17000527 RID: 1319
		// (get) Token: 0x06001231 RID: 4657 RVA: 0x000530E4 File Offset: 0x000512E4
		public override TextObject TitleText
		{
			get
			{
				GameTexts.SetVariable("DAY_OF_YEAR", CampaignSceneNotificationHelper.GetFormalDayAndSeasonText(this._creationCampaignTime));
				GameTexts.SetVariable("YEAR", this._creationCampaignTime.GetYear);
				GameTexts.SetVariable("NAME", this.DeadHero.Name);
				return GameTexts.FindText("str_main_hero_battle_death", null);
			}
		}

		// Token: 0x06001232 RID: 4658 RVA: 0x00053140 File Offset: 0x00051340
		public override IEnumerable<SceneNotificationData.SceneNotificationCharacter> GetSceneNotificationCharacters()
		{
			List<SceneNotificationData.SceneNotificationCharacter> list = new List<SceneNotificationData.SceneNotificationCharacter>();
			Equipment equipment = this.DeadHero.BattleEquipment.Clone(false);
			CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment, true, false);
			list.Add(CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(this.DeadHero, equipment, false, default(BodyProperties), uint.MaxValue, uint.MaxValue, false));
			for (int i = 0; i < 2; i++)
			{
				CharacterObject randomTroopForCulture = CampaignSceneNotificationHelper.GetRandomTroopForCulture(this.DeadHero.MapFaction.Culture);
				Equipment equipment2 = randomTroopForCulture.FirstBattleEquipment.Clone(false);
				CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment2, false, false);
				BodyProperties bodyProperties = randomTroopForCulture.GetBodyProperties(equipment2, MBRandom.RandomInt(100));
				list.Add(new SceneNotificationData.SceneNotificationCharacter(randomTroopForCulture, equipment2, bodyProperties, false, uint.MaxValue, uint.MaxValue, false));
			}
			List<CharacterObject> encounterAllyCharacters = this.EncounterAllyCharacters;
			foreach (CharacterObject characterObject in ((encounterAllyCharacters != null) ? encounterAllyCharacters.Take(3) : null))
			{
				if (characterObject.IsHero)
				{
					Equipment equipment3 = characterObject.HeroObject.BattleEquipment.Clone(false);
					CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment3, false, false);
					list.Add(CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(characterObject.HeroObject, equipment3, false, default(BodyProperties), uint.MaxValue, uint.MaxValue, false));
				}
				else
				{
					Equipment equipment4 = characterObject.FirstBattleEquipment.Clone(false);
					CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment4, false, false);
					list.Add(new SceneNotificationData.SceneNotificationCharacter(characterObject, equipment4, default(BodyProperties), false, uint.MaxValue, uint.MaxValue, false));
				}
			}
			return list;
		}

		// Token: 0x06001233 RID: 4659 RVA: 0x000532C0 File Offset: 0x000514C0
		public MainHeroBattleVictoryDeathNotificationItem(Hero deadHero, List<CharacterObject> encounterAllyCharacters)
		{
			this.DeadHero = deadHero;
			this.EncounterAllyCharacters = encounterAllyCharacters;
			this._creationCampaignTime = CampaignTime.Now;
		}

		// Token: 0x04000662 RID: 1634
		private const int NumberOfCorpses = 2;

		// Token: 0x04000663 RID: 1635
		private const int NumberOfCompanions = 3;

		// Token: 0x04000666 RID: 1638
		private readonly CampaignTime _creationCampaignTime;
	}
}
