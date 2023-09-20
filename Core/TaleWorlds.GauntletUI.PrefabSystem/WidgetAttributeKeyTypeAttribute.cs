using System;

namespace TaleWorlds.GauntletUI.PrefabSystem
{
	// Token: 0x0200000F RID: 15
	public class WidgetAttributeKeyTypeAttribute : WidgetAttributeKeyType
	{
		// Token: 0x06000062 RID: 98 RVA: 0x00002E4C File Offset: 0x0000104C
		public override bool CheckKeyType(string key)
		{
			return true;
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00002E4F File Offset: 0x0000104F
		public override string GetKeyName(string key)
		{
			return key;
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00002E52 File Offset: 0x00001052
		public override string GetSerializedKey(string key)
		{
			return key;
		}
	}
}
