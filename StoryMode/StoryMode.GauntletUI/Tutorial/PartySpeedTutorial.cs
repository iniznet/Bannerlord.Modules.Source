using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x0200001A RID: 26
	public class PartySpeedTutorial : TutorialItemBase
	{
		// Token: 0x0600007A RID: 122 RVA: 0x00002E19 File Offset: 0x00001019
		public PartySpeedTutorial()
		{
			base.Type = "PartySpeed";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "PartySpeedLabel";
			base.MouseRequired = true;
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00002E45 File Offset: 0x00001045
		public override bool IsConditionsMetForCompletion()
		{
			return this._isPlayerInspectedPartySpeed;
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00002E4D File Offset: 0x0000104D
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00002E50 File Offset: 0x00001050
		public override void OnPlayerInspectedPartySpeed(PlayerInspectedPartySpeedEvent obj)
		{
			if (this._isActivated)
			{
				this._isPlayerInspectedPartySpeed = true;
			}
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00002E64 File Offset: 0x00001064
		public override bool IsConditionsMetForActivation()
		{
			this._isActivated = TutorialHelper.CurrentContext == 4 && Campaign.Current.CurrentMenuContext == null && MobileParty.MainParty.Ai.PartyMoveMode != null && MobileParty.MainParty.Speed < TutorialHelper.MaximumSpeedForPartyForSpeedTutorial && (float)MobileParty.MainParty.InventoryCapacity < MobileParty.MainParty.ItemRoster.TotalWeight;
			return this._isActivated;
		}

		// Token: 0x0400001E RID: 30
		private bool _isPlayerInspectedPartySpeed;

		// Token: 0x0400001F RID: 31
		private bool _isActivated;
	}
}
