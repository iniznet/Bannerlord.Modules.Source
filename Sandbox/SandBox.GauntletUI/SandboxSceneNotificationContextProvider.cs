using System;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;

namespace SandBox.GauntletUI
{
	// Token: 0x02000011 RID: 17
	public class SandboxSceneNotificationContextProvider : ISceneNotificationContextProvider
	{
		// Token: 0x060000B4 RID: 180 RVA: 0x00006F28 File Offset: 0x00005128
		public bool IsContextAllowed(SceneNotificationData.RelevantContextType relevantType)
		{
			return relevantType != 4 || GameStateManager.Current.ActiveState is MapState;
		}
	}
}
