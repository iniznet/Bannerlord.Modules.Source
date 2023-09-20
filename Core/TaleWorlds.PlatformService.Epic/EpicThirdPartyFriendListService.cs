using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaleWorlds.Localization;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.PlatformService.Epic
{
	public class EpicThirdPartyFriendListService : IFriendListService
	{
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
			return "EpicThirdParty";
		}

		TextObject IFriendListService.GetServiceLocalizedName()
		{
			return new TextObject("{=!}Epic Third Party", null);
		}

		FriendListServiceType IFriendListService.GetFriendListServiceType()
		{
			return FriendListServiceType.EpicThirdParty;
		}

		IEnumerable<PlayerId> IFriendListService.GetAllFriends()
		{
			return null;
		}

		Task<bool> IFriendListService.GetUserOnlineStatus(PlayerId providedId)
		{
			return Task.FromResult<bool>(false);
		}

		Task<bool> IFriendListService.IsPlayingThisGame(PlayerId providedId)
		{
			return Task.FromResult<bool>(false);
		}

		Task<string> IFriendListService.GetUserName(PlayerId providedId)
		{
			return Task.FromResult<string>("-");
		}

		Task<PlayerId> IFriendListService.GetUserWithName(string name)
		{
			return Task.FromResult<PlayerId>(PlayerId.Empty);
		}

		private void Dummy()
		{
			if (this.OnUserStatusChanged != null)
			{
				this.OnUserStatusChanged(default(PlayerId));
			}
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
	}
}
