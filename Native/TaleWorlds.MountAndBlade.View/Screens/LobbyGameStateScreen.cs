using System;
using TaleWorlds.Core;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.View.Screens
{
	// Token: 0x0200002F RID: 47
	[GameStateScreen(typeof(LobbyGameStateMatchmakerClient))]
	[GameStateScreen(typeof(LobbyGameStatePlayerBasedCustomServer))]
	public class LobbyGameStateScreen : ScreenBase, IGameStateListener
	{
		// Token: 0x060001CB RID: 459 RVA: 0x0000F3D3 File Offset: 0x0000D5D3
		public LobbyGameStateScreen(LobbyGameState lobbyGameState)
		{
		}

		// Token: 0x060001CC RID: 460 RVA: 0x0000F3DB File Offset: 0x0000D5DB
		void IGameStateListener.OnActivate()
		{
		}

		// Token: 0x060001CD RID: 461 RVA: 0x0000F3DD File Offset: 0x0000D5DD
		void IGameStateListener.OnDeactivate()
		{
		}

		// Token: 0x060001CE RID: 462 RVA: 0x0000F3DF File Offset: 0x0000D5DF
		void IGameStateListener.OnInitialize()
		{
		}

		// Token: 0x060001CF RID: 463 RVA: 0x0000F3E1 File Offset: 0x0000D5E1
		void IGameStateListener.OnFinalize()
		{
		}
	}
}
