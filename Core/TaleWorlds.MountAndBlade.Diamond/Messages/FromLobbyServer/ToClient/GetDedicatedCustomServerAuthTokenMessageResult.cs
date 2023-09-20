using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[Serializable]
	public class GetDedicatedCustomServerAuthTokenMessageResult : FunctionResult
	{
		public string AuthToken { get; private set; }

		public GetDedicatedCustomServerAuthTokenMessageResult(string authToken)
		{
			this.AuthToken = authToken;
		}
	}
}
