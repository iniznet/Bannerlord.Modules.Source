using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[Serializable]
	public class GetUserCosmeticsInfoMessageResult : FunctionResult
	{
		[JsonProperty]
		public bool Successful { get; private set; }

		[JsonProperty]
		public List<string> OwnedCosmetics { get; private set; }

		[JsonProperty]
		public Dictionary<string, List<string>> UsedCosmetics { get; private set; }

		public GetUserCosmeticsInfoMessageResult()
		{
		}

		public GetUserCosmeticsInfoMessageResult(bool successful, List<string> ownedCosmetics, Dictionary<string, List<string>> usedCosmetics)
		{
			this.Successful = successful;
			this.OwnedCosmetics = ownedCosmetics;
			this.UsedCosmetics = usedCosmetics;
		}
	}
}
