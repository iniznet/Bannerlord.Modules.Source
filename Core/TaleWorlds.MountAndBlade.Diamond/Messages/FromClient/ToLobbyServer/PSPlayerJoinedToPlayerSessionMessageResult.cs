using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[Serializable]
	public class PSPlayerJoinedToPlayerSessionMessageResult : FunctionResult
	{
		public bool Successful { get; private set; }

		public PSPlayerJoinedToPlayerSessionMessageResult(bool successful)
		{
			this.Successful = successful;
		}
	}
}
