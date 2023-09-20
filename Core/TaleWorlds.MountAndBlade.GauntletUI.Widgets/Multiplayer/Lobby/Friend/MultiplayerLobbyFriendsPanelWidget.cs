using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby.Friend
{
	public class MultiplayerLobbyFriendsPanelWidget : Widget
	{
		public MultiplayerLobbyFriendsPanelWidget(UIContext context)
			: base(context)
		{
		}

		private void OnShowListTogglePropertyChanged(PropertyOwnerObject owner, string propertyName, bool value)
		{
			if (propertyName == "IsSelected")
			{
				this.FriendsListPanel.IsVisible = this.ShowListToggle.IsSelected;
			}
		}

		private void IsForcedOpenUpdated()
		{
			this.FriendsListPanel.IsVisible = this.IsForcedOpen;
			this.ShowListToggle.IsSelected = this.IsForcedOpen;
		}

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

		private bool _isForcedOpen;

		private Widget _friendsListPanel;

		private ToggleStateButtonWidget _showListToggle;
	}
}
