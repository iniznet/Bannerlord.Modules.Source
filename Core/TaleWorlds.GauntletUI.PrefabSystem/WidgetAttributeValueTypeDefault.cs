using System;

namespace TaleWorlds.GauntletUI.PrefabSystem
{
	// Token: 0x02000015 RID: 21
	public class WidgetAttributeValueTypeDefault : WidgetAttributeValueType
	{
		// Token: 0x0600007F RID: 127 RVA: 0x00002F3C File Offset: 0x0000113C
		public override bool CheckValueType(string value)
		{
			return true;
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00002F3F File Offset: 0x0000113F
		public override string GetAttributeValue(string value)
		{
			return value;
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00002F42 File Offset: 0x00001142
		public override string GetSerializedValue(string value)
		{
			return value;
		}
	}
}
