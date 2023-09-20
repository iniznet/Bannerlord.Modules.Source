using System;

namespace TaleWorlds.GauntletUI.PrefabSystem
{
	// Token: 0x0200000E RID: 14
	public abstract class WidgetAttributeKeyType
	{
		// Token: 0x0600005E RID: 94
		public abstract bool CheckKeyType(string key);

		// Token: 0x0600005F RID: 95
		public abstract string GetKeyName(string key);

		// Token: 0x06000060 RID: 96
		public abstract string GetSerializedKey(string key);
	}
}
