using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Steamworks;
using TaleWorlds.Localization;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.PlatformService.Steam
{
	public class SteamFriendListService : IFriendListService
	{
		public SteamFriendListService(SteamPlatformServices steamPlatformServices)
		{
			this._steamPlatformServices = steamPlatformServices;
		}

		public event Action<PlayerId> OnUserStatusChanged;

		public event Action<PlayerId> OnFriendRemoved;

		public event Action OnFriendListChanged;

		bool IFriendListService.InGameStatusFetchable
		{
			get
			{
				return true;
			}
		}

		bool IFriendListService.AllowsFriendOperations
		{
			get
			{
				return false;
			}
		}

		bool IFriendListService.CanInvitePlayersToPlatformSession
		{
			get
			{
				return false;
			}
		}

		bool IFriendListService.IncludeInAllFriends
		{
			get
			{
				return true;
			}
		}

		string IFriendListService.GetServiceCodeName()
		{
			return "Steam";
		}

		TextObject IFriendListService.GetServiceLocalizedName()
		{
			return new TextObject("{=!}Steam", null);
		}

		FriendListServiceType IFriendListService.GetFriendListServiceType()
		{
			return FriendListServiceType.Steam;
		}

		IEnumerable<PlayerId> IFriendListService.GetAllFriends()
		{
			if (SteamAPI.IsSteamRunning() && this._steamPlatformServices.Initialized)
			{
				int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
				int num;
				for (int i = 0; i < friendCount; i = num)
				{
					yield return SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate).ToPlayerId();
					num = i + 1;
				}
			}
			yield break;
		}

		Task<bool> IFriendListService.GetUserOnlineStatus(PlayerId providedId)
		{
			return this._steamPlatformServices.GetUserOnlineStatus(providedId);
		}

		Task<bool> IFriendListService.IsPlayingThisGame(PlayerId providedId)
		{
			return this._steamPlatformServices.IsPlayingThisGame(providedId);
		}

		Task<string> IFriendListService.GetUserName(PlayerId providedId)
		{
			return this._steamPlatformServices.GetUserName(providedId);
		}

		Task<PlayerId> IFriendListService.GetUserWithName(string name)
		{
			return this._steamPlatformServices.GetUserWithName(name);
		}

		internal void HandleOnUserStatusChanged(PlayerId playerId)
		{
			if (this.OnUserStatusChanged != null)
			{
				this.OnUserStatusChanged(playerId);
			}
		}

		private void Dummy()
		{
			if (this.OnFriendRemoved != null)
			{
				this.OnFriendRemoved(default(PlayerId));
			}
			if (this.OnFriendListChanged != null)
			{
				this.OnFriendListChanged();
			}
		}

		IEnumerable<PlayerId> IFriendListService.GetPendingRequests()
		{
			return null;
		}

		IEnumerable<PlayerId> IFriendListService.GetReceivedRequests()
		{
			return null;
		}

		private SteamPlatformServices _steamPlatformServices;
	}
}
