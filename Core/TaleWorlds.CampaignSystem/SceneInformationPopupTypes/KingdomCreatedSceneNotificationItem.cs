using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	// Token: 0x020000BB RID: 187
	public class KingdomCreatedSceneNotificationItem : SceneNotificationData
	{
		// Token: 0x17000518 RID: 1304
		// (get) Token: 0x0600121A RID: 4634 RVA: 0x00052CB9 File Offset: 0x00050EB9
		public Kingdom NewKingdom { get; }

		// Token: 0x17000519 RID: 1305
		// (get) Token: 0x0600121B RID: 4635 RVA: 0x00052CC1 File Offset: 0x00050EC1
		public override string SceneID
		{
			get
			{
				return "scn_kingdom_made";
			}
		}

		// Token: 0x1700051A RID: 1306
		// (get) Token: 0x0600121C RID: 4636 RVA: 0x00052CC8 File Offset: 0x00050EC8
		public override bool PauseActiveState
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700051B RID: 1307
		// (get) Token: 0x0600121D RID: 4637 RVA: 0x00052CCC File Offset: 0x00050ECC
		public override TextObject TitleText
		{
			get
			{
				GameTexts.SetVariable("KINGDOM_NAME", this.NewKingdom.Name);
				GameTexts.SetVariable("DAY_OF_YEAR", CampaignSceneNotificationHelper.GetFormalDayAndSeasonText(this._creationCampaignTime));
				GameTexts.SetVariable("YEAR", this._creationCampaignTime.GetYear);
				GameTexts.SetVariable("LEADER_NAME", this.NewKingdom.Leader.Name);
				return GameTexts.FindText("str_kingdom_created", null);
			}
		}

		// Token: 0x1700051C RID: 1308
		// (get) Token: 0x0600121E RID: 4638 RVA: 0x00052D40 File Offset: 0x00050F40
		public override TextObject AffirmativeText
		{
			get
			{
				return GameTexts.FindText("str_ok", null);
			}
		}

		// Token: 0x0600121F RID: 4639 RVA: 0x00052D4D File Offset: 0x00050F4D
		public override IEnumerable<Banner> GetBanners()
		{
			return new List<Banner>
			{
				this.NewKingdom.Banner,
				this.NewKingdom.Banner
			};
		}

		// Token: 0x06001220 RID: 4640 RVA: 0x00052D78 File Offset: 0x00050F78
		public override IEnumerable<SceneNotificationData.SceneNotificationCharacter> GetSceneNotificationCharacters()
		{
			List<SceneNotificationData.SceneNotificationCharacter> list = new List<SceneNotificationData.SceneNotificationCharacter>();
			Hero leader = this.NewKingdom.Leader;
			Equipment equipment = leader.BattleEquipment.Clone(false);
			CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment, true, false);
			list.Add(CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(leader, equipment, false, default(BodyProperties), uint.MaxValue, uint.MaxValue, false));
			foreach (Hero hero in CampaignSceneNotificationHelper.GetMilitaryAudienceForKingdom(this.NewKingdom, false).Take(5))
			{
				Equipment equipment2 = hero.CivilianEquipment.Clone(false);
				CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment2, true, false);
				list.Add(CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(hero, equipment2, false, default(BodyProperties), uint.MaxValue, uint.MaxValue, false));
			}
			return list;
		}

		// Token: 0x06001221 RID: 4641 RVA: 0x00052E48 File Offset: 0x00051048
		public KingdomCreatedSceneNotificationItem(Kingdom newKingdom)
		{
			this.NewKingdom = newKingdom;
			this._creationCampaignTime = CampaignTime.Now;
		}

		// Token: 0x04000658 RID: 1624
		private const int NumberOfKingdomMemberAudience = 5;

		// Token: 0x0400065A RID: 1626
		private readonly CampaignTime _creationCampaignTime;
	}
}
