using System;
using TaleWorlds.CampaignSystem.TournamentGames;

namespace SandBox.Tournaments
{
	// Token: 0x02000018 RID: 24
	public interface ITournamentGameBehavior
	{
		// Token: 0x060000E9 RID: 233
		void StartMatch(TournamentMatch match, bool isLastRound);

		// Token: 0x060000EA RID: 234
		void SkipMatch(TournamentMatch match);

		// Token: 0x060000EB RID: 235
		bool IsMatchEnded();

		// Token: 0x060000EC RID: 236
		void OnMatchEnded();
	}
}
