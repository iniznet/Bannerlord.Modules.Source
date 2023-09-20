using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x02000033 RID: 51
	public class OrderOfBattleTutorialStep1Tutorial : TutorialItemBase
	{
		// Token: 0x060000FB RID: 251 RVA: 0x0000425C File Offset: 0x0000245C
		public OrderOfBattleTutorialStep1Tutorial()
		{
			base.Type = "OrderOfBattleTutorialStep1";
			base.Placement = TutorialItemVM.ItemPlacements.Bottom;
			base.HighlightedVisualElementID = "AssignCaptain";
			base.MouseRequired = false;
		}

		// Token: 0x060000FC RID: 252 RVA: 0x00004288 File Offset: 0x00002488
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 8;
		}

		// Token: 0x060000FD RID: 253 RVA: 0x0000428B File Offset: 0x0000248B
		public override bool IsConditionsMetForActivation()
		{
			return TutorialHelper.IsOrderOfBattleOpenAndReady && TutorialHelper.IsPlayerEncounterLeader;
		}

		// Token: 0x060000FE RID: 254 RVA: 0x0000429B File Offset: 0x0000249B
		public override void OnOrderOfBattleHeroAssignedToFormation(OrderOfBattleHeroAssignedToFormationEvent obj)
		{
			this._playerAssignedACaptainToFormationInOoB = true;
		}

		// Token: 0x060000FF RID: 255 RVA: 0x000042A4 File Offset: 0x000024A4
		public override bool IsConditionsMetForCompletion()
		{
			return this._playerAssignedACaptainToFormationInOoB;
		}

		// Token: 0x0400004D RID: 77
		private bool _playerAssignedACaptainToFormationInOoB;
	}
}
