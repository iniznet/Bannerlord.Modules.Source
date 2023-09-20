using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Options;
using TaleWorlds.PlatformService;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000234 RID: 564
	public class ProfileSelectionState : GameState
	{
		// Token: 0x17000620 RID: 1568
		// (get) Token: 0x06001F2B RID: 7979 RVA: 0x0006ED1B File Offset: 0x0006CF1B
		// (set) Token: 0x06001F2C RID: 7980 RVA: 0x0006ED23 File Offset: 0x0006CF23
		public bool IsDirectPlayPossible { get; private set; } = true;

		// Token: 0x14000027 RID: 39
		// (add) Token: 0x06001F2D RID: 7981 RVA: 0x0006ED2C File Offset: 0x0006CF2C
		// (remove) Token: 0x06001F2E RID: 7982 RVA: 0x0006ED64 File Offset: 0x0006CF64
		public event ProfileSelectionState.OnProfileSelectionEvent OnProfileSelection;

		// Token: 0x06001F2F RID: 7983 RVA: 0x0006ED99 File Offset: 0x0006CF99
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

		// Token: 0x06001F30 RID: 7984 RVA: 0x0006EDBC File Offset: 0x0006CFBC
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

		// Token: 0x02000555 RID: 1365
		// (Invoke) Token: 0x06003A27 RID: 14887
		public delegate void OnProfileSelectionEvent();
	}
}
