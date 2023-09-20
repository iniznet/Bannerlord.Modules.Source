using System;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;

namespace SandBox.GauntletUI
{
	public class SandboxSceneNotificationContextProvider : ISceneNotificationContextProvider
	{
		public bool IsContextAllowed(SceneNotificationData.RelevantContextType relevantType)
		{
			return relevantType != 4 || GameStateManager.Current.ActiveState is MapState;
		}
	}
}
