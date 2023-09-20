using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.GauntletUI.SceneNotification
{
	public class NativeSceneNotificationContextProvider : ISceneNotificationContextProvider
	{
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
