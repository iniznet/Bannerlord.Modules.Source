using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond.Ranked;

namespace Messages.FromLobbyServer.ToClient
{
	[Serializable]
	public class GetPlayerGameTypeRankInfoMessageResult : FunctionResult
	{
		public GameTypeRankInfo[] GameTypeRankInfo { get; private set; }

		public GetPlayerGameTypeRankInfoMessageResult(GameTypeRankInfo[] gameTypeRankInfo)
		{
			this.GameTypeRankInfo = gameTypeRankInfo;
		}
	}
}
