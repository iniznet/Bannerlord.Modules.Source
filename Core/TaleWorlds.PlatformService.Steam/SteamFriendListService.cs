using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Steamworks;
using TaleWorlds.Localization;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.PlatformService.Steam
{
	// Token: 0x02000003 RID: 3
	public class SteamFriendListService : IFriendListService
	{
		// Token: 0x0600000F RID: 15 RVA: 0x000021F7 File Offset: 0x000003F7
		public SteamFriendListService(SteamPlatformServices steamPlatformServices)
		{
			this._steamPlatformServices = steamPlatformServices;
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000010 RID: 16 RVA: 0x00002208 File Offset: 0x00000408
		// (remove) Token: 0x06000011 RID: 17 RVA: 0x00002240 File Offset: 0x00000440
		public event Action<PlayerId> OnUserStatusChanged;

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000012 RID: 18 RVA: 0x00002278 File Offset: 0x00000478
		// (remove) Token: 0x06000013 RID: 19 RVA: 0x000022B0 File Offset: 0x000004B0
		public event Action<PlayerId> OnFriendRemoved;

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06000014 RID: 20 RVA: 0x000022E8 File Offset: 0x000004E8
		// (remove) Token: 0x06000015 RID: 21 RVA: 0x00002320 File Offset: 0x00000520
		public event Action OnFriendListChanged;

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000016 RID: 22 RVA: 0x00002355 File Offset: 0x00000555
		bool IFriendListService.InGameStatusFetchable
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000017 RID: 23 RVA: 0x00002358 File Offset: 0x00000558
		bool IFriendListService.AllowsFriendOperations
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000018 RID: 24 RVA: 0x0000235B File Offset: 0x0000055B
		bool IFriendListService.CanInvitePlayersToPlatformSession
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000019 RID: 25 RVA: 0x0000235E File Offset: 0x0000055E
		bool IFriendListService.IncludeInAllFriends
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002361 File Offset: 0x00000561
		string IFriendListService.GetServiceCodeName()
		{
			return "Steam";
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002368 File Offset: 0x00000568
		TextObject IFriendListService.GetServiceLocalizedName()
		{
			return new TextObject("{=!}Steam", null);
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002375 File Offset: 0x00000575
		FriendListServiceType IFriendListService.GetFriendListServiceType()
		{
			return FriendListServiceType.Steam;
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002378 File Offset: 0x00000578
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

		// Token: 0x0600001E RID: 30 RVA: 0x00002388 File Offset: 0x00000588
		Task<bool> IFriendListService.GetUserOnlineStatus(PlayerId providedId)
		{
			return this._steamPlatformServices.GetUserOnlineStatus(providedId);
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002396 File Offset: 0x00000596
		Task<bool> IFriendListService.IsPlayingThisGame(PlayerId providedId)
		{
			return this._steamPlatformServices.IsPlayingThisGame(providedId);
		}

		// Token: 0x06000020 RID: 32 RVA: 0x000023A4 File Offset: 0x000005A4
		Task<string> IFriendListService.GetUserName(PlayerId providedId)
		{
			return this._steamPlatformServices.GetUserName(providedId);
		}

		// Token: 0x06000021 RID: 33 RVA: 0x000023B2 File Offset: 0x000005B2
		Task<PlayerId> IFriendListService.GetUserWithName(string name)
		{
			return this._steamPlatformServices.GetUserWithName(name);
		}

		// Token: 0x06000022 RID: 34 RVA: 0x000023C0 File Offset: 0x000005C0
		internal void HandleOnUserStatusChanged(PlayerId playerId)
		{
			if (this.OnUserStatusChanged != null)
			{
				this.OnUserStatusChanged(playerId);
			}
		}

		// Token: 0x06000023 RID: 35 RVA: 0x000023D8 File Offset: 0x000005D8
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

		// Token: 0x06000024 RID: 36 RVA: 0x00002414 File Offset: 0x00000614
		IEnumerable<PlayerId> IFriendListService.GetPendingRequests()
		{
			return null;
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002417 File Offset: 0x00000617
		IEnumerable<PlayerId> IFriendListService.GetReceivedRequests()
		{
			return null;
		}

		// Token: 0x04000009 RID: 9
		private SteamPlatformServices _steamPlatformServices;
	}
}
