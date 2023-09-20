using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Authentication
{
	// Token: 0x020000A2 RID: 162
	public class MPAuthenticationDebugVM : ViewModel
	{
		// Token: 0x06000F81 RID: 3969 RVA: 0x000338BE File Offset: 0x00031ABE
		public MPAuthenticationDebugVM()
		{
			this.RefreshValues();
		}

		// Token: 0x06000F82 RID: 3970 RVA: 0x000338CC File Offset: 0x00031ACC
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = new TextObject("{=!}For Debug Purposes", null).ToString();
			this.UsernameText = new TextObject("{=!}Username:", null).ToString();
			this.PasswordText = new TextObject("{=!}Password:", null).ToString();
			this.LoginText = new TextObject("{=!}Login", null).ToString();
		}

		// Token: 0x06000F83 RID: 3971 RVA: 0x00033938 File Offset: 0x00031B38
		private async void ExecuteLogin()
		{
			LobbyState lobbyState = Game.Current.GameStateManager.ActiveState as LobbyState;
			this.IsLoginRequestActive = true;
			await lobbyState.TryLogin(this.Username, this.Password);
			this.IsLoginRequestActive = false;
		}

		// Token: 0x170004F8 RID: 1272
		// (get) Token: 0x06000F84 RID: 3972 RVA: 0x00033971 File Offset: 0x00031B71
		// (set) Token: 0x06000F85 RID: 3973 RVA: 0x00033979 File Offset: 0x00031B79
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

		// Token: 0x170004F9 RID: 1273
		// (get) Token: 0x06000F86 RID: 3974 RVA: 0x00033997 File Offset: 0x00031B97
		// (set) Token: 0x06000F87 RID: 3975 RVA: 0x0003399F File Offset: 0x00031B9F
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

		// Token: 0x170004FA RID: 1274
		// (get) Token: 0x06000F88 RID: 3976 RVA: 0x000339BD File Offset: 0x00031BBD
		// (set) Token: 0x06000F89 RID: 3977 RVA: 0x000339C5 File Offset: 0x00031BC5
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

		// Token: 0x170004FB RID: 1275
		// (get) Token: 0x06000F8A RID: 3978 RVA: 0x000339E8 File Offset: 0x00031BE8
		// (set) Token: 0x06000F8B RID: 3979 RVA: 0x000339F0 File Offset: 0x00031BF0
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

		// Token: 0x170004FC RID: 1276
		// (get) Token: 0x06000F8C RID: 3980 RVA: 0x00033A13 File Offset: 0x00031C13
		// (set) Token: 0x06000F8D RID: 3981 RVA: 0x00033A1B File Offset: 0x00031C1B
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

		// Token: 0x170004FD RID: 1277
		// (get) Token: 0x06000F8E RID: 3982 RVA: 0x00033A3E File Offset: 0x00031C3E
		// (set) Token: 0x06000F8F RID: 3983 RVA: 0x00033A46 File Offset: 0x00031C46
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

		// Token: 0x170004FE RID: 1278
		// (get) Token: 0x06000F90 RID: 3984 RVA: 0x00033A69 File Offset: 0x00031C69
		// (set) Token: 0x06000F91 RID: 3985 RVA: 0x00033A71 File Offset: 0x00031C71
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

		// Token: 0x170004FF RID: 1279
		// (get) Token: 0x06000F92 RID: 3986 RVA: 0x00033A94 File Offset: 0x00031C94
		// (set) Token: 0x06000F93 RID: 3987 RVA: 0x00033A9C File Offset: 0x00031C9C
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

		// Token: 0x0400074C RID: 1868
		private bool _isEnabled;

		// Token: 0x0400074D RID: 1869
		private bool _isLoginRequestActive;

		// Token: 0x0400074E RID: 1870
		private string _titleText;

		// Token: 0x0400074F RID: 1871
		private string _usernameText;

		// Token: 0x04000750 RID: 1872
		private string _passwordText;

		// Token: 0x04000751 RID: 1873
		private string _username;

		// Token: 0x04000752 RID: 1874
		private string _password;

		// Token: 0x04000753 RID: 1875
		private string _loginText;
	}
}
