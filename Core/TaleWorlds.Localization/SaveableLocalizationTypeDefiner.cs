using System;
using System.Collections.Generic;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.Localization
{
	// Token: 0x0200000B RID: 11
	public class SaveableLocalizationTypeDefiner : SaveableTypeDefiner
	{
		// Token: 0x06000084 RID: 132 RVA: 0x000041EC File Offset: 0x000023EC
		public SaveableLocalizationTypeDefiner()
			: base(20000)
		{
		}

		// Token: 0x06000085 RID: 133 RVA: 0x000041F9 File Offset: 0x000023F9
		protected override void DefineClassTypes()
		{
			base.AddClassDefinition(typeof(TextObject), 1, null);
		}

		// Token: 0x06000086 RID: 134 RVA: 0x0000420D File Offset: 0x0000240D
		protected override void DefineContainerDefinitions()
		{
			base.ConstructContainerDefinition(typeof(Dictionary<string, TextObject>));
		}
	}
}
