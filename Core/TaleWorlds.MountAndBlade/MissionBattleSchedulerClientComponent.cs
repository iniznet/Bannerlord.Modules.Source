using System;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200028C RID: 652
	public class MissionBattleSchedulerClientComponent : MissionLobbyComponent
	{
		// Token: 0x06002297 RID: 8855 RVA: 0x0007DC58 File Offset: 0x0007BE58
		public override void QuitMission()
		{
			base.QuitMission();
			if (base.CurrentMultiplayerState != MissionLobbyComponent.MultiplayerGameState.Ending && NetworkMain.GameClient.LoggedIn && NetworkMain.GameClient.CurrentState == LobbyClient.State.AtBattle)
			{
				NetworkMain.GameClient.QuitFromMatchmakerGame();
			}
		}
	}
}
