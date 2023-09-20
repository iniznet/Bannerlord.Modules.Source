using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[Serializable]
	public class GetClanHomeInfoResult : FunctionResult
	{
		public ClanHomeInfo ClanHomeInfo { get; private set; }

		public GetClanHomeInfoResult(ClanHomeInfo clanHomeInfo)
		{
			this.ClanHomeInfo = clanHomeInfo;
		}
	}
}
