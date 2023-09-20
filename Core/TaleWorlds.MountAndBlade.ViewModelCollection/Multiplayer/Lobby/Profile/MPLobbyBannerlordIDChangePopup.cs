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
	// Token: 0x02000069 RID: 105
	public class MPLobbyBannerlordIDChangePopup : ViewModel
	{
		// Token: 0x060009B5 RID: 2485 RVA: 0x00023D66 File Offset: 0x00021F66
		public MPLobbyBannerlordIDChangePopup()
		{
			this.BannerlordIDInputText = "";
			this.RefreshValues();
		}

		// Token: 0x060009B6 RID: 2486 RVA: 0x00023D80 File Offset: 0x00021F80
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

		// Token: 0x060009B7 RID: 2487 RVA: 0x00023E0C File Offset: 0x0002200C
		public void ExecuteOpenPopup()
		{
			this.IsSelected = true;
			this.HasRequestSent = false;
		}

		// Token: 0x060009B8 RID: 2488 RVA: 0x00023E1C File Offset: 0x0002201C
		public void ExecuteClosePopup()
		{
			this.IsSelected = false;
			this.BannerlordIDInputText = "";
			this.ErrorText = "";
		}

		// Token: 0x060009B9 RID: 2489 RVA: 0x00023E3C File Offset: 0x0002203C
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

		// Token: 0x060009BA RID: 2490 RVA: 0x00023E84 File Offset: 0x00022084
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

		// Token: 0x060009BB RID: 2491 RVA: 0x00023EBD File Offset: 0x000220BD
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

		// Token: 0x060009BC RID: 2492 RVA: 0x00023EE6 File Offset: 0x000220E6
		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x060009BD RID: 2493 RVA: 0x00023EF5 File Offset: 0x000220F5
		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x170002FC RID: 764
		// (get) Token: 0x060009BE RID: 2494 RVA: 0x00023F04 File Offset: 0x00022104
		// (set) Token: 0x060009BF RID: 2495 RVA: 0x00023F0C File Offset: 0x0002210C
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

		// Token: 0x170002FD RID: 765
		// (get) Token: 0x060009C0 RID: 2496 RVA: 0x00023F29 File Offset: 0x00022129
		// (set) Token: 0x060009C1 RID: 2497 RVA: 0x00023F31 File Offset: 0x00022131
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

		// Token: 0x170002FE RID: 766
		// (get) Token: 0x060009C2 RID: 2498 RVA: 0x00023F4E File Offset: 0x0002214E
		// (set) Token: 0x060009C3 RID: 2499 RVA: 0x00023F56 File Offset: 0x00022156
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

		// Token: 0x170002FF RID: 767
		// (get) Token: 0x060009C4 RID: 2500 RVA: 0x00023F73 File Offset: 0x00022173
		// (set) Token: 0x060009C5 RID: 2501 RVA: 0x00023F7B File Offset: 0x0002217B
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

		// Token: 0x17000300 RID: 768
		// (get) Token: 0x060009C6 RID: 2502 RVA: 0x00023FA8 File Offset: 0x000221A8
		// (set) Token: 0x060009C7 RID: 2503 RVA: 0x00023FB0 File Offset: 0x000221B0
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

		// Token: 0x17000301 RID: 769
		// (get) Token: 0x060009C8 RID: 2504 RVA: 0x00023FD2 File Offset: 0x000221D2
		// (set) Token: 0x060009C9 RID: 2505 RVA: 0x00023FDA File Offset: 0x000221DA
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

		// Token: 0x17000302 RID: 770
		// (get) Token: 0x060009CA RID: 2506 RVA: 0x00023FFC File Offset: 0x000221FC
		// (set) Token: 0x060009CB RID: 2507 RVA: 0x00024004 File Offset: 0x00022204
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

		// Token: 0x17000303 RID: 771
		// (get) Token: 0x060009CC RID: 2508 RVA: 0x00024026 File Offset: 0x00022226
		// (set) Token: 0x060009CD RID: 2509 RVA: 0x0002402E File Offset: 0x0002222E
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

		// Token: 0x17000304 RID: 772
		// (get) Token: 0x060009CE RID: 2510 RVA: 0x0002404B File Offset: 0x0002224B
		// (set) Token: 0x060009CF RID: 2511 RVA: 0x00024053 File Offset: 0x00022253
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

		// Token: 0x17000305 RID: 773
		// (get) Token: 0x060009D0 RID: 2512 RVA: 0x00024075 File Offset: 0x00022275
		// (set) Token: 0x060009D1 RID: 2513 RVA: 0x0002407D File Offset: 0x0002227D
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

		// Token: 0x17000306 RID: 774
		// (get) Token: 0x060009D2 RID: 2514 RVA: 0x0002409F File Offset: 0x0002229F
		// (set) Token: 0x060009D3 RID: 2515 RVA: 0x000240A7 File Offset: 0x000222A7
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

		// Token: 0x040004BF RID: 1215
		private InputKeyItemVM _cancelInputKey;

		// Token: 0x040004C0 RID: 1216
		private InputKeyItemVM _doneInputKey;

		// Token: 0x040004C1 RID: 1217
		private bool _isSelected;

		// Token: 0x040004C2 RID: 1218
		private bool _hasRequestSent;

		// Token: 0x040004C3 RID: 1219
		private string _bannerlordIDInputText;

		// Token: 0x040004C4 RID: 1220
		private string _changeBannerlordIDText;

		// Token: 0x040004C5 RID: 1221
		private string _typeYourNameText;

		// Token: 0x040004C6 RID: 1222
		private string _requestSentText;

		// Token: 0x040004C7 RID: 1223
		private string _errorText;

		// Token: 0x040004C8 RID: 1224
		private string _cancelText;

		// Token: 0x040004C9 RID: 1225
		private string _doneText;
	}
}
