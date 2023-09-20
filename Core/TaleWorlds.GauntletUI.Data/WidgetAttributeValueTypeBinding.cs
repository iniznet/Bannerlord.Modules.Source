using System;
using TaleWorlds.GauntletUI.PrefabSystem;

namespace TaleWorlds.GauntletUI.Data
{
	// Token: 0x02000010 RID: 16
	public class WidgetAttributeValueTypeBinding : WidgetAttributeValueType
	{
		// Token: 0x060000A5 RID: 165 RVA: 0x0000441C File Offset: 0x0000261C
		public override bool CheckValueType(string value)
		{
			return value.StartsWith("@");
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00004429 File Offset: 0x00002629
		public override string GetAttributeValue(string value)
		{
			return value.Substring("@".Length);
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x0000443B File Offset: 0x0000263B
		public override string GetSerializedValue(string value)
		{
			return "@" + value;
		}
	}
}
