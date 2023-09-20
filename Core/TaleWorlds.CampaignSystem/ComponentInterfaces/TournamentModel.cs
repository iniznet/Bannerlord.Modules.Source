using System;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001B9 RID: 441
	public abstract class TournamentModel : GameModel
	{
		// Token: 0x06001AFC RID: 6908
		public abstract float GetTournamentStartChance(Town town);

		// Token: 0x06001AFD RID: 6909
		public abstract TournamentGame CreateTournament(Town town);

		// Token: 0x06001AFE RID: 6910
		public abstract float GetTournamentEndChance(TournamentGame tournament);

		// Token: 0x06001AFF RID: 6911
		public abstract int GetNumLeaderboardVictoriesAtGameStart();

		// Token: 0x06001B00 RID: 6912
		public abstract float GetTournamentSimulationScore(CharacterObject character);

		// Token: 0x06001B01 RID: 6913
		public abstract int GetRenownReward(Hero winner, Town town);

		// Token: 0x06001B02 RID: 6914
		public abstract int GetInfluenceReward(Hero winner, Town town);

		// Token: 0x06001B03 RID: 6915
		[return: TupleElementNames(new string[] { "skill", "xp" })]
		public abstract ValueTuple<SkillObject, int> GetSkillXpGainFromTournament(Town town);

		// Token: 0x06001B04 RID: 6916
		public abstract Equipment GetParticipantArmor(CharacterObject participant);
	}
}
