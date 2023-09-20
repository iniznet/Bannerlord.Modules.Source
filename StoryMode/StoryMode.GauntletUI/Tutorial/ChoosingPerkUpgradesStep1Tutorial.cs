using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class ChoosingPerkUpgradesStep1Tutorial : TutorialItemBase
	{
		public ChoosingPerkUpgradesStep1Tutorial()
		{
			base.Type = "ChoosingPerkUpgradesStep1";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "CharacterButton";
			base.MouseRequired = true;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._contextChangedToCharacterScreen;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		public override bool IsConditionsMetForActivation()
		{
			return (TutorialHelper.PlayerIsInAnySettlement || TutorialHelper.PlayerIsSafeOnMap) && Hero.MainHero.HeroDeveloper.GetOneAvailablePerkForEachPerkPair().Count > 1 && TutorialHelper.CurrentContext == 4;
		}

		public override void OnTutorialContextChanged(TutorialContextChangedEvent obj)
		{
			this._contextChangedToCharacterScreen = obj.NewContext == 3;
		}

		private bool _contextChangedToCharacterScreen;
	}
}
