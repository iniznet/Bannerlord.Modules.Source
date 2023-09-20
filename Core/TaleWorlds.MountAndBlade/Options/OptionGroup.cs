using System;
using System.Collections.Generic;
using TaleWorlds.Engine.Options;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.Options
{
	// Token: 0x02000398 RID: 920
	public class OptionGroup
	{
		// Token: 0x06003261 RID: 12897 RVA: 0x000D10BB File Offset: 0x000CF2BB
		public OptionGroup(TextObject groupName, IEnumerable<IOptionData> options)
		{
			this.GroupName = groupName;
			this.Options = options;
		}

		// Token: 0x04001547 RID: 5447
		public readonly TextObject GroupName;

		// Token: 0x04001548 RID: 5448
		public readonly IEnumerable<IOptionData> Options;
	}
}
