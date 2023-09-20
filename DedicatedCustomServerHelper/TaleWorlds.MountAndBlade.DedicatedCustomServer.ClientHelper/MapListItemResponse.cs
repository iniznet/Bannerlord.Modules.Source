using System;
using Newtonsoft.Json;

namespace TaleWorlds.MountAndBlade.DedicatedCustomServer.ClientHelper
{
	// Token: 0x02000006 RID: 6
	[Serializable]
	public class MapListItemResponse
	{
		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000050 RID: 80 RVA: 0x00002D78 File Offset: 0x00000F78
		// (set) Token: 0x06000051 RID: 81 RVA: 0x00002D80 File Offset: 0x00000F80
		public string Name { get; private set; }

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000052 RID: 82 RVA: 0x00002D89 File Offset: 0x00000F89
		// (set) Token: 0x06000053 RID: 83 RVA: 0x00002D91 File Offset: 0x00000F91
		public string UniqueToken { get; private set; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000054 RID: 84 RVA: 0x00002D9A File Offset: 0x00000F9A
		// (set) Token: 0x06000055 RID: 85 RVA: 0x00002DA2 File Offset: 0x00000FA2
		public string Revision { get; private set; }

		// Token: 0x06000056 RID: 86 RVA: 0x00002DAB File Offset: 0x00000FAB
		[JsonConstructor]
		public MapListItemResponse(string name, string uniqueToken, string revision)
		{
			this.Name = name;
			this.UniqueToken = uniqueToken;
			this.Revision = revision;
		}
	}
}
