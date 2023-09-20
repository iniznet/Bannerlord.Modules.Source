using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Flags]
	public enum Features
	{
		None = 0,
		Matchmaking = 1,
		CustomGame = 2,
		Party = 4,
		Clan = 8,
		BannerlordFriendList = 16,
		TextChat = 32,
		All = -1
	}
}
