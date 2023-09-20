using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.Library;

namespace Messages.FromBattleServer.ToBattleServerManager
{
	[MessageDescription("BattleServer", "BattleServerManager")]
	[Serializable]
	public class BattleServerReadyMessage : LoginMessage
	{
		[JsonProperty]
		public ApplicationVersion ApplicationVersion { get; private set; }

		[JsonProperty]
		public string AssignedAddress { get; private set; }

		[JsonProperty]
		public ushort AssignedPort { get; private set; }

		[JsonProperty]
		public string Region { get; private set; }

		[JsonProperty]
		public sbyte Priority { get; private set; }

		[JsonProperty]
		public string Password { get; private set; }

		[JsonProperty]
		public string GameType { get; private set; }

		public BattleServerReadyMessage()
		{
		}

		public BattleServerReadyMessage(PeerId peerId, ApplicationVersion applicationVersion, string assignedAddress, ushort assignedPort, string region, sbyte priority, string password, string gameType)
			: base(peerId)
		{
			this.ApplicationVersion = applicationVersion;
			this.AssignedAddress = assignedAddress;
			this.AssignedPort = assignedPort;
			this.Region = region;
			this.Priority = priority;
			this.Password = password;
			this.GameType = gameType;
		}
	}
}
