using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Galaxy.Api;
using TaleWorlds.Localization;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.PlatformService.GOG
{
	// Token: 0x02000005 RID: 5
	public class GOGFriendListService : IFriendListService
	{
		// Token: 0x06000008 RID: 8 RVA: 0x00002118 File Offset: 0x00000318
		public GOGFriendListService(GOGPlatformServices gogPlatformServices)
		{
			this._gogPlatformServices = gogPlatformServices;
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000009 RID: 9 RVA: 0x00002128 File Offset: 0x00000328
		// (remove) Token: 0x0600000A RID: 10 RVA: 0x00002160 File Offset: 0x00000360
		public event Action<PlayerId> OnUserStatusChanged;

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x0600000B RID: 11 RVA: 0x00002198 File Offset: 0x00000398
		// (remove) Token: 0x0600000C RID: 12 RVA: 0x000021D0 File Offset: 0x000003D0
		public event Action<PlayerId> OnFriendRemoved;

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x0600000D RID: 13 RVA: 0x00002208 File Offset: 0x00000408
		// (remove) Token: 0x0600000E RID: 14 RVA: 0x00002240 File Offset: 0x00000440
		public event Action OnFriendListChanged;

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600000F RID: 15 RVA: 0x00002275 File Offset: 0x00000475
		bool IFriendListService.InGameStatusFetchable
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000010 RID: 16 RVA: 0x00002278 File Offset: 0x00000478
		bool IFriendListService.AllowsFriendOperations
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000011 RID: 17 RVA: 0x0000227B File Offset: 0x0000047B
		bool IFriendListService.CanInvitePlayersToPlatformSession
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000012 RID: 18 RVA: 0x0000227E File Offset: 0x0000047E
		bool IFriendListService.IncludeInAllFriends
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002281 File Offset: 0x00000481
		string IFriendListService.GetServiceCodeName()
		{
			return "GOG";
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002288 File Offset: 0x00000488
		TextObject IFriendListService.GetServiceLocalizedName()
		{
			return new TextObject("{=!}GOG", null);
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002295 File Offset: 0x00000495
		FriendListServiceType IFriendListService.GetFriendListServiceType()
		{
			return FriendListServiceType.GOG;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002298 File Offset: 0x00000498
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

		// Token: 0x06000017 RID: 23 RVA: 0x000022CB File Offset: 0x000004CB
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

		// Token: 0x06000018 RID: 24 RVA: 0x000022D4 File Offset: 0x000004D4
		Task<bool> IFriendListService.GetUserOnlineStatus(PlayerId providedId)
		{
			return Task.FromResult<bool>(false);
		}

		// Token: 0x06000019 RID: 25 RVA: 0x000022DC File Offset: 0x000004DC
		Task<bool> IFriendListService.IsPlayingThisGame(PlayerId providedId)
		{
			return Task.FromResult<bool>(false);
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000022E4 File Offset: 0x000004E4
		async Task<string> IFriendListService.GetUserName(PlayerId providedId)
		{
			return await this._gogPlatformServices.GetUserName(providedId);
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002331 File Offset: 0x00000531
		Task<PlayerId> IFriendListService.GetUserWithName(string name)
		{
			return Task.FromResult<PlayerId>(PlayerId.Empty);
		}

		// Token: 0x0600001C RID: 28 RVA: 0x0000233D File Offset: 0x0000053D
		internal void HandleOnUserStatusChanged(PlayerId playerId)
		{
			if (this.OnUserStatusChanged != null)
			{
				this.OnUserStatusChanged(playerId);
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002354 File Offset: 0x00000554
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

		// Token: 0x0600001E RID: 30 RVA: 0x00002390 File Offset: 0x00000590
		IEnumerable<PlayerId> IFriendListService.GetPendingRequests()
		{
			return null;
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002393 File Offset: 0x00000593
		IEnumerable<PlayerId> IFriendListService.GetReceivedRequests()
		{
			return null;
		}

		// Token: 0x04000002 RID: 2
		private GOGPlatformServices _gogPlatformServices;
	}
}
