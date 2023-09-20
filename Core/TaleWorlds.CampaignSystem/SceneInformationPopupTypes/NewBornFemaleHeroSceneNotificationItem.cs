using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	// Token: 0x020000C1 RID: 193
	public class NewBornFemaleHeroSceneNotificationItem : SceneNotificationData
	{
		// Token: 0x17000531 RID: 1329
		// (get) Token: 0x06001244 RID: 4676 RVA: 0x00053C55 File Offset: 0x00051E55
		public Hero MaleHero { get; }

		// Token: 0x17000532 RID: 1330
		// (get) Token: 0x06001245 RID: 4677 RVA: 0x00053C5D File Offset: 0x00051E5D
		public Hero FemaleHero { get; }

		// Token: 0x17000533 RID: 1331
		// (get) Token: 0x06001246 RID: 4678 RVA: 0x00053C65 File Offset: 0x00051E65
		public override string SceneID
		{
			get
			{
				return "scn_born_baby_female_hero";
			}
		}

		// Token: 0x17000534 RID: 1332
		// (get) Token: 0x06001247 RID: 4679 RVA: 0x00053C6C File Offset: 0x00051E6C
		public override TextObject TitleText
		{
			get
			{
				GameTexts.SetVariable("MOTHER_NAME", this.FemaleHero.Name);
				GameTexts.SetVariable("DAY_OF_YEAR", CampaignSceneNotificationHelper.GetFormalDayAndSeasonText(this._creationCampaignTime));
				GameTexts.SetVariable("YEAR", this._creationCampaignTime.GetYear);
				return GameTexts.FindText("str_baby_born_only_mother", null);
			}
		}

		// Token: 0x06001248 RID: 4680 RVA: 0x00053CC8 File Offset: 0x00051EC8
		public override IEnumerable<SceneNotificationData.SceneNotificationCharacter> GetSceneNotificationCharacters()
		{
			List<SceneNotificationData.SceneNotificationCharacter> list = new List<SceneNotificationData.SceneNotificationCharacter>();
			CharacterObject characterObject = CharacterObject.All.First((CharacterObject h) => h.StringId == "cutscene_midwife");
			Equipment equipment = this.MaleHero.CivilianEquipment.Clone(false);
			CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment, true, false);
			Equipment equipment2 = this.FemaleHero.CivilianEquipment.Clone(false);
			CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment2, true, true);
			Equipment equipment3 = characterObject.FirstCivilianEquipment.Clone(false);
			CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment3, false, false);
			list.Add(CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(this.MaleHero, equipment, false, default(BodyProperties), uint.MaxValue, uint.MaxValue, false));
			list.Add(CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(this.FemaleHero, equipment2, false, default(BodyProperties), uint.MaxValue, uint.MaxValue, false));
			list.Add(new SceneNotificationData.SceneNotificationCharacter(characterObject, equipment3, default(BodyProperties), false, uint.MaxValue, uint.MaxValue, false));
			return list;
		}

		// Token: 0x06001249 RID: 4681 RVA: 0x00053DAB File Offset: 0x00051FAB
		public NewBornFemaleHeroSceneNotificationItem(Hero maleHero, Hero femaleHero)
		{
			this.MaleHero = maleHero;
			this.FemaleHero = femaleHero;
			this._creationCampaignTime = CampaignTime.Now;
		}

		// Token: 0x04000671 RID: 1649
		private readonly CampaignTime _creationCampaignTime;
	}
}
