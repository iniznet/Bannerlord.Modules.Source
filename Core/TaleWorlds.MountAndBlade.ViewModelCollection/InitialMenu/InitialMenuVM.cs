using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.InitialMenu
{
	public class InitialMenuVM : ViewModel
	{
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

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.MenuOptions.ApplyActionOnAllItems(delegate(InitialMenuOptionVM o)
			{
				o.RefreshValues();
			});
		}

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

		public override void OnFinalize()
		{
			base.OnFinalize();
		}

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

		private MBBindingList<InitialMenuOptionVM> _menuOptions;

		private bool _isProfileSelectionEnabled;

		private bool _isDownloadingContent;

		private string _selectProfileText;

		private string _profileName;

		private string _downloadingText;

		private string _gameVersionText;
	}
}
