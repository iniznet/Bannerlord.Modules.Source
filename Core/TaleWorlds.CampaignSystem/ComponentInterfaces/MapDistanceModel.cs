using System;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x02000173 RID: 371
	public abstract class MapDistanceModel : GameModel
	{
		// Token: 0x1700067E RID: 1662
		// (get) Token: 0x060018FC RID: 6396
		// (set) Token: 0x060018FD RID: 6397
		public abstract float MaximumDistanceBetweenTwoSettlements { get; set; }

		// Token: 0x060018FE RID: 6398
		public abstract float GetDistance(Settlement fromSettlement, Settlement toSettlement);

		// Token: 0x060018FF RID: 6399
		public abstract float GetDistance(MobileParty fromParty, Settlement toSettlement);

		// Token: 0x06001900 RID: 6400
		public abstract float GetDistance(MobileParty fromParty, MobileParty toParty);

		// Token: 0x06001901 RID: 6401
		public abstract bool GetDistance(Settlement fromSettlement, Settlement toSettlement, float maximumDistance, out float distance);

		// Token: 0x06001902 RID: 6402
		public abstract bool GetDistance(MobileParty fromParty, Settlement toSettlement, float maximumDistance, out float distance);

		// Token: 0x06001903 RID: 6403
		public abstract bool GetDistance(IMapPoint fromMapPoint, MobileParty toParty, float maximumDistance, out float distance);

		// Token: 0x06001904 RID: 6404
		public abstract bool GetDistance(IMapPoint fromMapPoint, Settlement toSettlement, float maximumDistance, out float distance);

		// Token: 0x06001905 RID: 6405
		public abstract bool GetDistance(IMapPoint fromMapPoint, in Vec2 toPoint, float maximumDistance, out float distance);

		// Token: 0x06001906 RID: 6406
		public abstract Settlement GetClosestSettlementForNavigationMesh(PathFaceRecord face);
	}
}
