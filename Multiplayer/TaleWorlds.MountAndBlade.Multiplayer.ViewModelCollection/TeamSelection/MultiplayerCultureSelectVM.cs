using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.TeamSelection
{
	public class MultiplayerCultureSelectVM : ViewModel
	{
		public MultiplayerCultureSelectVM(Action<BasicCultureObject> onCultureSelected, Action onClose)
		{
			this._onCultureSelected = onCultureSelected;
			this._onClose = onClose;
			this._firstCulture = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptionsExtensions.GetStrValue(14, 0));
			this._secondCulture = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptionsExtensions.GetStrValue(15, 0));
			this.FirstCultureCode = this._firstCulture.StringId;
			this.SecondCultureCode = this._secondCulture.StringId;
			this.FirstCultureColor1 = Color.FromUint(this._firstCulture.Color);
			this.FirstCultureColor2 = Color.FromUint(this._firstCulture.Color2);
			this.SecondCultureColor1 = Color.FromUint(this._secondCulture.Color);
			this.SecondCultureColor2 = Color.FromUint(this._secondCulture.Color2);
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.GameModeText = GameTexts.FindText("str_multiplayer_official_game_type_name", MultiplayerOptionsExtensions.GetStrValue(11, 0)).ToString();
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
					Debug.FailedAssert("Invalid Culture Index!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection\\TeamSelection\\MultiplayerCultureSelectVM.cs", "ExecuteSelectCulture", 62);
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

		[DataSourceProperty]
		public Color FirstCultureColor1
		{
			get
			{
				return this._firstCultureColor1;
			}
			set
			{
				if (value != this._firstCultureColor1)
				{
					this._firstCultureColor1 = value;
					base.OnPropertyChangedWithValue(value, "FirstCultureColor1");
				}
			}
		}

		[DataSourceProperty]
		public Color FirstCultureColor2
		{
			get
			{
				return this._firstCultureColor2;
			}
			set
			{
				if (value != this._firstCultureColor2)
				{
					this._firstCultureColor2 = value;
					base.OnPropertyChangedWithValue(value, "FirstCultureColor2");
				}
			}
		}

		[DataSourceProperty]
		public Color SecondCultureColor1
		{
			get
			{
				return this._secondCultureColor1;
			}
			set
			{
				if (value != this._secondCultureColor1)
				{
					this._secondCultureColor1 = value;
					base.OnPropertyChangedWithValue(value, "SecondCultureColor1");
				}
			}
		}

		[DataSourceProperty]
		public Color SecondCultureColor2
		{
			get
			{
				return this._secondCultureColor2;
			}
			set
			{
				if (value != this._secondCultureColor2)
				{
					this._secondCultureColor2 = value;
					base.OnPropertyChangedWithValue(value, "SecondCultureColor2");
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

		private Color _firstCultureColor1;

		private Color _firstCultureColor2;

		private Color _secondCultureColor1;

		private Color _secondCultureColor2;

		private string _firstCultureCode;

		private string _secondCultureCode;
	}
}
