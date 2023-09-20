using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond.Ranked;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class PlayerMMRUpdateMessage : Message
	{
		public RankBarInfo OldInfo { get; private set; }

		public RankBarInfo NewInfo { get; private set; }

		public PlayerMMRUpdateMessage(RankBarInfo oldInfo, RankBarInfo newInfo)
		{
			this.OldInfo = oldInfo;
			this.NewInfo = newInfo;
		}
	}
}
