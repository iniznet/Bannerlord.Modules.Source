using System;
using System.Runtime.CompilerServices;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Profile
{
	// Token: 0x02000068 RID: 104
	public class MPLobbyBannerlordIDAddFriendPopupVM : ViewModel
	{
		// Token: 0x0600099F RID: 2463 RVA: 0x00023B42 File Offset: 0x00021D42
		public MPLobbyBannerlordIDAddFriendPopupVM()
		{
			this.BannerlordIDInputText = "";
			this.ErrorText = "";
			this.RefreshValues();
		}

		// Token: 0x060009A0 RID: 2464 RVA: 0x00023B66 File Offset: 0x00021D66
		public void ExecuteOpenPopup()
		{
			this.IsSelected = true;
		}

		// Token: 0x060009A1 RID: 2465 RVA: 0x00023B6F File Offset: 0x00021D6F
		public void ExecuteClosePopup()
		{
			this.IsSelected = false;
			this.BannerlordIDInputText = "";
			this.ErrorText = "";
		}

		// Token: 0x060009A2 RID: 2466 RVA: 0x00023B8E File Offset: 0x00021D8E
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = new TextObject("{=L3DJHTdY}Enter Bannerlord ID", null).ToString();
			this.AddText = new TextObject("{=tC9C8TLi}Add Friend", null).ToString();
		}

		// Token: 0x060009A3 RID: 2467 RVA: 0x00023BC4 File Offset: 0x00021DC4
		public async void ExecuteTryAddFriend()
		{
			string[] array = this.BannerlordIDInputText.Split(new char[] { '#' });
			if (array.Length == 2 && !array[1].IsEmpty<char>())
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

		// Token: 0x060009A4 RID: 2468 RVA: 0x00023BFD File Offset: 0x00021DFD
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

		// Token: 0x060009A5 RID: 2469 RVA: 0x00023C26 File Offset: 0x00021E26
		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x060009A6 RID: 2470 RVA: 0x00023C35 File Offset: 0x00021E35
		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x170002F5 RID: 757
		// (get) Token: 0x060009A7 RID: 2471 RVA: 0x00023C44 File Offset: 0x00021E44
		// (set) Token: 0x060009A8 RID: 2472 RVA: 0x00023C4C File Offset: 0x00021E4C
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

		// Token: 0x170002F6 RID: 758
		// (get) Token: 0x060009A9 RID: 2473 RVA: 0x00023C69 File Offset: 0x00021E69
		// (set) Token: 0x060009AA RID: 2474 RVA: 0x00023C71 File Offset: 0x00021E71
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

		// Token: 0x170002F7 RID: 759
		// (get) Token: 0x060009AB RID: 2475 RVA: 0x00023C8E File Offset: 0x00021E8E
		// (set) Token: 0x060009AC RID: 2476 RVA: 0x00023C96 File Offset: 0x00021E96
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

		// Token: 0x170002F8 RID: 760
		// (get) Token: 0x060009AD RID: 2477 RVA: 0x00023CB3 File Offset: 0x00021EB3
		// (set) Token: 0x060009AE RID: 2478 RVA: 0x00023CBB File Offset: 0x00021EBB
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

		// Token: 0x170002F9 RID: 761
		// (get) Token: 0x060009AF RID: 2479 RVA: 0x00023CDD File Offset: 0x00021EDD
		// (set) Token: 0x060009B0 RID: 2480 RVA: 0x00023CE5 File Offset: 0x00021EE5
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

		// Token: 0x170002FA RID: 762
		// (get) Token: 0x060009B1 RID: 2481 RVA: 0x00023D07 File Offset: 0x00021F07
		// (set) Token: 0x060009B2 RID: 2482 RVA: 0x00023D0F File Offset: 0x00021F0F
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

		// Token: 0x170002FB RID: 763
		// (get) Token: 0x060009B3 RID: 2483 RVA: 0x00023D31 File Offset: 0x00021F31
		// (set) Token: 0x060009B4 RID: 2484 RVA: 0x00023D39 File Offset: 0x00021F39
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

		// Token: 0x040004B8 RID: 1208
		private InputKeyItemVM _cancelInputKey;

		// Token: 0x040004B9 RID: 1209
		private InputKeyItemVM _doneInputKey;

		// Token: 0x040004BA RID: 1210
		private bool _isSelected;

		// Token: 0x040004BB RID: 1211
		private string _titleText;

		// Token: 0x040004BC RID: 1212
		private string _addText;

		// Token: 0x040004BD RID: 1213
		private string _errorText;

		// Token: 0x040004BE RID: 1214
		private string _bannerlordIDInputText;
	}
}
