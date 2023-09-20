using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class ClanInfoChangedMessage : Message
	{
		public ClanHomeInfo ClanHomeInfo { get; private set; }

		public ClanInfoChangedMessage(ClanHomeInfo clanHomeInfo)
		{
			this.ClanHomeInfo = clanHomeInfo;
		}
	}
}
