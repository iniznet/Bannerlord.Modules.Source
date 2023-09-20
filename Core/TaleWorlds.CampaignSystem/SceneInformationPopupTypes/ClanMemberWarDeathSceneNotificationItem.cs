using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	// Token: 0x020000AD RID: 173
	public class ClanMemberWarDeathSceneNotificationItem : SceneNotificationData
	{
		// Token: 0x170004DF RID: 1247
		// (get) Token: 0x060011BA RID: 4538 RVA: 0x000514D6 File Offset: 0x0004F6D6
		public Hero DeadHero { get; }

		// Token: 0x170004E0 RID: 1248
		// (get) Token: 0x060011BB RID: 4539 RVA: 0x000514DE File Offset: 0x0004F6DE
		public override string SceneID
		{
			get
			{
				return "scn_cutscene_family_member_death_war";
			}
		}

		// Token: 0x170004E1 RID: 1249
		// (get) Token: 0x060011BC RID: 4540 RVA: 0x000514E8 File Offset: 0x0004F6E8
		public override TextObject TitleText
		{
			get
			{
				GameTexts.SetVariable("DAY_OF_YEAR", CampaignSceneNotificationHelper.GetFormalDayAndSeasonText(this._creationCampaignTime));
				GameTexts.SetVariable("YEAR", this._creationCampaignTime.GetYear);
				GameTexts.SetVariable("NAME", this.DeadHero.Name);
				return GameTexts.FindText("str_family_member_death_war", null);
			}
		}

		// Token: 0x060011BD RID: 4541 RVA: 0x00051542 File Offset: 0x0004F742
		public override IEnumerable<Banner> GetBanners()
		{
			return new List<Banner> { this.DeadHero.ClanBanner };
		}

		// Token: 0x060011BE RID: 4542 RVA: 0x0005155C File Offset: 0x0004F75C
		public override IEnumerable<SceneNotificationData.SceneNotificationCharacter> GetSceneNotificationCharacters()
		{
			List<SceneNotificationData.SceneNotificationCharacter> list = new List<SceneNotificationData.SceneNotificationCharacter>();
			Equipment equipment = this.DeadHero.CivilianEquipment.Clone(false);
			CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment, false, false);
			list.Add(CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(this.DeadHero, equipment, false, default(BodyProperties), uint.MaxValue, uint.MaxValue, false));
			foreach (Hero hero in CampaignSceneNotificationHelper.GetMilitaryAudienceForHero(this.DeadHero, true, false).Take(5))
			{
				Equipment equipment2 = hero.CivilianEquipment.Clone(false);
				CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment2, false, false);
				list.Add(CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(hero, equipment2, false, default(BodyProperties), uint.MaxValue, uint.MaxValue, false));
			}
			return list;
		}

		// Token: 0x060011BF RID: 4543 RVA: 0x00051628 File Offset: 0x0004F828
		public ClanMemberWarDeathSceneNotificationItem(Hero deadHero)
		{
			this.DeadHero = deadHero;
			this._creationCampaignTime = CampaignTime.Now;
		}

		// Token: 0x0400062A RID: 1578
		private const int NumberOfAudienceHeroes = 5;

		// Token: 0x0400062C RID: 1580
		private readonly CampaignTime _creationCampaignTime;
	}
}
