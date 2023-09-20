using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaleWorlds.Localization;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.PlatformService
{
	public interface IFriendListService
	{
		bool InGameStatusFetchable { get; }

		bool AllowsFriendOperations { get; }

		bool CanInvitePlayersToPlatformSession { get; }

		bool IncludeInAllFriends { get; }

		string GetServiceCodeName();

		TextObject GetServiceLocalizedName();

		FriendListServiceType GetFriendListServiceType();

		IEnumerable<PlayerId> GetAllFriends();

		event Action<PlayerId> OnUserStatusChanged;

		event Action<PlayerId> OnFriendRemoved;

		Task<bool> GetUserOnlineStatus(PlayerId providedId);

		Task<bool> IsPlayingThisGame(PlayerId providedId);

		Task<string> GetUserName(PlayerId providedId);

		Task<PlayerId> GetUserWithName(string name);

		event Action OnFriendListChanged;

		IEnumerable<PlayerId> GetPendingRequests();

		IEnumerable<PlayerId> GetReceivedRequests();
	}
}
