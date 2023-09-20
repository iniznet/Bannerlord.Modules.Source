using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby.Friend
{
	// Token: 0x020000A4 RID: 164
	public class MultiplayerLobbyFriendsPanelWidget : Widget
	{
		// Token: 0x0600089C RID: 2204 RVA: 0x00018D00 File Offset: 0x00016F00
		public MultiplayerLobbyFriendsPanelWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600089D RID: 2205 RVA: 0x00018D09 File Offset: 0x00016F09
		private void OnShowListTogglePropertyChanged(PropertyOwnerObject owner, string propertyName, bool value)
		{
			if (propertyName == "IsSelected")
			{
				this.FriendsListPanel.IsVisible = this.ShowListToggle.IsSelected;
			}
		}

		// Token: 0x0600089E RID: 2206 RVA: 0x00018D2E File Offset: 0x00016F2E
		private void IsForcedOpenUpdated()
		{
			this.FriendsListPanel.IsVisible = this.IsForcedOpen;
			this.ShowListToggle.IsSelected = this.IsForcedOpen;
		}

		// Token: 0x17000305 RID: 773
		// (get) Token: 0x0600089F RID: 2207 RVA: 0x00018D52 File Offset: 0x00016F52
		// (set) Token: 0x060008A0 RID: 2208 RVA: 0x00018D5A File Offset: 0x00016F5A
		[Editor(false)]
		public bool IsForcedOpen
		{
			get
			{
				return this._isForcedOpen;
			}
			set
			{
				if (this._isForcedOpen != value)
				{
					this._isForcedOpen = value;
					base.OnPropertyChanged(value, "IsForcedOpen");
					this.IsForcedOpenUpdated();
				}
			}
		}

		// Token: 0x17000306 RID: 774
		// (get) Token: 0x060008A1 RID: 2209 RVA: 0x00018D7E File Offset: 0x00016F7E
		// (set) Token: 0x060008A2 RID: 2210 RVA: 0x00018D86 File Offset: 0x00016F86
		[Editor(false)]
		public Widget FriendsListPanel
		{
			get
			{
				return this._friendsListPanel;
			}
			set
			{
				if (this._friendsListPanel != value)
				{
					this._friendsListPanel = value;
					base.OnPropertyChanged<Widget>(value, "FriendsListPanel");
				}
			}
		}

		// Token: 0x17000307 RID: 775
		// (get) Token: 0x060008A3 RID: 2211 RVA: 0x00018DA4 File Offset: 0x00016FA4
		// (set) Token: 0x060008A4 RID: 2212 RVA: 0x00018DAC File Offset: 0x00016FAC
		[Editor(false)]
		public ToggleStateButtonWidget ShowListToggle
		{
			get
			{
				return this._showListToggle;
			}
			set
			{
				if (this._showListToggle != value)
				{
					if (this._showListToggle != null)
					{
						this._showListToggle.boolPropertyChanged -= this.OnShowListTogglePropertyChanged;
					}
					this._showListToggle = value;
					if (this._showListToggle != null)
					{
						this._showListToggle.boolPropertyChanged += this.OnShowListTogglePropertyChanged;
					}
					base.OnPropertyChanged<ToggleStateButtonWidget>(value, "ShowListToggle");
				}
			}
		}

		// Token: 0x040003EF RID: 1007
		private bool _isForcedOpen;

		// Token: 0x040003F0 RID: 1008
		private Widget _friendsListPanel;

		// Token: 0x040003F1 RID: 1009
		private ToggleStateButtonWidget _showListToggle;
	}
}
