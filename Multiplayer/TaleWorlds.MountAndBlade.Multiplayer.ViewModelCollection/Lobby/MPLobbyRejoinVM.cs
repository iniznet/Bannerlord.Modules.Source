using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby
{
	public class MPLobbyRejoinVM : ViewModel
	{
		public MPLobbyRejoinVM(Action<MPLobbyVM.LobbyPage> onChangePageRequest)
		{
			this._onChangePageRequest = onChangePageRequest;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = new TextObject("{=6zYeU0VO}Disconnected from a match", null).ToString();
			this.DescriptionText = new TextObject("{=1A1t1naG}You have left a ranked game in progress. Please reconnect to the game.", null).ToString();
			this.RejoinText = new TextObject("{=5gGyaTPL}Reconnect", null).ToString();
			this.FleeText = new TextObject("{=3sRdGQou}Leave", null).ToString();
		}

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

		private readonly Action<MPLobbyVM.LobbyPage> _onChangePageRequest;

		public Action OnRejoinRequested;

		private bool _isEnabled;

		private bool _isRejoining;

		private string _titleText;

		private string _descriptionText;

		private string _rejoinText;

		private string _fleeText;
	}
}
