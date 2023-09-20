using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.CustomBattle
{
	// Token: 0x02000012 RID: 18
	public class CustomGameManager : MBGameManager
	{
		// Token: 0x060000FA RID: 250 RVA: 0x00008244 File Offset: 0x00006444
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

		// Token: 0x060000FB RID: 251 RVA: 0x00008318 File Offset: 0x00006518
		public override void OnLoadFinished()
		{
			base.OnLoadFinished();
			Game.Current.GameStateManager.CleanAndPushState(Game.Current.GameStateManager.CreateState<CustomBattleState>(), 0);
		}
	}
}
