using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.ArmyManagement;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x0200001E RID: 30
	public class CreateArmyStep2Tutorial : TutorialItemBase
	{
		// Token: 0x0600008E RID: 142 RVA: 0x00003103 File Offset: 0x00001303
		public CreateArmyStep2Tutorial()
		{
			base.Type = "CreateArmyStep2";
			base.Placement = TutorialItemVM.ItemPlacements.TopRight;
			base.HighlightedVisualElementID = "GatherArmyPartiesPanel";
			base.MouseRequired = true;
		}

		// Token: 0x0600008F RID: 143 RVA: 0x0000312F File Offset: 0x0000132F
		public override bool IsConditionsMetForCompletion()
		{
			return this._playerAddedPartyToArmy;
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00003137 File Offset: 0x00001337
		public override void OnPartyAddedToArmyByPlayer(PartyAddedToArmyByPlayerEvent obj)
		{
			this._playerAddedPartyToArmy = true;
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00003140 File Offset: 0x00001340
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 10;
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00003144 File Offset: 0x00001344
		public override bool IsConditionsMetForActivation()
		{
			return TutorialHelper.CurrentContext == 10 && Campaign.Current.CurrentMenuContext == null && Clan.PlayerClan.Kingdom != null && MobileParty.MainParty.Army == null;
		}

		// Token: 0x04000024 RID: 36
		private bool _playerAddedPartyToArmy;
	}
}
