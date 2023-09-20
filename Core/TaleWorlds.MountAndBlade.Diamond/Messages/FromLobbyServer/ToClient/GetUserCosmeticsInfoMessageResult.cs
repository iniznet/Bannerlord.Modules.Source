using System;
using System.Collections.Generic;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[Serializable]
	public class GetUserCosmeticsInfoMessageResult : FunctionResult
	{
		public bool Successful { get; }

		public List<string> OwnedCosmetics { get; }

		public Dictionary<string, List<string>> UsedCosmetics { get; }

		public GetUserCosmeticsInfoMessageResult(bool successful, List<string> ownedCosmetics, Dictionary<string, List<string>> usedCosmetics)
		{
			this.Successful = successful;
			this.OwnedCosmetics = ownedCosmetics;
			this.UsedCosmetics = usedCosmetics;
		}
	}
}
