using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Options;
using TaleWorlds.PlatformService;

namespace TaleWorlds.MountAndBlade
{
	public class ProfileSelectionState : GameState
	{
		public bool IsDirectPlayPossible { get; private set; } = true;

		public event ProfileSelectionState.OnProfileSelectionEvent OnProfileSelection;

		public void OnProfileSelected()
		{
			NativeOptions.ReadRGLConfigFiles();
			BannerlordConfig.Initialize();
			ProfileSelectionState.OnProfileSelectionEvent onProfileSelection = this.OnProfileSelection;
			if (onProfileSelection != null)
			{
				onProfileSelection();
			}
			this.StartGame();
		}

		public void StartGame()
		{
			if (Module.CurrentModule.StartupInfo.StartupType == GameStartupType.Multiplayer || PlatformServices.SessionInvitationType == SessionInvitationType.Multiplayer || PlatformServices.IsPlatformRequestedMultiplayer)
			{
				MBGameManager.StartNewGame(new MultiplayerGameManager());
				return;
			}
			Module.CurrentModule.GlobalGameStateManager.CleanAndPushState(Module.CurrentModule.GlobalGameStateManager.CreateState<InitialState>(), 0);
			LoadingWindow.EnableGlobalLoadingWindow();
		}

		public delegate void OnProfileSelectionEvent();
	}
}
