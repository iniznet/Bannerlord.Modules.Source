using System;

namespace TaleWorlds.GauntletUI.PrefabSystem
{
	// Token: 0x02000014 RID: 20
	public class WidgetAttributeValueTypeConstant : WidgetAttributeValueType
	{
		// Token: 0x0600007B RID: 123 RVA: 0x00002F08 File Offset: 0x00001108
		public override bool CheckValueType(string value)
		{
			return value.StartsWith("!");
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00002F15 File Offset: 0x00001115
		public override string GetAttributeValue(string value)
		{
			return value.Substring("!".Length);
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00002F27 File Offset: 0x00001127
		public override string GetSerializedValue(string value)
		{
			return "!" + value;
		}
	}
}
