using System;
using TaleWorlds.GauntletUI.PrefabSystem;

namespace TaleWorlds.GauntletUI.Data
{
	// Token: 0x0200000D RID: 13
	public class WidgetAttributeKeyTypeCommand : WidgetAttributeKeyType
	{
		// Token: 0x06000099 RID: 153 RVA: 0x00004391 File Offset: 0x00002591
		public override bool CheckKeyType(string key)
		{
			return key.StartsWith("Command.");
		}

		// Token: 0x0600009A RID: 154 RVA: 0x0000439E File Offset: 0x0000259E
		public override string GetKeyName(string key)
		{
			return key.Substring("Command.".Length);
		}

		// Token: 0x0600009B RID: 155 RVA: 0x000043B0 File Offset: 0x000025B0
		public override string GetSerializedKey(string key)
		{
			return "Command." + key;
		}
	}
}
