using System;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class PlayerLeaderboardData
	{
		public PlayerId PlayerId { get; set; }

		public string RankId { get; set; }

		public int Rating { get; set; }

		public string Name { get; set; }

		public PlayerLeaderboardData(PlayerId playerId, string rankId, int rating, string name)
		{
			this.PlayerId = playerId;
			this.RankId = rankId;
			this.Rating = rating;
			this.Name = name;
		}
	}
}
