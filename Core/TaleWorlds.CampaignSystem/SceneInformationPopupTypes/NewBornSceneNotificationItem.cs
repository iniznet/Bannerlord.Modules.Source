using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	public class NewBornSceneNotificationItem : SceneNotificationData
	{
		public Hero MaleHero { get; }

		public Hero FemaleHero { get; }

		public override string SceneID
		{
			get
			{
				return "scn_born_baby";
			}
		}

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

		public NewBornSceneNotificationItem(Hero maleHero, Hero femaleHero)
		{
			this.MaleHero = maleHero;
			this.FemaleHero = femaleHero;
			this._creationCampaignTime = CampaignTime.Now;
		}

		private readonly CampaignTime _creationCampaignTime;
	}
}
