using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.CustomGame;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.HostGame.HostGameOptions;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.HostGame
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
				text = MultiplayerOptionsExtensions.GetStrValue(11, 0);
			}
			else
			{
				text = "Skirmish";
				MultiplayerOptions.Instance.SetValueForOptionWithMultipleSelectionFromText(10, text);
			}
			this.OnGameModeChanged(text);
			foreach (GenericHostGameOptionDataVM genericHostGameOptionDataVM in this.GeneralOptions.ToList<GenericHostGameOptionDataVM>())
			{
				if ((genericHostGameOptionDataVM.OptionType == 11 || genericHostGameOptionDataVM.OptionType == 10) && genericHostGameOptionDataVM is MultipleSelectionHostGameOptionDataVM)
				{
					(genericHostGameOptionDataVM as MultipleSelectionHostGameOptionDataVM).OnChangedSelection = new Action<MultipleSelectionHostGameOptionDataVM>(this.OnChangeSelected);
				}
			}
			this.IsRefreshed = true;
		}

		private void OnChangeSelected(MultipleSelectionHostGameOptionDataVM option)
		{
			this.IsRefreshed = false;
			if (option.OptionType == 11 || option.OptionType == 10)
			{
				this.OnGameModeChanged(MultiplayerOptions.Instance.GetMultiplayerOptionsList(11)[option.Selector.SelectedIndex]);
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
			MultipleSelectionHostGameOptionDataVM multipleSelectionHostGameOptionDataVM = this.GeneralOptions.First((GenericHostGameOptionDataVM o) => o.OptionType == 13) as MultipleSelectionHostGameOptionDataVM;
			if (multipleSelectionHostGameOptionDataVM != null)
			{
				multipleSelectionHostGameOptionDataVM.RefreshList();
			}
			this.GeneralOptions.Sort(this._optionComparer);
		}

		private void FillOptionsForCustomServer(string gameModeName)
		{
			for (MultiplayerOptions.OptionType optionType = 0; optionType < 43; optionType++)
			{
				MultiplayerOptionsProperty optionProperty = MultiplayerOptionsExtensions.GetOptionProperty(optionType);
				if (optionType != 10 && optionType != 12 && optionProperty != null)
				{
					int preferredIndex = optionType;
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
			for (MultiplayerOptions.OptionType optionType = 0; optionType < 43; optionType++)
			{
				MultiplayerOptionsProperty optionProperty = MultiplayerOptionsExtensions.GetOptionProperty(optionType);
				bool flag = false;
				if (optionType == null || optionType == 2 || optionType == 14 || optionType == 15 || optionType == 13 || optionType == 10 || optionType == 12)
				{
					flag = true;
				}
				if (flag && optionProperty != null)
				{
					int preferredIndex = optionType;
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
			case 0:
				genericHostGameOptionDataVM = new BooleanHostGameOptionDataVM(type, preferredIndex);
				break;
			case 1:
				genericHostGameOptionDataVM = new NumericHostGameOptionDataVM(type, preferredIndex);
				break;
			case 3:
				genericHostGameOptionDataVM = new MultipleSelectionHostGameOptionDataVM(type, preferredIndex);
				break;
			case 4:
				genericHostGameOptionDataVM = new InputHostGameOptionDataVM(type, preferredIndex);
				break;
			}
			if (genericHostGameOptionDataVM == null)
			{
				Debug.FailedAssert("Item was not added to host game options because it has an invalid type.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection\\Lobby\\HostGame\\MPHostGameOptionsVM.cs", "CreateOption", 218);
				return null;
			}
			return genericHostGameOptionDataVM;
		}

		private OptionsVM.OptionsDataType GetSpecificHostGameOptionTypeOf(MultiplayerOptions.OptionType type)
		{
			MultiplayerOptionsProperty optionProperty = MultiplayerOptionsExtensions.GetOptionProperty(type);
			switch (optionProperty.OptionValueType)
			{
			case 0:
				return 0;
			case 1:
				return 1;
			case 2:
				return 3;
			case 3:
				if (!optionProperty.HasMultipleSelections)
				{
					return 4;
				}
				return 3;
			default:
				return -1;
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
