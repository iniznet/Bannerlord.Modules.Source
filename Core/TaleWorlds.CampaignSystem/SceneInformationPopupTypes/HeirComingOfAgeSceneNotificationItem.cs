using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	// Token: 0x020000B8 RID: 184
	public class HeirComingOfAgeSceneNotificationItem : SceneNotificationData
	{
		// Token: 0x17000502 RID: 1282
		// (get) Token: 0x060011F8 RID: 4600 RVA: 0x00052292 File Offset: 0x00050492
		public Hero MentorHero { get; }

		// Token: 0x17000503 RID: 1283
		// (get) Token: 0x060011F9 RID: 4601 RVA: 0x0005229A File Offset: 0x0005049A
		public Hero HeroCameOfAge { get; }

		// Token: 0x17000504 RID: 1284
		// (get) Token: 0x060011FA RID: 4602 RVA: 0x000522A2 File Offset: 0x000504A2
		public override string SceneID
		{
			get
			{
				return "scn_cutscene_heir_coming_of_age";
			}
		}

		// Token: 0x17000505 RID: 1285
		// (get) Token: 0x060011FB RID: 4603 RVA: 0x000522AC File Offset: 0x000504AC
		public override TextObject TitleText
		{
			get
			{
				GameTexts.SetVariable("HERO_NAME", this.HeroCameOfAge.Name);
				GameTexts.SetVariable("DAY_OF_YEAR", CampaignSceneNotificationHelper.GetFormalDayAndSeasonText(this._creationCampaignTime));
				GameTexts.SetVariable("YEAR", this._creationCampaignTime.GetYear);
				return GameTexts.FindText("str_hero_came_of_age", null);
			}
		}

		// Token: 0x060011FC RID: 4604 RVA: 0x00052308 File Offset: 0x00050508
		public override IEnumerable<SceneNotificationData.SceneNotificationCharacter> GetSceneNotificationCharacters()
		{
			List<SceneNotificationData.SceneNotificationCharacter> list = new List<SceneNotificationData.SceneNotificationCharacter>();
			Equipment equipment = this.MentorHero.CivilianEquipment.Clone(false);
			CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment, true, false);
			list.Add(CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(this.MentorHero, equipment, false, default(BodyProperties), uint.MaxValue, uint.MaxValue, false));
			string childStageEquipmentIDFromCulture = CampaignSceneNotificationHelper.GetChildStageEquipmentIDFromCulture(this.HeroCameOfAge.Culture);
			Equipment equipment2 = MBObjectManager.Instance.GetObject<MBEquipmentRoster>(childStageEquipmentIDFromCulture).DefaultEquipment.Clone(false);
			BodyProperties bodyProperties = new BodyProperties(new DynamicBodyProperties(6f, this.HeroCameOfAge.Weight, this.HeroCameOfAge.Build), this.HeroCameOfAge.StaticBodyProperties);
			list.Add(CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(this.HeroCameOfAge, equipment2, false, bodyProperties, uint.MaxValue, uint.MaxValue, false));
			BodyProperties bodyProperties2 = new BodyProperties(new DynamicBodyProperties(14f, this.HeroCameOfAge.Weight, this.HeroCameOfAge.Build), this.HeroCameOfAge.StaticBodyProperties);
			list.Add(CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(this.HeroCameOfAge, equipment2, false, bodyProperties2, uint.MaxValue, uint.MaxValue, false));
			Equipment equipment3 = this.HeroCameOfAge.BattleEquipment.Clone(false);
			CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment3, true, false);
			list.Add(CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(this.HeroCameOfAge, equipment3, false, default(BodyProperties), uint.MaxValue, uint.MaxValue, false));
			return list;
		}

		// Token: 0x060011FD RID: 4605 RVA: 0x0005244D File Offset: 0x0005064D
		public HeirComingOfAgeSceneNotificationItem(Hero mentorHero, Hero heroCameOfAge)
		{
			this.MentorHero = mentorHero;
			this.HeroCameOfAge = heroCameOfAge;
			this._creationCampaignTime = CampaignTime.Now;
		}

		// Token: 0x04000647 RID: 1607
		private readonly CampaignTime _creationCampaignTime;
	}
}
