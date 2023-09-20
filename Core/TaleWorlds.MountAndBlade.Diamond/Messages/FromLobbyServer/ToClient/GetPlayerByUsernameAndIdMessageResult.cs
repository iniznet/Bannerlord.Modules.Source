using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromLobbyServer.ToClient
{
	[Serializable]
	public class GetPlayerByUsernameAndIdMessageResult : FunctionResult
	{
		public PlayerId PlayerId { get; private set; }

		public GetPlayerByUsernameAndIdMessageResult(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
