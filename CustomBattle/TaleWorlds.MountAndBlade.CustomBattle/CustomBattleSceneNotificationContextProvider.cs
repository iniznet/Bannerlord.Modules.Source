using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.CustomBattle
{
	// Token: 0x0200000B RID: 11
	public class CustomBattleSceneNotificationContextProvider : ISceneNotificationContextProvider
	{
		// Token: 0x060000A6 RID: 166 RVA: 0x00007075 File Offset: 0x00005275
		public bool IsContextAllowed(SceneNotificationData.RelevantContextType relevantType)
		{
			return relevantType != 2 || GameStateManager.Current.ActiveState is CustomBattleState;
		}
	}
}
