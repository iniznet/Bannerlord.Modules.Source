using System;
using Steamworks;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.PlatformService.Steam
{
	public static class SteamPlayerIdExtensions
	{
		public static PlayerId ToPlayerId(this CSteamID steamId)
		{
			return new PlayerId(2, 0UL, steamId.m_SteamID);
		}

		public static CSteamID ToSteamId(this PlayerId playerId)
		{
			if (playerId.IsValidSteamId())
			{
				return new CSteamID(playerId.Part4);
			}
			return new CSteamID(0UL);
		}

		public static bool IsValidSteamId(this PlayerId playerId)
		{
			return playerId.IsValid && playerId.ProvidedType == PlayerIdProvidedTypes.Steam;
		}
	}
}
