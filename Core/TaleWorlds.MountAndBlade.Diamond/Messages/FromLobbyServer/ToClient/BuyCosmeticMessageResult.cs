using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[Serializable]
	public class BuyCosmeticMessageResult : FunctionResult
	{
		public bool Successful { get; }

		public int Gold { get; }

		public BuyCosmeticMessageResult(bool successful, int gold)
		{
			this.Successful = successful;
			this.Gold = gold;
		}
	}
}
