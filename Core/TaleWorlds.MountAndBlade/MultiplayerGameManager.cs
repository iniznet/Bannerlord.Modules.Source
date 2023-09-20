using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.PlatformService;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002DC RID: 732
	public class MultiplayerGameManager : MBGameManager
	{
		// Token: 0x06002837 RID: 10295 RVA: 0x0009BA70 File Offset: 0x00099C70
		public MultiplayerGameManager()
		{
			MBMusicManager mbmusicManager = MBMusicManager.Current;
			if (mbmusicManager == null)
			{
				return;
			}
			mbmusicManager.PauseMusicManagerSystem();
		}

		// Token: 0x06002838 RID: 10296 RVA: 0x0009BA88 File Offset: 0x00099C88
		protected override void DoLoadingForGameManager(GameManagerLoadingSteps gameManagerLoadingStep, out GameManagerLoadingSteps nextStep)
		{
			nextStep = GameManagerLoadingSteps.None;
			switch (gameManagerLoadingStep)
			{
			case GameManagerLoadingSteps.PreInitializeZerothStep:
				nextStep = GameManagerLoadingSteps.FirstInitializeFirstStep;
				return;
			case GameManagerLoadingSteps.FirstInitializeFirstStep:
				MBGameManager.LoadModuleData(false);
				MBDebug.Print("Game creating...", 0, Debug.DebugColor.White, 17592186044416UL);
				MBGlobals.InitializeReferences();
				Game.CreateGame(new MultiplayerGame(), this).DoLoading();
				nextStep = GameManagerLoadingSteps.WaitSecondStep;
				return;
			case GameManagerLoadingSteps.WaitSecondStep:
				MBGameManager.StartNewGame();
				nextStep = GameManagerLoadingSteps.SecondInitializeThirdState;
				return;
			case GameManagerLoadingSteps.SecondInitializeThirdState:
				nextStep = (Game.Current.DoLoading() ? GameManagerLoadingSteps.PostInitializeFourthState : GameManagerLoadingSteps.SecondInitializeThirdState);
				return;
			case GameManagerLoadingSteps.PostInitializeFourthState:
			{
				bool flag = true;
				foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.SubModules)
				{
					flag = flag && mbsubModuleBase.DoLoading(Game.Current);
				}
				nextStep = (flag ? GameManagerLoadingSteps.FinishLoadingFifthStep : GameManagerLoadingSteps.PostInitializeFourthState);
				return;
			}
			case GameManagerLoadingSteps.FinishLoadingFifthStep:
				nextStep = GameManagerLoadingSteps.None;
				return;
			default:
				return;
			}
		}

		// Token: 0x06002839 RID: 10297 RVA: 0x0009BB74 File Offset: 0x00099D74
		public override void OnLoadFinished()
		{
			base.OnLoadFinished();
			MBGlobals.InitializeReferences();
			GameState gameState;
			if (GameNetwork.IsDedicatedServer)
			{
				DedicatedServerType dedicatedServerType = Module.CurrentModule.StartupInfo.DedicatedServerType;
				gameState = Game.Current.GameStateManager.CreateState<UnspecifiedDedicatedServerState>();
			}
			else
			{
				gameState = Game.Current.GameStateManager.CreateState<LobbyState>();
			}
			Game.Current.GameStateManager.CleanAndPushState(gameState, 0);
		}

		// Token: 0x0600283A RID: 10298 RVA: 0x0009BBD8 File Offset: 0x00099DD8
		public override void OnAfterCampaignStart(Game game)
		{
			if (GameNetwork.IsDedicatedServer)
			{
				NetworkMain.InitializeAsDedicatedServer();
				return;
			}
			NetworkMain.Initialize();
		}

		// Token: 0x0600283B RID: 10299 RVA: 0x0009BBEC File Offset: 0x00099DEC
		public override void OnNewCampaignStart(Game game, object starterObject)
		{
			foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.SubModules)
			{
				mbsubModuleBase.OnMultiplayerGameStart(game, starterObject);
			}
		}

		// Token: 0x0600283C RID: 10300 RVA: 0x0009BC44 File Offset: 0x00099E44
		public override void OnSessionInvitationAccepted(SessionInvitationType sessionInvitationType)
		{
			if (sessionInvitationType == SessionInvitationType.Multiplayer)
			{
				return;
			}
			base.OnSessionInvitationAccepted(sessionInvitationType);
		}

		// Token: 0x0600283D RID: 10301 RVA: 0x0009BC52 File Offset: 0x00099E52
		public override void OnPlatformRequestedMultiplayer()
		{
		}
	}
}
