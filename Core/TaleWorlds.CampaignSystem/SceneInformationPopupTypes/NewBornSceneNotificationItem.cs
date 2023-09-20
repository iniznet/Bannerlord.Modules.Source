using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	// Token: 0x020000C2 RID: 194
	public class NewBornSceneNotificationItem : SceneNotificationData
	{
		// Token: 0x17000535 RID: 1333
		// (get) Token: 0x0600124A RID: 4682 RVA: 0x00053DCC File Offset: 0x00051FCC
		public Hero MaleHero { get; }

		// Token: 0x17000536 RID: 1334
		// (get) Token: 0x0600124B RID: 4683 RVA: 0x00053DD4 File Offset: 0x00051FD4
		public Hero FemaleHero { get; }

		// Token: 0x17000537 RID: 1335
		// (get) Token: 0x0600124C RID: 4684 RVA: 0x00053DDC File Offset: 0x00051FDC
		public override string SceneID
		{
			get
			{
				return "scn_born_baby";
			}
		}

		// Token: 0x17000538 RID: 1336
		// (get) Token: 0x0600124D RID: 4685 RVA: 0x00053DE4 File Offset: 0x00051FE4
		public override TextObject TitleText
		{
			get
			{
				GameTexts.SetVariable("FATHER_NAME", this.MaleHero.Name);
				GameTexts.SetVariable("MOTHER_NAME", this.FemaleHero.Name);
				GameTexts.SetVariable("DAY_OF_YEAR", CampaignSceneNotificationHelper.GetFormalDayAndSeasonText(this._creationCampaignTime));
				GameTexts.SetVariable("YEAR", this._creationCampaignTime.GetYear);
				return GameTexts.FindText("str_baby_born", null);
			}
		}

		// Token: 0x0600124E RID: 4686 RVA: 0x00053E54 File Offset: 0x00052054
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

		// Token: 0x0600124F RID: 4687 RVA: 0x00053F37 File Offset: 0x00052137
		public NewBornSceneNotificationItem(Hero maleHero, Hero femaleHero)
		{
			this.MaleHero = maleHero;
			this.FemaleHero = femaleHero;
			this._creationCampaignTime = CampaignTime.Now;
		}

		// Token: 0x04000674 RID: 1652
		private readonly CampaignTime _creationCampaignTime;
	}
}
