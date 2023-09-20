using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.CustomGame;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.HostGame.HostGameOptions;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.HostGame
{
	public class MPHostGameOptionsVM : ViewModel
	{
		public MPHostGameOptionsVM(bool isInMission, MPCustomGameVM.CustomGameMode customGameMode = MPCustomGameVM.CustomGameMode.CustomServer)
		{
			this.IsInMission = isInMission;
			this.GeneralOptions = new MBBindingList<GenericHostGameOptionDataVM>();
			this._optionComparer = new MPHostGameOptionsVM.OptionPreferredIndexComparer();
			this._customGameMode = customGameMode;
			this.InitializeDefaultOptionList();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.GeneralOptions.ApplyActionOnAllItems(delegate(GenericHostGameOptionDataVM x)
			{
				x.RefreshValues();
			});
		}

		private void InitializeDefaultOptionList()
		{
			this.IsRefreshed = false;
			string text;
			if (this._customGameMode == MPCustomGameVM.CustomGameMode.CustomServer)
			{
				text = MultiplayerOptions.OptionType.GameType.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
			}
			else
			{
				text = "Skirmish";
				MultiplayerOptions.Instance.SetValueForOptionWithMultipleSelectionFromText(MultiplayerOptions.OptionType.PremadeMatchGameMode, text);
			}
			this.OnGameModeChanged(text);
			foreach (GenericHostGameOptionDataVM genericHostGameOptionDataVM in this.GeneralOptions.ToList<GenericHostGameOptionDataVM>())
			{
				if ((genericHostGameOptionDataVM.OptionType == MultiplayerOptions.OptionType.GameType || genericHostGameOptionDataVM.OptionType == MultiplayerOptions.OptionType.PremadeMatchGameMode) && genericHostGameOptionDataVM is MultipleSelectionHostGameOptionDataVM)
				{
					(genericHostGameOptionDataVM as MultipleSelectionHostGameOptionDataVM).OnChangedSelection = new Action<MultipleSelectionHostGameOptionDataVM>(this.OnChangeSelected);
				}
			}
			this.IsRefreshed = true;
		}

		private void OnChangeSelected(MultipleSelectionHostGameOptionDataVM option)
		{
			this.IsRefreshed = false;
			if (option.OptionType == MultiplayerOptions.OptionType.GameType || option.OptionType == MultiplayerOptions.OptionType.PremadeMatchGameMode)
			{
				this.OnGameModeChanged(MultiplayerOptions.Instance.GetMultiplayerOptionsList(MultiplayerOptions.OptionType.GameType)[option.Selector.SelectedIndex]);
			}
			this.IsRefreshed = true;
		}

		private void OnGameModeChanged(string gameModeName)
		{
			this._hostGameItemsForNextTick.Clear();
			if (this._customGameMode == MPCustomGameVM.CustomGameMode.CustomServer)
			{
				this.FillOptionsForCustomServer(gameModeName);
			}
			else if (this._customGameMode == MPCustomGameVM.CustomGameMode.PremadeGame)
			{
				this.FillOptionsForPremadeGame();
			}
			MultipleSelectionHostGameOptionDataVM multipleSelectionHostGameOptionDataVM = this.GeneralOptions.First((GenericHostGameOptionDataVM o) => o.OptionType == MultiplayerOptions.OptionType.Map) as MultipleSelectionHostGameOptionDataVM;
			if (multipleSelectionHostGameOptionDataVM != null)
			{
				multipleSelectionHostGameOptionDataVM.RefreshList();
			}
			this.GeneralOptions.Sort(this._optionComparer);
		}

		private void FillOptionsForCustomServer(string gameModeName)
		{
			for (MultiplayerOptions.OptionType optionType = MultiplayerOptions.OptionType.ServerName; optionType < MultiplayerOptions.OptionType.NumOfSlots; optionType++)
			{
				MultiplayerOptionsProperty optionProperty = optionType.GetOptionProperty();
				if (optionType != MultiplayerOptions.OptionType.PremadeMatchGameMode && optionType != MultiplayerOptions.OptionType.PremadeGameType && optionProperty != null)
				{
					int preferredIndex = (int)optionType;
					bool flag = optionProperty.ValidGameModes == null;
					if (optionProperty.ValidGameModes != null && optionProperty.ValidGameModes.Contains(gameModeName))
					{
						flag = true;
					}
					GenericHostGameOptionDataVM genericHostGameOptionDataVM = this.GeneralOptions.FirstOrDefault((GenericHostGameOptionDataVM o) => o.PreferredIndex == preferredIndex);
					if (flag)
					{
						if (genericHostGameOptionDataVM == null)
						{
							GenericHostGameOptionDataVM genericHostGameOptionDataVM2 = this.CreateOption(optionType, preferredIndex);
							this.GeneralOptions.Add(genericHostGameOptionDataVM2);
						}
						else
						{
							genericHostGameOptionDataVM.RefreshData();
						}
					}
					else if (genericHostGameOptionDataVM != null)
					{
						this.GeneralOptions.Remove(genericHostGameOptionDataVM);
					}
				}
			}
		}

		private void FillOptionsForPremadeGame()
		{
			for (MultiplayerOptions.OptionType optionType = MultiplayerOptions.OptionType.ServerName; optionType < MultiplayerOptions.OptionType.NumOfSlots; optionType++)
			{
				MultiplayerOptionsProperty optionProperty = optionType.GetOptionProperty();
				bool flag = false;
				if (optionType == MultiplayerOptions.OptionType.ServerName || optionType == MultiplayerOptions.OptionType.GamePassword || optionType == MultiplayerOptions.OptionType.CultureTeam1 || optionType == MultiplayerOptions.OptionType.CultureTeam2 || optionType == MultiplayerOptions.OptionType.Map || optionType == MultiplayerOptions.OptionType.PremadeMatchGameMode || optionType == MultiplayerOptions.OptionType.PremadeGameType)
				{
					flag = true;
				}
				if (flag && optionProperty != null)
				{
					int preferredIndex = (int)optionType;
					GenericHostGameOptionDataVM genericHostGameOptionDataVM = this.GeneralOptions.FirstOrDefault((GenericHostGameOptionDataVM o) => o.PreferredIndex == preferredIndex);
					if (genericHostGameOptionDataVM == null)
					{
						GenericHostGameOptionDataVM genericHostGameOptionDataVM2 = this.CreateOption(optionType, preferredIndex);
						this.GeneralOptions.Add(genericHostGameOptionDataVM2);
					}
					else
					{
						genericHostGameOptionDataVM.RefreshData();
					}
				}
			}
		}

		private GenericHostGameOptionDataVM CreateOption(MultiplayerOptions.OptionType type, int preferredIndex)
		{
			GenericHostGameOptionDataVM genericHostGameOptionDataVM = null;
			switch (this.GetSpecificHostGameOptionTypeOf(type))
			{
			case OptionsVM.OptionsDataType.BooleanOption:
				genericHostGameOptionDataVM = new BooleanHostGameOptionDataVM(type, preferredIndex);
				break;
			case OptionsVM.OptionsDataType.NumericOption:
				genericHostGameOptionDataVM = new NumericHostGameOptionDataVM(type, preferredIndex);
				break;
			case OptionsVM.OptionsDataType.MultipleSelectionOption:
				genericHostGameOptionDataVM = new MultipleSelectionHostGameOptionDataVM(type, preferredIndex);
				break;
			case OptionsVM.OptionsDataType.InputOption:
				genericHostGameOptionDataVM = new InputHostGameOptionDataVM(type, preferredIndex);
				break;
			}
			if (genericHostGameOptionDataVM == null)
			{
				Debug.FailedAssert("Item was not added to host game options because it has an invalid type.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\Multiplayer\\Lobby\\HostGame\\MPHostGameOptionsVM.cs", "CreateOption", 218);
				return null;
			}
			return genericHostGameOptionDataVM;
		}

		private OptionsVM.OptionsDataType GetSpecificHostGameOptionTypeOf(MultiplayerOptions.OptionType type)
		{
			MultiplayerOptionsProperty optionProperty = type.GetOptionProperty();
			switch (optionProperty.OptionValueType)
			{
			case MultiplayerOptions.OptionValueType.Bool:
				return OptionsVM.OptionsDataType.BooleanOption;
			case MultiplayerOptions.OptionValueType.Integer:
				return OptionsVM.OptionsDataType.NumericOption;
			case MultiplayerOptions.OptionValueType.Enum:
				return OptionsVM.OptionsDataType.MultipleSelectionOption;
			case MultiplayerOptions.OptionValueType.String:
				if (!optionProperty.HasMultipleSelections)
				{
					return OptionsVM.OptionsDataType.InputOption;
				}
				return OptionsVM.OptionsDataType.MultipleSelectionOption;
			default:
				return OptionsVM.OptionsDataType.None;
			}
		}

		[DataSourceProperty]
		public MBBindingList<GenericHostGameOptionDataVM> GeneralOptions
		{
			get
			{
				return this._generalOptions;
			}
			set
			{
				if (value != this._generalOptions)
				{
					this._generalOptions = value;
					base.OnPropertyChangedWithValue<MBBindingList<GenericHostGameOptionDataVM>>(value, "GeneralOptions");
				}
			}
		}

		[DataSourceProperty]
		public bool IsRefreshed
		{
			get
			{
				return this._isRefreshed;
			}
			set
			{
				if (value != this._isRefreshed)
				{
					this._isRefreshed = value;
					base.OnPropertyChangedWithValue(value, "IsRefreshed");
				}
			}
		}

		[DataSourceProperty]
		public bool IsInMission
		{
			get
			{
				return this._isInMission;
			}
			set
			{
				if (value != this._isInMission)
				{
					this._isInMission = value;
					base.OnPropertyChangedWithValue(value, "IsInMission");
				}
			}
		}

		private List<GenericHostGameOptionDataVM> _hostGameItemsForNextTick = new List<GenericHostGameOptionDataVM>();

		private MPHostGameOptionsVM.OptionPreferredIndexComparer _optionComparer;

		private MPCustomGameVM.CustomGameMode _customGameMode;

		private bool _isRefreshed;

		private bool _isInMission;

		private MBBindingList<GenericHostGameOptionDataVM> _generalOptions;

		private class OptionPreferredIndexComparer : IComparer<GenericHostGameOptionDataVM>
		{
			public int Compare(GenericHostGameOptionDataVM x, GenericHostGameOptionDataVM y)
			{
				return x.PreferredIndex.CompareTo(y.PreferredIndex);
			}
		}
	}
}
