using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TaleWorlds.Library.NewsManager
{
	// Token: 0x020000A1 RID: 161
	public struct NewsItem
	{
		// Token: 0x170000AC RID: 172
		// (get) Token: 0x060005E6 RID: 1510 RVA: 0x00012EB1 File Offset: 0x000110B1
		// (set) Token: 0x060005E7 RID: 1511 RVA: 0x00012EB9 File Offset: 0x000110B9
		public string Title { get; set; }

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x060005E8 RID: 1512 RVA: 0x00012EC2 File Offset: 0x000110C2
		// (set) Token: 0x060005E9 RID: 1513 RVA: 0x00012ECA File Offset: 0x000110CA
		public string Description { get; set; }

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x060005EA RID: 1514 RVA: 0x00012ED3 File Offset: 0x000110D3
		// (set) Token: 0x060005EB RID: 1515 RVA: 0x00012EDB File Offset: 0x000110DB
		public string ImageSourcePath { get; set; }

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x060005EC RID: 1516 RVA: 0x00012EE4 File Offset: 0x000110E4
		// (set) Token: 0x060005ED RID: 1517 RVA: 0x00012EEC File Offset: 0x000110EC
		public List<NewsType> Feeds { get; set; }

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x060005EE RID: 1518 RVA: 0x00012EF5 File Offset: 0x000110F5
		// (set) Token: 0x060005EF RID: 1519 RVA: 0x00012EFD File Offset: 0x000110FD
		public string NewsLink { get; set; }

		// Token: 0x020000E6 RID: 230
		[JsonConverter(typeof(StringEnumConverter))]
		public enum NewsTypes
		{
			// Token: 0x040002CE RID: 718
			LauncherSingleplayer,
			// Token: 0x040002CF RID: 719
			LauncherMultiplayer,
			// Token: 0x040002D0 RID: 720
			MultiplayerLobby
		}
	}
}
