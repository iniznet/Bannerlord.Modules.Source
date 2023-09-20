using System;
using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x020000E3 RID: 227
	[Serializable]
	public class AvailableScenes
	{
		// Token: 0x17000159 RID: 345
		// (get) Token: 0x0600036B RID: 875 RVA: 0x00004740 File Offset: 0x00002940
		// (set) Token: 0x0600036C RID: 876 RVA: 0x00004747 File Offset: 0x00002947
		public static AvailableScenes Empty { get; private set; } = new AvailableScenes(new Dictionary<string, string[]>());

		// Token: 0x1700015A RID: 346
		// (get) Token: 0x0600036E RID: 878 RVA: 0x00004760 File Offset: 0x00002960
		// (set) Token: 0x0600036F RID: 879 RVA: 0x00004768 File Offset: 0x00002968
		public Dictionary<string, string[]> ScenesByGameTypes { get; private set; }

		// Token: 0x06000370 RID: 880 RVA: 0x00004771 File Offset: 0x00002971
		public AvailableScenes(Dictionary<string, string[]> scenesByGameTypes)
		{
			this.ScenesByGameTypes = scenesByGameTypes;
		}
	}
}
