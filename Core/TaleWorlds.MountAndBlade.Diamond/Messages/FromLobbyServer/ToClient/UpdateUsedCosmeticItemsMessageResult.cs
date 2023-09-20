using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[Serializable]
	public class UpdateUsedCosmeticItemsMessageResult : FunctionResult
	{
		public bool Successful { get; }

		public UpdateUsedCosmeticItemsMessageResult(bool successful)
		{
			this.Successful = successful;
		}
	}
}
