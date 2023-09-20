using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x0200019F RID: 415
	public abstract class DisguiseDetectionModel : GameModel
	{
		// Token: 0x06001A49 RID: 6729
		public abstract float CalculateDisguiseDetectionProbability(Settlement settlement);
	}
}
