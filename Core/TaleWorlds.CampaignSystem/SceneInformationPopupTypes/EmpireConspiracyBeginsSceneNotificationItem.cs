using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	public class EmpireConspiracyBeginsSceneNotificationItem : SceneNotificationData
	{
		public Hero PlayerHero { get; }

		public Kingdom Empire { get; }

		public bool IsConspiracyAgainstEmpire { get; }

		public override string SceneID
		{
			get
			{
				return "scn_empire_conspiracy_start_notification";
			}
		}

		public override TextObject TitleText
		{
			get
			{
				GameTexts.SetVariable("DAY_OF_YEAR", CampaignSceneNotificationHelper.GetFormalDayAndSeasonText(this._creationCampaignTime));
				GameTexts.SetVariable("YEAR", this._creationCampaignTime.GetYear);
				if (this.IsConspiracyAgainstEmpire)
				{
					return GameTexts.FindText("str_empire_conspiracy_begins_antiempire", null);
				}
				return GameTexts.FindText("str_empire_conspiracy_begins_proempire", null);
			}
		}

		public override IEnumerable<Banner> GetBanners()
		{
			return new List<Banner> { this.Empire.Banner };
		}

		public override IEnumerable<SceneNotificationData.SceneNotificationCharacter> GetSceneNotificationCharacters()
		{
			List<SceneNotificationData.SceneNotificationCharacter> list = new List<SceneNotificationData.SceneNotificationCharacter>();
			for (int i = 0; i < 8; i++)
			{
				Equipment equipment = MBObjectManager.Instance.GetObject<MBEquipmentRoster>("conspirator_cutscene_template").DefaultEquipment.Clone(false);
				CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment, false, false);
				CharacterObject facePropertiesFromAudienceIndex = this.GetFacePropertiesFromAudienceIndex(false, i);
				BodyProperties bodyProperties = facePropertiesFromAudienceIndex.GetBodyProperties(equipment, MBRandom.RandomInt(100));
				uint num = this._audienceColors[MBRandom.RandomInt(this._audienceColors.Length)];
				uint num2 = this._audienceColors[MBRandom.RandomInt(this._audienceColors.Length)];
				list.Add(new SceneNotificationData.SceneNotificationCharacter(facePropertiesFromAudienceIndex, equipment, bodyProperties, false, num, num2, false));
			}
			return list;
		}

		public EmpireConspiracyBeginsSceneNotificationItem(Hero playerHero, Kingdom empire, bool isConspiracyAgainstEmpire)
		{
			this.PlayerHero = playerHero;
			this.Empire = empire;
			this.IsConspiracyAgainstEmpire = isConspiracyAgainstEmpire;
			this._creationCampaignTime = CampaignTime.Now;
		}

		private CharacterObject GetFacePropertiesFromAudienceIndex(bool playerWantsRestore, int audienceMemberIndex)
		{
			if (!playerWantsRestore)
			{
				return MBObjectManager.Instance.GetObject<CharacterObject>("villager_empire");
			}
			string text;
			switch (audienceMemberIndex % 8)
			{
			case 0:
				text = "villager_battania";
				break;
			case 1:
				text = "villager_khuzait";
				break;
			case 2:
				text = "villager_vlandia";
				break;
			case 3:
				text = "villager_aserai";
				break;
			case 4:
				text = "villager_battania";
				break;
			case 5:
				text = "villager_sturgia";
				break;
			default:
				text = "villager_battania";
				break;
			}
			return MBObjectManager.Instance.GetObject<CharacterObject>(text);
		}

		private const int AudienceNumber = 8;

		private readonly uint[] _audienceColors = new uint[] { 4278914065U, 4284308292U, 4281543757U, 4282199842U };

		private readonly CampaignTime _creationCampaignTime;
	}
}
