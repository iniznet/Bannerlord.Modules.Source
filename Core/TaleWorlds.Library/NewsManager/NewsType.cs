using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TaleWorlds.Library.NewsManager
{
	// Token: 0x020000A2 RID: 162
	public struct NewsType
	{
		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x060005F0 RID: 1520 RVA: 0x00012F06 File Offset: 0x00011106
		// (set) Token: 0x060005F1 RID: 1521 RVA: 0x00012F0E File Offset: 0x0001110E
		[JsonConverter(typeof(StringEnumConverter))]
		public NewsItem.NewsTypes Type { get; set; }

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x060005F2 RID: 1522 RVA: 0x00012F17 File Offset: 0x00011117
		// (set) Token: 0x060005F3 RID: 1523 RVA: 0x00012F1F File Offset: 0x0001111F
		public int Index { get; set; }
	}
}
