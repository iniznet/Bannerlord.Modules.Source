using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class ChoosingSkillFocusStep2Tutorial : TutorialItemBase
	{
		public ChoosingSkillFocusStep2Tutorial()
		{
			base.Type = "ChoosingSkillFocusStep2";
			base.Placement = TutorialItemVM.ItemPlacements.TopRight;
			base.HighlightedVisualElementID = "AddFocusButton";
			base.MouseRequired = true;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._focusAdded;
		}

		public override void OnFocusAddedByPlayer(FocusAddedByPlayerEvent obj)
		{
			this._focusAdded = true;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 3;
		}

		public override bool IsConditionsMetForActivation()
		{
			return Hero.MainHero.HeroDeveloper.UnspentFocusPoints > 1 && TutorialHelper.CurrentContext == 3;
		}

		private bool _focusAdded;
	}
}
