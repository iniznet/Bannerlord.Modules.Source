using System;
using System.Collections.Generic;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
	internal sealed class NetworkStatusReplicationComponent : UdpNetworkComponent
	{
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

		public NetworkStatusReplicationComponent()
		{
			if (GameNetwork.IsClientOrReplay)
			{
				NetworkStatusReplicationComponent.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add);
			}
		}

		public override void OnUdpNetworkHandlerClose()
		{
			base.OnUdpNetworkHandlerClose();
			if (GameNetwork.IsClientOrReplay)
			{
				NetworkStatusReplicationComponent.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Remove);
			}
		}

		private static void HandleServerMessagePingReplication(PingReplication message)
		{
			NetworkCommunicator peer = message.Peer;
			if (peer == null)
			{
				return;
			}
			peer.SetAveragePingInMillisecondsAsClient((double)message.PingValue);
		}

		private static void HandleServerMessageLossReplication(LossReplicationMessage message)
		{
			if (GameNetwork.IsMyPeerReady)
			{
				GameNetwork.MyPeer.SetAverageLossPercentAsClient((double)message.LossValue);
			}
		}

		private static void HandleServerMessageServerPerformanceStateReplication(ServerPerformanceStateReplicationMessage message)
		{
			if (GameNetwork.IsMyPeerReady)
			{
				GameNetwork.MyPeer.SetServerPerformanceProblemStateAsClient(message.ServerPerformanceProblemState);
			}
		}

		private static void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode mode)
		{
			GameNetwork.NetworkMessageHandlerRegisterer networkMessageHandlerRegisterer = new GameNetwork.NetworkMessageHandlerRegisterer(mode);
			networkMessageHandlerRegisterer.Register<PingReplication>(new GameNetworkMessage.ServerMessageHandlerDelegate<PingReplication>(NetworkStatusReplicationComponent.HandleServerMessagePingReplication));
			networkMessageHandlerRegisterer.Register<LossReplicationMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<LossReplicationMessage>(NetworkStatusReplicationComponent.HandleServerMessageLossReplication));
			networkMessageHandlerRegisterer.Register<ServerPerformanceStateReplicationMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<ServerPerformanceStateReplicationMessage>(NetworkStatusReplicationComponent.HandleServerMessageServerPerformanceStateReplication));
		}

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

		private List<NetworkStatusReplicationComponent.NetworkStatusData> _peerData = new List<NetworkStatusReplicationComponent.NetworkStatusData>();

		private float _nextPerformanceStateTrySendTime;

		private ServerPerformanceState _lastSentPerformanceState;

		private class NetworkStatusData
		{
			public float NextPingForceSendTime;

			public float NextPingTrySendTime;

			public int LastSentPingValue = -1;

			public float NextLossTrySendTime;

			public int LastSentLossValue;
		}
	}
}
