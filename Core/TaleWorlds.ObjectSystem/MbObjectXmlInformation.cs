using System;
using System.Collections.Generic;

namespace TaleWorlds.ObjectSystem
{
	// Token: 0x0200000A RID: 10
	public struct MbObjectXmlInformation
	{
		// Token: 0x06000076 RID: 118 RVA: 0x00004330 File Offset: 0x00002530
		public MbObjectXmlInformation(string id, string name, string moduleName, List<string> gameTypesIncluded)
		{
			this.Id = id;
			this.Name = name;
			this.ModuleName = moduleName;
			this.GameTypesIncluded = gameTypesIncluded;
		}

		// Token: 0x0400000C RID: 12
		public string Id;

		// Token: 0x0400000D RID: 13
		public string Name;

		// Token: 0x0400000E RID: 14
		public string ModuleName;

		// Token: 0x0400000F RID: 15
		public List<string> GameTypesIncluded;
	}
}
