using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby.Friend
{
	// Token: 0x020000A2 RID: 162
	public class MultiplayerLobbyFriendGroupToggleWidget : ToggleButtonWidget
	{
		// Token: 0x06000884 RID: 2180 RVA: 0x00018A2C File Offset: 0x00016C2C
		public MultiplayerLobbyFriendGroupToggleWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000885 RID: 2181 RVA: 0x00018A35 File Offset: 0x00016C35
		protected override void OnClick(Widget widget)
		{
			base.OnClick(widget);
			this.UpdateCollapseIndicator();
		}

		// Token: 0x06000886 RID: 2182 RVA: 0x00018A44 File Offset: 0x00016C44
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

		// Token: 0x06000887 RID: 2183 RVA: 0x00018A62 File Offset: 0x00016C62
		private void CollapseIndicatorUpdated()
		{
			this.CollapseIndicator.AddState("Collapsed");
			this.CollapseIndicator.AddState("Expanded");
			this.UpdateCollapseIndicator();
		}

		// Token: 0x06000888 RID: 2184 RVA: 0x00018A8A File Offset: 0x00016C8A
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

		// Token: 0x06000889 RID: 2185 RVA: 0x00018ACA File Offset: 0x00016CCA
		private void PlayerCountUpdated()
		{
			if (this.PlayerCountText == null)
			{
				return;
			}
			this.PlayerCountText.Text = "(" + this.PlayerCount + ")";
		}

		// Token: 0x0600088A RID: 2186 RVA: 0x00018AFA File Offset: 0x00016CFA
		private void InitialClosedStateUpdated()
		{
			base.IsSelected = !this.InitialClosedState;
			this.CollapseIndicatorUpdated();
		}

		// Token: 0x170002FE RID: 766
		// (get) Token: 0x0600088B RID: 2187 RVA: 0x00018B11 File Offset: 0x00016D11
		// (set) Token: 0x0600088C RID: 2188 RVA: 0x00018B19 File Offset: 0x00016D19
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

		// Token: 0x170002FF RID: 767
		// (get) Token: 0x0600088D RID: 2189 RVA: 0x00018B3D File Offset: 0x00016D3D
		// (set) Token: 0x0600088E RID: 2190 RVA: 0x00018B45 File Offset: 0x00016D45
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

		// Token: 0x17000300 RID: 768
		// (get) Token: 0x0600088F RID: 2191 RVA: 0x00018B63 File Offset: 0x00016D63
		// (set) Token: 0x06000890 RID: 2192 RVA: 0x00018B6B File Offset: 0x00016D6B
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

		// Token: 0x17000301 RID: 769
		// (get) Token: 0x06000891 RID: 2193 RVA: 0x00018B89 File Offset: 0x00016D89
		// (set) Token: 0x06000892 RID: 2194 RVA: 0x00018B91 File Offset: 0x00016D91
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

		// Token: 0x17000302 RID: 770
		// (get) Token: 0x06000893 RID: 2195 RVA: 0x00018BB5 File Offset: 0x00016DB5
		// (set) Token: 0x06000894 RID: 2196 RVA: 0x00018BBD File Offset: 0x00016DBD
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

		// Token: 0x040003E8 RID: 1000
		private Widget _collapseIndicator;

		// Token: 0x040003E9 RID: 1001
		private Widget _titleContainer;

		// Token: 0x040003EA RID: 1002
		private TextWidget _playerCountText;

		// Token: 0x040003EB RID: 1003
		private int _playerCount;

		// Token: 0x040003EC RID: 1004
		private bool _initialClosedState;
	}
}
