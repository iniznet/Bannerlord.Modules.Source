using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby.Friend
{
	public class MultiplayerLobbyFriendGroupWidget : Widget
	{
		public MultiplayerLobbyFriendGroupWidget(UIContext context)
			: base(context)
		{
		}

		private void FriendCountChanged(Widget widget)
		{
			this.Toggle.PlayerCount = this.List.ChildCount;
		}

		private void FriendCountChanged(Widget parentWidget, Widget addedWidget)
		{
			this.Toggle.PlayerCount = this.List.ChildCount;
		}

		[Editor(false)]
		public ListPanel List
		{
			get
			{
				return this._list;
			}
			set
			{
				if (this._list != value)
				{
					ListPanel list = this._list;
					if (list != null)
					{
						list.ItemAddEventHandlers.Remove(new Action<Widget, Widget>(this.FriendCountChanged));
					}
					ListPanel list2 = this._list;
					if (list2 != null)
					{
						list2.ItemAfterRemoveEventHandlers.Remove(new Action<Widget>(this.FriendCountChanged));
					}
					this._list = value;
					ListPanel list3 = this._list;
					if (list3 != null)
					{
						list3.ItemAddEventHandlers.Add(new Action<Widget, Widget>(this.FriendCountChanged));
					}
					ListPanel list4 = this._list;
					if (list4 != null)
					{
						list4.ItemAfterRemoveEventHandlers.Add(new Action<Widget>(this.FriendCountChanged));
					}
					base.OnPropertyChanged<ListPanel>(value, "List");
				}
			}
		}

		[Editor(false)]
		public MultiplayerLobbyFriendGroupToggleWidget Toggle
		{
			get
			{
				return this._toggle;
			}
			set
			{
				if (this._toggle != value)
				{
					this._toggle = value;
					base.OnPropertyChanged<MultiplayerLobbyFriendGroupToggleWidget>(value, "Toggle");
				}
			}
		}

		private ListPanel _list;

		private MultiplayerLobbyFriendGroupToggleWidget _toggle;
	}
}
