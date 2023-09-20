using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle;

namespace StoryMode.GauntletUI.Tutorial
{
	public class OrderOfBattleTutorialStep1Tutorial : TutorialItemBase
	{
		public OrderOfBattleTutorialStep1Tutorial()
		{
			base.Type = "OrderOfBattleTutorialStep1";
			base.Placement = TutorialItemVM.ItemPlacements.Center;
			base.HighlightedVisualElementID = "AssignCaptain";
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

		public override void OnOrderOfBattleHeroAssignedToFormation(OrderOfBattleHeroAssignedToFormationEvent obj)
		{
			this._playerAssignedACaptainToFormationInOoB = true;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._playerAssignedACaptainToFormationInOoB;
		}

		private bool _playerAssignedACaptainToFormationInOoB;
	}
}
