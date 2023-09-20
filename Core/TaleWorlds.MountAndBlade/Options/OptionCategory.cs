using System;
using System.Collections.Generic;
using TaleWorlds.Engine.Options;

namespace TaleWorlds.MountAndBlade.Options
{
	// Token: 0x02000397 RID: 919
	public class OptionCategory
	{
		// Token: 0x06003260 RID: 12896 RVA: 0x000D10A5 File Offset: 0x000CF2A5
		public OptionCategory(IEnumerable<IOptionData> baseOptions, IEnumerable<OptionGroup> groups)
		{
			this.BaseOptions = baseOptions;
			this.Groups = groups;
		}

		// Token: 0x04001545 RID: 5445
		public readonly IEnumerable<IOptionData> BaseOptions;

		// Token: 0x04001546 RID: 5446
		public readonly IEnumerable<OptionGroup> Groups;
	}
}
