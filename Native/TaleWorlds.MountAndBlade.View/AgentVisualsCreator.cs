using System;

namespace TaleWorlds.MountAndBlade.View
{
	// Token: 0x02000004 RID: 4
	public class AgentVisualsCreator : IAgentVisualCreator
	{
		// Token: 0x06000003 RID: 3 RVA: 0x00002058 File Offset: 0x00000258
		public IAgentVisual Create(AgentVisualsData data, string name, bool needBatchedVersionForWeaponMeshes, bool forceUseFaceCache)
		{
			return AgentVisuals.Create(data, name, false, needBatchedVersionForWeaponMeshes, forceUseFaceCache);
		}
	}
}
