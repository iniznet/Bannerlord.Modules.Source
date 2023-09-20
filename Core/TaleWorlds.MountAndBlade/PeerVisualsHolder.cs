using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002B7 RID: 695
	public class PeerVisualsHolder
	{
		// Token: 0x17000718 RID: 1816
		// (get) Token: 0x060026AE RID: 9902 RVA: 0x0009116C File Offset: 0x0008F36C
		// (set) Token: 0x060026AF RID: 9903 RVA: 0x00091174 File Offset: 0x0008F374
		public MissionPeer Peer { get; private set; }

		// Token: 0x17000719 RID: 1817
		// (get) Token: 0x060026B0 RID: 9904 RVA: 0x0009117D File Offset: 0x0008F37D
		// (set) Token: 0x060026B1 RID: 9905 RVA: 0x00091185 File Offset: 0x0008F385
		public int VisualsIndex { get; private set; }

		// Token: 0x1700071A RID: 1818
		// (get) Token: 0x060026B2 RID: 9906 RVA: 0x0009118E File Offset: 0x0008F38E
		// (set) Token: 0x060026B3 RID: 9907 RVA: 0x00091196 File Offset: 0x0008F396
		public IAgentVisual AgentVisuals { get; private set; }

		// Token: 0x1700071B RID: 1819
		// (get) Token: 0x060026B4 RID: 9908 RVA: 0x0009119F File Offset: 0x0008F39F
		// (set) Token: 0x060026B5 RID: 9909 RVA: 0x000911A7 File Offset: 0x0008F3A7
		public IAgentVisual MountAgentVisuals { get; private set; }

		// Token: 0x060026B6 RID: 9910 RVA: 0x000911B0 File Offset: 0x0008F3B0
		public PeerVisualsHolder(MissionPeer peer, int index, IAgentVisual agentVisuals, IAgentVisual mountVisuals)
		{
			this.Peer = peer;
			this.VisualsIndex = index;
			this.AgentVisuals = agentVisuals;
			this.MountAgentVisuals = mountVisuals;
		}

		// Token: 0x060026B7 RID: 9911 RVA: 0x000911D5 File Offset: 0x0008F3D5
		public void SetMountVisuals(IAgentVisual mountAgentVisuals)
		{
			this.MountAgentVisuals = mountAgentVisuals;
		}
	}
}
