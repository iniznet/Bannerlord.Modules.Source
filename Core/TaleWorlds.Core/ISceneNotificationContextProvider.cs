using System;

namespace TaleWorlds.Core
{
	// Token: 0x020000BC RID: 188
	public interface ISceneNotificationContextProvider
	{
		// Token: 0x0600096F RID: 2415
		bool IsContextAllowed(SceneNotificationData.RelevantContextType relevantType);
	}
}
