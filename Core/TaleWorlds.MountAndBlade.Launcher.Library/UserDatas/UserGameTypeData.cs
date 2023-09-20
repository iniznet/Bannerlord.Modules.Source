using System;
using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade.Launcher.Library.UserDatas
{
	// Token: 0x0200001A RID: 26
	public class UserGameTypeData
	{
		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000110 RID: 272 RVA: 0x0000574F File Offset: 0x0000394F
		// (set) Token: 0x06000111 RID: 273 RVA: 0x00005757 File Offset: 0x00003957
		public List<UserModData> ModDatas { get; set; }

		// Token: 0x06000112 RID: 274 RVA: 0x00005760 File Offset: 0x00003960
		public UserGameTypeData()
		{
			this.ModDatas = new List<UserModData>();
		}
	}
}
