using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class PartySpeedTutorial : TutorialItemBase
	{
		public PartySpeedTutorial()
		{
			base.Type = "PartySpeed";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "PartySpeedLabel";
			base.MouseRequired = true;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._isPlayerInspectedPartySpeed;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		public override void OnPlayerInspectedPartySpeed(PlayerInspectedPartySpeedEvent obj)
		{
			if (this._isActivated)
			{
				this._isPlayerInspectedPartySpeed = true;
			}
		}

		public override bool IsConditionsMetForActivation()
		{
			this._isActivated = TutorialHelper.CurrentContext == 4 && Campaign.Current.CurrentMenuContext == null && MobileParty.MainParty.Ai.PartyMoveMode != null && MobileParty.MainParty.Speed < TutorialHelper.MaximumSpeedForPartyForSpeedTutorial && (float)MobileParty.MainParty.InventoryCapacity < MobileParty.MainParty.ItemRoster.TotalWeight;
			return this._isActivated;
		}

		private bool _isPlayerInspectedPartySpeed;

		private bool _isActivated;
	}
}
