using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.TeamSelection
{
	public class MultiplayerCultureSelectVM : ViewModel
	{
		public MultiplayerCultureSelectVM(Action<BasicCultureObject> onCultureSelected, Action onClose)
		{
			this._onCultureSelected = onCultureSelected;
			this._onClose = onClose;
			this._firstCulture = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
			this._secondCulture = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam2.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
			this.FirstCultureCode = this._firstCulture.StringId;
			this.SecondCultureCode = this._secondCulture.StringId;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.GameModeText = GameTexts.FindText("str_multiplayer_official_game_type_name", MultiplayerOptions.OptionType.GameType.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)).ToString();
			this.CultureSelectionText = new TextObject("{=yQ0p8Glo}Select Culture", null).ToString();
			this.FirstCultureName = this._firstCulture.Name.ToString();
			this.SecondCultureName = this._secondCulture.Name.ToString();
		}

		public void ExecuteSelectCulture(int cultureIndex)
		{
			if (cultureIndex == 0)
			{
				Action<BasicCultureObject> onCultureSelected = this._onCultureSelected;
				if (onCultureSelected == null)
				{
					return;
				}
				onCultureSelected(this._firstCulture);
				return;
			}
			else
			{
				if (cultureIndex != 1)
				{
					Debug.FailedAssert("Invalid Culture Index!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\Multiplayer\\TeamSelection\\MultiplayerCultureSelectVM.cs", "ExecuteSelectCulture", 56);
					return;
				}
				Action<BasicCultureObject> onCultureSelected2 = this._onCultureSelected;
				if (onCultureSelected2 == null)
				{
					return;
				}
				onCultureSelected2(this._secondCulture);
				return;
			}
		}

		public void ExecuteClose()
		{
			Action onClose = this._onClose;
			if (onClose == null)
			{
				return;
			}
			onClose();
		}

		[DataSourceProperty]
		public string GameModeText
		{
			get
			{
				return this._gameModeText;
			}
			set
			{
				if (value != this._gameModeText)
				{
					this._gameModeText = value;
					base.OnPropertyChangedWithValue<string>(value, "GameModeText");
				}
			}
		}

		[DataSourceProperty]
		public string CultureSelectionText
		{
			get
			{
				return this._cultureSelectionText;
			}
			set
			{
				if (value != this._cultureSelectionText)
				{
					this._cultureSelectionText = value;
					base.OnPropertyChangedWithValue<string>(value, "CultureSelectionText");
				}
			}
		}

		[DataSourceProperty]
		public string FirstCultureName
		{
			get
			{
				return this._firstCultureName;
			}
			set
			{
				if (value != this._firstCultureName)
				{
					this._firstCultureName = value;
					base.OnPropertyChangedWithValue<string>(value, "FirstCultureName");
				}
			}
		}

		[DataSourceProperty]
		public string SecondCultureName
		{
			get
			{
				return this._secondCultureName;
			}
			set
			{
				if (value != this._secondCultureName)
				{
					this._secondCultureName = value;
					base.OnPropertyChangedWithValue<string>(value, "SecondCultureName");
				}
			}
		}

		[DataSourceProperty]
		public string FirstCultureCode
		{
			get
			{
				return this._firstCultureCode;
			}
			set
			{
				if (value != this._firstCultureCode)
				{
					this._firstCultureCode = value;
					base.OnPropertyChangedWithValue<string>(value, "FirstCultureCode");
				}
			}
		}

		[DataSourceProperty]
		public string SecondCultureCode
		{
			get
			{
				return this._secondCultureCode;
			}
			set
			{
				if (value != this._secondCultureCode)
				{
					this._secondCultureCode = value;
					base.OnPropertyChangedWithValue<string>(value, "SecondCultureCode");
				}
			}
		}

		private BasicCultureObject _firstCulture;

		private BasicCultureObject _secondCulture;

		private Action<BasicCultureObject> _onCultureSelected;

		private Action _onClose;

		private string _gameModeText;

		private string _cultureSelectionText;

		private string _firstCultureName;

		private string _secondCultureName;

		private string _firstCultureCode;

		private string _secondCultureCode;
	}
}
