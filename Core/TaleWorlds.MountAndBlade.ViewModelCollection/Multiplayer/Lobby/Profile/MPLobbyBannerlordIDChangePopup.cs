using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;
using TaleWorlds.PlatformService;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Profile
{
	public class MPLobbyBannerlordIDChangePopup : ViewModel
	{
		public MPLobbyBannerlordIDChangePopup()
		{
			this.BannerlordIDInputText = "";
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ChangeBannerlordIDText = new TextObject("{=ozREO8ev}Change Bannerlord ID", null).ToString();
			this.TypeYourNameText = new TextObject("{=clxT9H4T}Type Your Name", null).ToString();
			this.RequestSentText = new TextObject("{=V2lpn6dc}Your Bannerlord ID changing request has been successfully sent.", null).ToString();
			this.DoneText = GameTexts.FindText("str_done", null).ToString();
			this.CancelText = GameTexts.FindText("str_cancel", null).ToString();
			this.ErrorText = "";
		}

		public void ExecuteOpenPopup()
		{
			this.IsSelected = true;
			this.HasRequestSent = false;
		}

		public void ExecuteClosePopup()
		{
			this.IsSelected = false;
			this.BannerlordIDInputText = "";
			this.ErrorText = "";
		}

		private async Task<bool> IsInputValid()
		{
			bool flag;
			if (this.BannerlordIDInputText.Length < Parameters.UsernameMinLength)
			{
				GameTexts.SetVariable("STR1", new TextObject("{=k7fJ7TF0}Has to be at least", null));
				GameTexts.SetVariable("STR2", Parameters.UsernameMinLength);
				string text = GameTexts.FindText("str_STR1_space_STR2", null).ToString();
				GameTexts.SetVariable("STR1", text);
				GameTexts.SetVariable("STR2", new TextObject("{=nWJGjCgy}characters", null));
				this.ErrorText = GameTexts.FindText("str_STR1_space_STR2", null).ToString();
				flag = false;
			}
			else if (!Common.IsAllLetters(this.BannerlordIDInputText))
			{
				this.ErrorText = new TextObject("{=Po8jNaXb}Can only contain letters", null).ToString();
				flag = false;
			}
			else
			{
				TaskAwaiter<bool> taskAwaiter = PlatformServices.Instance.VerifyString(this.BannerlordIDInputText).GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					TaskAwaiter<bool> taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<bool>);
				}
				if (!taskAwaiter.GetResult())
				{
					this.ErrorText = new TextObject("{=bXAIlBHv}Can not contain offensive language", null).ToString();
					flag = false;
				}
				else
				{
					flag = true;
				}
			}
			return flag;
		}

		public async void ExecuteApply()
		{
			if (!this.HasRequestSent)
			{
				TaskAwaiter<bool> taskAwaiter = this.IsInputValid().GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					TaskAwaiter<bool> taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<bool>);
				}
				if (taskAwaiter.GetResult())
				{
					NetworkMain.GameClient.ChangeUsername(this.BannerlordIDInputText);
					this.HasRequestSent = true;
					this.ErrorText = "";
				}
			}
			else
			{
				this.ExecuteClosePopup();
			}
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			InputKeyItemVM cancelInputKey = this.CancelInputKey;
			if (cancelInputKey != null)
			{
				cancelInputKey.OnFinalize();
			}
			InputKeyItemVM doneInputKey = this.DoneInputKey;
			if (doneInputKey == null)
			{
				return;
			}
			doneInputKey.OnFinalize();
		}

		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
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
					base.OnPropertyChanged("CancelInputKey");
				}
			}
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
					base.OnPropertyChanged("DoneInputKey");
				}
			}
		}

		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChanged("IsSelected");
				}
			}
		}

		[DataSourceProperty]
		public string BannerlordIDInputText
		{
			get
			{
				return this._bannerlordIDInputText;
			}
			set
			{
				if (value != this._bannerlordIDInputText)
				{
					this._bannerlordIDInputText = value;
					this.ErrorText = "";
					base.OnPropertyChanged("BannerlordIDInputText");
				}
			}
		}

		[DataSourceProperty]
		public string ChangeBannerlordIDText
		{
			get
			{
				return this._changeBannerlordIDText;
			}
			set
			{
				if (value != this._changeBannerlordIDText)
				{
					this._changeBannerlordIDText = value;
					base.OnPropertyChanged("ChangeBannerlordIDText");
				}
			}
		}

		[DataSourceProperty]
		public string TypeYourNameText
		{
			get
			{
				return this._typeYourNameText;
			}
			set
			{
				if (value != this._typeYourNameText)
				{
					this._typeYourNameText = value;
					base.OnPropertyChanged("TypeYourNameText");
				}
			}
		}

		[DataSourceProperty]
		public string RequestSentText
		{
			get
			{
				return this._requestSentText;
			}
			set
			{
				if (value != this._requestSentText)
				{
					this._requestSentText = value;
					base.OnPropertyChanged("RequestSentText");
				}
			}
		}

		[DataSourceProperty]
		public bool HasRequestSent
		{
			get
			{
				return this._hasRequestSent;
			}
			set
			{
				if (value != this._hasRequestSent)
				{
					this._hasRequestSent = value;
					base.OnPropertyChanged("HasRequestSent");
				}
			}
		}

		[DataSourceProperty]
		public string ErrorText
		{
			get
			{
				return this._errorText;
			}
			set
			{
				if (value != this._errorText)
				{
					this._errorText = value;
					base.OnPropertyChanged("ErrorText");
				}
			}
		}

		[DataSourceProperty]
		public string CancelText
		{
			get
			{
				return this._cancelText;
			}
			set
			{
				if (value != this._cancelText)
				{
					this._cancelText = value;
					base.OnPropertyChanged("CancelText");
				}
			}
		}

		[DataSourceProperty]
		public string DoneText
		{
			get
			{
				return this._doneText;
			}
			set
			{
				if (value != this._doneText)
				{
					this._doneText = value;
					base.OnPropertyChanged("DoneText");
				}
			}
		}

		private InputKeyItemVM _cancelInputKey;

		private InputKeyItemVM _doneInputKey;

		private bool _isSelected;

		private bool _hasRequestSent;

		private string _bannerlordIDInputText;

		private string _changeBannerlordIDText;

		private string _typeYourNameText;

		private string _requestSentText;

		private string _errorText;

		private string _cancelText;

		private string _doneText;
	}
}
