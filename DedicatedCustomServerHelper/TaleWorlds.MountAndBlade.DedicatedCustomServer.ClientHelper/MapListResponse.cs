using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TaleWorlds.MountAndBlade.DedicatedCustomServer.ClientHelper
{
	// Token: 0x02000005 RID: 5
	[Serializable]
	public class MapListResponse
	{
		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600004B RID: 75 RVA: 0x00002D40 File Offset: 0x00000F40
		// (set) Token: 0x0600004C RID: 76 RVA: 0x00002D48 File Offset: 0x00000F48
		public string CurrentlyPlaying { get; private set; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600004D RID: 77 RVA: 0x00002D51 File Offset: 0x00000F51
		// (set) Token: 0x0600004E RID: 78 RVA: 0x00002D59 File Offset: 0x00000F59
		public List<MapListItemResponse> Maps { get; private set; }

		// Token: 0x0600004F RID: 79 RVA: 0x00002D62 File Offset: 0x00000F62
		[JsonConstructor]
		public MapListResponse(string currentlyPlaying, List<MapListItemResponse> maps)
		{
			this.CurrentlyPlaying = currentlyPlaying;
			this.Maps = maps;
		}
	}
}
