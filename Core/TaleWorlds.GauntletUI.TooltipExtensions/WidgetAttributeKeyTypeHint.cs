using System;
using TaleWorlds.GauntletUI.PrefabSystem;

namespace TaleWorlds.GauntletUI.TooltipExtensions
{
	// Token: 0x02000004 RID: 4
	public class WidgetAttributeKeyTypeHint : WidgetAttributeKeyType
	{
		// Token: 0x06000017 RID: 23 RVA: 0x000022ED File Offset: 0x000004ED
		public override bool CheckKeyType(string key)
		{
			return key == "Hint";
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000022FA File Offset: 0x000004FA
		public override string GetKeyName(string key)
		{
			return "Hint";
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002301 File Offset: 0x00000501
		public override string GetSerializedKey(string key)
		{
			return "Hint";
		}
	}
}
