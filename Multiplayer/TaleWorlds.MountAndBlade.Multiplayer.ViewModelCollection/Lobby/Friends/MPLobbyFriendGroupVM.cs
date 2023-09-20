using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Friends
{
	public class MPLobbyFriendGroupVM : ViewModel
	{
		public MPLobbyFriendGroupVM(MPLobbyFriendGroupVM.FriendGroupType groupType)
		{
			this.GroupType = groupType;
			this.FriendList = new MBBindingList<MPLobbyFriendItemVM>();
			this.RefreshValues();
		}

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

		public void AddFriend(MPLobbyFriendItemVM player)
		{
			if (player.ProvidedID != NetworkMain.GameClient.PlayerID)
			{
				List<MPLobbyFriendItemVM> list = this._friendsToRemove;
				lock (list)
				{
					MPLobbyFriendItemVM mplobbyFriendItemVM = LinQuick.FirstOrDefaultQ<MPLobbyFriendItemVM>(this._friendsToRemove, (MPLobbyFriendItemVM f) => f.ProvidedID == player.ProvidedID);
					if (mplobbyFriendItemVM != null)
					{
						this._friendsToRemove.Remove(mplobbyFriendItemVM);
						return;
					}
				}
				list = this._friendsToAdd;
				lock (list)
				{
					if (LinQuick.FirstOrDefaultQ<MPLobbyFriendItemVM>(this._friendsToAdd, (MPLobbyFriendItemVM f) => f.ProvidedID == player.ProvidedID) == null)
					{
						this._friendsToAdd.Add(player);
					}
				}
			}
		}

		public void RemoveFriend(MPLobbyFriendItemVM player)
		{
			List<MPLobbyFriendItemVM> list = this._friendsToAdd;
			lock (list)
			{
				MPLobbyFriendItemVM mplobbyFriendItemVM = LinQuick.FirstOrDefaultQ<MPLobbyFriendItemVM>(this._friendsToAdd, (MPLobbyFriendItemVM f) => f.ProvidedID == player.ProvidedID);
				if (mplobbyFriendItemVM != null)
				{
					this._friendsToAdd.Remove(mplobbyFriendItemVM);
					return;
				}
			}
			list = this._friendsToRemove;
			lock (list)
			{
				if (LinQuick.FirstOrDefaultQ<MPLobbyFriendItemVM>(this._friendsToRemove, (MPLobbyFriendItemVM f) => f.ProvidedID == player.ProvidedID) == null)
				{
					this._friendsToRemove.Add(player);
				}
			}
		}

		private void AddFriendInternal(MPLobbyFriendItemVM player)
		{
			this.FriendList.Add(player);
			player.UpdateNameAndAvatar(false);
		}

		private void RemoveFriendInternal(MPLobbyFriendItemVM player)
		{
			this.FriendList.Remove(player);
		}

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

		private List<MPLobbyFriendItemVM> _friendsToAdd = new List<MPLobbyFriendItemVM>();

		private List<MPLobbyFriendItemVM> _friendsToRemove = new List<MPLobbyFriendItemVM>();

		private string _title;

		private MPLobbyFriendGroupVM.FriendGroupType _groupType;

		private MBBindingList<MPLobbyFriendItemVM> _friendList;

		public enum FriendGroupType
		{
			InGame,
			Online,
			Offline,
			FriendRequests,
			PendingRequests
		}
	}
}
