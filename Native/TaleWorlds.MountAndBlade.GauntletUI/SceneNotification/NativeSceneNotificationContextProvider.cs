using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.GauntletUI.SceneNotification
{
	// Token: 0x0200001D RID: 29
	public class NativeSceneNotificationContextProvider : ISceneNotificationContextProvider
	{
		// Token: 0x06000116 RID: 278 RVA: 0x00006EA6 File Offset: 0x000050A6
		public bool IsContextAllowed(SceneNotificationData.RelevantContextType relevantType)
		{
			if (relevantType != 1)
			{
				return relevantType != 3 || GameStateManager.Current.ActiveState is MissionState;
			}
			return GameStateManager.Current.ActiveState is LobbyState;
		}
	}
}
