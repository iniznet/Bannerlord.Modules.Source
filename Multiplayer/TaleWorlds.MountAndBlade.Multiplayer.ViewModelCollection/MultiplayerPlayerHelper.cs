using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlatformService;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection
{
	public static class MultiplayerPlayerHelper
	{
		private static IReadOnlyCollection<PlayerId> PlatformBlocks
		{
			get
			{
				return PlatformServices.Instance.BlockedUsers;
			}
		}

		public static bool IsBlocked(PlayerId playerID)
		{
			return PermaMuteList.IsPlayerMuted(playerID) || (MultiplayerPlayerHelper.PlatformBlocks != null && MultiplayerPlayerHelper.PlatformBlocks.Contains(playerID));
		}
	}
}
