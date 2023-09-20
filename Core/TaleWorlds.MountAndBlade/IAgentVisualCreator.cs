using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002B4 RID: 692
	public interface IAgentVisualCreator
	{
		// Token: 0x0600268F RID: 9871
		IAgentVisual Create(AgentVisualsData data, string name, bool needBatchedVersionForWeaponMeshes, bool forceUseFaceCache);
	}
}
