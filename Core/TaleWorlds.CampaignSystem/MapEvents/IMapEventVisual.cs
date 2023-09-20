using System;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.MapEvents
{
	// Token: 0x020002C5 RID: 709
	public interface IMapEventVisual
	{
		// Token: 0x0600296B RID: 10603
		void Initialize(Vec2 position, int battleSizeValue, bool hasSound, bool isVisible);

		// Token: 0x0600296C RID: 10604
		void OnMapEventEnd();

		// Token: 0x0600296D RID: 10605
		void SetVisibility(bool isVisible);
	}
}
