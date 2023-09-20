using System;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Encyclopedia
{
	// Token: 0x0200015B RID: 347
	public class EncyclopediaFilterItem
	{
		// Token: 0x06001852 RID: 6226 RVA: 0x0007B404 File Offset: 0x00079604
		public EncyclopediaFilterItem(TextObject name, Predicate<object> predicate)
		{
			this.Name = name;
			this.Predicate = predicate;
			this.IsActive = false;
		}

		// Token: 0x04000893 RID: 2195
		public readonly TextObject Name;

		// Token: 0x04000894 RID: 2196
		public readonly Predicate<object> Predicate;

		// Token: 0x04000895 RID: 2197
		public bool IsActive;
	}
}
