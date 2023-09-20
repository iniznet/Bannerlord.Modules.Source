using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle;

namespace StoryMode.GauntletUI.Tutorial
{
	public class OrderOfBattleTutorialStep3Tutorial : TutorialItemBase
	{
		public OrderOfBattleTutorialStep3Tutorial()
		{
			base.Type = "OrderOfBattleTutorialStep3";
			base.Placement = TutorialItemVM.ItemPlacements.Bottom;
			base.HighlightedVisualElementID = "AssignCaptain";
			base.MouseRequired = false;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 8;
		}

		public override bool IsConditionsMetForActivation()
		{
			return TutorialHelper.IsOrderOfBattleOpenAndReady && !TutorialHelper.IsPlayerEncounterLeader;
		}

		public override void OnOrderOfBattleHeroAssignedToFormation(OrderOfBattleHeroAssignedToFormationEvent obj)
		{
			if (!TutorialHelper.IsPlayerEncounterLeader)
			{
				this._playerAssignedACaptainToFormationInOoB = obj.AssignedHero == Agent.Main;
			}
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._playerAssignedACaptainToFormationInOoB;
		}

		private bool _playerAssignedACaptainToFormationInOoB;
	}
}
