using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.CustomGame;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.HostGame
{
	public class MPHostGameVM : ViewModel
	{
		public MPHostGameVM(LobbyState lobbyState, MPCustomGameVM.CustomGameMode customGameMode)
		{
			this._lobbyState = lobbyState;
			this._customGameMode = customGameMode;
			this.HostGameOptions = new MPHostGameOptionsVM(false, this._customGameMode);
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.CreateText = new TextObject("{=aRzlp5XH}CREATE", null).ToString();
			this.HostGameOptions.RefreshValues();
		}

		public void ExecuteStart()
		{
			if (this._customGameMode == MPCustomGameVM.CustomGameMode.CustomServer)
			{
				this._lobbyState.HostGame();
				return;
			}
			if (this._customGameMode == MPCustomGameVM.CustomGameMode.PremadeGame)
			{
				this._lobbyState.CreatePremadeGame();
			}
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
		public MPHostGameOptionsVM HostGameOptions
		{
			get
			{
				return this._hostGameOptions;
			}
			set
			{
				if (value != this._hostGameOptions)
				{
					this._hostGameOptions = value;
					base.OnPropertyChangedWithValue<MPHostGameOptionsVM>(value, "HostGameOptions");
				}
			}
		}

		[DataSourceProperty]
		public string CreateText
		{
			get
			{
				return this._createText;
			}
			set
			{
				if (value != this._createText)
				{
					this._createText = value;
					base.OnPropertyChangedWithValue<string>(value, "CreateText");
				}
			}
		}

		private LobbyState _lobbyState;

		private MPCustomGameVM.CustomGameMode _customGameMode;

		private bool _isEnabled;

		private MPHostGameOptionsVM _hostGameOptions;

		private string _createText;
	}
}
