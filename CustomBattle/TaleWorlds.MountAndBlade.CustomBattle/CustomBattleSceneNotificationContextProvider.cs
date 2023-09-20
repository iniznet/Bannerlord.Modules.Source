using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.CustomBattle
{
	public class CustomBattleSceneNotificationContextProvider : ISceneNotificationContextProvider
	{
		public bool IsContextAllowed(SceneNotificationData.RelevantContextType relevantType)
		{
			return relevantType != 2 || GameStateManager.Current.ActiveState is CustomBattleState;
		}
	}
}
