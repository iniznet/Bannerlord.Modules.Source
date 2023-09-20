using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.PlayerServices;
using TaleWorlds.PlayerServices.Avatar;

namespace TaleWorlds.Avatar.PlayerServices
{
	public static class AvatarServices
	{
		public static int ForcedAvatarCount { get; private set; }

		public static int GetForcedAvatarIndexOfPlayer(PlayerId playerID)
		{
			return MathF.Abs(playerID.GetHashCode()) % AvatarServices.ForcedAvatarCount;
		}

		static AvatarServices()
		{
			AvatarServices.AddAvatarService(PlayerIdProvidedTypes.Steam, new SteamAvatarService());
			AvatarServices.AddAvatarService(PlayerIdProvidedTypes.GOG, new GOGAvatarService());
			AvatarServices.InitializeFallbackAvatarService();
		}

		public static void UpdateAvatarServices(float dt)
		{
			foreach (IAvatarService avatarService in AvatarServices._allAvatarServices.Values)
			{
				avatarService.Tick(dt);
			}
		}

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

		private static void InitializeFallbackAvatarService()
		{
			AvatarServices._forcedAvatarService = new ForcedAvatarService();
			AvatarServices._forcedAvatarService.Initialize();
			AvatarServices.ForcedAvatarCount = AvatarServices._forcedAvatarService.AvatarCount;
			AvatarServices.AddAvatarService(PlayerIdProvidedTypes.Forced, AvatarServices._forcedAvatarService);
		}

		public static void AddAvatarService(PlayerIdProvidedTypes type, IAvatarService avatarService)
		{
			if (AvatarServices._allAvatarServices.ContainsKey(type))
			{
				AvatarServices._allAvatarServices[type] = avatarService;
				return;
			}
			AvatarServices._allAvatarServices.Add(type, avatarService);
		}

		public static void ClearAvatarCaches()
		{
			foreach (KeyValuePair<PlayerIdProvidedTypes, IAvatarService> keyValuePair in AvatarServices._allAvatarServices)
			{
				keyValuePair.Value.ClearCache();
			}
		}

		private static Dictionary<PlayerIdProvidedTypes, IAvatarService> _allAvatarServices = new Dictionary<PlayerIdProvidedTypes, IAvatarService>();

		private static ForcedAvatarService _forcedAvatarService;
	}
}
