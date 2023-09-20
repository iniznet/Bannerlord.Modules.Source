using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000113 RID: 275
	internal class BannerlordConfig
	{
		// Token: 0x17000202 RID: 514
		// (get) Token: 0x0600053E RID: 1342 RVA: 0x00007DD3 File Offset: 0x00005FD3
		// (set) Token: 0x0600053F RID: 1343 RVA: 0x00007DDB File Offset: 0x00005FDB
		public int AdmittancePercentage { get; private set; }

		// Token: 0x06000540 RID: 1344 RVA: 0x00007DE4 File Offset: 0x00005FE4
		public BannerlordConfig(int admittancePercentage)
		{
			this.AdmittancePercentage = admittancePercentage;
		}
	}
}
