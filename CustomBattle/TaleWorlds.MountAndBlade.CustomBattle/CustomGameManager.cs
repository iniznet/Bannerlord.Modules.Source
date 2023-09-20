using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.CustomBattle
{
	public class CustomGameManager : MBGameManager
	{
		protected override void DoLoadingForGameManager(GameManagerLoadingSteps gameManagerLoadingStep, out GameManagerLoadingSteps nextStep)
		{
			nextStep = -1;
			switch (gameManagerLoadingStep)
			{
			case 0:
				MBGameManager.LoadModuleData(false);
				MBGlobals.InitializeReferences();
				Game.CreateGame(new CustomGame(), this).DoLoading();
				nextStep = 1;
				return;
			case 1:
			{
				bool flag = true;
				foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.SubModules)
				{
					flag = flag && mbsubModuleBase.DoLoading(Game.Current);
				}
				nextStep = (flag ? 2 : 1);
				return;
			}
			case 2:
				MBGameManager.StartNewGame();
				nextStep = 3;
				return;
			case 3:
				nextStep = (Game.Current.DoLoading() ? 4 : 3);
				return;
			case 4:
				nextStep = 5;
				return;
			case 5:
				nextStep = -1;
				return;
			default:
				return;
			}
		}

		public override void OnLoadFinished()
		{
			base.OnLoadFinished();
			Game.Current.GameStateManager.CleanAndPushState(Game.Current.GameStateManager.CreateState<CustomBattleState>(), 0);
		}
	}
}
