using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.ArmyManagement;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class CreateArmyStep2Tutorial : TutorialItemBase
	{
		public CreateArmyStep2Tutorial()
		{
			base.Type = "CreateArmyStep2";
			base.Placement = TutorialItemVM.ItemPlacements.TopRight;
			base.HighlightedVisualElementID = "GatherArmyPartiesPanel";
			base.MouseRequired = true;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._playerAddedPartyToArmy;
		}

		public override void OnPartyAddedToArmyByPlayer(PartyAddedToArmyByPlayerEvent obj)
		{
			this._playerAddedPartyToArmy = true;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 10;
		}

		public override bool IsConditionsMetForActivation()
		{
			return TutorialHelper.CurrentContext == 10 && Campaign.Current.CurrentMenuContext == null && Clan.PlayerClan.Kingdom != null && MobileParty.MainParty.Army == null;
		}

		private bool _playerAddedPartyToArmy;
	}
}
