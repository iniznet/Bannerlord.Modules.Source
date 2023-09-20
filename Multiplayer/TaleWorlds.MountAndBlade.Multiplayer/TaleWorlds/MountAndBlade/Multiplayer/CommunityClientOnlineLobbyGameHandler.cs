using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade.Multiplayer
{
	public class CommunityClientOnlineLobbyGameHandler : ICommunityClientHandler
	{
		public LobbyState LobbyState { get; private set; }

		public CommunityClientOnlineLobbyGameHandler(LobbyState lobbyState)
		{
			this.LobbyState = lobbyState;
		}

		void ICommunityClientHandler.OnQuitFromGame()
		{
			if (Game.Current != null)
			{
				GameStateManager gameStateManager = Game.Current.GameStateManager;
				if (!(gameStateManager.ActiveState is LobbyState))
				{
					if (Game.Current.GameStateManager.ActiveState is MissionState)
					{
						BannerlordNetwork.EndMultiplayerLobbyMission();
						return;
					}
					gameStateManager.PopState(0);
				}
			}
		}

		void ICommunityClientHandler.OnJoinCustomGameResponse(string address, int port, PlayerJoinGameResponseDataFromHost response)
		{
			if (Game.Current != null)
			{
				GameStateManager gameStateManager = Game.Current.GameStateManager;
				if (response != null)
				{
					LobbyGameStateCommunityClient lobbyGameStateCommunityClient = Game.Current.GameStateManager.CreateState<LobbyGameStateCommunityClient>();
					lobbyGameStateCommunityClient.SetStartingParameters(NetworkMain.CommunityClient, address, port, response.PeerIndex, response.SessionKey);
					Game.Current.GameStateManager.PushState(lobbyGameStateCommunityClient, 0);
					Debug.Print("Join game successful", 0, 4, 17592186044416UL);
				}
			}
		}
	}
}
