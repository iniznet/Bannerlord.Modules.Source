using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby
{
	// Token: 0x02000061 RID: 97
	public class MPLobbyRejoinVM : ViewModel
	{
		// Token: 0x0600082B RID: 2091 RVA: 0x0001F033 File Offset: 0x0001D233
		public MPLobbyRejoinVM(Action<MPLobbyVM.LobbyPage> onChangePageRequest)
		{
			this._onChangePageRequest = onChangePageRequest;
			this.RefreshValues();
		}

		// Token: 0x0600082C RID: 2092 RVA: 0x0001F048 File Offset: 0x0001D248
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = new TextObject("{=6zYeU0VO}Disconnected from a match", null).ToString();
			this.DescriptionText = new TextObject("{=1A1t1naG}You have left a ranked game in progress. Please reconnect to the game.", null).ToString();
			this.RejoinText = new TextObject("{=5gGyaTPL}Reconnect", null).ToString();
			this.FleeText = new TextObject("{=3sRdGQou}Leave", null).ToString();
		}

		// Token: 0x0600082D RID: 2093 RVA: 0x0001F0B4 File Offset: 0x0001D2B4
		private void ExecuteRejoin()
		{
			NetworkMain.GameClient.RejoinBattle();
			this.TitleText = new TextObject("{=N0DXasar}Reconnecting", null).ToString();
			this.DescriptionText = new TextObject("{=BZcFB1My}Please wait while you are reconnecting to the game", null).ToString();
			this.IsRejoining = true;
			Action onRejoinRequested = this.OnRejoinRequested;
			if (onRejoinRequested == null)
			{
				return;
			}
			onRejoinRequested();
		}

		// Token: 0x0600082E RID: 2094 RVA: 0x0001F10E File Offset: 0x0001D30E
		private void ExecuteFlee()
		{
			NetworkMain.GameClient.FleeBattle();
			Action<MPLobbyVM.LobbyPage> onChangePageRequest = this._onChangePageRequest;
			if (onChangePageRequest == null)
			{
				return;
			}
			onChangePageRequest(MPLobbyVM.LobbyPage.Home);
		}

		// Token: 0x17000283 RID: 643
		// (get) Token: 0x0600082F RID: 2095 RVA: 0x0001F12B File Offset: 0x0001D32B
		// (set) Token: 0x06000830 RID: 2096 RVA: 0x0001F133 File Offset: 0x0001D333
		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		// Token: 0x17000284 RID: 644
		// (get) Token: 0x06000831 RID: 2097 RVA: 0x0001F151 File Offset: 0x0001D351
		// (set) Token: 0x06000832 RID: 2098 RVA: 0x0001F159 File Offset: 0x0001D359
		[DataSourceProperty]
		public bool IsRejoining
		{
			get
			{
				return this._isRejoining;
			}
			set
			{
				if (value != this._isRejoining)
				{
					this._isRejoining = value;
					base.OnPropertyChangedWithValue(value, "IsRejoining");
				}
			}
		}

		// Token: 0x17000285 RID: 645
		// (get) Token: 0x06000833 RID: 2099 RVA: 0x0001F177 File Offset: 0x0001D377
		// (set) Token: 0x06000834 RID: 2100 RVA: 0x0001F17F File Offset: 0x0001D37F
		[DataSourceProperty]
		public string TitleText
		{
			get
			{
				return this._titleText;
			}
			set
			{
				if (value != this._titleText)
				{
					this._titleText = value;
					base.OnPropertyChangedWithValue<string>(value, "TitleText");
				}
			}
		}

		// Token: 0x17000286 RID: 646
		// (get) Token: 0x06000835 RID: 2101 RVA: 0x0001F1A2 File Offset: 0x0001D3A2
		// (set) Token: 0x06000836 RID: 2102 RVA: 0x0001F1AA File Offset: 0x0001D3AA
		[DataSourceProperty]
		public string DescriptionText
		{
			get
			{
				return this._descriptionText;
			}
			set
			{
				if (value != this._descriptionText)
				{
					this._descriptionText = value;
					base.OnPropertyChangedWithValue<string>(value, "DescriptionText");
				}
			}
		}

		// Token: 0x17000287 RID: 647
		// (get) Token: 0x06000837 RID: 2103 RVA: 0x0001F1CD File Offset: 0x0001D3CD
		// (set) Token: 0x06000838 RID: 2104 RVA: 0x0001F1D5 File Offset: 0x0001D3D5
		[DataSourceProperty]
		public string RejoinText
		{
			get
			{
				return this._rejoinText;
			}
			set
			{
				if (value != this._rejoinText)
				{
					this._rejoinText = value;
					base.OnPropertyChangedWithValue<string>(value, "RejoinText");
				}
			}
		}

		// Token: 0x17000288 RID: 648
		// (get) Token: 0x06000839 RID: 2105 RVA: 0x0001F1F8 File Offset: 0x0001D3F8
		// (set) Token: 0x0600083A RID: 2106 RVA: 0x0001F200 File Offset: 0x0001D400
		[DataSourceProperty]
		public string FleeText
		{
			get
			{
				return this._fleeText;
			}
			set
			{
				if (value != this._fleeText)
				{
					this._fleeText = value;
					base.OnPropertyChangedWithValue<string>(value, "FleeText");
				}
			}
		}

		// Token: 0x0400041F RID: 1055
		private readonly Action<MPLobbyVM.LobbyPage> _onChangePageRequest;

		// Token: 0x04000420 RID: 1056
		public Action OnRejoinRequested;

		// Token: 0x04000421 RID: 1057
		private bool _isEnabled;

		// Token: 0x04000422 RID: 1058
		private bool _isRejoining;

		// Token: 0x04000423 RID: 1059
		private string _titleText;

		// Token: 0x04000424 RID: 1060
		private string _descriptionText;

		// Token: 0x04000425 RID: 1061
		private string _rejoinText;

		// Token: 0x04000426 RID: 1062
		private string _fleeText;
	}
}
