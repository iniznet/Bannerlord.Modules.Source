using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;

namespace StoryMode.GameComponents
{
	public class StoryModePrisonerRecruitmentCalculationModel : DefaultPrisonerRecruitmentCalculationModel
	{
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
