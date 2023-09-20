using System;
using TaleWorlds.GauntletUI.PrefabSystem;

namespace TaleWorlds.GauntletUI.Data
{
	// Token: 0x0200000E RID: 14
	public class WidgetAttributeKeyTypeCommandParameter : WidgetAttributeKeyType
	{
		// Token: 0x0600009D RID: 157 RVA: 0x000043C5 File Offset: 0x000025C5
		public override bool CheckKeyType(string key)
		{
			return key.StartsWith("CommandParameter.");
		}

		// Token: 0x0600009E RID: 158 RVA: 0x000043D2 File Offset: 0x000025D2
		public override string GetKeyName(string key)
		{
			return key.Substring("CommandParameter.".Length);
		}

		// Token: 0x0600009F RID: 159 RVA: 0x000043E4 File Offset: 0x000025E4
		public override string GetSerializedKey(string key)
		{
			return "CommandParameter." + key;
		}
	}
}
