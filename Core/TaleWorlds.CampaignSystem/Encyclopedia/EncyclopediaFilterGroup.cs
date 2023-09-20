using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Encyclopedia
{
	// Token: 0x0200015A RID: 346
	public class EncyclopediaFilterGroup : ViewModel
	{
		// Token: 0x0600184F RID: 6223 RVA: 0x0007B345 File Offset: 0x00079545
		public EncyclopediaFilterGroup(List<EncyclopediaFilterItem> filters, TextObject name)
		{
			this.Filters = filters;
			this.Name = name;
		}

		// Token: 0x1700066A RID: 1642
		// (get) Token: 0x06001850 RID: 6224 RVA: 0x0007B35B File Offset: 0x0007955B
		public Predicate<object> Predicate
		{
			get
			{
				return delegate(object item)
				{
					if (!this.Filters.Any((EncyclopediaFilterItem f) => f.IsActive))
					{
						return true;
					}
					foreach (EncyclopediaFilterItem encyclopediaFilterItem in this.Filters)
					{
						if (encyclopediaFilterItem.IsActive && encyclopediaFilterItem.Predicate(item))
						{
							return true;
						}
					}
					return false;
				};
			}
		}

		// Token: 0x04000891 RID: 2193
		public readonly List<EncyclopediaFilterItem> Filters;

		// Token: 0x04000892 RID: 2194
		public readonly TextObject Name;
	}
}
