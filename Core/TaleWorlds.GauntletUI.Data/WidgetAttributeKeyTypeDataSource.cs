using System;
using TaleWorlds.GauntletUI.PrefabSystem;

namespace TaleWorlds.GauntletUI.Data
{
	// Token: 0x0200000F RID: 15
	public class WidgetAttributeKeyTypeDataSource : WidgetAttributeKeyType
	{
		// Token: 0x060000A1 RID: 161 RVA: 0x000043F9 File Offset: 0x000025F9
		public override bool CheckKeyType(string key)
		{
			return key == "DataSource";
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00004406 File Offset: 0x00002606
		public override string GetKeyName(string key)
		{
			return "DataSource";
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x0000440D File Offset: 0x0000260D
		public override string GetSerializedKey(string key)
		{
			return "DataSource";
		}
	}
}
