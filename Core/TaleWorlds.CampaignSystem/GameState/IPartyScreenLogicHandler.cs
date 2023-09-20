using System;

namespace TaleWorlds.CampaignSystem.GameState
{
	// Token: 0x02000341 RID: 833
	public interface IPartyScreenLogicHandler
	{
		// Token: 0x06002EAF RID: 11951
		void RequestUserInput(string text, Action accept, Action cancel);
	}
}
