using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x0200001D RID: 29
	public class CreateArmyStep1Tutorial : TutorialItemBase
	{
		// Token: 0x06000089 RID: 137 RVA: 0x00003068 File Offset: 0x00001268
		public CreateArmyStep1Tutorial()
		{
			base.Type = "CreateArmyStep1";
			base.Placement = TutorialItemVM.ItemPlacements.TopRight;
			base.HighlightedVisualElementID = "MapGatherArmyButton";
			base.MouseRequired = true;
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00003094 File Offset: 0x00001294
		public override bool IsConditionsMetForCompletion()
		{
			return this._playerOpenedGatherArmy;
		}

		// Token: 0x0600008B RID: 139 RVA: 0x0000309C File Offset: 0x0000129C
		public override void OnTutorialContextChanged(TutorialContextChangedEvent obj)
		{
			this._playerOpenedGatherArmy = obj.NewContext == 10;
		}

		// Token: 0x0600008C RID: 140 RVA: 0x000030AE File Offset: 0x000012AE
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		// Token: 0x0600008D RID: 141 RVA: 0x000030B4 File Offset: 0x000012B4
		public override bool IsConditionsMetForActivation()
		{
			return TutorialHelper.CurrentContext == 4 && Campaign.Current.CurrentMenuContext == null && Clan.PlayerClan.Kingdom != null && MobileParty.MainParty.Army == null && Clan.PlayerClan.Influence >= 30f;
		}

		// Token: 0x04000023 RID: 35
		private bool _playerOpenedGatherArmy;
	}
}
