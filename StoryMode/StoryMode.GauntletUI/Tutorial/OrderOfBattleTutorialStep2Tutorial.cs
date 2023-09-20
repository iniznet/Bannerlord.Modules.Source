using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x02000034 RID: 52
	public class OrderOfBattleTutorialStep2Tutorial : TutorialItemBase
	{
		// Token: 0x06000100 RID: 256 RVA: 0x000042AC File Offset: 0x000024AC
		public OrderOfBattleTutorialStep2Tutorial()
		{
			base.Type = "OrderOfBattleTutorialStep2";
			base.Placement = TutorialItemVM.ItemPlacements.Top;
			base.HighlightedVisualElementID = "CreateFormation";
			base.MouseRequired = false;
		}

		// Token: 0x06000101 RID: 257 RVA: 0x000042D8 File Offset: 0x000024D8
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 8;
		}

		// Token: 0x06000102 RID: 258 RVA: 0x000042DB File Offset: 0x000024DB
		public override bool IsConditionsMetForActivation()
		{
			return TutorialHelper.IsOrderOfBattleOpenAndReady && TutorialHelper.IsPlayerEncounterLeader;
		}

		// Token: 0x06000103 RID: 259 RVA: 0x000042EB File Offset: 0x000024EB
		public override void OnOrderOfBattleFormationClassChanged(OrderOfBattleFormationClassChangedEvent obj)
		{
			this._playerChangedAFormationType = true;
		}

		// Token: 0x06000104 RID: 260 RVA: 0x000042F4 File Offset: 0x000024F4
		public override void OnOrderOfBattleFormationWeightChanged(OrderOfBattleFormationWeightChangedEvent obj)
		{
			this._playerChangedAFormationWeight = this._playerChangedAFormationType;
		}

		// Token: 0x06000105 RID: 261 RVA: 0x00004302 File Offset: 0x00002502
		public override bool IsConditionsMetForCompletion()
		{
			return this._playerChangedAFormationType && this._playerChangedAFormationWeight;
		}

		// Token: 0x0400004E RID: 78
		private bool _playerChangedAFormationType;

		// Token: 0x0400004F RID: 79
		private bool _playerChangedAFormationWeight;
	}
}
