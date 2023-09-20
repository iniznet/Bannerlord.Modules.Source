using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class CreatePremadeGameMessage : Message
	{
		[JsonProperty]
		public string PremadeGameName { get; private set; }

		[JsonProperty]
		public string GameType { get; private set; }

		[JsonProperty]
		public string MapName { get; private set; }

		[JsonProperty]
		public string FactionA { get; private set; }

		[JsonProperty]
		public string FactionB { get; private set; }

		[JsonProperty]
		public string Password { get; private set; }

		[JsonProperty]
		public PremadeGameType PremadeGameType { get; private set; }

		public CreatePremadeGameMessage()
		{
		}

		public CreatePremadeGameMessage(string premadeGameName, string gameType, string mapName, string factionA, string factionB, string password, PremadeGameType premadeGameType)
		{
			this.PremadeGameName = premadeGameName;
			this.GameType = gameType;
			this.MapName = mapName;
			this.FactionA = factionA;
			this.FactionB = factionB;
			this.Password = password;
			this.PremadeGameType = premadeGameType;
		}
	}
}
