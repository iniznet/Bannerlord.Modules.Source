using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	// Token: 0x020000C0 RID: 192
	public class NewBornFemaleHeroSceneAlternateNotificationItem : SceneNotificationData
	{
		// Token: 0x1700052D RID: 1325
		// (get) Token: 0x0600123E RID: 4670 RVA: 0x00053B01 File Offset: 0x00051D01
		public Hero MaleHero { get; }

		// Token: 0x1700052E RID: 1326
		// (get) Token: 0x0600123F RID: 4671 RVA: 0x00053B09 File Offset: 0x00051D09
		public Hero FemaleHero { get; }

		// Token: 0x1700052F RID: 1327
		// (get) Token: 0x06001240 RID: 4672 RVA: 0x00053B11 File Offset: 0x00051D11
		public override string SceneID
		{
			get
			{
				return "scn_born_baby_female_hero2";
			}
		}

		// Token: 0x17000530 RID: 1328
		// (get) Token: 0x06001241 RID: 4673 RVA: 0x00053B18 File Offset: 0x00051D18
		public override TextObject TitleText
		{
			get
			{
				GameTexts.SetVariable("DAY_OF_YEAR", CampaignSceneNotificationHelper.GetFormalDayAndSeasonText(this._creationCampaignTime));
				GameTexts.SetVariable("YEAR", this._creationCampaignTime.GetYear);
				GameTexts.SetVariable("MOTHER_NAME", this.FemaleHero.Name);
				return GameTexts.FindText("str_baby_born_only_mother", null);
			}
		}

		// Token: 0x06001242 RID: 4674 RVA: 0x00053B74 File Offset: 0x00051D74
		public override IEnumerable<SceneNotificationData.SceneNotificationCharacter> GetSceneNotificationCharacters()
		{
			List<SceneNotificationData.SceneNotificationCharacter> list = new List<SceneNotificationData.SceneNotificationCharacter>();
			Equipment equipment = this.FemaleHero.CivilianEquipment.Clone(false);
			CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment, true, true);
			CharacterObject characterObject = CharacterObject.All.First((CharacterObject h) => h.StringId == "cutscene_midwife");
			Equipment equipment2 = characterObject.FirstCivilianEquipment.Clone(false);
			CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment2, false, false);
			list.Add(new SceneNotificationData.SceneNotificationCharacter(null, null, default(BodyProperties), false, uint.MaxValue, uint.MaxValue, false));
			list.Add(CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(this.FemaleHero, equipment, false, default(BodyProperties), uint.MaxValue, uint.MaxValue, false));
			list.Add(new SceneNotificationData.SceneNotificationCharacter(characterObject, equipment2, default(BodyProperties), false, uint.MaxValue, uint.MaxValue, false));
			return list;
		}

		// Token: 0x06001243 RID: 4675 RVA: 0x00053C34 File Offset: 0x00051E34
		public NewBornFemaleHeroSceneAlternateNotificationItem(Hero maleHero, Hero femaleHero)
		{
			this.MaleHero = maleHero;
			this.FemaleHero = femaleHero;
			this._creationCampaignTime = CampaignTime.Now;
		}

		// Token: 0x0400066E RID: 1646
		private readonly CampaignTime _creationCampaignTime;
	}
}
