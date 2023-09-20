using System;
using System.Runtime.CompilerServices;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Profile
{
	public class MPLobbyBannerlordIDAddFriendPopupVM : ViewModel
	{
		public MPLobbyBannerlordIDAddFriendPopupVM()
		{
			this.BannerlordIDInputText = "";
			this.ErrorText = "";
			this.RefreshValues();
		}

		public void ExecuteOpenPopup()
		{
			this.IsSelected = true;
		}

		public void ExecuteClosePopup()
		{
			this.IsSelected = false;
			this.BannerlordIDInputText = "";
			this.ErrorText = "";
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = new TextObject("{=L3DJHTdY}Enter Bannerlord ID", null).ToString();
			this.AddText = new TextObject("{=tC9C8TLi}Add Friend", null).ToString();
		}

		public async void ExecuteTryAddFriend()
		{
			string[] array = this.BannerlordIDInputText.Split(new char[] { '#' });
			if (array.Length == 2 && !Extensions.IsEmpty<char>(array[1]))
			{
				string username = array[0];
				int id = 0;
				bool flag = Common.IsAllLetters(array[0]) && array[0].Length >= Parameters.UsernameMinLength;
				if (int.TryParse(array[1], out id) && flag)
				{
					TaskAwaiter<bool> taskAwaiter = NetworkMain.GameClient.DoesPlayerWithUsernameAndIdExist(username, id).GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						await taskAwaiter;
						TaskAwaiter<bool> taskAwaiter2;
						taskAwaiter = taskAwaiter2;
						taskAwaiter2 = default(TaskAwaiter<bool>);
					}
					if (taskAwaiter.GetResult())
					{
						NetworkMain.GameClient.AddFriendByUsernameAndId(username, id, BannerlordConfig.EnableGenericNames);
						this.ExecuteClosePopup();
					}
					else
					{
						this.ErrorText = new TextObject("{=tTwQsP6j}Player does not exist", null).ToString();
					}
				}
				else
				{
					this.ErrorText = new TextObject("{=rWm5udCd}You must enter a valid Bannerlord ID", null).ToString();
				}
				username = null;
			}
			else
			{
				this.ErrorText = new TextObject("{=rWm5udCd}You must enter a valid Bannerlord ID", null).ToString();
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
					base.OnPropertyChanged("TitleText");
				}
			}
		}

		[DataSourceProperty]
		public string AddText
		{
			get
			{
				return this._addText;
			}
			set
			{
				if (value != this._addText)
				{
					this._addText = value;
					base.OnPropertyChanged("AddText");
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
					base.OnPropertyChanged("BannerlordIDInputText");
					this.ErrorText = "";
				}
			}
		}

		private InputKeyItemVM _cancelInputKey;

		private InputKeyItemVM _doneInputKey;

		private bool _isSelected;

		private string _titleText;

		private string _addText;

		private string _errorText;

		private string _bannerlordIDInputText;
	}
}
