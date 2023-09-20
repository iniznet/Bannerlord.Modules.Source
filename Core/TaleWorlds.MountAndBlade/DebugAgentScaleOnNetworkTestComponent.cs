using System;
using NetworkMessages.FromServer;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002F9 RID: 761
	internal sealed class DebugAgentScaleOnNetworkTestComponent : UdpNetworkComponent
	{
		// Token: 0x0600296C RID: 10604 RVA: 0x000A0538 File Offset: 0x0009E738
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
						GameNetwork.WriteMessage(new DebugAgentScaleOnNetworkTest(agent, agent.AgentScale));
						GameNetwork.EndBroadcastModuleEventUnreliable(GameNetwork.EventBroadcastFlags.None, null);
					}
				}
			}
		}

		// Token: 0x0600296D RID: 10605 RVA: 0x000A05B4 File Offset: 0x0009E7B4
		public DebugAgentScaleOnNetworkTestComponent()
		{
			if (GameNetwork.IsClientOrReplay)
			{
				DebugAgentScaleOnNetworkTestComponent.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add);
			}
		}

		// Token: 0x0600296E RID: 10606 RVA: 0x000A05C9 File Offset: 0x0009E7C9
		public override void OnUdpNetworkHandlerClose()
		{
			base.OnUdpNetworkHandlerClose();
			if (GameNetwork.IsClientOrReplay)
			{
				DebugAgentScaleOnNetworkTestComponent.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Remove);
			}
		}

		// Token: 0x0600296F RID: 10607 RVA: 0x000A05E0 File Offset: 0x0009E7E0
		private static void HandleServerMessageDebugAgentScaleOnNetworkTest(DebugAgentScaleOnNetworkTest message)
		{
			if (message.AgentToTest != null && message.AgentToTest.IsActive())
			{
				CompressionMission.DebugScaleValueCompressionInfo.GetPrecision();
				float agentScale = message.AgentToTest.AgentScale;
			}
		}

		// Token: 0x06002970 RID: 10608 RVA: 0x000A061C File Offset: 0x0009E81C
		private static void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode mode)
		{
			new GameNetwork.NetworkMessageHandlerRegisterer(mode).Register<DebugAgentScaleOnNetworkTest>(new GameNetworkMessage.ServerMessageHandlerDelegate<DebugAgentScaleOnNetworkTest>(DebugAgentScaleOnNetworkTestComponent.HandleServerMessageDebugAgentScaleOnNetworkTest));
		}

		// Token: 0x04000F78 RID: 3960
		private float _lastTestSendTime;
	}
}
