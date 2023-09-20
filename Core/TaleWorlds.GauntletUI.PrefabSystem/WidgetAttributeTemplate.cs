using System;

namespace TaleWorlds.GauntletUI.PrefabSystem
{
	// Token: 0x02000012 RID: 18
	public class WidgetAttributeTemplate
	{
		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600006E RID: 110 RVA: 0x00002EB4 File Offset: 0x000010B4
		// (set) Token: 0x0600006F RID: 111 RVA: 0x00002EBC File Offset: 0x000010BC
		public WidgetAttributeKeyType KeyType { get; set; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000070 RID: 112 RVA: 0x00002EC5 File Offset: 0x000010C5
		// (set) Token: 0x06000071 RID: 113 RVA: 0x00002ECD File Offset: 0x000010CD
		public WidgetAttributeValueType ValueType { get; set; }

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000072 RID: 114 RVA: 0x00002ED6 File Offset: 0x000010D6
		// (set) Token: 0x06000073 RID: 115 RVA: 0x00002EDE File Offset: 0x000010DE
		public string Key { get; set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000074 RID: 116 RVA: 0x00002EE7 File Offset: 0x000010E7
		// (set) Token: 0x06000075 RID: 117 RVA: 0x00002EEF File Offset: 0x000010EF
		public string Value { get; set; }
	}
}
