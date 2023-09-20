using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby.Friend
{
	// Token: 0x020000A3 RID: 163
	public class MultiplayerLobbyFriendGroupWidget : Widget
	{
		// Token: 0x06000895 RID: 2197 RVA: 0x00018BE1 File Offset: 0x00016DE1
		public MultiplayerLobbyFriendGroupWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000896 RID: 2198 RVA: 0x00018BEA File Offset: 0x00016DEA
		private void FriendCountChanged(Widget widget)
		{
			this.Toggle.PlayerCount = this.List.ChildCount;
		}

		// Token: 0x06000897 RID: 2199 RVA: 0x00018C02 File Offset: 0x00016E02
		private void FriendCountChanged(Widget parentWidget, Widget addedWidget)
		{
			this.Toggle.PlayerCount = this.List.ChildCount;
		}

		// Token: 0x17000303 RID: 771
		// (get) Token: 0x06000898 RID: 2200 RVA: 0x00018C1A File Offset: 0x00016E1A
		// (set) Token: 0x06000899 RID: 2201 RVA: 0x00018C24 File Offset: 0x00016E24
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

		// Token: 0x17000304 RID: 772
		// (get) Token: 0x0600089A RID: 2202 RVA: 0x00018CDA File Offset: 0x00016EDA
		// (set) Token: 0x0600089B RID: 2203 RVA: 0x00018CE2 File Offset: 0x00016EE2
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

		// Token: 0x040003ED RID: 1005
		private ListPanel _list;

		// Token: 0x040003EE RID: 1006
		private MultiplayerLobbyFriendGroupToggleWidget _toggle;
	}
}
