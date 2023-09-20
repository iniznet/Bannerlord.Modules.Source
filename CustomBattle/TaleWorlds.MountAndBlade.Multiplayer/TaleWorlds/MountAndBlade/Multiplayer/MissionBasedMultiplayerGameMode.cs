using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade.Multiplayer
{
	public class MissionBasedMultiplayerGameMode : MultiplayerGameMode
	{
		public MissionBasedMultiplayerGameMode(string name)
			: base(name)
		{
		}

		public override void JoinCustomGame(JoinGameData joinGameData)
		{
			LobbyGameStateCustomGameClient lobbyGameStateCustomGameClient = Game.Current.GameStateManager.CreateState<LobbyGameStateCustomGameClient>();
			lobbyGameStateCustomGameClient.SetStartingParameters(NetworkMain.GameClient, joinGameData.GameServerProperties.Address, joinGameData.GameServerProperties.Port, joinGameData.PeerIndex, joinGameData.SessionKey);
			Game.Current.GameStateManager.PushState(lobbyGameStateCustomGameClient, 0);
		}

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
