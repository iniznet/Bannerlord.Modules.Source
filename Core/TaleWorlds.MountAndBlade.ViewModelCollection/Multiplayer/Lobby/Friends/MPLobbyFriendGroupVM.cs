using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Friends
{
	// Token: 0x02000080 RID: 128
	public class MPLobbyFriendGroupVM : ViewModel
	{
		// Token: 0x06000B43 RID: 2883 RVA: 0x00027A67 File Offset: 0x00025C67
		public MPLobbyFriendGroupVM(MPLobbyFriendGroupVM.FriendGroupType groupType)
		{
			this.GroupType = groupType;
			this.FriendList = new MBBindingList<MPLobbyFriendItemVM>();
			this.RefreshValues();
		}

		// Token: 0x06000B44 RID: 2884 RVA: 0x00027AA0 File Offset: 0x00025CA0
		public void Tick()
		{
			List<MPLobbyFriendItemVM> list = this._friendsToAdd;
			lock (list)
			{
				if (this._friendsToAdd.Count > 0)
				{
					for (int i = this._friendsToAdd.Count - 1; i >= 0; i--)
					{
						this.AddFriendInternal(this._friendsToAdd[i]);
						this._friendsToAdd.RemoveAt(i);
					}
				}
			}
			list = this._friendsToRemove;
			lock (list)
			{
				if (this._friendsToRemove.Count > 0)
				{
					for (int j = this._friendsToRemove.Count - 1; j >= 0; j--)
					{
						this.RemoveFriendInternal(this._friendsToRemove[j]);
						this._friendsToRemove.RemoveAt(j);
					}
				}
			}
		}

		// Token: 0x06000B45 RID: 2885 RVA: 0x00027B8C File Offset: 0x00025D8C
		public override void RefreshValues()
		{
			base.RefreshValues();
			switch (this.GroupType)
			{
			case MPLobbyFriendGroupVM.FriendGroupType.InGame:
				this.Title = new TextObject("{=uUoSmCBS}In Bannerlord", null).ToString();
				break;
			case MPLobbyFriendGroupVM.FriendGroupType.Online:
				this.Title = new TextObject("{=V305MaOP}Online", null).ToString();
				break;
			case MPLobbyFriendGroupVM.FriendGroupType.Offline:
				this.Title = new TextObject("{=Zv1lg272}Offline", null).ToString();
				break;
			case MPLobbyFriendGroupVM.FriendGroupType.FriendRequests:
				this.Title = new TextObject("{=K8CGzQYL}Received Requests", null).ToString();
				break;
			case MPLobbyFriendGroupVM.FriendGroupType.PendingRequests:
				this.Title = new TextObject("{=QwbVdMLi}Sent Requests", null).ToString();
				break;
			}
			this.FriendList.ApplyActionOnAllItems(delegate(MPLobbyFriendItemVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x06000B46 RID: 2886 RVA: 0x00027C64 File Offset: 0x00025E64
		public void AddFriend(MPLobbyFriendItemVM player)
		{
			if (player.ProvidedID != NetworkMain.GameClient.PlayerID)
			{
				List<MPLobbyFriendItemVM> list = this._friendsToRemove;
				lock (list)
				{
					MPLobbyFriendItemVM mplobbyFriendItemVM = this._friendsToRemove.FirstOrDefaultQ((MPLobbyFriendItemVM f) => f.ProvidedID == player.ProvidedID);
					if (mplobbyFriendItemVM != null)
					{
						this._friendsToRemove.Remove(mplobbyFriendItemVM);
						return;
					}
				}
				list = this._friendsToAdd;
				lock (list)
				{
					if (this._friendsToAdd.FirstOrDefaultQ((MPLobbyFriendItemVM f) => f.ProvidedID == player.ProvidedID) == null)
					{
						this._friendsToAdd.Add(player);
					}
				}
			}
		}

		// Token: 0x06000B47 RID: 2887 RVA: 0x00027D48 File Offset: 0x00025F48
		public void RemoveFriend(MPLobbyFriendItemVM player)
		{
			List<MPLobbyFriendItemVM> list = this._friendsToAdd;
			lock (list)
			{
				MPLobbyFriendItemVM mplobbyFriendItemVM = this._friendsToAdd.FirstOrDefaultQ((MPLobbyFriendItemVM f) => f.ProvidedID == player.ProvidedID);
				if (mplobbyFriendItemVM != null)
				{
					this._friendsToAdd.Remove(mplobbyFriendItemVM);
					return;
				}
			}
			list = this._friendsToRemove;
			lock (list)
			{
				if (this._friendsToRemove.FirstOrDefaultQ((MPLobbyFriendItemVM f) => f.ProvidedID == player.ProvidedID) == null)
				{
					this._friendsToRemove.Add(player);
				}
			}
		}

		// Token: 0x06000B48 RID: 2888 RVA: 0x00027E0C File Offset: 0x0002600C
		private void AddFriendInternal(MPLobbyFriendItemVM player)
		{
			this.FriendList.Add(player);
			player.UpdateNameAndAvatar(false);
		}

		// Token: 0x06000B49 RID: 2889 RVA: 0x00027E21 File Offset: 0x00026021
		private void RemoveFriendInternal(MPLobbyFriendItemVM player)
		{
			this.FriendList.Remove(player);
		}

		// Token: 0x17000381 RID: 897
		// (get) Token: 0x06000B4A RID: 2890 RVA: 0x00027E30 File Offset: 0x00026030
		// (set) Token: 0x06000B4B RID: 2891 RVA: 0x00027E38 File Offset: 0x00026038
		[DataSourceProperty]
		public string Title
		{
			get
			{
				return this._title;
			}
			set
			{
				if (value != this._title)
				{
					this._title = value;
					base.OnPropertyChangedWithValue<string>(value, "Title");
				}
			}
		}

		// Token: 0x17000382 RID: 898
		// (get) Token: 0x06000B4C RID: 2892 RVA: 0x00027E5B File Offset: 0x0002605B
		// (set) Token: 0x06000B4D RID: 2893 RVA: 0x00027E63 File Offset: 0x00026063
		[DataSourceProperty]
		public MPLobbyFriendGroupVM.FriendGroupType GroupType
		{
			get
			{
				return this._groupType;
			}
			set
			{
				if (value != this._groupType)
				{
					this._groupType = value;
					base.OnPropertyChanged("GroupType");
				}
			}
		}

		// Token: 0x17000383 RID: 899
		// (get) Token: 0x06000B4E RID: 2894 RVA: 0x00027E80 File Offset: 0x00026080
		// (set) Token: 0x06000B4F RID: 2895 RVA: 0x00027E88 File Offset: 0x00026088
		[DataSourceProperty]
		public MBBindingList<MPLobbyFriendItemVM> FriendList
		{
			get
			{
				return this._friendList;
			}
			set
			{
				if (value != this._friendList)
				{
					this._friendList = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPLobbyFriendItemVM>>(value, "FriendList");
				}
			}
		}

		// Token: 0x04000569 RID: 1385
		private List<MPLobbyFriendItemVM> _friendsToAdd = new List<MPLobbyFriendItemVM>();

		// Token: 0x0400056A RID: 1386
		private List<MPLobbyFriendItemVM> _friendsToRemove = new List<MPLobbyFriendItemVM>();

		// Token: 0x0400056B RID: 1387
		private string _title;

		// Token: 0x0400056C RID: 1388
		private MPLobbyFriendGroupVM.FriendGroupType _groupType;

		// Token: 0x0400056D RID: 1389
		private MBBindingList<MPLobbyFriendItemVM> _friendList;

		// Token: 0x020001A7 RID: 423
		public enum FriendGroupType
		{
			// Token: 0x04000D4A RID: 3402
			InGame,
			// Token: 0x04000D4B RID: 3403
			Online,
			// Token: 0x04000D4C RID: 3404
			Offline,
			// Token: 0x04000D4D RID: 3405
			FriendRequests,
			// Token: 0x04000D4E RID: 3406
			PendingRequests
		}
	}
}
