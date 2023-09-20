using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class ClanInfoChangedMessage : Message
	{
		[JsonProperty]
		public ClanHomeInfo ClanHomeInfo { get; private set; }

		public ClanInfoChangedMessage()
		{
		}

		public ClanInfoChangedMessage(ClanHomeInfo clanHomeInfo)
		{
			this.ClanHomeInfo = clanHomeInfo;
		}
	}
}
