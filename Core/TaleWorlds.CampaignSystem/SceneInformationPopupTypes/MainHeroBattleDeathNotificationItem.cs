using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	// Token: 0x020000BD RID: 189
	public class MainHeroBattleDeathNotificationItem : SceneNotificationData
	{
		// Token: 0x17000520 RID: 1312
		// (get) Token: 0x06001228 RID: 4648 RVA: 0x00052F6C File Offset: 0x0005116C
		public Hero DeadHero { get; }

		// Token: 0x17000521 RID: 1313
		// (get) Token: 0x06001229 RID: 4649 RVA: 0x00052F74 File Offset: 0x00051174
		public CultureObject KillerCulture { get; }

		// Token: 0x17000522 RID: 1314
		// (get) Token: 0x0600122A RID: 4650 RVA: 0x00052F7C File Offset: 0x0005117C
		public override string SceneID
		{
			get
			{
				return "scn_cutscene_main_hero_battle_death";
			}
		}

		// Token: 0x17000523 RID: 1315
		// (get) Token: 0x0600122B RID: 4651 RVA: 0x00052F84 File Offset: 0x00051184
		public override TextObject TitleText
		{
			get
			{
				GameTexts.SetVariable("DAY_OF_YEAR", CampaignSceneNotificationHelper.GetFormalDayAndSeasonText(this._creationCampaignTime));
				GameTexts.SetVariable("YEAR", this._creationCampaignTime.GetYear);
				GameTexts.SetVariable("NAME", this.DeadHero.Name);
				return GameTexts.FindText("str_main_hero_battle_death", null);
			}
		}

		// Token: 0x0600122C RID: 4652 RVA: 0x00052FE0 File Offset: 0x000511E0
		public override IEnumerable<SceneNotificationData.SceneNotificationCharacter> GetSceneNotificationCharacters()
		{
			List<SceneNotificationData.SceneNotificationCharacter> list = new List<SceneNotificationData.SceneNotificationCharacter>();
			Equipment equipment = this.DeadHero.BattleEquipment.Clone(false);
			CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment, true, false);
			list.Add(CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(this.DeadHero, equipment, false, default(BodyProperties), uint.MaxValue, uint.MaxValue, false));
			for (int i = 0; i < 23; i++)
			{
				CharacterObject randomTroopForCulture = CampaignSceneNotificationHelper.GetRandomTroopForCulture((this.KillerCulture != null && (float)i > 11.5f) ? this.KillerCulture : this.DeadHero.MapFaction.Culture);
				Equipment equipment2 = randomTroopForCulture.FirstBattleEquipment.Clone(false);
				CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment2, false, false);
				BodyProperties bodyProperties = randomTroopForCulture.GetBodyProperties(equipment2, MBRandom.RandomInt(100));
				list.Add(new SceneNotificationData.SceneNotificationCharacter(randomTroopForCulture, equipment2, bodyProperties, false, uint.MaxValue, uint.MaxValue, false));
			}
			return list;
		}

		// Token: 0x0600122D RID: 4653 RVA: 0x000530AA File Offset: 0x000512AA
		public MainHeroBattleDeathNotificationItem(Hero deadHero, CultureObject killerCulture = null)
		{
			this.DeadHero = deadHero;
			this.KillerCulture = killerCulture;
			this._creationCampaignTime = CampaignTime.Now;
		}

		// Token: 0x0400065E RID: 1630
		private const int NumberOfCorpses = 23;

		// Token: 0x04000661 RID: 1633
		private readonly CampaignTime _creationCampaignTime;
	}
}
