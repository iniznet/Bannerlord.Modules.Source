using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper.PerkSelection;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class ChoosingPerkUpgradesStep2Tutorial : TutorialItemBase
	{
		public ChoosingPerkUpgradesStep2Tutorial()
		{
			base.Type = "ChoosingPerkUpgradesStep2";
			base.Placement = TutorialItemVM.ItemPlacements.TopRight;
			base.HighlightedVisualElementID = "AvailablePerks";
			base.MouseRequired = true;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._perkPopupOpened;
		}

		public override void OnPerkSelectionToggle(PerkSelectionToggleEvent obj)
		{
			this._perkPopupOpened = true;
		}

		public override bool IsConditionsMetForActivation()
		{
			return (TutorialHelper.PlayerIsInAnySettlement || TutorialHelper.PlayerIsSafeOnMap) && Hero.MainHero.HeroDeveloper.GetOneAvailablePerkForEachPerkPair().Count > 1 && TutorialHelper.CurrentContext == 3;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 3;
		}

		private bool _perkPopupOpened;
	}
}
