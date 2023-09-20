using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Encyclopedia
{
	public class EncyclopediaFilterGroup : ViewModel
	{
		public EncyclopediaFilterGroup(List<EncyclopediaFilterItem> filters, TextObject name)
		{
			this.Filters = filters;
			this.Name = name;
		}

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

		public readonly List<EncyclopediaFilterItem> Filters;

		public readonly TextObject Name;
	}
}
