using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	// Token: 0x020000AC RID: 172
	public class ClanMemberPeaceDeathSceneNotificationItem : SceneNotificationData
	{
		// Token: 0x170004DC RID: 1244
		// (get) Token: 0x060011B4 RID: 4532 RVA: 0x0005136C File Offset: 0x0004F56C
		public Hero DeadHero { get; }

		// Token: 0x170004DD RID: 1245
		// (get) Token: 0x060011B5 RID: 4533 RVA: 0x00051374 File Offset: 0x0004F574
		public override string SceneID
		{
			get
			{
				return "scn_cutscene_family_member_death";
			}
		}

		// Token: 0x170004DE RID: 1246
		// (get) Token: 0x060011B6 RID: 4534 RVA: 0x0005137C File Offset: 0x0004F57C
		public override TextObject TitleText
		{
			get
			{
				GameTexts.SetVariable("DAY_OF_YEAR", CampaignSceneNotificationHelper.GetFormalDayAndSeasonText(this._creationCampaignTime));
				GameTexts.SetVariable("YEAR", this._creationCampaignTime.GetYear);
				GameTexts.SetVariable("NAME", this.DeadHero.Name);
				return GameTexts.FindText("str_family_member_death", null);
			}
		}

		// Token: 0x060011B7 RID: 4535 RVA: 0x000513D8 File Offset: 0x0004F5D8
		public override IEnumerable<SceneNotificationData.SceneNotificationCharacter> GetSceneNotificationCharacters()
		{
			Equipment equipment = this.DeadHero.CivilianEquipment.Clone(false);
			List<SceneNotificationData.SceneNotificationCharacter> list = new List<SceneNotificationData.SceneNotificationCharacter>();
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

		// Token: 0x060011B8 RID: 4536 RVA: 0x000514A4 File Offset: 0x0004F6A4
		public override IEnumerable<Banner> GetBanners()
		{
			return new List<Banner> { this.DeadHero.ClanBanner };
		}

		// Token: 0x060011B9 RID: 4537 RVA: 0x000514BC File Offset: 0x0004F6BC
		public ClanMemberPeaceDeathSceneNotificationItem(Hero deadHero)
		{
			this.DeadHero = deadHero;
			this._creationCampaignTime = CampaignTime.Now;
		}

		// Token: 0x04000627 RID: 1575
		private const int NumberOfAudienceHeroes = 5;

		// Token: 0x04000629 RID: 1577
		private readonly CampaignTime _creationCampaignTime;
	}
}
