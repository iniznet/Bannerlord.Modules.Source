using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class EnterVillageTutorial : TutorialItemBase
	{
		public EnterVillageTutorial()
		{
			base.Type = "EnterVillageTutorial";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "storymode_tutorial_village_enter";
			base.MouseRequired = true;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		public override bool IsConditionsMetForActivation()
		{
			if (!TutorialHelper.IsCharacterPopUpWindowOpen && TutorialHelper.CurrentContext == 4)
			{
				Settlement currentSettlement = Settlement.CurrentSettlement;
				return ((currentSettlement != null) ? currentSettlement.StringId : null) == "village_ES3_2";
			}
			return false;
		}

		public override void OnGameMenuOptionSelected(GameMenuOption obj)
		{
			base.OnGameMenuOptionSelected(obj);
			this._isEnterOptionSelected = obj.IdString == "storymode_tutorial_village_enter";
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._isEnterOptionSelected;
		}

		private bool _isEnterOptionSelected;

		private const string _enterGameMenuOptionId = "storymode_tutorial_village_enter";
	}
}
