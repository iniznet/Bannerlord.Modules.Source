using System;
using TaleWorlds.Library;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.Diamond
{
	// Token: 0x0200000F RID: 15
	public interface ILoginAccessProvider
	{
		// Token: 0x06000042 RID: 66
		void Initialize(string preferredUserName, PlatformInitParams initParams);

		// Token: 0x06000043 RID: 67
		string GetUserName();

		// Token: 0x06000044 RID: 68
		PlayerId GetPlayerId();

		// Token: 0x06000045 RID: 69
		AccessObjectResult CreateAccessObject();
	}
}
