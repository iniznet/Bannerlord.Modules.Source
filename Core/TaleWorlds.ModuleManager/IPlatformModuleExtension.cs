using System;

namespace TaleWorlds.ModuleManager
{
	// Token: 0x02000003 RID: 3
	public interface IPlatformModuleExtension
	{
		// Token: 0x06000009 RID: 9
		void Initialize();

		// Token: 0x0600000A RID: 10
		void Destroy();

		// Token: 0x0600000B RID: 11
		string[] GetModulePaths();
	}
}
