using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	// Token: 0x020000B7 RID: 183
	public class HeirComingOfAgeFemaleSceneNotificationItem : SceneNotificationData
	{
		// Token: 0x170004FE RID: 1278
		// (get) Token: 0x060011F2 RID: 4594 RVA: 0x000520B9 File Offset: 0x000502B9
		public Hero MentorHero { get; }

		// Token: 0x170004FF RID: 1279
		// (get) Token: 0x060011F3 RID: 4595 RVA: 0x000520C1 File Offset: 0x000502C1
		public Hero HeroCameOfAge { get; }

		// Token: 0x17000500 RID: 1280
		// (get) Token: 0x060011F4 RID: 4596 RVA: 0x000520C9 File Offset: 0x000502C9
		public override string SceneID
		{
			get
			{
				return "scn_hero_come_of_age_female";
			}
		}

		// Token: 0x17000501 RID: 1281
		// (get) Token: 0x060011F5 RID: 4597 RVA: 0x000520D0 File Offset: 0x000502D0
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

		// Token: 0x060011F6 RID: 4598 RVA: 0x0005212C File Offset: 0x0005032C
		public override IEnumerable<SceneNotificationData.SceneNotificationCharacter> GetSceneNotificationCharacters()
		{
			List<SceneNotificationData.SceneNotificationCharacter> list = new List<SceneNotificationData.SceneNotificationCharacter>();
			Equipment equipment = this.MentorHero.CivilianEquipment.Clone(false);
			CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment, false, false);
			list.Add(CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(this.MentorHero, equipment, false, default(BodyProperties), uint.MaxValue, uint.MaxValue, false));
			string childStageEquipmentIDFromCulture = CampaignSceneNotificationHelper.GetChildStageEquipmentIDFromCulture(this.HeroCameOfAge.Culture);
			Equipment equipment2 = MBObjectManager.Instance.GetObject<MBEquipmentRoster>(childStageEquipmentIDFromCulture).DefaultEquipment.Clone(false);
			BodyProperties bodyProperties = new BodyProperties(new DynamicBodyProperties(6f, this.HeroCameOfAge.Weight, this.HeroCameOfAge.Build), this.HeroCameOfAge.StaticBodyProperties);
			list.Add(CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(this.HeroCameOfAge, equipment2, false, bodyProperties, uint.MaxValue, uint.MaxValue, false));
			BodyProperties bodyProperties2 = new BodyProperties(new DynamicBodyProperties(14f, this.HeroCameOfAge.Weight, this.HeroCameOfAge.Build), this.HeroCameOfAge.StaticBodyProperties);
			list.Add(CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(this.HeroCameOfAge, equipment2, false, bodyProperties2, uint.MaxValue, uint.MaxValue, false));
			Equipment equipment3 = this.HeroCameOfAge.BattleEquipment.Clone(false);
			CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment3, false, false);
			list.Add(CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(this.HeroCameOfAge, equipment3, false, default(BodyProperties), uint.MaxValue, uint.MaxValue, false));
			return list;
		}

		// Token: 0x060011F7 RID: 4599 RVA: 0x00052271 File Offset: 0x00050471
		public HeirComingOfAgeFemaleSceneNotificationItem(Hero mentorHero, Hero heroCameOfAge)
		{
			this.MentorHero = mentorHero;
			this.HeroCameOfAge = heroCameOfAge;
			this._creationCampaignTime = CampaignTime.Now;
		}

		// Token: 0x04000644 RID: 1604
		private readonly CampaignTime _creationCampaignTime;
	}
}
