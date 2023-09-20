using System;
using System.Collections.Generic;

namespace TaleWorlds.Localization.TextProcessor
{
	// Token: 0x0200002E RID: 46
	internal class CaseInsensitiveComparer : IEqualityComparer<string>
	{
		// Token: 0x0600013B RID: 315 RVA: 0x00006CCA File Offset: 0x00004ECA
		public bool Equals(string x, string y)
		{
			return x.Equals(y, StringComparison.InvariantCultureIgnoreCase);
		}

		// Token: 0x0600013C RID: 316 RVA: 0x00006CD4 File Offset: 0x00004ED4
		public int GetHashCode(string x)
		{
			return x.ToLowerInvariant().GetHashCode();
		}
	}
}
