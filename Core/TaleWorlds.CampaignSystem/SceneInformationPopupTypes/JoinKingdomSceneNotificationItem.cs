using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	// Token: 0x020000BA RID: 186
	public class JoinKingdomSceneNotificationItem : SceneNotificationData
	{
		// Token: 0x17000513 RID: 1299
		// (get) Token: 0x06001212 RID: 4626 RVA: 0x00052B14 File Offset: 0x00050D14
		public Clan NewMemberClan { get; }

		// Token: 0x17000514 RID: 1300
		// (get) Token: 0x06001213 RID: 4627 RVA: 0x00052B1C File Offset: 0x00050D1C
		public Kingdom KingdomToUse { get; }

		// Token: 0x17000515 RID: 1301
		// (get) Token: 0x06001214 RID: 4628 RVA: 0x00052B24 File Offset: 0x00050D24
		public override string SceneID
		{
			get
			{
				return "scn_cutscene_factionjoin";
			}
		}

		// Token: 0x17000516 RID: 1302
		// (get) Token: 0x06001215 RID: 4629 RVA: 0x00052B2B File Offset: 0x00050D2B
		public override SceneNotificationData.RelevantContextType RelevantContext
		{
			get
			{
				return SceneNotificationData.RelevantContextType.Map;
			}
		}

		// Token: 0x17000517 RID: 1303
		// (get) Token: 0x06001216 RID: 4630 RVA: 0x00052B30 File Offset: 0x00050D30
		public override TextObject TitleText
		{
			get
			{
				GameTexts.SetVariable("CLAN_NAME", this.NewMemberClan.Name);
				GameTexts.SetVariable("DAY_OF_YEAR", CampaignSceneNotificationHelper.GetFormalDayAndSeasonText(this._creationCampaignTime));
				GameTexts.SetVariable("YEAR", this._creationCampaignTime.GetYear);
				GameTexts.SetVariable("KINGDOM_FORMALNAME", CampaignSceneNotificationHelper.GetFormalNameForKingdom(this.KingdomToUse));
				return GameTexts.FindText("str_new_faction_member", null);
			}
		}

		// Token: 0x06001217 RID: 4631 RVA: 0x00052B9F File Offset: 0x00050D9F
		public override IEnumerable<Banner> GetBanners()
		{
			return new List<Banner>
			{
				this.KingdomToUse.Banner,
				this.KingdomToUse.Banner
			};
		}

		// Token: 0x06001218 RID: 4632 RVA: 0x00052BC8 File Offset: 0x00050DC8
		public override IEnumerable<SceneNotificationData.SceneNotificationCharacter> GetSceneNotificationCharacters()
		{
			List<SceneNotificationData.SceneNotificationCharacter> list = new List<SceneNotificationData.SceneNotificationCharacter>();
			Hero leader = this.NewMemberClan.Leader;
			Equipment equipment = leader.CivilianEquipment.Clone(false);
			CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment, true, false);
			list.Add(CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(leader, equipment, false, default(BodyProperties), uint.MaxValue, uint.MaxValue, false));
			foreach (Hero hero in CampaignSceneNotificationHelper.GetMilitaryAudienceForKingdom(this.KingdomToUse, true).Take(5))
			{
				Equipment equipment2 = hero.CivilianEquipment.Clone(false);
				CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment2, true, false);
				list.Add(CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(hero, equipment2, false, default(BodyProperties), uint.MaxValue, uint.MaxValue, false));
			}
			return list;
		}

		// Token: 0x06001219 RID: 4633 RVA: 0x00052C98 File Offset: 0x00050E98
		public JoinKingdomSceneNotificationItem(Clan newMember, Kingdom kingdom)
		{
			this.NewMemberClan = newMember;
			this.KingdomToUse = kingdom;
			this._creationCampaignTime = CampaignTime.Now;
		}

		// Token: 0x04000654 RID: 1620
		private const int NumberOfKingdomMembers = 5;

		// Token: 0x04000657 RID: 1623
		private readonly CampaignTime _creationCampaignTime;
	}
}
