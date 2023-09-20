using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x02000101 RID: 257
	public class DefaultCutsceneSelectionModel : CutsceneSelectionModel
	{
		// Token: 0x06001521 RID: 5409 RVA: 0x00060FE5 File Offset: 0x0005F1E5
		public override SceneNotificationData GetKingdomDestroyedSceneNotification(Kingdom kingdom)
		{
			return new KingdomDestroyedSceneNotificationItem(kingdom);
		}
	}
}
