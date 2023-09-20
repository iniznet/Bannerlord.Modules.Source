using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.PlatformService;

namespace TaleWorlds.MountAndBlade
{
	public class MultiplayerGameManager : MBGameManager
	{
		public MultiplayerGameManager()
		{
			MBMusicManager mbmusicManager = MBMusicManager.Current;
			if (mbmusicManager == null)
			{
				return;
			}
			mbmusicManager.PauseMusicManagerSystem();
		}

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

		public override void OnAfterCampaignStart(Game game)
		{
			if (GameNetwork.IsDedicatedServer)
			{
				NetworkMain.InitializeAsDedicatedServer();
				return;
			}
			NetworkMain.Initialize();
		}

		public override void OnNewCampaignStart(Game game, object starterObject)
		{
			foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.SubModules)
			{
				mbsubModuleBase.OnMultiplayerGameStart(game, starterObject);
			}
		}

		public override void OnSessionInvitationAccepted(SessionInvitationType sessionInvitationType)
		{
			if (sessionInvitationType == SessionInvitationType.Multiplayer)
			{
				return;
			}
			base.OnSessionInvitationAccepted(sessionInvitationType);
		}

		public override void OnPlatformRequestedMultiplayer()
		{
		}
	}
}
