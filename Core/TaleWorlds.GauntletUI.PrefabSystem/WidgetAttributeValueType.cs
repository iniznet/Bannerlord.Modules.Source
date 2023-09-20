using System;

namespace TaleWorlds.GauntletUI.PrefabSystem
{
	// Token: 0x02000013 RID: 19
	public abstract class WidgetAttributeValueType
	{
		// Token: 0x06000077 RID: 119
		public abstract bool CheckValueType(string value);

		// Token: 0x06000078 RID: 120
		public abstract string GetAttributeValue(string value);

		// Token: 0x06000079 RID: 121
		public abstract string GetSerializedValue(string value);
	}
}
