using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002EE RID: 750
	public class GameNetworkHandler : IGameNetworkHandler
	{
		// Token: 0x060028E7 RID: 10471 RVA: 0x0009EE83 File Offset: 0x0009D083
		void IGameNetworkHandler.OnNewPlayerConnect(PlayerConnectionInfo playerConnectionInfo, NetworkCommunicator networkPeer)
		{
			if (networkPeer != null)
			{
				GameManagerBase.Current.OnPlayerConnect(networkPeer.VirtualPlayer);
			}
		}

		// Token: 0x060028E8 RID: 10472 RVA: 0x0009EE98 File Offset: 0x0009D098
		void IGameNetworkHandler.OnInitialize()
		{
			MultiplayerGameTypes.Initialize();
		}

		// Token: 0x060028E9 RID: 10473 RVA: 0x0009EEA0 File Offset: 0x0009D0A0
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

		// Token: 0x060028EA RID: 10474 RVA: 0x0009EF08 File Offset: 0x0009D108
		void IGameNetworkHandler.OnDisconnectedFromServer()
		{
			if (Mission.Current != null)
			{
				BannerlordNetwork.EndMultiplayerLobbyMission();
			}
		}

		// Token: 0x060028EB RID: 10475 RVA: 0x0009EF18 File Offset: 0x0009D118
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

		// Token: 0x060028EC RID: 10476 RVA: 0x0009EF90 File Offset: 0x0009D190
		void IGameNetworkHandler.OnStartMultiplayer()
		{
			GameNetwork.AddNetworkComponent<BaseNetworkComponent>();
			GameNetwork.AddNetworkComponent<LobbyNetworkComponent>();
			GameNetwork.AddNetworkComponent<MultiplayerPermissionHandler>();
			GameNetwork.AddNetworkComponent<NetworkStatusReplicationComponent>();
			GameManagerBase.Current.OnGameNetworkBegin();
		}

		// Token: 0x060028ED RID: 10477 RVA: 0x0009EFB4 File Offset: 0x0009D1B4
		void IGameNetworkHandler.OnEndMultiplayer()
		{
			GameManagerBase.Current.OnGameNetworkEnd();
			GameNetwork.DestroyComponent(GameNetwork.GetNetworkComponent<LobbyNetworkComponent>());
			GameNetwork.DestroyComponent(GameNetwork.GetNetworkComponent<NetworkStatusReplicationComponent>());
			GameNetwork.DestroyComponent(GameNetwork.GetNetworkComponent<MultiplayerPermissionHandler>());
			GameNetwork.DestroyComponent(GameNetwork.GetNetworkComponent<BaseNetworkComponent>());
		}

		// Token: 0x060028EE RID: 10478 RVA: 0x0009EFE8 File Offset: 0x0009D1E8
		void IGameNetworkHandler.OnStartReplay()
		{
			GameNetwork.AddNetworkComponent<BaseNetworkComponent>();
			GameNetwork.AddNetworkComponent<LobbyNetworkComponent>();
		}

		// Token: 0x060028EF RID: 10479 RVA: 0x0009EFF6 File Offset: 0x0009D1F6
		void IGameNetworkHandler.OnEndReplay()
		{
			GameNetwork.DestroyComponent(GameNetwork.GetNetworkComponent<LobbyNetworkComponent>());
			GameNetwork.DestroyComponent(GameNetwork.GetNetworkComponent<BaseNetworkComponent>());
		}

		// Token: 0x060028F0 RID: 10480 RVA: 0x0009F00C File Offset: 0x0009D20C
		void IGameNetworkHandler.OnHandleConsoleCommand(string command)
		{
			DedicatedServerConsoleCommandManager.HandleConsoleCommand(command);
		}
	}
}
