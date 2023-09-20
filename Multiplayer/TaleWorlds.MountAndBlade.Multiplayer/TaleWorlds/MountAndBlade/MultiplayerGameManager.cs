using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade.Multiplayer;
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
			nextStep = -1;
			switch (gameManagerLoadingStep)
			{
			case 0:
				nextStep = 1;
				return;
			case 1:
				MBGameManager.LoadModuleData(false);
				MBDebug.Print("Game creating...", 0, 12, 17592186044416UL);
				MBGlobals.InitializeReferences();
				Game.CreateGame(new MultiplayerGame(), this).DoLoading();
				nextStep = 2;
				return;
			case 2:
				MBGameManager.StartNewGame();
				nextStep = 3;
				return;
			case 3:
				nextStep = (Game.Current.DoLoading() ? 4 : 3);
				return;
			case 4:
			{
				bool flag = true;
				foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.SubModules)
				{
					flag = flag && mbsubModuleBase.DoLoading(Game.Current);
				}
				nextStep = (flag ? 5 : 4);
				return;
			}
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
			MBGlobals.InitializeReferences();
			GameState gameState;
			if (GameNetwork.IsDedicatedServer)
			{
				DedicatedServerType dedicatedServerType = Module.CurrentModule.StartupInfo.DedicatedServerType;
				gameState = Game.Current.GameStateManager.CreateState<UnspecifiedDedicatedServerState>();
				Utilities.SetFrameLimiterWithSleep(true);
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
				MultiplayerMain.InitializeAsDedicatedServer(new GameNetworkHandler());
				return;
			}
			MultiplayerMain.Initialize(new GameNetworkHandler());
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
			if (sessionInvitationType == 1)
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
