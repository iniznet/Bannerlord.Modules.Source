using System;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Save
{
	// Token: 0x02000028 RID: 40
	internal interface ISaveContext
	{
		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000166 RID: 358
		DefinitionContext DefinitionContext { get; }

		// Token: 0x06000167 RID: 359
		int AddOrGetStringId(string text);

		// Token: 0x06000168 RID: 360
		int GetObjectId(object target);

		// Token: 0x06000169 RID: 361
		int GetContainerId(object target);

		// Token: 0x0600016A RID: 362
		int GetStringId(string target);
	}
}
