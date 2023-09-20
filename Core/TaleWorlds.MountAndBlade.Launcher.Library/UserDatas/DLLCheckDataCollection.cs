using System;
using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade.Launcher.Library.UserDatas
{
	// Token: 0x0200001C RID: 28
	public class DLLCheckDataCollection
	{
		// Token: 0x17000050 RID: 80
		// (get) Token: 0x0600011C RID: 284 RVA: 0x00005802 File Offset: 0x00003A02
		// (set) Token: 0x0600011D RID: 285 RVA: 0x0000580A File Offset: 0x00003A0A
		public List<DLLCheckData> DLLData { get; set; }

		// Token: 0x0600011E RID: 286 RVA: 0x00005813 File Offset: 0x00003A13
		public DLLCheckDataCollection()
		{
			this.DLLData = new List<DLLCheckData>();
		}
	}
}
