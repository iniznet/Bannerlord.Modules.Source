using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlatformService;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
	public class ClanFriendListService : IFriendListService
	{
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
				return false;
			}
		}

		public ClanFriendListService()
		{
			this._clanPlayerInfos = new Dictionary<PlayerId, ClanPlayerInfo>();
		}

		string IFriendListService.GetServiceCodeName()
		{
			return "ClanFriends";
		}

		TextObject IFriendListService.GetServiceLocalizedName()
		{
			return new TextObject("{=j4F7tTzy}Clan", null);
		}

		FriendListServiceType IFriendListService.GetFriendListServiceType()
		{
			return FriendListServiceType.Clan;
		}

		IEnumerable<PlayerId> IFriendListService.GetAllFriends()
		{
			return this._clanPlayerInfos.Keys;
		}

		public event Action<PlayerId> OnUserStatusChanged;

		public event Action<PlayerId> OnFriendRemoved;

		async Task<bool> IFriendListService.GetUserOnlineStatus(PlayerId providedId)
		{
			bool flag = false;
			ClanPlayerInfo clanPlayerInfo;
			this._clanPlayerInfos.TryGetValue(providedId, out clanPlayerInfo);
			if (clanPlayerInfo != null)
			{
				flag = clanPlayerInfo.State == AnotherPlayerState.InMultiplayerGame || clanPlayerInfo.State == AnotherPlayerState.AtLobby;
			}
			return await Task.FromResult<bool>(flag);
		}

		async Task<bool> IFriendListService.IsPlayingThisGame(PlayerId providedId)
		{
			return await ((IFriendListService)this).GetUserOnlineStatus(providedId);
		}

		async Task<string> IFriendListService.GetUserName(PlayerId providedId)
		{
			ClanPlayerInfo clanPlayerInfo;
			this._clanPlayerInfos.TryGetValue(providedId, out clanPlayerInfo);
			return await Task.FromResult<string>((clanPlayerInfo != null) ? clanPlayerInfo.PlayerName : null);
		}

		public async Task<PlayerId> GetUserWithName(string name)
		{
			ClanPlayerInfo clanPlayerInfo = this._clanPlayerInfos.Values.FirstOrDefaultQ((ClanPlayerInfo playerInfo) => playerInfo.PlayerName == name);
			return await Task.FromResult<PlayerId>((clanPlayerInfo != null) ? clanPlayerInfo.PlayerId : PlayerId.Empty);
		}

		public event Action OnFriendListChanged;

		public IEnumerable<PlayerId> GetPendingRequests()
		{
			return null;
		}

		public IEnumerable<PlayerId> GetReceivedRequests()
		{
			return null;
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
		}

		public void OnClanInfoChanged(List<ClanPlayerInfo> playerInfosInClan)
		{
			this._clanPlayerInfos.Clear();
			if (playerInfosInClan != null)
			{
				foreach (ClanPlayerInfo clanPlayerInfo in playerInfosInClan)
				{
					this._clanPlayerInfos.Add(clanPlayerInfo.PlayerId, clanPlayerInfo);
				}
			}
			Action onFriendListChanged = this.OnFriendListChanged;
			if (onFriendListChanged == null)
			{
				return;
			}
			onFriendListChanged();
		}

		public const string CodeName = "ClanFriends";

		private readonly Dictionary<PlayerId, ClanPlayerInfo> _clanPlayerInfos;
	}
}
