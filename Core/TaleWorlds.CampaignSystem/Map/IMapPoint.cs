using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Map
{
	// Token: 0x020000C9 RID: 201
	public interface IMapPoint
	{
		// Token: 0x06001270 RID: 4720
		void OnGameInitialized();

		// Token: 0x17000547 RID: 1351
		// (get) Token: 0x06001271 RID: 4721
		TextObject Name { get; }

		// Token: 0x17000548 RID: 1352
		// (get) Token: 0x06001272 RID: 4722
		Vec2 Position2D { get; }

		// Token: 0x17000549 RID: 1353
		// (get) Token: 0x06001273 RID: 4723
		PathFaceRecord CurrentNavigationFace { get; }

		// Token: 0x06001274 RID: 4724
		Vec3 GetLogicalPosition();

		// Token: 0x1700054A RID: 1354
		// (get) Token: 0x06001275 RID: 4725
		IFaction MapFaction { get; }

		// Token: 0x1700054B RID: 1355
		// (get) Token: 0x06001276 RID: 4726
		bool IsInspected { get; }

		// Token: 0x1700054C RID: 1356
		// (get) Token: 0x06001277 RID: 4727
		bool IsVisible { get; }

		// Token: 0x1700054D RID: 1357
		// (get) Token: 0x06001278 RID: 4728
		// (set) Token: 0x06001279 RID: 4729
		bool IsActive { get; set; }
	}
}
