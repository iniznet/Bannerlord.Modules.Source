using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlatformService;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer
{
	// Token: 0x02000046 RID: 70
	public static class MultiplayerPlayerHelper
	{
		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x060005D6 RID: 1494 RVA: 0x00018B3E File Offset: 0x00016D3E
		private static IReadOnlyCollection<PlayerId> PlatformBlocks
		{
			get
			{
				return PlatformServices.Instance.BlockedUsers;
			}
		}

		// Token: 0x060005D7 RID: 1495 RVA: 0x00018B4A File Offset: 0x00016D4A
		public static bool IsBlocked(PlayerId playerID)
		{
			return PermaMuteList.IsPlayerMuted(playerID) || (MultiplayerPlayerHelper.PlatformBlocks != null && MultiplayerPlayerHelper.PlatformBlocks.Contains(playerID));
		}
	}
}
