using System;
using System.Collections.Generic;

namespace SandBox.ViewModelCollection.GameOver
{
	// Token: 0x02000038 RID: 56
	public class StatCategory
	{
		// Token: 0x0600041B RID: 1051 RVA: 0x0001281E File Offset: 0x00010A1E
		public StatCategory(string id, IEnumerable<StatItem> items)
		{
			this.ID = id;
			this.Items = items;
		}

		// Token: 0x0400021E RID: 542
		public readonly IEnumerable<StatItem> Items;

		// Token: 0x0400021F RID: 543
		public readonly string ID;
	}
}
