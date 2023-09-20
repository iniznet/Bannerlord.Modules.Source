using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class CreateArmyStep3Tutorial : TutorialItemBase
	{
		public CreateArmyStep3Tutorial()
		{
			base.Type = "CreateArmyStep3";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = string.Empty;
			base.MouseRequired = true;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._playerClosedArmyManagement;
		}

		public override void OnTutorialContextChanged(TutorialContextChangedEvent obj)
		{
			this._playerClosedArmyManagement = obj.NewContext != 10;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 10;
		}

		public override bool IsConditionsMetForActivation()
		{
			return TutorialHelper.CurrentContext == 10 && Campaign.Current.CurrentMenuContext == null && Clan.PlayerClan.Kingdom != null && MobileParty.MainParty.Army == null;
		}

		private bool _playerClosedArmyManagement;
	}
}
