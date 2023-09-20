using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002DE RID: 734
	public class MissionBasedMultiplayerGameMode : MultiplayerGameMode
	{
		// Token: 0x06002843 RID: 10307 RVA: 0x0009BC74 File Offset: 0x00099E74
		public MissionBasedMultiplayerGameMode(string name)
			: base(name)
		{
		}

		// Token: 0x06002844 RID: 10308 RVA: 0x0009BC80 File Offset: 0x00099E80
		public override void JoinCustomGame(JoinGameData joinGameData)
		{
			LobbyGameStateCustomGameClient lobbyGameStateCustomGameClient = Game.Current.GameStateManager.CreateState<LobbyGameStateCustomGameClient>();
			lobbyGameStateCustomGameClient.SetStartingParameters(NetworkMain.GameClient, joinGameData.GameServerProperties.Address, joinGameData.GameServerProperties.Port, joinGameData.PeerIndex, joinGameData.SessionKey);
			Game.Current.GameStateManager.PushState(lobbyGameStateCustomGameClient, 0);
		}

		// Token: 0x06002845 RID: 10309 RVA: 0x0009BCDC File Offset: 0x00099EDC
		public override void StartMultiplayerGame(string scene)
		{
			if (base.Name == "FreeForAll")
			{
				MultiplayerMissions.OpenFreeForAllMission(scene);
				return;
			}
			if (base.Name == "TeamDeathmatch")
			{
				MultiplayerMissions.OpenTeamDeathmatchMission(scene);
				return;
			}
			if (base.Name == "Duel")
			{
				MultiplayerMissions.OpenDuelMission(scene);
				return;
			}
			if (base.Name == "Siege")
			{
				MultiplayerMissions.OpenSiegeMission(scene);
				return;
			}
			if (base.Name == "Battle")
			{
				MultiplayerMissions.OpenBattleMission(scene);
				return;
			}
			if (base.Name == "Captain")
			{
				MultiplayerMissions.OpenCaptainMission(scene);
				return;
			}
			if (base.Name == "Skirmish")
			{
				MultiplayerMissions.OpenSkirmishMission(scene);
			}
		}
	}
}
