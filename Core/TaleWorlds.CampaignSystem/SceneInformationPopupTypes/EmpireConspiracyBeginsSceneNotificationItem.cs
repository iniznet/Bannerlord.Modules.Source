using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	// Token: 0x020000B0 RID: 176
	public class EmpireConspiracyBeginsSceneNotificationItem : SceneNotificationData
	{
		// Token: 0x170004E8 RID: 1256
		// (get) Token: 0x060011CD RID: 4557 RVA: 0x00051A71 File Offset: 0x0004FC71
		public Hero PlayerHero { get; }

		// Token: 0x170004E9 RID: 1257
		// (get) Token: 0x060011CE RID: 4558 RVA: 0x00051A79 File Offset: 0x0004FC79
		public Kingdom Empire { get; }

		// Token: 0x170004EA RID: 1258
		// (get) Token: 0x060011CF RID: 4559 RVA: 0x00051A81 File Offset: 0x0004FC81
		public bool IsConspiracyAgainstEmpire { get; }

		// Token: 0x170004EB RID: 1259
		// (get) Token: 0x060011D0 RID: 4560 RVA: 0x00051A89 File Offset: 0x0004FC89
		public override string SceneID
		{
			get
			{
				return "scn_empire_conspiracy_start_notification";
			}
		}

		// Token: 0x170004EC RID: 1260
		// (get) Token: 0x060011D1 RID: 4561 RVA: 0x00051A90 File Offset: 0x0004FC90
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

		// Token: 0x060011D2 RID: 4562 RVA: 0x00051AE9 File Offset: 0x0004FCE9
		public override IEnumerable<Banner> GetBanners()
		{
			return new List<Banner> { this.Empire.Banner };
		}

		// Token: 0x060011D3 RID: 4563 RVA: 0x00051B04 File Offset: 0x0004FD04
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

		// Token: 0x060011D4 RID: 4564 RVA: 0x00051BA8 File Offset: 0x0004FDA8
		public EmpireConspiracyBeginsSceneNotificationItem(Hero playerHero, Kingdom empire, bool isConspiracyAgainstEmpire)
		{
			this.PlayerHero = playerHero;
			this.Empire = empire;
			this.IsConspiracyAgainstEmpire = isConspiracyAgainstEmpire;
			this._creationCampaignTime = CampaignTime.Now;
		}

		// Token: 0x060011D5 RID: 4565 RVA: 0x00051BE8 File Offset: 0x0004FDE8
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

		// Token: 0x04000633 RID: 1587
		private const int AudienceNumber = 8;

		// Token: 0x04000634 RID: 1588
		private readonly uint[] _audienceColors = new uint[] { 4278914065U, 4284308292U, 4281543757U, 4282199842U };

		// Token: 0x04000638 RID: 1592
		private readonly CampaignTime _creationCampaignTime;
	}
}
