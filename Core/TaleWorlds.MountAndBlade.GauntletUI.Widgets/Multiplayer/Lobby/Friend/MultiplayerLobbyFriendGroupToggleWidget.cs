using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby.Friend
{
	public class MultiplayerLobbyFriendGroupToggleWidget : ToggleButtonWidget
	{
		public MultiplayerLobbyFriendGroupToggleWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnClick(Widget widget)
		{
			base.OnClick(widget);
			this.UpdateCollapseIndicator();
		}

		protected override void RefreshState()
		{
			base.RefreshState();
			Widget titleContainer = this.TitleContainer;
			if (titleContainer == null)
			{
				return;
			}
			titleContainer.SetState(base.CurrentState);
		}

		private void CollapseIndicatorUpdated()
		{
			this.CollapseIndicator.AddState("Collapsed");
			this.CollapseIndicator.AddState("Expanded");
			this.UpdateCollapseIndicator();
		}

		private void UpdateCollapseIndicator()
		{
			if (base.WidgetToClose != null && this.CollapseIndicator != null)
			{
				if (base.WidgetToClose.IsVisible)
				{
					this.CollapseIndicator.SetState("Expanded");
					return;
				}
				this.CollapseIndicator.SetState("Collapsed");
			}
		}

		private void PlayerCountUpdated()
		{
			if (this.PlayerCountText == null)
			{
				return;
			}
			this.PlayerCountText.Text = "(" + this.PlayerCount + ")";
		}

		private void InitialClosedStateUpdated()
		{
			base.IsSelected = !this.InitialClosedState;
			this.CollapseIndicatorUpdated();
		}

		[Editor(false)]
		public Widget CollapseIndicator
		{
			get
			{
				return this._collapseIndicator;
			}
			set
			{
				if (this._collapseIndicator != value)
				{
					this._collapseIndicator = value;
					base.OnPropertyChanged<Widget>(value, "CollapseIndicator");
					this.CollapseIndicatorUpdated();
				}
			}
		}

		[Editor(false)]
		public Widget TitleContainer
		{
			get
			{
				return this._titleContainer;
			}
			set
			{
				if (this._titleContainer != value)
				{
					this._titleContainer = value;
					base.OnPropertyChanged<Widget>(value, "TitleContainer");
				}
			}
		}

		[Editor(false)]
		public TextWidget PlayerCountText
		{
			get
			{
				return this._playerCountText;
			}
			set
			{
				if (this._playerCountText != value)
				{
					this._playerCountText = value;
					base.OnPropertyChanged<TextWidget>(value, "PlayerCountText");
				}
			}
		}

		[Editor(false)]
		public int PlayerCount
		{
			get
			{
				return this._playerCount;
			}
			set
			{
				if (this._playerCount != value)
				{
					this._playerCount = value;
					base.OnPropertyChanged(value, "PlayerCount");
					this.PlayerCountUpdated();
				}
			}
		}

		[Editor(false)]
		public bool InitialClosedState
		{
			get
			{
				return this._initialClosedState;
			}
			set
			{
				if (this._initialClosedState != value)
				{
					this._initialClosedState = value;
					base.OnPropertyChanged(value, "InitialClosedState");
					this.InitialClosedStateUpdated();
				}
			}
		}

		private Widget _collapseIndicator;

		private Widget _titleContainer;

		private TextWidget _playerCountText;

		private int _playerCount;

		private bool _initialClosedState;
	}
}
