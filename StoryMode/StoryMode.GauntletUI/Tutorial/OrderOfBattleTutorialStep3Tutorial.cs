using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x02000035 RID: 53
	public class OrderOfBattleTutorialStep3Tutorial : TutorialItemBase
	{
		// Token: 0x06000106 RID: 262 RVA: 0x00004314 File Offset: 0x00002514
		public OrderOfBattleTutorialStep3Tutorial()
		{
			base.Type = "OrderOfBattleTutorialStep3";
			base.Placement = TutorialItemVM.ItemPlacements.Bottom;
			base.HighlightedVisualElementID = "AssignCaptain";
			base.MouseRequired = false;
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00004340 File Offset: 0x00002540
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 8;
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00004343 File Offset: 0x00002543
		public override bool IsConditionsMetForActivation()
		{
			return TutorialHelper.IsOrderOfBattleOpenAndReady && !TutorialHelper.IsPlayerEncounterLeader;
		}

		// Token: 0x06000109 RID: 265 RVA: 0x00004356 File Offset: 0x00002556
		public override void OnOrderOfBattleHeroAssignedToFormation(OrderOfBattleHeroAssignedToFormationEvent obj)
		{
			if (!TutorialHelper.IsPlayerEncounterLeader)
			{
				this._playerAssignedACaptainToFormationInOoB = obj.AssignedHero == Agent.Main;
			}
		}

		// Token: 0x0600010A RID: 266 RVA: 0x00004372 File Offset: 0x00002572
		public override bool IsConditionsMetForCompletion()
		{
			return this._playerAssignedACaptainToFormationInOoB;
		}

		// Token: 0x04000050 RID: 80
		private bool _playerAssignedACaptainToFormationInOoB;
	}
}
