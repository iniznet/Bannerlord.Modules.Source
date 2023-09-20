using System;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x0200010E RID: 270
	[Serializable]
	public class FriendInfo
	{
		// Token: 0x170001E4 RID: 484
		// (get) Token: 0x060004F9 RID: 1273 RVA: 0x000079C0 File Offset: 0x00005BC0
		// (set) Token: 0x060004FA RID: 1274 RVA: 0x000079C8 File Offset: 0x00005BC8
		public PlayerId Id { get; set; }

		// Token: 0x170001E5 RID: 485
		// (get) Token: 0x060004FB RID: 1275 RVA: 0x000079D1 File Offset: 0x00005BD1
		// (set) Token: 0x060004FC RID: 1276 RVA: 0x000079D9 File Offset: 0x00005BD9
		public FriendStatus Status { get; set; }

		// Token: 0x170001E6 RID: 486
		// (get) Token: 0x060004FD RID: 1277 RVA: 0x000079E2 File Offset: 0x00005BE2
		// (set) Token: 0x060004FE RID: 1278 RVA: 0x000079EA File Offset: 0x00005BEA
		public string Name { get; set; }

		// Token: 0x170001E7 RID: 487
		// (get) Token: 0x060004FF RID: 1279 RVA: 0x000079F3 File Offset: 0x00005BF3
		// (set) Token: 0x06000500 RID: 1280 RVA: 0x000079FB File Offset: 0x00005BFB
		public bool IsOnline { get; set; }
	}
}
