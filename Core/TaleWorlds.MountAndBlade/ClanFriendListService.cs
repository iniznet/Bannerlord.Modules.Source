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
	// Token: 0x020001D4 RID: 468
	public class ClanFriendListService : IFriendListService
	{
		// Token: 0x1700052D RID: 1325
		// (get) Token: 0x06001A4B RID: 6731 RVA: 0x0005CD2F File Offset: 0x0005AF2F
		bool IFriendListService.InGameStatusFetchable
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700052E RID: 1326
		// (get) Token: 0x06001A4C RID: 6732 RVA: 0x0005CD32 File Offset: 0x0005AF32
		bool IFriendListService.AllowsFriendOperations
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700052F RID: 1327
		// (get) Token: 0x06001A4D RID: 6733 RVA: 0x0005CD35 File Offset: 0x0005AF35
		bool IFriendListService.CanInvitePlayersToPlatformSession
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000530 RID: 1328
		// (get) Token: 0x06001A4E RID: 6734 RVA: 0x0005CD38 File Offset: 0x0005AF38
		bool IFriendListService.IncludeInAllFriends
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06001A4F RID: 6735 RVA: 0x0005CD3B File Offset: 0x0005AF3B
		public ClanFriendListService()
		{
			this._clanPlayerInfos = new Dictionary<PlayerId, ClanPlayerInfo>();
		}

		// Token: 0x06001A50 RID: 6736 RVA: 0x0005CD4E File Offset: 0x0005AF4E
		string IFriendListService.GetServiceCodeName()
		{
			return "ClanFriends";
		}

		// Token: 0x06001A51 RID: 6737 RVA: 0x0005CD55 File Offset: 0x0005AF55
		TextObject IFriendListService.GetServiceLocalizedName()
		{
			return new TextObject("{=j4F7tTzy}Clan", null);
		}

		// Token: 0x06001A52 RID: 6738 RVA: 0x0005CD62 File Offset: 0x0005AF62
		FriendListServiceType IFriendListService.GetFriendListServiceType()
		{
			return FriendListServiceType.Clan;
		}

		// Token: 0x06001A53 RID: 6739 RVA: 0x0005CD65 File Offset: 0x0005AF65
		IEnumerable<PlayerId> IFriendListService.GetAllFriends()
		{
			return this._clanPlayerInfos.Keys;
		}

		// Token: 0x14000018 RID: 24
		// (add) Token: 0x06001A54 RID: 6740 RVA: 0x0005CD74 File Offset: 0x0005AF74
		// (remove) Token: 0x06001A55 RID: 6741 RVA: 0x0005CDAC File Offset: 0x0005AFAC
		public event Action<PlayerId> OnUserStatusChanged;

		// Token: 0x14000019 RID: 25
		// (add) Token: 0x06001A56 RID: 6742 RVA: 0x0005CDE4 File Offset: 0x0005AFE4
		// (remove) Token: 0x06001A57 RID: 6743 RVA: 0x0005CE1C File Offset: 0x0005B01C
		public event Action<PlayerId> OnFriendRemoved;

		// Token: 0x06001A58 RID: 6744 RVA: 0x0005CE54 File Offset: 0x0005B054
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

		// Token: 0x06001A59 RID: 6745 RVA: 0x0005CEA4 File Offset: 0x0005B0A4
		async Task<bool> IFriendListService.IsPlayingThisGame(PlayerId providedId)
		{
			return await ((IFriendListService)this).GetUserOnlineStatus(providedId);
		}

		// Token: 0x06001A5A RID: 6746 RVA: 0x0005CEF4 File Offset: 0x0005B0F4
		async Task<string> IFriendListService.GetUserName(PlayerId providedId)
		{
			ClanPlayerInfo clanPlayerInfo;
			this._clanPlayerInfos.TryGetValue(providedId, out clanPlayerInfo);
			return await Task.FromResult<string>((clanPlayerInfo != null) ? clanPlayerInfo.PlayerName : null);
		}

		// Token: 0x06001A5B RID: 6747 RVA: 0x0005CF44 File Offset: 0x0005B144
		public async Task<PlayerId> GetUserWithName(string name)
		{
			ClanPlayerInfo clanPlayerInfo = this._clanPlayerInfos.Values.FirstOrDefaultQ((ClanPlayerInfo playerInfo) => playerInfo.PlayerName == name);
			return await Task.FromResult<PlayerId>((clanPlayerInfo != null) ? clanPlayerInfo.PlayerId : PlayerId.Empty);
		}

		// Token: 0x1400001A RID: 26
		// (add) Token: 0x06001A5C RID: 6748 RVA: 0x0005CF94 File Offset: 0x0005B194
		// (remove) Token: 0x06001A5D RID: 6749 RVA: 0x0005CFCC File Offset: 0x0005B1CC
		public event Action OnFriendListChanged;

		// Token: 0x06001A5E RID: 6750 RVA: 0x0005D001 File Offset: 0x0005B201
		public IEnumerable<PlayerId> GetPendingRequests()
		{
			return null;
		}

		// Token: 0x06001A5F RID: 6751 RVA: 0x0005D004 File Offset: 0x0005B204
		public IEnumerable<PlayerId> GetReceivedRequests()
		{
			return null;
		}

		// Token: 0x06001A60 RID: 6752 RVA: 0x0005D008 File Offset: 0x0005B208
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

		// Token: 0x06001A61 RID: 6753 RVA: 0x0005D050 File Offset: 0x0005B250
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

		// Token: 0x04000863 RID: 2147
		public const string CodeName = "ClanFriends";

		// Token: 0x04000864 RID: 2148
		private readonly Dictionary<PlayerId, ClanPlayerInfo> _clanPlayerInfos;
	}
}
