using System;
using System.Collections.Generic;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond.Ranked;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class BattleOverMessage : Message
	{
		public int OldExperience { get; private set; }

		public int NewExperience { get; private set; }

		public List<string> EarnedBadges { get; private set; }

		public int GoldGained { get; private set; }

		public RankBarInfo OldInfo { get; private set; }

		public RankBarInfo NewInfo { get; private set; }

		public BattleOverMessage(int oldExperience, int newExperience, List<string> earnedBadges, int goldGained)
		{
			this.OldExperience = oldExperience;
			this.NewExperience = newExperience;
			this.EarnedBadges = earnedBadges;
			this.GoldGained = goldGained;
		}
	}
}
