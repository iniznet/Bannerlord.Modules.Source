using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x0200010A RID: 266
	public class DisconnectInfo
	{
		// Token: 0x170001E2 RID: 482
		// (get) Token: 0x060004F3 RID: 1267 RVA: 0x0000797F File Offset: 0x00005B7F
		// (set) Token: 0x060004F4 RID: 1268 RVA: 0x00007987 File Offset: 0x00005B87
		public DisconnectType Type { get; set; }

		// Token: 0x060004F5 RID: 1269 RVA: 0x00007990 File Offset: 0x00005B90
		public DisconnectInfo()
		{
			this.Type = DisconnectType.Unknown;
		}
	}
}
