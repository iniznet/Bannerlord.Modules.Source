using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	public abstract class EmpireConspiracySupportsSceneNotificationItemBase : SceneNotificationData
	{
		public Hero King { get; }

		public override string SceneID
		{
			get
			{
				return "scn_empire_conspiracy_supports_notification";
			}
		}

		public override TextObject AffirmativeText
		{
			get
			{
				return GameTexts.FindText("str_ok", null);
			}
		}

		public override IEnumerable<Banner> GetBanners()
		{
			return new List<Banner>
			{
				this.King.MapFaction.Banner,
				this.King.MapFaction.Banner
			};
		}

		public override IEnumerable<SceneNotificationData.SceneNotificationCharacter> GetSceneNotificationCharacters()
		{
			List<SceneNotificationData.SceneNotificationCharacter> list = new List<SceneNotificationData.SceneNotificationCharacter>();
			Equipment equipment = this.King.CivilianEquipment.Clone(false);
			CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment, false, false);
			list.Add(CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(this.King, equipment, false, default(BodyProperties), uint.MaxValue, uint.MaxValue, false));
			CharacterObject @object = MBObjectManager.Instance.GetObject<CharacterObject>("villager_battania");
			Equipment equipment2 = MBObjectManager.Instance.GetObject<MBEquipmentRoster>("conspirator_cutscene_template").DefaultEquipment.Clone(false);
			CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment2, false, false);
			BodyProperties bodyProperties = @object.GetBodyProperties(equipment2, MBRandom.RandomInt(100));
			list.Add(new SceneNotificationData.SceneNotificationCharacter(@object, equipment2, bodyProperties, false, 0U, 0U, false));
			bodyProperties = @object.GetBodyProperties(equipment2, MBRandom.RandomInt(100));
			list.Add(new SceneNotificationData.SceneNotificationCharacter(@object, equipment2, bodyProperties, false, 0U, 0U, false));
			bodyProperties = @object.GetBodyProperties(equipment2, MBRandom.RandomInt(100));
			list.Add(new SceneNotificationData.SceneNotificationCharacter(@object, equipment2, bodyProperties, false, 0U, 0U, false));
			list.Add(CampaignSceneNotificationHelper.GetBodyguardOfCulture(this.King.MapFaction.Culture));
			list.Add(CampaignSceneNotificationHelper.GetBodyguardOfCulture(this.King.MapFaction.Culture));
			return list;
		}

		protected EmpireConspiracySupportsSceneNotificationItemBase(Hero kingHero)
		{
			this.King = kingHero;
		}
	}
}
