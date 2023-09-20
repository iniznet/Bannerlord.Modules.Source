using System;

namespace TaleWorlds.DotNet
{
	// Token: 0x02000013 RID: 19
	public interface IManagedComponent
	{
		// Token: 0x06000044 RID: 68
		void OnCustomCallbackMethodPassed(string name, Delegate method);

		// Token: 0x06000045 RID: 69
		void OnStart();

		// Token: 0x06000046 RID: 70
		void OnApplicationTick(float dt);
	}
}
