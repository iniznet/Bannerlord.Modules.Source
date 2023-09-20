using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Authentication
{
	public class MPAuthenticationDebugVM : ViewModel
	{
		public MPAuthenticationDebugVM()
		{
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = new TextObject("{=!}For Debug Purposes", null).ToString();
			this.UsernameText = new TextObject("{=!}Username:", null).ToString();
			this.PasswordText = new TextObject("{=!}Password:", null).ToString();
			this.LoginText = new TextObject("{=!}Login", null).ToString();
		}

		private async void ExecuteLogin()
		{
			LobbyState lobbyState = Game.Current.GameStateManager.ActiveState as LobbyState;
			this.IsLoginRequestActive = true;
			await lobbyState.TryLogin(this.Username, this.Password);
			this.IsLoginRequestActive = false;
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
		public bool IsLoginRequestActive
		{
			get
			{
				return this._isLoginRequestActive;
			}
			set
			{
				if (value != this._isLoginRequestActive)
				{
					this._isLoginRequestActive = value;
					base.OnPropertyChangedWithValue(value, "IsLoginRequestActive");
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
		public string UsernameText
		{
			get
			{
				return this._usernameText;
			}
			set
			{
				if (value != this._usernameText)
				{
					this._usernameText = value;
					base.OnPropertyChangedWithValue<string>(value, "UsernameText");
				}
			}
		}

		[DataSourceProperty]
		public string PasswordText
		{
			get
			{
				return this._passwordText;
			}
			set
			{
				if (value != this._passwordText)
				{
					this._passwordText = value;
					base.OnPropertyChangedWithValue<string>(value, "PasswordText");
				}
			}
		}

		[DataSourceProperty]
		public string Username
		{
			get
			{
				return this._username;
			}
			set
			{
				if (value != this._username)
				{
					this._username = value;
					base.OnPropertyChangedWithValue<string>(value, "Username");
				}
			}
		}

		[DataSourceProperty]
		public string Password
		{
			get
			{
				return this._password;
			}
			set
			{
				if (value != this._password)
				{
					this._password = value;
					base.OnPropertyChangedWithValue<string>(value, "Password");
				}
			}
		}

		[DataSourceProperty]
		public string LoginText
		{
			get
			{
				return this._loginText;
			}
			set
			{
				if (value != this._loginText)
				{
					this._loginText = value;
					base.OnPropertyChangedWithValue<string>(value, "LoginText");
				}
			}
		}

		private bool _isEnabled;

		private bool _isLoginRequestActive;

		private string _titleText;

		private string _usernameText;

		private string _passwordText;

		private string _username;

		private string _password;

		private string _loginText;
	}
}
