using System;
using Galaxy.Api;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.PlatformService.GOG
{
	// Token: 0x0200000F RID: 15
	public static class SteamPlayerIdExtensions
	{
		// Token: 0x06000084 RID: 132 RVA: 0x000032BB File Offset: 0x000014BB
		public static PlayerId ToPlayerId(this GalaxyID galaxyID)
		{
			return new PlayerId(5, 0UL, galaxyID.ToUint64());
		}

		// Token: 0x06000085 RID: 133 RVA: 0x000032CB File Offset: 0x000014CB
		public static GalaxyID ToGOGID(this PlayerId playerId)
		{
			if (playerId.IsValidGOGId())
			{
				return new GalaxyID(playerId.Part4);
			}
			return new GalaxyID(0UL);
		}

		// Token: 0x06000086 RID: 134 RVA: 0x000032E9 File Offset: 0x000014E9
		public static bool IsValidGOGId(this PlayerId playerId)
		{
			return playerId.IsValid && playerId.ProvidedType == PlayerIdProvidedTypes.GOG;
		}
	}
}
