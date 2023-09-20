using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[Serializable]
	public class CreatePremadeGameMessageResult : FunctionResult
	{
		public bool Successful { get; private set; }

		public CreatePremadeGameMessageResult(bool successful)
		{
			this.Successful = successful;
		}
	}
}
