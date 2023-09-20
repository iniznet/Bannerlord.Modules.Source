using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class CreateArmyStep1Tutorial : TutorialItemBase
	{
		public CreateArmyStep1Tutorial()
		{
			base.Type = "CreateArmyStep1";
			base.Placement = TutorialItemVM.ItemPlacements.TopRight;
			base.HighlightedVisualElementID = "MapGatherArmyButton";
			base.MouseRequired = true;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._playerOpenedGatherArmy;
		}

		public override void OnTutorialContextChanged(TutorialContextChangedEvent obj)
		{
			this._playerOpenedGatherArmy = obj.NewContext == 10;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		public override bool IsConditionsMetForActivation()
		{
			return TutorialHelper.CurrentContext == 4 && Campaign.Current.CurrentMenuContext == null && Clan.PlayerClan.Kingdom != null && MobileParty.MainParty.Army == null && Clan.PlayerClan.Influence >= 30f;
		}

		private bool _playerOpenedGatherArmy;
	}
}
