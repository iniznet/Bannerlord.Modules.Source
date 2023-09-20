using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x02000026 RID: 38
	public class TakingPrisonersTutorial : TutorialItemBase
	{
		// Token: 0x060000B8 RID: 184 RVA: 0x00003637 File Offset: 0x00001837
		public TakingPrisonersTutorial()
		{
			base.Type = "TakeAndRescuePrisonerTutorial";
			base.Placement = TutorialItemVM.ItemPlacements.Top;
			base.HighlightedVisualElementID = "TransferButtonOnlyOtherPrisoners";
			base.MouseRequired = true;
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00003663 File Offset: 0x00001863
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 1;
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00003668 File Offset: 0x00001868
		public override bool IsConditionsMetForActivation()
		{
			PartyScreenManager instance = PartyScreenManager.Instance;
			PartyState partyState;
			return instance != null && instance.CurrentMode == 2 && (partyState = GameStateManager.Current.ActiveState as PartyState) != null && partyState.PartyScreenLogic.PrisonerRosters[0].Count > 0 && TutorialHelper.CurrentContext == 2;
		}

		// Token: 0x060000BB RID: 187 RVA: 0x000036BD File Offset: 0x000018BD
		public override void OnPlayerMoveTroop(PlayerMoveTroopEvent obj)
		{
			base.OnPlayerMoveTroop(obj);
			if (obj.IsPrisoner && obj.ToSide == 1 && obj.Amount > 0)
			{
				this._playerMovedOtherPrisonerTroop = true;
			}
		}

		// Token: 0x060000BC RID: 188 RVA: 0x000036E7 File Offset: 0x000018E7
		public override bool IsConditionsMetForCompletion()
		{
			return this._playerMovedOtherPrisonerTroop;
		}

		// Token: 0x04000033 RID: 51
		private bool _playerMovedOtherPrisonerTroop;
	}
}
