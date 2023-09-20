using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle;

namespace StoryMode.GauntletUI.Tutorial
{
	public class OrderOfBattleTutorialStep2Tutorial : TutorialItemBase
	{
		public OrderOfBattleTutorialStep2Tutorial()
		{
			base.Type = "OrderOfBattleTutorialStep2";
			base.Placement = TutorialItemVM.ItemPlacements.Top;
			base.HighlightedVisualElementID = "CreateFormation";
			base.MouseRequired = false;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 8;
		}

		public override bool IsConditionsMetForActivation()
		{
			return TutorialHelper.IsOrderOfBattleOpenAndReady && TutorialHelper.IsPlayerEncounterLeader;
		}

		public override void OnOrderOfBattleFormationClassChanged(OrderOfBattleFormationClassChangedEvent obj)
		{
			this._playerChangedAFormationType = true;
		}

		public override void OnOrderOfBattleFormationWeightChanged(OrderOfBattleFormationWeightChangedEvent obj)
		{
			this._playerChangedAFormationWeight = this._playerChangedAFormationType;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._playerChangedAFormationType && this._playerChangedAFormationWeight;
		}

		private bool _playerChangedAFormationType;

		private bool _playerChangedAFormationWeight;
	}
}
