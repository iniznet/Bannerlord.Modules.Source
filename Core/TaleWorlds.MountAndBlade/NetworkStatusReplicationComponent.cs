using System;
using System.Collections.Generic;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002FD RID: 765
	internal sealed class NetworkStatusReplicationComponent : UdpNetworkComponent
	{
		// Token: 0x0600298C RID: 10636 RVA: 0x000A0DE0 File Offset: 0x0009EFE0
		public override void OnUdpNetworkHandlerTick(float dt)
		{
			if (GameNetwork.IsServer)
			{
				float totalMissionTime = MBCommon.GetTotalMissionTime();
				foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
				{
					if (networkCommunicator.IsSynchronized)
					{
						while (this._peerData.Count <= networkCommunicator.Index)
						{
							NetworkStatusReplicationComponent.NetworkStatusData networkStatusData = new NetworkStatusReplicationComponent.NetworkStatusData();
							this._peerData.Add(networkStatusData);
						}
						double num = networkCommunicator.RefreshAndGetAveragePingInMilliseconds();
						NetworkStatusReplicationComponent.NetworkStatusData networkStatusData2 = this._peerData[networkCommunicator.Index];
						bool flag = networkStatusData2.NextPingForceSendTime <= totalMissionTime;
						if (flag || networkStatusData2.NextPingTrySendTime <= totalMissionTime)
						{
							int num2 = MathF.Round(num);
							if (flag || networkStatusData2.LastSentPingValue != num2)
							{
								networkStatusData2.LastSentPingValue = num2;
								networkStatusData2.NextPingForceSendTime = totalMissionTime + 10f + MBRandom.RandomFloatRanged(1.5f, 2.5f);
								GameNetwork.BeginBroadcastModuleEvent();
								GameNetwork.WriteMessage(new PingReplication(networkCommunicator, num2));
								GameNetwork.EndBroadcastModuleEventUnreliable(GameNetwork.EventBroadcastFlags.None, null);
							}
							networkStatusData2.NextPingTrySendTime = totalMissionTime + MBRandom.RandomFloatRanged(1.5f, 2.5f);
						}
						if (!networkCommunicator.IsServerPeer && networkStatusData2.NextLossTrySendTime <= totalMissionTime)
						{
							networkStatusData2.NextLossTrySendTime = totalMissionTime + MBRandom.RandomFloatRanged(1.5f, 2.5f);
							int num3 = (int)networkCommunicator.RefreshAndGetAverageLossPercent();
							if (networkStatusData2.LastSentLossValue != num3)
							{
								networkStatusData2.LastSentLossValue = num3;
								GameNetwork.BeginModuleEventAsServer(networkCommunicator);
								GameNetwork.WriteMessage(new LossReplicationMessage(num3));
								GameNetwork.EndModuleEventAsServer();
							}
						}
					}
				}
				if (this._nextPerformanceStateTrySendTime <= totalMissionTime)
				{
					this._nextPerformanceStateTrySendTime = totalMissionTime + MBRandom.RandomFloatRanged(1.5f, 2.5f);
					ServerPerformanceState serverPerformanceState = this.GetServerPerformanceState();
					if (serverPerformanceState != this._lastSentPerformanceState)
					{
						this._lastSentPerformanceState = serverPerformanceState;
						GameNetwork.BeginBroadcastModuleEvent();
						GameNetwork.WriteMessage(new ServerPerformanceStateReplicationMessage(serverPerformanceState));
						GameNetwork.EndBroadcastModuleEventUnreliable(GameNetwork.EventBroadcastFlags.None, null);
					}
				}
			}
		}

		// Token: 0x0600298D RID: 10637 RVA: 0x000A0FD4 File Offset: 0x0009F1D4
		public NetworkStatusReplicationComponent()
		{
			if (GameNetwork.IsClientOrReplay)
			{
				NetworkStatusReplicationComponent.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add);
			}
		}

		// Token: 0x0600298E RID: 10638 RVA: 0x000A0FF4 File Offset: 0x0009F1F4
		public override void OnUdpNetworkHandlerClose()
		{
			base.OnUdpNetworkHandlerClose();
			if (GameNetwork.IsClientOrReplay)
			{
				NetworkStatusReplicationComponent.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Remove);
			}
		}

		// Token: 0x0600298F RID: 10639 RVA: 0x000A1009 File Offset: 0x0009F209
		private static void HandleServerMessagePingReplication(PingReplication message)
		{
			NetworkCommunicator peer = message.Peer;
			if (peer == null)
			{
				return;
			}
			peer.SetAveragePingInMillisecondsAsClient((double)message.PingValue);
		}

		// Token: 0x06002990 RID: 10640 RVA: 0x000A1022 File Offset: 0x0009F222
		private static void HandleServerMessageLossReplication(LossReplicationMessage message)
		{
			if (GameNetwork.IsMyPeerReady)
			{
				GameNetwork.MyPeer.SetAverageLossPercentAsClient((double)message.LossValue);
			}
		}

		// Token: 0x06002991 RID: 10641 RVA: 0x000A103C File Offset: 0x0009F23C
		private static void HandleServerMessageServerPerformanceStateReplication(ServerPerformanceStateReplicationMessage message)
		{
			if (GameNetwork.IsMyPeerReady)
			{
				GameNetwork.MyPeer.SetServerPerformanceProblemStateAsClient(message.ServerPerformanceProblemState);
			}
		}

		// Token: 0x06002992 RID: 10642 RVA: 0x000A1055 File Offset: 0x0009F255
		private static void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode mode)
		{
			GameNetwork.NetworkMessageHandlerRegisterer networkMessageHandlerRegisterer = new GameNetwork.NetworkMessageHandlerRegisterer(mode);
			networkMessageHandlerRegisterer.Register<PingReplication>(new GameNetworkMessage.ServerMessageHandlerDelegate<PingReplication>(NetworkStatusReplicationComponent.HandleServerMessagePingReplication));
			networkMessageHandlerRegisterer.Register<LossReplicationMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<LossReplicationMessage>(NetworkStatusReplicationComponent.HandleServerMessageLossReplication));
			networkMessageHandlerRegisterer.Register<ServerPerformanceStateReplicationMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<ServerPerformanceStateReplicationMessage>(NetworkStatusReplicationComponent.HandleServerMessageServerPerformanceStateReplication));
		}

		// Token: 0x06002993 RID: 10643 RVA: 0x000A1094 File Offset: 0x0009F294
		private ServerPerformanceState GetServerPerformanceState()
		{
			if (Mission.Current == null)
			{
				return ServerPerformanceState.High;
			}
			float averageFps = Mission.Current.GetAverageFps();
			if (averageFps >= 50f)
			{
				return ServerPerformanceState.High;
			}
			if (averageFps >= 30f)
			{
				return ServerPerformanceState.Medium;
			}
			return ServerPerformanceState.Low;
		}

		// Token: 0x04000F81 RID: 3969
		private List<NetworkStatusReplicationComponent.NetworkStatusData> _peerData = new List<NetworkStatusReplicationComponent.NetworkStatusData>();

		// Token: 0x04000F82 RID: 3970
		private float _nextPerformanceStateTrySendTime;

		// Token: 0x04000F83 RID: 3971
		private ServerPerformanceState _lastSentPerformanceState;

		// Token: 0x02000611 RID: 1553
		private class NetworkStatusData
		{
			// Token: 0x04001F90 RID: 8080
			public float NextPingForceSendTime;

			// Token: 0x04001F91 RID: 8081
			public float NextPingTrySendTime;

			// Token: 0x04001F92 RID: 8082
			public int LastSentPingValue = -1;

			// Token: 0x04001F93 RID: 8083
			public float NextLossTrySendTime;

			// Token: 0x04001F94 RID: 8084
			public int LastSentLossValue;
		}
	}
}
