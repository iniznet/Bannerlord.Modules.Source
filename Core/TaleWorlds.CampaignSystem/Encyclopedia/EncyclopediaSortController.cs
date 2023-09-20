using System;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Encyclopedia
{
	// Token: 0x02000161 RID: 353
	public class EncyclopediaSortController
	{
		// Token: 0x1700066F RID: 1647
		// (get) Token: 0x0600187F RID: 6271 RVA: 0x0007BB57 File Offset: 0x00079D57
		public TextObject Name { get; }

		// Token: 0x17000670 RID: 1648
		// (get) Token: 0x06001880 RID: 6272 RVA: 0x0007BB5F File Offset: 0x00079D5F
		public EncyclopediaListItemComparerBase Comparer { get; }

		// Token: 0x06001881 RID: 6273 RVA: 0x0007BB67 File Offset: 0x00079D67
		public EncyclopediaSortController(TextObject name, EncyclopediaListItemComparerBase comparer)
		{
			this.Name = name;
			this.Comparer = comparer;
		}
	}
}
