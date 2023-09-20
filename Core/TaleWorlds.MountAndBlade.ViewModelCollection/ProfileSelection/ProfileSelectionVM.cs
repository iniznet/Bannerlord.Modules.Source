using System;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;
using TaleWorlds.PlatformService;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.ProfileSelection
{
	public class ProfileSelectionVM : ViewModel
	{
		public ProfileSelectionVM(bool isDirectPlayPossible)
		{
			this.SelectProfileText = new TextObject("{=wubDWOlh}Select Profile", null).ToString();
			this.SelectProfileKey = InputKeyItemVM.CreateFromHotKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("SelectProfile"), false);
			this.PlayKey = InputKeyItemVM.CreateFromHotKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Play"), false);
		}

		public void OnActivate(bool isDirectPlayPossible)
		{
			this.IsPlayEnabled = isDirectPlayPossible;
			if (!string.IsNullOrEmpty(PlatformServices.Instance.UserDisplayName))
			{
				this.PlayText = new TextObject("{=FTXx0aRp}Play as", null).ToString() + PlatformServices.Instance.UserDisplayName;
				return;
			}
			this.PlayText = new TextObject("{=playgame}Play", null).ToString();
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			this.SelectProfileKey.OnFinalize();
			this.PlayKey.OnFinalize();
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
		public bool IsPlayEnabled
		{
			get
			{
				return this._isPlayEnabled;
			}
			set
			{
				if (value != this._isPlayEnabled)
				{
					this._isPlayEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsPlayEnabled");
				}
			}
		}

		[DataSourceProperty]
		public string PlayText
		{
			get
			{
				return this._playText;
			}
			set
			{
				if (value != this._playText)
				{
					this._playText = value;
					base.OnPropertyChangedWithValue<string>(value, "PlayText");
				}
			}
		}

		[DataSourceProperty]
		public InputKeyItemVM SelectProfileKey
		{
			get
			{
				return this._selectProfileKey;
			}
			set
			{
				if (value != this._selectProfileKey)
				{
					this._selectProfileKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "SelectProfileKey");
				}
			}
		}

		[DataSourceProperty]
		public InputKeyItemVM PlayKey
		{
			get
			{
				return this._playKey;
			}
			set
			{
				if (value != this._playKey)
				{
					this._playKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "PlayKey");
				}
			}
		}

		private bool _isPlayEnabled;

		private string _selectProfileText;

		private string _playText;

		private InputKeyItemVM _playKey;

		private InputKeyItemVM _selectProfileKey;
	}
}
