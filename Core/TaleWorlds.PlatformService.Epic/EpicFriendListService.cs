using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaleWorlds.Localization;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.PlatformService.Epic
{
	public class EpicFriendListService : IFriendListService
	{
		public EpicFriendListService(EpicPlatformServices epicPlatformServices)
		{
			this._epicPlatformServices = epicPlatformServices;
		}

		public event Action<PlayerId> OnUserStatusChanged;

		public event Action<PlayerId> OnFriendRemoved;

		public event Action OnFriendListChanged;

		bool IFriendListService.InGameStatusFetchable
		{
			get
			{
				return false;
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
			return "Epic";
		}

		TextObject IFriendListService.GetServiceLocalizedName()
		{
			return new TextObject("{=!}Epic", null);
		}

		FriendListServiceType IFriendListService.GetFriendListServiceType()
		{
			return FriendListServiceType.Epic;
		}

		IEnumerable<PlayerId> IFriendListService.GetAllFriends()
		{
			return this._epicPlatformServices.GetAllFriends();
		}

		Task<bool> IFriendListService.GetUserOnlineStatus(PlayerId providedId)
		{
			return this._epicPlatformServices.GetUserOnlineStatus(providedId);
		}

		Task<bool> IFriendListService.IsPlayingThisGame(PlayerId providedId)
		{
			return this._epicPlatformServices.IsPlayingThisGame(providedId);
		}

		Task<string> IFriendListService.GetUserName(PlayerId providedId)
		{
			return this._epicPlatformServices.GetUserName(providedId);
		}

		Task<PlayerId> IFriendListService.GetUserWithName(string name)
		{
			return this._epicPlatformServices.GetUserWithName(name);
		}

		public void UserStatusChanged(PlayerId playerId)
		{
			if (this.OnUserStatusChanged != null)
			{
				this.OnUserStatusChanged(default(PlayerId));
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

		private EpicPlatformServices _epicPlatformServices;
	}
}
