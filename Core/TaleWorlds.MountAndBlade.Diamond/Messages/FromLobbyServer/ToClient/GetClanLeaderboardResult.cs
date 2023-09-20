using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[Serializable]
	public class GetClanLeaderboardResult : FunctionResult
	{
		public ClanLeaderboardInfo ClanLeaderboardInfo { get; private set; }

		public GetClanLeaderboardResult(ClanLeaderboardInfo info)
		{
			this.ClanLeaderboardInfo = info;
		}
	}
}
