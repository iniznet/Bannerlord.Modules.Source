using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	public class GameNetworkHandler : IGameNetworkHandler
	{
		void IGameNetworkHandler.OnNewPlayerConnect(PlayerConnectionInfo playerConnectionInfo, NetworkCommunicator networkPeer)
		{
			if (networkPeer != null)
			{
				GameManagerBase.Current.OnPlayerConnect(networkPeer.VirtualPlayer);
			}
		}

		void IGameNetworkHandler.OnInitialize()
		{
			MultiplayerGameTypes.Initialize();
		}

		void IGameNetworkHandler.OnPlayerConnectedToServer(NetworkCommunicator networkPeer)
		{
			if (Mission.Current != null)
			{
				using (List<MissionBehavior>.Enumerator enumerator = Mission.Current.MissionBehaviors.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MissionNetwork missionNetwork;
						if ((missionNetwork = enumerator.Current as MissionNetwork) != null)
						{
							missionNetwork.OnPlayerConnectedToServer(networkPeer);
						}
					}
				}
			}
		}

		void IGameNetworkHandler.OnDisconnectedFromServer()
		{
			if (Mission.Current != null)
			{
				BannerlordNetwork.EndMultiplayerLobbyMission();
			}
		}

		void IGameNetworkHandler.OnPlayerDisconnectedFromServer(NetworkCommunicator networkPeer)
		{
			GameManagerBase.Current.OnPlayerDisconnect(networkPeer.VirtualPlayer);
			if (Mission.Current != null)
			{
				using (List<MissionBehavior>.Enumerator enumerator = Mission.Current.MissionBehaviors.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MissionNetwork missionNetwork;
						if ((missionNetwork = enumerator.Current as MissionNetwork) != null)
						{
							missionNetwork.OnPlayerDisconnectedFromServer(networkPeer);
						}
					}
				}
			}
		}

		void IGameNetworkHandler.OnStartMultiplayer()
		{
			GameNetwork.AddNetworkComponent<BaseNetworkComponent>();
			GameNetwork.AddNetworkComponent<LobbyNetworkComponent>();
			GameNetwork.AddNetworkComponent<MultiplayerPermissionHandler>();
			GameNetwork.AddNetworkComponent<NetworkStatusReplicationComponent>();
			GameManagerBase.Current.OnGameNetworkBegin();
		}

		void IGameNetworkHandler.OnEndMultiplayer()
		{
			GameManagerBase.Current.OnGameNetworkEnd();
			GameNetwork.DestroyComponent(GameNetwork.GetNetworkComponent<LobbyNetworkComponent>());
			GameNetwork.DestroyComponent(GameNetwork.GetNetworkComponent<NetworkStatusReplicationComponent>());
			GameNetwork.DestroyComponent(GameNetwork.GetNetworkComponent<MultiplayerPermissionHandler>());
			GameNetwork.DestroyComponent(GameNetwork.GetNetworkComponent<BaseNetworkComponent>());
		}

		void IGameNetworkHandler.OnStartReplay()
		{
			GameNetwork.AddNetworkComponent<BaseNetworkComponent>();
			GameNetwork.AddNetworkComponent<LobbyNetworkComponent>();
		}

		void IGameNetworkHandler.OnEndReplay()
		{
			GameNetwork.DestroyComponent(GameNetwork.GetNetworkComponent<LobbyNetworkComponent>());
			GameNetwork.DestroyComponent(GameNetwork.GetNetworkComponent<BaseNetworkComponent>());
		}

		void IGameNetworkHandler.OnHandleConsoleCommand(string command)
		{
			DedicatedServerConsoleCommandManager.HandleConsoleCommand(command);
		}
	}
}
