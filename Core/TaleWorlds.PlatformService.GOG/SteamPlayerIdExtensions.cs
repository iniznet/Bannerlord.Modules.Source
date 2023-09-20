using System;
using Galaxy.Api;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.PlatformService.GOG
{
	public static class SteamPlayerIdExtensions
	{
		public static PlayerId ToPlayerId(this GalaxyID galaxyID)
		{
			return new PlayerId(5, 0UL, galaxyID.ToUint64());
		}

		public static GalaxyID ToGOGID(this PlayerId playerId)
		{
			if (playerId.IsValidGOGId())
			{
				return new GalaxyID(playerId.Part4);
			}
			return new GalaxyID(0UL);
		}

		public static bool IsValidGOGId(this PlayerId playerId)
		{
			return playerId.IsValid && playerId.ProvidedType == PlayerIdProvidedTypes.GOG;
		}
	}
}
