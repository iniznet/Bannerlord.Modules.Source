using System;
using System.Reflection;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001B2 RID: 434
	public class MissionInfo
	{
		// Token: 0x17000500 RID: 1280
		// (get) Token: 0x0600193C RID: 6460 RVA: 0x0005AEC7 File Offset: 0x000590C7
		// (set) Token: 0x0600193D RID: 6461 RVA: 0x0005AECF File Offset: 0x000590CF
		public string Name { get; set; }

		// Token: 0x17000501 RID: 1281
		// (get) Token: 0x0600193E RID: 6462 RVA: 0x0005AED8 File Offset: 0x000590D8
		// (set) Token: 0x0600193F RID: 6463 RVA: 0x0005AEE0 File Offset: 0x000590E0
		public MethodInfo Creator { get; set; }

		// Token: 0x17000502 RID: 1282
		// (get) Token: 0x06001940 RID: 6464 RVA: 0x0005AEE9 File Offset: 0x000590E9
		// (set) Token: 0x06001941 RID: 6465 RVA: 0x0005AEF1 File Offset: 0x000590F1
		public Type Manager { get; set; }

		// Token: 0x17000503 RID: 1283
		// (get) Token: 0x06001942 RID: 6466 RVA: 0x0005AEFA File Offset: 0x000590FA
		// (set) Token: 0x06001943 RID: 6467 RVA: 0x0005AF02 File Offset: 0x00059102
		public bool UsableByEditor { get; set; }
	}
}
