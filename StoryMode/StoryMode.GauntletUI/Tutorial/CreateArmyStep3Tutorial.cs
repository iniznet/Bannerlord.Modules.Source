using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x0200001F RID: 31
	public class CreateArmyStep3Tutorial : TutorialItemBase
	{
		// Token: 0x06000093 RID: 147 RVA: 0x00003176 File Offset: 0x00001376
		public CreateArmyStep3Tutorial()
		{
			base.Type = "CreateArmyStep3";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = string.Empty;
			base.MouseRequired = true;
		}

		// Token: 0x06000094 RID: 148 RVA: 0x000031A2 File Offset: 0x000013A2
		public override bool IsConditionsMetForCompletion()
		{
			return this._playerClosedArmyManagement;
		}

		// Token: 0x06000095 RID: 149 RVA: 0x000031AA File Offset: 0x000013AA
		public override void OnTutorialContextChanged(TutorialContextChangedEvent obj)
		{
			this._playerClosedArmyManagement = obj.NewContext != 10;
		}

		// Token: 0x06000096 RID: 150 RVA: 0x000031BF File Offset: 0x000013BF
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 10;
		}

		// Token: 0x06000097 RID: 151 RVA: 0x000031C3 File Offset: 0x000013C3
		public override bool IsConditionsMetForActivation()
		{
			return TutorialHelper.CurrentContext == 10 && Campaign.Current.CurrentMenuContext == null && Clan.PlayerClan.Kingdom != null && MobileParty.MainParty.Army == null;
		}

		// Token: 0x04000025 RID: 37
		private bool _playerClosedArmyManagement;
	}
}
