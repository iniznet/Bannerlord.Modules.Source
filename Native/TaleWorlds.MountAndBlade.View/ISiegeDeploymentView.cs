using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade.View
{
	// Token: 0x0200000E RID: 14
	public interface ISiegeDeploymentView
	{
		// Token: 0x0600005B RID: 91
		void OnEntitySelection(GameEntity selectedEntity);

		// Token: 0x0600005C RID: 92
		void OnEntityHover(GameEntity hoveredEntity);
	}
}
