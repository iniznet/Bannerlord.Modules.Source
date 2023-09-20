using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class ChoosingSkillFocusStep1Tutorial : TutorialItemBase
	{
		public ChoosingSkillFocusStep1Tutorial()
		{
			base.Type = "ChoosingSkillFocusStep1";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "CharacterButton";
			base.MouseRequired = true;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._characterWindowOpened;
		}

		public override void OnTutorialContextChanged(TutorialContextChangedEvent obj)
		{
			this._characterWindowOpened = obj.NewContext == 3;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		public override bool IsConditionsMetForActivation()
		{
			return Settlement.CurrentSettlement == null && Hero.MainHero.HeroDeveloper.UnspentFocusPoints > 1 && (TutorialHelper.PlayerIsInAnySettlement || TutorialHelper.PlayerIsSafeOnMap) && TutorialHelper.CurrentContext == 4;
		}

		private bool _characterWindowOpened;
	}
}
