using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper.PerkSelection;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class ChoosingPerkUpgradesStep3Tutorial : TutorialItemBase
	{
		public ChoosingPerkUpgradesStep3Tutorial()
		{
			base.Type = "ChoosingPerkUpgradesStep3";
			base.Placement = TutorialItemVM.ItemPlacements.BottomRight;
			base.HighlightedVisualElementID = "PerkSelectionContainer";
			base.MouseRequired = true;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._perkSelectedByPlayer;
		}

		public override void OnPerkSelectedByPlayer(PerkSelectedByPlayerEvent obj)
		{
			this._perkSelectedByPlayer = true;
		}

		public override bool IsConditionsMetForActivation()
		{
			return (TutorialHelper.PlayerIsInAnySettlement || TutorialHelper.PlayerIsSafeOnMap) && Hero.MainHero.HeroDeveloper.GetOneAvailablePerkForEachPerkPair().Count > 1 && TutorialHelper.CurrentContext == 3;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 3;
		}

		private bool _perkSelectedByPlayer;
	}
}
