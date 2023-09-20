using System;
using NetworkMessages.FromServer;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
	internal sealed class DebugAgentScaleOnNetworkTestComponent : UdpNetworkComponent
	{
		public override void OnUdpNetworkHandlerTick(float dt)
		{
			if (GameNetwork.IsServer)
			{
				float totalMissionTime = MBCommon.GetTotalMissionTime();
				if (this._lastTestSendTime < totalMissionTime + 10f)
				{
					MBReadOnlyList<Agent> agents = Mission.Current.Agents;
					int count = agents.Count;
					this._lastTestSendTime = totalMissionTime;
					int num = (int)(new Random().NextDouble() * (double)count);
					Agent agent = agents[num];
					if (agent.IsActive())
					{
						GameNetwork.BeginBroadcastModuleEvent();
						GameNetwork.WriteMessage(new DebugAgentScaleOnNetworkTest(agent.Index, agent.AgentScale));
						GameNetwork.EndBroadcastModuleEventUnreliable(0, null);
					}
				}
			}
		}

		public DebugAgentScaleOnNetworkTestComponent()
		{
			if (GameNetwork.IsClientOrReplay)
			{
				DebugAgentScaleOnNetworkTestComponent.AddRemoveMessageHandlers(0);
			}
		}

		public override void OnUdpNetworkHandlerClose()
		{
			base.OnUdpNetworkHandlerClose();
			if (GameNetwork.IsClientOrReplay)
			{
				DebugAgentScaleOnNetworkTestComponent.AddRemoveMessageHandlers(1);
			}
		}

		private static void HandleServerMessageDebugAgentScaleOnNetworkTest(DebugAgentScaleOnNetworkTest message)
		{
			Agent agentFromIndex = Mission.MissionNetworkHelper.GetAgentFromIndex(message.AgentToTestIndex, true);
			if (agentFromIndex != null && agentFromIndex.IsActive())
			{
				CompressionInfo.Float debugScaleValueCompressionInfo = CompressionMission.DebugScaleValueCompressionInfo;
				debugScaleValueCompressionInfo.GetPrecision();
				float agentScale = agentFromIndex.AgentScale;
			}
		}

		private static void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode mode)
		{
			new GameNetwork.NetworkMessageHandlerRegisterer(mode).Register<DebugAgentScaleOnNetworkTest>(new GameNetworkMessage.ServerMessageHandlerDelegate<DebugAgentScaleOnNetworkTest>(DebugAgentScaleOnNetworkTestComponent.HandleServerMessageDebugAgentScaleOnNetworkTest));
		}

		private float _lastTestSendTime;
	}
}
