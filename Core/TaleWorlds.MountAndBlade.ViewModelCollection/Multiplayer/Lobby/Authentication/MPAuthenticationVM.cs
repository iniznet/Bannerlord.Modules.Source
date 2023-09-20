using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Authentication
{
	// Token: 0x020000A3 RID: 163
	public class MPAuthenticationVM : ViewModel
	{
		// Token: 0x06000F94 RID: 3988 RVA: 0x00033AC0 File Offset: 0x00031CC0
		public MPAuthenticationVM(LobbyState lobbyState)
		{
			this._lobbyState = lobbyState;
			this._hasPrivilege = this._lobbyState.HasMultiplayerPrivilege;
			LobbyState lobbyState2 = this._lobbyState;
			lobbyState2.OnMultiplayerPrivilegeUpdated = (Action<bool>)Delegate.Combine(lobbyState2.OnMultiplayerPrivilegeUpdated, new Action<bool>(this.OnMultiplayerPrivilegeUpdated));
			NetworkMain.OnInternetConnectionAvailabilityChanged = (Action<bool>)Delegate.Combine(NetworkMain.OnInternetConnectionAvailabilityChanged, new Action<bool>(this.OnInternetConnectionAvailabilityChanged));
			this.AuthenticationDebug = new MPAuthenticationDebugVM();
			this.AuthenticationDebug.IsEnabled = false;
			this.RefreshValues();
		}

		// Token: 0x06000F95 RID: 3989 RVA: 0x00033BA4 File Offset: 0x00031DA4
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ExitText = new TextObject("{=3CsACce8}Exit", null).ToString();
			this.LoginText = new TextObject("{=lugGPVOb}Login", null).ToString();
			this.TitleText = this._idleTitle.ToString();
			this.MessageText = this._idleMessage.ToString();
			this.AuthenticationDebug.RefreshValues();
		}

		// Token: 0x06000F96 RID: 3990 RVA: 0x00033C10 File Offset: 0x00031E10
		public override void OnFinalize()
		{
			base.OnFinalize();
			LobbyState lobbyState = this._lobbyState;
			lobbyState.OnMultiplayerPrivilegeUpdated = (Action<bool>)Delegate.Remove(lobbyState.OnMultiplayerPrivilegeUpdated, new Action<bool>(this.OnMultiplayerPrivilegeUpdated));
			NetworkMain.OnInternetConnectionAvailabilityChanged = (Action<bool>)Delegate.Remove(NetworkMain.OnInternetConnectionAvailabilityChanged, new Action<bool>(this.OnInternetConnectionAvailabilityChanged));
		}

		// Token: 0x06000F97 RID: 3991 RVA: 0x00033C6C File Offset: 0x00031E6C
		public void OnTick(float dt)
		{
			if (!this.IsEnabled || this._lobbyState == null)
			{
				return;
			}
			LobbyClient.State currentState = NetworkMain.GameClient.CurrentState;
			if (currentState != LobbyClient.State.Working && currentState != LobbyClient.State.Connected)
			{
				bool flag = currentState == LobbyClient.State.SessionRequested;
			}
			if (this._hasPrivilege != null && !this._hasPrivilege.Value)
			{
				this.TitleText = this._idleTitle.ToString();
				this.MessageText = this._noAccessMessage.ToString();
				return;
			}
			if (this._lobbyState.IsLoggingIn)
			{
				this.IsLoginRequestActive = true;
				this.TitleText = this._loggingInTitle.ToString();
				this.MessageText = this._loggingInMessage.ToString();
				return;
			}
			this.IsLoginRequestActive = false;
			this.TitleText = this._idleTitle.ToString();
			this.MessageText = this._idleMessage.ToString();
		}

		// Token: 0x06000F98 RID: 3992 RVA: 0x00033D44 File Offset: 0x00031F44
		public void ExecuteExit()
		{
			LobbyClient.State currentState = NetworkMain.GameClient.CurrentState;
			if (currentState == LobbyClient.State.Idle)
			{
				InformationManager.ShowInquiry(new InquiryData(GameTexts.FindText("str_exit", null).ToString(), GameTexts.FindText("str_mp_exit_query", null).ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), delegate
				{
					this.OnExit();
				}, null, "", 0f, null, null, null), false, false);
				return;
			}
			TextObject textObject = MPAuthenticationVM.CantLogoutSearchingForMatchTextObject;
			if (currentState == LobbyClient.State.Working || currentState == LobbyClient.State.Connected || currentState == LobbyClient.State.SessionRequested)
			{
				textObject = MPAuthenticationVM.CantLogoutLoggingInTextObject;
			}
			InformationManager.ShowInquiry(new InquiryData(GameTexts.FindText("str_exit", null).ToString(), textObject.ToString(), true, false, GameTexts.FindText("str_ok", null).ToString(), null, null, null, "", 0f, null, null, null), false, false);
		}

		// Token: 0x06000F99 RID: 3993 RVA: 0x00033E24 File Offset: 0x00032024
		private void OnExit()
		{
			LobbyState lobbyState = this._lobbyState;
			if (lobbyState == null || !lobbyState.IsLoggingIn)
			{
				if (Module.CurrentModule.StartupInfo.StartupType == GameStartupType.Multiplayer)
				{
					MBInitialScreenBase.DoExitButtonAction();
					return;
				}
				Game.Current.GameStateManager.PopState(0);
			}
		}

		// Token: 0x06000F9A RID: 3994 RVA: 0x00033E70 File Offset: 0x00032070
		private async void ExecuteLogin()
		{
			LobbyState lobbyState = this._lobbyState;
			if (lobbyState == null || !lobbyState.IsLoggingIn)
			{
				try
				{
					await this._lobbyState.TryLogin();
				}
				catch (Exception ex)
				{
					Debug.Print(ex.StackTrace ?? "", 0, Debug.DebugColor.White, 17592186044416UL);
				}
			}
		}

		// Token: 0x06000F9B RID: 3995 RVA: 0x00033EA9 File Offset: 0x000320A9
		private void OnMultiplayerPrivilegeUpdated(bool hasPrivilege)
		{
			this._hasPrivilege = new bool?(hasPrivilege);
			this.UpdateCanTryLogin();
		}

		// Token: 0x06000F9C RID: 3996 RVA: 0x00033EBD File Offset: 0x000320BD
		private void OnInternetConnectionAvailabilityChanged(bool isInternetAvailable)
		{
			this.UpdatePrivilegeInformation();
		}

		// Token: 0x06000F9D RID: 3997 RVA: 0x00033EC5 File Offset: 0x000320C5
		private void UpdateCanTryLogin()
		{
			this.CanTryLogin = this._hasPrivilege.GetValueOrDefault() && !this._lobbyState.IsLoggingIn;
		}

		// Token: 0x06000F9E RID: 3998 RVA: 0x00033EEB File Offset: 0x000320EB
		private void UpdatePrivilegeInformation()
		{
			LobbyState lobbyState = this._lobbyState;
			if (lobbyState == null)
			{
				return;
			}
			lobbyState.UpdateHasMultiplayerPrivilege();
		}

		// Token: 0x17000500 RID: 1280
		// (get) Token: 0x06000F9F RID: 3999 RVA: 0x00033EFE File Offset: 0x000320FE
		// (set) Token: 0x06000FA0 RID: 4000 RVA: 0x00033F06 File Offset: 0x00032106
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
					if (this.IsEnabled)
					{
						this.UpdatePrivilegeInformation();
					}
				}
			}
		}

		// Token: 0x17000501 RID: 1281
		// (get) Token: 0x06000FA1 RID: 4001 RVA: 0x00033F32 File Offset: 0x00032132
		// (set) Token: 0x06000FA2 RID: 4002 RVA: 0x00033F3A File Offset: 0x0003213A
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
					this.UpdateCanTryLogin();
				}
			}
		}

		// Token: 0x17000502 RID: 1282
		// (get) Token: 0x06000FA3 RID: 4003 RVA: 0x00033F5E File Offset: 0x0003215E
		// (set) Token: 0x06000FA4 RID: 4004 RVA: 0x00033F66 File Offset: 0x00032166
		[DataSourceProperty]
		public bool CanTryLogin
		{
			get
			{
				return this._canTryLogin;
			}
			set
			{
				if (value != this._canTryLogin)
				{
					this._canTryLogin = value;
					base.OnPropertyChangedWithValue(value, "CanTryLogin");
				}
			}
		}

		// Token: 0x17000503 RID: 1283
		// (get) Token: 0x06000FA5 RID: 4005 RVA: 0x00033F84 File Offset: 0x00032184
		// (set) Token: 0x06000FA6 RID: 4006 RVA: 0x00033F8C File Offset: 0x0003218C
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

		// Token: 0x17000504 RID: 1284
		// (get) Token: 0x06000FA7 RID: 4007 RVA: 0x00033FAF File Offset: 0x000321AF
		// (set) Token: 0x06000FA8 RID: 4008 RVA: 0x00033FB7 File Offset: 0x000321B7
		[DataSourceProperty]
		public string MessageText
		{
			get
			{
				return this._messageText;
			}
			set
			{
				if (value != this._messageText)
				{
					this._messageText = value;
					base.OnPropertyChangedWithValue<string>(value, "MessageText");
				}
			}
		}

		// Token: 0x17000505 RID: 1285
		// (get) Token: 0x06000FA9 RID: 4009 RVA: 0x00033FDA File Offset: 0x000321DA
		// (set) Token: 0x06000FAA RID: 4010 RVA: 0x00033FE2 File Offset: 0x000321E2
		[DataSourceProperty]
		public string ExitText
		{
			get
			{
				return this._exitText;
			}
			set
			{
				if (value != this._exitText)
				{
					this._exitText = value;
					base.OnPropertyChangedWithValue<string>(value, "ExitText");
				}
			}
		}

		// Token: 0x17000506 RID: 1286
		// (get) Token: 0x06000FAB RID: 4011 RVA: 0x00034005 File Offset: 0x00032205
		// (set) Token: 0x06000FAC RID: 4012 RVA: 0x0003400D File Offset: 0x0003220D
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

		// Token: 0x17000507 RID: 1287
		// (get) Token: 0x06000FAD RID: 4013 RVA: 0x00034030 File Offset: 0x00032230
		// (set) Token: 0x06000FAE RID: 4014 RVA: 0x00034038 File Offset: 0x00032238
		[DataSourceProperty]
		public MPAuthenticationDebugVM AuthenticationDebug
		{
			get
			{
				return this._authenticationDebug;
			}
			set
			{
				if (value != this._authenticationDebug)
				{
					this._authenticationDebug = value;
					base.OnPropertyChangedWithValue<MPAuthenticationDebugVM>(value, "AuthenticationDebug");
				}
			}
		}

		// Token: 0x04000754 RID: 1876
		private readonly LobbyState _lobbyState;

		// Token: 0x04000755 RID: 1877
		private bool? _hasPrivilege;

		// Token: 0x04000756 RID: 1878
		private readonly TextObject _idleTitle = new TextObject("{=g1lgiwn1}Not Logged In", null);

		// Token: 0x04000757 RID: 1879
		private readonly TextObject _idleMessage = new TextObject("{=saZ1OvPt}You can press the login button to establish connection", null);

		// Token: 0x04000758 RID: 1880
		private readonly TextObject _noAccessMessage = new TextObject("{=9P0VL49j}You don't have access to multiplayer.", null);

		// Token: 0x04000759 RID: 1881
		private readonly TextObject _loggingInTitle = new TextObject("{=iNqucBor}Logging In", null);

		// Token: 0x0400075A RID: 1882
		private readonly TextObject _loggingInMessage = new TextObject("{=U4dzbzNb}Please wait while you are being connected to the server", null);

		// Token: 0x0400075B RID: 1883
		private static readonly TextObject CantLogoutLoggingInTextObject = new TextObject("{=E0q43haK}Please wait until you are logged in.", null);

		// Token: 0x0400075C RID: 1884
		private static readonly TextObject CantLogoutSearchingForMatchTextObject = new TextObject("{=DyeaObj5}Please cancel game search request before logging out.", null);

		// Token: 0x0400075D RID: 1885
		private bool _isEnabled;

		// Token: 0x0400075E RID: 1886
		private bool _isLoginRequestActive;

		// Token: 0x0400075F RID: 1887
		private bool _canTryLogin;

		// Token: 0x04000760 RID: 1888
		private string _titleText;

		// Token: 0x04000761 RID: 1889
		private string _messageText;

		// Token: 0x04000762 RID: 1890
		private string _exitText;

		// Token: 0x04000763 RID: 1891
		private string _loginText;

		// Token: 0x04000764 RID: 1892
		private MPAuthenticationDebugVM _authenticationDebug;
	}
}
