using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class ClanLeaderboardEntry
	{
		public Guid ClanId { get; private set; }

		public string Name { get; private set; }

		public string Tag { get; private set; }

		public string Sigil { get; private set; }

		public int WinCount { get; private set; }

		public int LossCount { get; private set; }

		public float Score { get; private set; }

		public ClanLeaderboardEntry(Guid clanId, string name, string tag, string sigil, int winCount, int lossCount, float score)
		{
			this.ClanId = clanId;
			this.Name = name;
			this.Tag = tag;
			this.Sigil = sigil;
			this.WinCount = winCount;
			this.LossCount = lossCount;
			this.Score = score;
		}
	}
}
