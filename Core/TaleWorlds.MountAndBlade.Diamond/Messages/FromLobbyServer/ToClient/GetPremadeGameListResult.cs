using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[Serializable]
	public class GetPremadeGameListResult : FunctionResult
	{
		public PremadeGameList GameList { get; private set; }

		public GetPremadeGameListResult(PremadeGameList gameList)
		{
			this.GameList = gameList;
		}
	}
}
