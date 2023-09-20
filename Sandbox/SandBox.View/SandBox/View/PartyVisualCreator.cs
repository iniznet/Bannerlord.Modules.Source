using System;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;

namespace SandBox.View
{
	// Token: 0x0200000D RID: 13
	public class PartyVisualCreator : IPartyVisualCreator
	{
		// Token: 0x06000054 RID: 84 RVA: 0x00004488 File Offset: 0x00002688
		IPartyVisual IPartyVisualCreator.CreatePartyVisual()
		{
			return new PartyVisual();
		}
	}
}
