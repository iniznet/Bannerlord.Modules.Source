using System;

namespace TaleWorlds.MountAndBlade
{
	public class PeerVisualsHolder
	{
		public MissionPeer Peer { get; private set; }

		public int VisualsIndex { get; private set; }

		public IAgentVisual AgentVisuals { get; private set; }

		public IAgentVisual MountAgentVisuals { get; private set; }

		public PeerVisualsHolder(MissionPeer peer, int index, IAgentVisual agentVisuals, IAgentVisual mountVisuals)
		{
			this.Peer = peer;
			this.VisualsIndex = index;
			this.AgentVisuals = agentVisuals;
			this.MountAgentVisuals = mountVisuals;
		}

		public void SetMountVisuals(IAgentVisual mountAgentVisuals)
		{
			this.MountAgentVisuals = mountAgentVisuals;
		}
	}
}
