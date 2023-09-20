using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[Serializable]
	public class UpdateShownBadgeIdMessageResult : FunctionResult
	{
		public bool Successful { get; }

		public UpdateShownBadgeIdMessageResult(bool successful)
		{
			this.Successful = successful;
		}
	}
}
