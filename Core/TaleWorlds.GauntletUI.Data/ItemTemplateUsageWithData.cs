using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI.PrefabSystem;

namespace TaleWorlds.GauntletUI.Data
{
	// Token: 0x02000009 RID: 9
	public class ItemTemplateUsageWithData
	{
		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000077 RID: 119 RVA: 0x000039DB File Offset: 0x00001BDB
		// (set) Token: 0x06000078 RID: 120 RVA: 0x000039E3 File Offset: 0x00001BE3
		public Dictionary<string, WidgetAttributeTemplate> GivenParameters { get; private set; }

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000079 RID: 121 RVA: 0x000039EC File Offset: 0x00001BEC
		// (set) Token: 0x0600007A RID: 122 RVA: 0x000039F4 File Offset: 0x00001BF4
		public ItemTemplateUsage ItemTemplateUsage { get; private set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x0600007B RID: 123 RVA: 0x000039FD File Offset: 0x00001BFD
		public WidgetTemplate DefaultItemTemplate
		{
			get
			{
				return this.ItemTemplateUsage.DefaultItemTemplate;
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x0600007C RID: 124 RVA: 0x00003A0A File Offset: 0x00001C0A
		public WidgetTemplate FirstItemTemplate
		{
			get
			{
				return this.ItemTemplateUsage.FirstItemTemplate;
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x0600007D RID: 125 RVA: 0x00003A17 File Offset: 0x00001C17
		public WidgetTemplate LastItemTemplate
		{
			get
			{
				return this.ItemTemplateUsage.LastItemTemplate;
			}
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00003A24 File Offset: 0x00001C24
		public ItemTemplateUsageWithData(ItemTemplateUsage itemTemplateUsage)
		{
			this.GivenParameters = new Dictionary<string, WidgetAttributeTemplate>();
			this.ItemTemplateUsage = itemTemplateUsage;
		}
	}
}
