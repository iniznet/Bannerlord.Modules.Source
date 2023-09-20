using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	public class KingdomDestroyedSceneNotificationItem : SceneNotificationData
	{
		public Kingdom DestroyedKingdom { get; }

		public override string SceneID
		{
			get
			{
				return "scn_cutscene_enemykingdom_destroyed";
			}
		}

		public override TextObject TitleText
		{
			get
			{
				GameTexts.SetVariable("DAY_OF_YEAR", CampaignSceneNotificationHelper.GetFormalDayAndSeasonText(this._creationCampaignTime));
				GameTexts.SetVariable("YEAR", this._creationCampaignTime.GetYear);
				GameTexts.SetVariable("FORMAL_NAME", CampaignSceneNotificationHelper.GetFormalNameForKingdom(this.DestroyedKingdom));
				return GameTexts.FindText("str_kingdom_destroyed_scene_notification", null);
			}
		}

		public override IEnumerable<Banner> GetBanners()
		{
			return new List<Banner> { this.DestroyedKingdom.Banner };
		}

		public override IEnumerable<SceneNotificationData.SceneNotificationCharacter> GetSceneNotificationCharacters()
		{
			List<SceneNotificationData.SceneNotificationCharacter> list = new List<SceneNotificationData.SceneNotificationCharacter>();
			for (int i = 0; i < 2; i++)
			{
				CharacterObject randomTroopForCulture = CampaignSceneNotificationHelper.GetRandomTroopForCulture(this.DestroyedKingdom.Culture);
				Equipment equipment = randomTroopForCulture.FirstBattleEquipment.Clone(false);
				CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment, false, false);
				BodyProperties bodyProperties = randomTroopForCulture.GetBodyProperties(equipment, MBRandom.RandomInt(100));
				list.Add(new SceneNotificationData.SceneNotificationCharacter(randomTroopForCulture, equipment, bodyProperties, false, uint.MaxValue, uint.MaxValue, false));
			}
			return list;
		}

		public KingdomDestroyedSceneNotificationItem(Kingdom destroyedKingdom)
		{
			this.DestroyedKingdom = destroyedKingdom;
			this._creationCampaignTime = CampaignTime.Now;
		}

		private const int NumberOfDeadTroops = 2;

		private readonly CampaignTime _creationCampaignTime;
	}
}
