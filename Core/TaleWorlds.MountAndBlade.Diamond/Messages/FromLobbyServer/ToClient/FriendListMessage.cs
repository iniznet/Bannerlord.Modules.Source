﻿using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class FriendListMessage : Message
	{
		public FriendInfo[] Friends { get; private set; }

		public FriendListMessage(FriendInfo[] friends)
		{
			this.Friends = friends;
		}
	}
}
