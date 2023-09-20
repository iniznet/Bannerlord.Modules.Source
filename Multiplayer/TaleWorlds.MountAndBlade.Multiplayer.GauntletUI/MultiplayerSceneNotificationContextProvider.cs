using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.Multiplayer.GauntletUI
{
	public class MultiplayerSceneNotificationContextProvider : ISceneNotificationContextProvider
	{
		public bool IsContextAllowed(SceneNotificationData.RelevantContextType relevantType)
		{
			return relevantType != 1 || GameStateManager.Current.ActiveState is LobbyState;
		}
	}
}
