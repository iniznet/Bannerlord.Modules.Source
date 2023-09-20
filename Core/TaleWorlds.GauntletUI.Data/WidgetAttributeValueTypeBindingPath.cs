using System;
using TaleWorlds.GauntletUI.PrefabSystem;

namespace TaleWorlds.GauntletUI.Data
{
	// Token: 0x02000011 RID: 17
	public class WidgetAttributeValueTypeBindingPath : WidgetAttributeValueType
	{
		// Token: 0x060000A9 RID: 169 RVA: 0x00004450 File Offset: 0x00002650
		public override bool CheckValueType(string value)
		{
			return value.Length > 2 && value[0] == '{' && value[value.Length - 1] == '}';
		}

		// Token: 0x060000AA RID: 170 RVA: 0x0000447A File Offset: 0x0000267A
		public override string GetAttributeValue(string value)
		{
			return value.Substring(1, value.Length - 2);
		}

		// Token: 0x060000AB RID: 171 RVA: 0x0000448B File Offset: 0x0000268B
		public override string GetSerializedValue(string value)
		{
			return "{" + value + "}";
		}
	}
}
