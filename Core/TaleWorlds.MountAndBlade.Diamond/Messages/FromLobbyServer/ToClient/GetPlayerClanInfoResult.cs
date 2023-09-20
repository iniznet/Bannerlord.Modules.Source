using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[Serializable]
	public class GetPlayerClanInfoResult : FunctionResult
	{
		public ClanInfo ClanInfo { get; private set; }

		public GetPlayerClanInfoResult(ClanInfo clanInfo)
		{
			this.ClanInfo = clanInfo;
		}
	}
}
