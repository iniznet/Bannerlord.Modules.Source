using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	// Token: 0x020000AE RID: 174
	public class DeathOldAgeSceneNotificationItem : SceneNotificationData
	{
		// Token: 0x170004E2 RID: 1250
		// (get) Token: 0x060011C0 RID: 4544 RVA: 0x00051642 File Offset: 0x0004F842
		public Hero DeadHero { get; }

		// Token: 0x170004E3 RID: 1251
		// (get) Token: 0x060011C1 RID: 4545 RVA: 0x0005164A File Offset: 0x0004F84A
		public override string SceneID
		{
			get
			{
				return "scn_cutscene_death_old_age";
			}
		}

		// Token: 0x170004E4 RID: 1252
		// (get) Token: 0x060011C2 RID: 4546 RVA: 0x00051654 File Offset: 0x0004F854
		public override TextObject TitleText
		{
			get
			{
				GameTexts.SetVariable("DAY_OF_YEAR", CampaignSceneNotificationHelper.GetFormalDayAndSeasonText(this._creationCampaignTime));
				GameTexts.SetVariable("YEAR", this._creationCampaignTime.GetYear);
				GameTexts.SetVariable("NAME", this.DeadHero.Name);
				return GameTexts.FindText("str_died_of_old_age", null);
			}
		}

		// Token: 0x060011C3 RID: 4547 RVA: 0x000516AE File Offset: 0x0004F8AE
		public override IEnumerable<Banner> GetBanners()
		{
			return new List<Banner> { this.DeadHero.ClanBanner };
		}

		// Token: 0x060011C4 RID: 4548 RVA: 0x000516C8 File Offset: 0x0004F8C8
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

		// Token: 0x060011C5 RID: 4549 RVA: 0x00051794 File Offset: 0x0004F994
		public DeathOldAgeSceneNotificationItem(Hero deadHero)
		{
			this.DeadHero = deadHero;
			this._creationCampaignTime = CampaignTime.Now;
		}

		// Token: 0x0400062D RID: 1581
		private const int NumberOfAudienceHeroes = 5;

		// Token: 0x0400062F RID: 1583
		private readonly CampaignTime _creationCampaignTime;
	}
}
