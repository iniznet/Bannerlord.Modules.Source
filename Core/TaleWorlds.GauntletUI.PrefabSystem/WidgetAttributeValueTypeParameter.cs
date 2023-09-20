using System;

namespace TaleWorlds.GauntletUI.PrefabSystem
{
	// Token: 0x02000016 RID: 22
	public class WidgetAttributeValueTypeParameter : WidgetAttributeValueType
	{
		// Token: 0x06000083 RID: 131 RVA: 0x00002F4D File Offset: 0x0000114D
		public override bool CheckValueType(string value)
		{
			return value.StartsWith("*");
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00002F5A File Offset: 0x0000115A
		public override string GetAttributeValue(string value)
		{
			return value.Substring("*".Length);
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00002F6C File Offset: 0x0000116C
		public override string GetSerializedValue(string value)
		{
			return "*" + value;
		}
	}
}
