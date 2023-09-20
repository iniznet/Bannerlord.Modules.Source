using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	public class MainHeroBattleDeathNotificationItem : SceneNotificationData
	{
		public Hero DeadHero { get; }

		public CultureObject KillerCulture { get; }

		public override string SceneID
		{
			get
			{
				return "scn_cutscene_main_hero_battle_death";
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
			for (int i = 0; i < 23; i++)
			{
				CharacterObject randomTroopForCulture = CampaignSceneNotificationHelper.GetRandomTroopForCulture((this.KillerCulture != null && (float)i > 11.5f) ? this.KillerCulture : this.DeadHero.MapFaction.Culture);
				Equipment equipment2 = randomTroopForCulture.FirstBattleEquipment.Clone(false);
				CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment2, false, false);
				BodyProperties bodyProperties = randomTroopForCulture.GetBodyProperties(equipment2, MBRandom.RandomInt(100));
				list.Add(new SceneNotificationData.SceneNotificationCharacter(randomTroopForCulture, equipment2, bodyProperties, false, uint.MaxValue, uint.MaxValue, false));
			}
			return list;
		}

		public MainHeroBattleDeathNotificationItem(Hero deadHero, CultureObject killerCulture = null)
		{
			this.DeadHero = deadHero;
			this.KillerCulture = killerCulture;
			this._creationCampaignTime = CampaignTime.Now;
		}

		private const int NumberOfCorpses = 23;

		private readonly CampaignTime _creationCampaignTime;
	}
}
