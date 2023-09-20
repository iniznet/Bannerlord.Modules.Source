using System;
using System.Collections.Generic;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromLobbyServer.ToClient
{
	[Serializable]
	public class GetOtherPlayersStateMessageResult : FunctionResult
	{
		public List<ValueTuple<PlayerId, AnotherPlayerData>> States { get; }

		public GetOtherPlayersStateMessageResult(List<ValueTuple<PlayerId, AnotherPlayerData>> states)
		{
			this.States = states;
		}
	}
}
