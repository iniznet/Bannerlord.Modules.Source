using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Galaxy.Api;
using TaleWorlds.Localization;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.PlatformService.GOG
{
	public class GOGFriendListService : IFriendListService
	{
		public GOGFriendListService(GOGPlatformServices gogPlatformServices)
		{
			this._gogPlatformServices = gogPlatformServices;
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
			return "GOG";
		}

		TextObject IFriendListService.GetServiceLocalizedName()
		{
			return new TextObject("{=!}GOG", null);
		}

		FriendListServiceType IFriendListService.GetFriendListServiceType()
		{
			return FriendListServiceType.GOG;
		}

		public void RequestFriendList()
		{
			IFriends friends = GalaxyInstance.Friends();
			FriendListListener friendListListener = new FriendListListener();
			friends.RequestFriendList(friendListListener);
			while (!friendListListener.GotResult)
			{
				GalaxyInstance.ProcessData();
				Thread.Sleep(5);
			}
		}

		IEnumerable<PlayerId> IFriendListService.GetAllFriends()
		{
			IFriends friends = GalaxyInstance.Friends();
			int friendCount = (int)friends.GetFriendCount();
			int num;
			for (int i = 0; i < friendCount; i = num)
			{
				yield return friends.GetFriendByIndex((uint)i).ToPlayerId();
				num = i + 1;
			}
			yield break;
		}

		Task<bool> IFriendListService.GetUserOnlineStatus(PlayerId providedId)
		{
			return Task.FromResult<bool>(false);
		}

		Task<bool> IFriendListService.IsPlayingThisGame(PlayerId providedId)
		{
			return Task.FromResult<bool>(false);
		}

		async Task<string> IFriendListService.GetUserName(PlayerId providedId)
		{
			return await this._gogPlatformServices.GetUserName(providedId);
		}

		Task<PlayerId> IFriendListService.GetUserWithName(string name)
		{
			return Task.FromResult<PlayerId>(PlayerId.Empty);
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

		private GOGPlatformServices _gogPlatformServices;
	}
}
