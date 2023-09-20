using System;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Authentication
{
	public class MPAuthenticationVM : ViewModel
	{
		public MPAuthenticationVM(LobbyState lobbyState)
		{
			this._lobbyState = lobbyState;
			this._hasPrivilege = this._lobbyState.HasMultiplayerPrivilege;
			LobbyState lobbyState2 = this._lobbyState;
			lobbyState2.OnMultiplayerPrivilegeUpdated = (Action<bool>)Delegate.Combine(lobbyState2.OnMultiplayerPrivilegeUpdated, new Action<bool>(this.OnMultiplayerPrivilegeUpdated));
			InternetAvailabilityChecker.OnInternetConnectionAvailabilityChanged = (Action<bool>)Delegate.Combine(InternetAvailabilityChecker.OnInternetConnectionAvailabilityChanged, new Action<bool>(this.OnInternetConnectionAvailabilityChanged));
			this.AuthenticationDebug = new MPAuthenticationDebugVM();
			this.AuthenticationDebug.IsEnabled = false;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ExitText = new TextObject("{=3CsACce8}Exit", null).ToString();
			this.LoginText = new TextObject("{=lugGPVOb}Login", null).ToString();
			this.TitleText = this._idleTitle.ToString();
			this.MessageText = this._idleMessage.ToString();
			this.CommunityGamesText = new TextObject("{=SIIgjILk}Community Games", null).ToString();
			this.AuthenticationDebug.RefreshValues();
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			InputKeyItemVM doneInputKey = this.DoneInputKey;
			if (doneInputKey != null)
			{
				doneInputKey.OnFinalize();
			}
			InputKeyItemVM cancelInputKey = this.CancelInputKey;
			if (cancelInputKey != null)
			{
				cancelInputKey.OnFinalize();
			}
			LobbyState lobbyState = this._lobbyState;
			lobbyState.OnMultiplayerPrivilegeUpdated = (Action<bool>)Delegate.Remove(lobbyState.OnMultiplayerPrivilegeUpdated, new Action<bool>(this.OnMultiplayerPrivilegeUpdated));
			InternetAvailabilityChecker.OnInternetConnectionAvailabilityChanged = (Action<bool>)Delegate.Remove(InternetAvailabilityChecker.OnInternetConnectionAvailabilityChanged, new Action<bool>(this.OnInternetConnectionAvailabilityChanged));
		}

		public void OnTick(float dt)
		{
			if (!this.IsEnabled || this._lobbyState == null)
			{
				return;
			}
			LobbyClient.State currentState = NetworkMain.GameClient.CurrentState;
			if (currentState != 1 && currentState != 2)
			{
				bool flag = currentState == 3;
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

		public void ExecuteExit()
		{
			LobbyClient.State currentState = NetworkMain.GameClient.CurrentState;
			if (currentState == null)
			{
				InformationManager.ShowInquiry(new InquiryData(GameTexts.FindText("str_exit", null).ToString(), GameTexts.FindText("str_mp_exit_query", null).ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), delegate
				{
					this.OnExit();
				}, null, "", 0f, null, null, null), false, false);
				return;
			}
			TextObject textObject = MPAuthenticationVM.CantLogoutSearchingForMatchTextObject;
			if (currentState == 1 || currentState == 2 || currentState == 3)
			{
				textObject = MPAuthenticationVM.CantLogoutLoggingInTextObject;
			}
			InformationManager.ShowInquiry(new InquiryData(GameTexts.FindText("str_exit", null).ToString(), textObject.ToString(), true, false, GameTexts.FindText("str_ok", null).ToString(), null, null, null, "", 0f, null, null, null), false, false);
		}

		private void OnExit()
		{
			LobbyState lobbyState = this._lobbyState;
			if (lobbyState == null || !lobbyState.IsLoggingIn)
			{
				if (Module.CurrentModule.StartupInfo.StartupType == 4)
				{
					MBInitialScreenBase.DoExitButtonAction();
					return;
				}
				Game.Current.GameStateManager.PopState(0);
			}
		}

		public async void ExecuteLogin()
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
					Debug.Print(ex.StackTrace ?? "", 0, 12, 17592186044416UL);
				}
			}
		}

		private void OnMultiplayerPrivilegeUpdated(bool hasPrivilege)
		{
			this._hasPrivilege = new bool?(hasPrivilege);
			this.UpdateCanTryLogin();
		}

		private void OnInternetConnectionAvailabilityChanged(bool isInternetAvailable)
		{
			this.UpdatePrivilegeInformation();
		}

		private void UpdateCanTryLogin()
		{
			this.CanTryLogin = this._hasPrivilege.GetValueOrDefault() && !this._lobbyState.IsLoggingIn;
		}

		private void UpdatePrivilegeInformation()
		{
			LobbyState lobbyState = this._lobbyState;
			if (lobbyState == null)
			{
				return;
			}
			lobbyState.UpdateHasMultiplayerPrivilege();
		}

		[DataSourceProperty]
		public InputKeyItemVM DoneInputKey
		{
			get
			{
				return this._doneInputKey;
			}
			set
			{
				if (value != this._doneInputKey)
				{
					this._doneInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "DoneInputKey");
				}
			}
		}

		[DataSourceProperty]
		public InputKeyItemVM CancelInputKey
		{
			get
			{
				return this._cancelInputKey;
			}
			set
			{
				if (value != this._cancelInputKey)
				{
					this._cancelInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "CancelInputKey");
				}
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
					if (this.IsEnabled)
					{
						this.UpdatePrivilegeInformation();
					}
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
					this.UpdateCanTryLogin();
				}
			}
		}

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

		[DataSourceProperty]
		public string CommunityGamesText
		{
			get
			{
				return this._communityGamesText;
			}
			set
			{
				if (value != this._communityGamesText)
				{
					this._communityGamesText = value;
					base.OnPropertyChangedWithValue<string>(value, "CommunityGamesText");
				}
			}
		}

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

		public void SetDoneInputKey(HotKey hotkey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		public void SetCancelInputKey(HotKey hotkey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		private readonly LobbyState _lobbyState;

		private bool? _hasPrivilege;

		private readonly TextObject _idleTitle = new TextObject("{=g1lgiwn1}Not Logged In", null);

		private readonly TextObject _idleMessage = new TextObject("{=saZ1OvPt}You can press the login button to establish connection", null);

		private readonly TextObject _noAccessMessage = new TextObject("{=9P0VL49j}You don't have access to multiplayer.", null);

		private readonly TextObject _loggingInTitle = new TextObject("{=iNqucBor}Logging In", null);

		private readonly TextObject _loggingInMessage = new TextObject("{=U4dzbzNb}Please wait while you are being connected to the server", null);

		private static readonly TextObject CantLogoutLoggingInTextObject = new TextObject("{=E0q43haK}Please wait until you are logged in.", null);

		private static readonly TextObject CantLogoutSearchingForMatchTextObject = new TextObject("{=DyeaObj5}Please cancel game search request before logging out.", null);

		private bool _isEnabled;

		private InputKeyItemVM _doneInputKey;

		private InputKeyItemVM _cancelInputKey;

		private bool _isLoginRequestActive;

		private bool _canTryLogin;

		private string _titleText;

		private string _messageText;

		private string _exitText;

		private string _loginText;

		private string _communityGamesText;

		private MPAuthenticationDebugVM _authenticationDebug;
	}
}
