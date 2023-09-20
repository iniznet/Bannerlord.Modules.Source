using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;

namespace StoryMode.GameComponents
{
	// Token: 0x02000046 RID: 70
	public class StoryModePrisonerRecruitmentCalculationModel : DefaultPrisonerRecruitmentCalculationModel
	{
		// Token: 0x060003C5 RID: 965 RVA: 0x0001758E File Offset: 0x0001578E
		public override int GetConformityChangePerHour(PartyBase party, CharacterObject character)
		{
			if (party == PartyBase.MainParty && !StoryModeManager.Current.MainStoryLine.TutorialPhase.IsCompleted)
			{
				return 0;
			}
			return base.GetConformityChangePerHour(party, character);
		}
	}
}
