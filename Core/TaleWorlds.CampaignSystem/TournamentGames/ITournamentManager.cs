using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.TournamentGames
{
	// Token: 0x0200027F RID: 639
	public interface ITournamentManager
	{
		// Token: 0x0600217F RID: 8575
		void AddTournament(TournamentGame game);

		// Token: 0x06002180 RID: 8576
		TournamentGame GetTournamentGame(Town town);

		// Token: 0x06002181 RID: 8577
		void OnPlayerJoinMatch(Type gameType);

		// Token: 0x06002182 RID: 8578
		void OnPlayerJoinTournament(Type gameType, Settlement settlement);

		// Token: 0x06002183 RID: 8579
		void OnPlayerWatchTournament(Type gameType, Settlement settlement);

		// Token: 0x06002184 RID: 8580
		void OnPlayerWinMatch(Type gameType);

		// Token: 0x06002185 RID: 8581
		void OnPlayerWinTournament(Type gameType);

		// Token: 0x06002186 RID: 8582
		void InitializeLeaderboardEntry(Hero hero, int initialVictories = 0);

		// Token: 0x06002187 RID: 8583
		void AddLeaderboardEntry(Hero hero);

		// Token: 0x06002188 RID: 8584
		void GivePrizeToWinner(TournamentGame tournament, Hero winner, bool isPlayerParticipated);

		// Token: 0x06002189 RID: 8585
		void DeleteLeaderboardEntry(Hero hero);

		// Token: 0x0600218A RID: 8586
		List<KeyValuePair<Hero, int>> GetLeaderboard();

		// Token: 0x0600218B RID: 8587
		int GetLeaderBoardRank(Hero hero);

		// Token: 0x0600218C RID: 8588
		Hero GetLeaderBoardLeader();

		// Token: 0x0600218D RID: 8589
		void ResolveTournament(TournamentGame tournament, Town town);
	}
}
