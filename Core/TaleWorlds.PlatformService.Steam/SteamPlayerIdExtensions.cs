using System;
using Steamworks;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.PlatformService.Steam
{
	// Token: 0x02000007 RID: 7
	public static class SteamPlayerIdExtensions
	{
		// Token: 0x06000066 RID: 102 RVA: 0x00002C22 File Offset: 0x00000E22
		public static PlayerId ToPlayerId(this CSteamID steamId)
		{
			return new PlayerId(2, 0UL, steamId.m_SteamID);
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00002C32 File Offset: 0x00000E32
		public static CSteamID ToSteamId(this PlayerId playerId)
		{
			if (playerId.IsValidSteamId())
			{
				return new CSteamID(playerId.Part4);
			}
			return new CSteamID(0UL);
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00002C50 File Offset: 0x00000E50
		public static bool IsValidSteamId(this PlayerId playerId)
		{
			return playerId.IsValid && playerId.ProvidedType == PlayerIdProvidedTypes.Steam;
		}
	}
}
