using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.PlayerServices;
using TaleWorlds.PlayerServices.Avatar;

namespace TaleWorlds.Avatar.PlayerServices
{
	// Token: 0x02000002 RID: 2
	public static class AvatarServices
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
		// (set) Token: 0x06000002 RID: 2 RVA: 0x0000204F File Offset: 0x0000024F
		public static int ForcedAvatarCount { get; private set; }

		// Token: 0x06000003 RID: 3 RVA: 0x00002057 File Offset: 0x00000257
		public static int GetForcedAvatarIndexOfPlayer(PlayerId playerID)
		{
			return MathF.Abs(playerID.GetHashCode()) % AvatarServices.ForcedAvatarCount;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002071 File Offset: 0x00000271
		static AvatarServices()
		{
			AvatarServices.AddAvatarService(PlayerIdProvidedTypes.Steam, new SteamAvatarService());
			AvatarServices.AddAvatarService(PlayerIdProvidedTypes.GOG, new GOGAvatarService());
			AvatarServices.InitializeFallbackAvatarService();
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002098 File Offset: 0x00000298
		public static void UpdateAvatarServices(float dt)
		{
			foreach (IAvatarService avatarService in AvatarServices._allAvatarServices.Values)
			{
				avatarService.Tick(dt);
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000020F0 File Offset: 0x000002F0
		public static AvatarData GetPlayerAvatar(PlayerId playerId, int forcedIndex)
		{
			IAvatarService forcedAvatarService;
			AvatarServices._allAvatarServices.TryGetValue(playerId.ProvidedType, out forcedAvatarService);
			if (forcedIndex >= 0 || (forcedIndex < 0 && forcedAvatarService == null))
			{
				forcedAvatarService = AvatarServices._forcedAvatarService;
			}
			if (forcedAvatarService != null && !forcedAvatarService.IsInitialized() && forcedAvatarService != null)
			{
				forcedAvatarService.Initialize();
			}
			return forcedAvatarService.GetPlayerAvatar(playerId);
		}

		// Token: 0x06000007 RID: 7 RVA: 0x0000213D File Offset: 0x0000033D
		private static void InitializeFallbackAvatarService()
		{
			AvatarServices._forcedAvatarService = new ForcedAvatarService();
			AvatarServices._forcedAvatarService.Initialize();
			AvatarServices.ForcedAvatarCount = AvatarServices._forcedAvatarService.AvatarCount;
			AvatarServices.AddAvatarService(PlayerIdProvidedTypes.Forced, AvatarServices._forcedAvatarService);
		}

		// Token: 0x06000008 RID: 8 RVA: 0x0000216D File Offset: 0x0000036D
		public static void AddAvatarService(PlayerIdProvidedTypes type, IAvatarService avatarService)
		{
			if (AvatarServices._allAvatarServices.ContainsKey(type))
			{
				AvatarServices._allAvatarServices[type] = avatarService;
				return;
			}
			AvatarServices._allAvatarServices.Add(type, avatarService);
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002198 File Offset: 0x00000398
		public static void ClearAvatarCaches()
		{
			foreach (KeyValuePair<PlayerIdProvidedTypes, IAvatarService> keyValuePair in AvatarServices._allAvatarServices)
			{
				keyValuePair.Value.ClearCache();
			}
		}

		// Token: 0x04000001 RID: 1
		private static Dictionary<PlayerIdProvidedTypes, IAvatarService> _allAvatarServices = new Dictionary<PlayerIdProvidedTypes, IAvatarService>();

		// Token: 0x04000003 RID: 3
		private static ForcedAvatarService _forcedAvatarService;
	}
}
