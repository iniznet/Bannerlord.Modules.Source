using System;
using System.Collections.Generic;

namespace TaleWorlds.Core
{
	// Token: 0x020000AB RID: 171
	public readonly struct CampaignSaveMetaDataArgs
	{
		// Token: 0x06000849 RID: 2121 RVA: 0x0001C1C3 File Offset: 0x0001A3C3
		public CampaignSaveMetaDataArgs(string[] moduleName, params KeyValuePair<string, string>[] otherArgs)
		{
			this.ModuleNames = moduleName;
			this.OtherData = otherArgs;
		}

		// Token: 0x040004B4 RID: 1204
		public readonly string[] ModuleNames;

		// Token: 0x040004B5 RID: 1205
		public readonly KeyValuePair<string, string>[] OtherData;
	}
}
