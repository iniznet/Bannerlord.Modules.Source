using System;
using TaleWorlds.Diamond;
using TaleWorlds.Library;

namespace Messages.FromBattleServer.ToBattleServerManager
{
	[MessageDescription("BattleServer", "BattleServerManager")]
	[Serializable]
	public class BattleServerReadyMessage : LoginMessage
	{
		public ApplicationVersion ApplicationVersion { get; private set; }

		public string AssignedAddress { get; private set; }

		public ushort AssignedPort { get; private set; }

		public string Region { get; private set; }

		public sbyte Priority { get; private set; }

		public string Password { get; private set; }

		public string GameType { get; private set; }

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
