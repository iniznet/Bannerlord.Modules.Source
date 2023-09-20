using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class ClanLeaderboardInfo
	{
		public static ClanLeaderboardInfo Empty { get; private set; } = new ClanLeaderboardInfo(new ClanLeaderboardEntry[0]);

		public ClanLeaderboardEntry[] ClanEntries { get; private set; }

		public ClanLeaderboardInfo(ClanLeaderboardEntry[] entries)
		{
			this.ClanEntries = entries;
		}
	}
}
