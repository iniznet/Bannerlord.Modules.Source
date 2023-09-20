using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	public class MainHeroBattleVictoryDeathNotificationItem : SceneNotificationData
	{
		public Hero DeadHero { get; }

		public List<CharacterObject> EncounterAllyCharacters { get; }

		public override string SceneID
		{
			get
			{
				return "scn_cutscene_main_hero_battle_victory_death";
			}
		}

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

		public MainHeroBattleVictoryDeathNotificationItem(Hero deadHero, List<CharacterObject> encounterAllyCharacters)
		{
			this.DeadHero = deadHero;
			this.EncounterAllyCharacters = encounterAllyCharacters;
			this._creationCampaignTime = CampaignTime.Now;
		}

		private const int NumberOfCorpses = 2;

		private const int NumberOfCompanions = 3;

		private readonly CampaignTime _creationCampaignTime;
	}
}
