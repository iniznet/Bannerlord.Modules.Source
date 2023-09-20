﻿using System;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	public abstract class TournamentModel : GameModel
	{
		public abstract float GetTournamentStartChance(Town town);

		public abstract TournamentGame CreateTournament(Town town);

		public abstract float GetTournamentEndChance(TournamentGame tournament);

		public abstract int GetNumLeaderboardVictoriesAtGameStart();

		public abstract float GetTournamentSimulationScore(CharacterObject character);

		public abstract int GetRenownReward(Hero winner, Town town);

		public abstract int GetInfluenceReward(Hero winner, Town town);

		[return: TupleElementNames(new string[] { "skill", "xp" })]
		public abstract ValueTuple<SkillObject, int> GetSkillXpGainFromTournament(Town town);

		public abstract Equipment GetParticipantArmor(CharacterObject participant);
	}
}
