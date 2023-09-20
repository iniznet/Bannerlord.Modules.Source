using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001CC RID: 460
	public abstract class CutsceneSelectionModel : GameModel
	{
		// Token: 0x06001B7F RID: 7039
		public abstract SceneNotificationData GetKingdomDestroyedSceneNotification(Kingdom kingdom);
	}
}
