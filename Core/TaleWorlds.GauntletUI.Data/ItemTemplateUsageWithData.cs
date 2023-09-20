using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI.PrefabSystem;

namespace TaleWorlds.GauntletUI.Data
{
	public class ItemTemplateUsageWithData
	{
		public Dictionary<string, WidgetAttributeTemplate> GivenParameters { get; private set; }

		public ItemTemplateUsage ItemTemplateUsage { get; private set; }

		public WidgetTemplate DefaultItemTemplate
		{
			get
			{
				return this.ItemTemplateUsage.DefaultItemTemplate;
			}
		}

		public WidgetTemplate FirstItemTemplate
		{
			get
			{
				return this.ItemTemplateUsage.FirstItemTemplate;
			}
		}

		public WidgetTemplate LastItemTemplate
		{
			get
			{
				return this.ItemTemplateUsage.LastItemTemplate;
			}
		}

		public ItemTemplateUsageWithData(ItemTemplateUsage itemTemplateUsage)
		{
			this.GivenParameters = new Dictionary<string, WidgetAttributeTemplate>();
			this.ItemTemplateUsage = itemTemplateUsage;
		}
	}
}
