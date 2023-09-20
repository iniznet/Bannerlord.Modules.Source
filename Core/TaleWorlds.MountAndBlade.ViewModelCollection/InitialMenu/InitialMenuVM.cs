using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.InitialMenu
{
	// Token: 0x020000D7 RID: 215
	public class InitialMenuVM : ViewModel
	{
		// Token: 0x060013EC RID: 5100 RVA: 0x0004174C File Offset: 0x0003F94C
		public InitialMenuVM(InitialState initialState)
		{
			this.SelectProfileText = new TextObject("{=wubDWOlh}Select Profile", null).ToString();
			this.DownloadingText = new TextObject("{=i4Oo6aoM}Downloading Content...", null).ToString();
			if (HotKeyManager.ShouldNotifyDocumentVersionDifferent())
			{
				MBInformationManager.AddQuickInformation(new TextObject("{=0Itt3bZM}Current keybind document version is outdated. Keybinds have been reverted to defaults.", null), 0, null, "");
			}
			this.GameVersionText = Utilities.GetApplicationVersionWithBuildNumber().ToString();
		}

		// Token: 0x060013ED RID: 5101 RVA: 0x000417C2 File Offset: 0x0003F9C2
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.MenuOptions.ApplyActionOnAllItems(delegate(InitialMenuOptionVM o)
			{
				o.RefreshValues();
			});
		}

		// Token: 0x060013EE RID: 5102 RVA: 0x000417F4 File Offset: 0x0003F9F4
		public void RefreshMenuOptions()
		{
			this.MenuOptions = new MBBindingList<InitialMenuOptionVM>();
			GameState activeState = GameStateManager.Current.ActiveState;
			foreach (InitialStateOption initialStateOption in Module.CurrentModule.GetInitialStateOptions())
			{
				this.MenuOptions.Add(new InitialMenuOptionVM(initialStateOption));
			}
			this.IsDownloadingContent = Utilities.IsOnlyCoreContentEnabled();
		}

		// Token: 0x060013EF RID: 5103 RVA: 0x00041870 File Offset: 0x0003FA70
		public override void OnFinalize()
		{
			base.OnFinalize();
		}

		// Token: 0x17000690 RID: 1680
		// (get) Token: 0x060013F0 RID: 5104 RVA: 0x00041878 File Offset: 0x0003FA78
		// (set) Token: 0x060013F1 RID: 5105 RVA: 0x00041880 File Offset: 0x0003FA80
		[DataSourceProperty]
		public MBBindingList<InitialMenuOptionVM> MenuOptions
		{
			get
			{
				return this._menuOptions;
			}
			set
			{
				if (value != this._menuOptions)
				{
					this._menuOptions = value;
					base.OnPropertyChangedWithValue<MBBindingList<InitialMenuOptionVM>>(value, "MenuOptions");
				}
			}
		}

		// Token: 0x17000691 RID: 1681
		// (get) Token: 0x060013F2 RID: 5106 RVA: 0x0004189E File Offset: 0x0003FA9E
		// (set) Token: 0x060013F3 RID: 5107 RVA: 0x000418A6 File Offset: 0x0003FAA6
		[DataSourceProperty]
		public string DownloadingText
		{
			get
			{
				return this._downloadingText;
			}
			set
			{
				if (value != this._downloadingText)
				{
					this._downloadingText = value;
					base.OnPropertyChangedWithValue<string>(value, "DownloadingText");
				}
			}
		}

		// Token: 0x17000692 RID: 1682
		// (get) Token: 0x060013F4 RID: 5108 RVA: 0x000418C9 File Offset: 0x0003FAC9
		// (set) Token: 0x060013F5 RID: 5109 RVA: 0x000418D1 File Offset: 0x0003FAD1
		[DataSourceProperty]
		public string SelectProfileText
		{
			get
			{
				return this._selectProfileText;
			}
			set
			{
				if (value != this._selectProfileText)
				{
					this._selectProfileText = value;
					base.OnPropertyChangedWithValue<string>(value, "SelectProfileText");
				}
			}
		}

		// Token: 0x17000693 RID: 1683
		// (get) Token: 0x060013F6 RID: 5110 RVA: 0x000418F4 File Offset: 0x0003FAF4
		// (set) Token: 0x060013F7 RID: 5111 RVA: 0x000418FC File Offset: 0x0003FAFC
		[DataSourceProperty]
		public string ProfileName
		{
			get
			{
				return this._profileName;
			}
			set
			{
				if (value != this._profileName)
				{
					this._profileName = value;
					base.OnPropertyChangedWithValue<string>(value, "ProfileName");
				}
			}
		}

		// Token: 0x17000694 RID: 1684
		// (get) Token: 0x060013F8 RID: 5112 RVA: 0x0004191F File Offset: 0x0003FB1F
		// (set) Token: 0x060013F9 RID: 5113 RVA: 0x00041927 File Offset: 0x0003FB27
		[DataSourceProperty]
		public string GameVersionText
		{
			get
			{
				return this._gameVersionText;
			}
			set
			{
				if (value != this._gameVersionText)
				{
					this._gameVersionText = value;
					base.OnPropertyChangedWithValue<string>(value, "GameVersionText");
				}
			}
		}

		// Token: 0x17000695 RID: 1685
		// (get) Token: 0x060013FA RID: 5114 RVA: 0x0004194A File Offset: 0x0003FB4A
		// (set) Token: 0x060013FB RID: 5115 RVA: 0x00041952 File Offset: 0x0003FB52
		[DataSourceProperty]
		public bool IsProfileSelectionEnabled
		{
			get
			{
				return this._isProfileSelectionEnabled;
			}
			set
			{
				if (value != this._isProfileSelectionEnabled)
				{
					this._isProfileSelectionEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsProfileSelectionEnabled");
				}
			}
		}

		// Token: 0x17000696 RID: 1686
		// (get) Token: 0x060013FC RID: 5116 RVA: 0x00041970 File Offset: 0x0003FB70
		// (set) Token: 0x060013FD RID: 5117 RVA: 0x00041978 File Offset: 0x0003FB78
		[DataSourceProperty]
		public bool IsDownloadingContent
		{
			get
			{
				return this._isDownloadingContent;
			}
			set
			{
				if (value != this._isDownloadingContent)
				{
					this._isDownloadingContent = value;
					base.OnPropertyChangedWithValue(value, "IsDownloadingContent");
				}
			}
		}

		// Token: 0x0400098E RID: 2446
		private MBBindingList<InitialMenuOptionVM> _menuOptions;

		// Token: 0x0400098F RID: 2447
		private bool _isProfileSelectionEnabled;

		// Token: 0x04000990 RID: 2448
		private bool _isDownloadingContent;

		// Token: 0x04000991 RID: 2449
		private string _selectProfileText;

		// Token: 0x04000992 RID: 2450
		private string _profileName;

		// Token: 0x04000993 RID: 2451
		private string _downloadingText;

		// Token: 0x04000994 RID: 2452
		private string _gameVersionText;
	}
}
