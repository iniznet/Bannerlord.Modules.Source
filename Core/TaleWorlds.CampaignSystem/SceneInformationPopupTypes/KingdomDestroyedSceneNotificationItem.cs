using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	// Token: 0x020000BC RID: 188
	public class KingdomDestroyedSceneNotificationItem : SceneNotificationData
	{
		// Token: 0x1700051D RID: 1309
		// (get) Token: 0x06001222 RID: 4642 RVA: 0x00052E62 File Offset: 0x00051062
		public Kingdom DestroyedKingdom { get; }

		// Token: 0x1700051E RID: 1310
		// (get) Token: 0x06001223 RID: 4643 RVA: 0x00052E6A File Offset: 0x0005106A
		public override string SceneID
		{
			get
			{
				return "scn_cutscene_enemykingdom_destroyed";
			}
		}

		// Token: 0x1700051F RID: 1311
		// (get) Token: 0x06001224 RID: 4644 RVA: 0x00052E74 File Offset: 0x00051074
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

		// Token: 0x06001225 RID: 4645 RVA: 0x00052ECE File Offset: 0x000510CE
		public override IEnumerable<Banner> GetBanners()
		{
			return new List<Banner> { this.DestroyedKingdom.Banner };
		}

		// Token: 0x06001226 RID: 4646 RVA: 0x00052EE8 File Offset: 0x000510E8
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

		// Token: 0x06001227 RID: 4647 RVA: 0x00052F52 File Offset: 0x00051152
		public KingdomDestroyedSceneNotificationItem(Kingdom destroyedKingdom)
		{
			this.DestroyedKingdom = destroyedKingdom;
			this._creationCampaignTime = CampaignTime.Now;
		}

		// Token: 0x0400065B RID: 1627
		private const int NumberOfDeadTroops = 2;

		// Token: 0x0400065D RID: 1629
		private readonly CampaignTime _creationCampaignTime;
	}
}
