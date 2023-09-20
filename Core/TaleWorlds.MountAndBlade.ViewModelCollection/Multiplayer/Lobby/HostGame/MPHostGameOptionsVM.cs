using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.CustomGame;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.HostGame.HostGameOptions;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.HostGame
{
	// Token: 0x02000075 RID: 117
	public class MPHostGameOptionsVM : ViewModel
	{
		// Token: 0x06000AB0 RID: 2736 RVA: 0x000263F1 File Offset: 0x000245F1
		public MPHostGameOptionsVM(bool isInMission, MPCustomGameVM.CustomGameMode customGameMode = MPCustomGameVM.CustomGameMode.CustomServer)
		{
			this.IsInMission = isInMission;
			this.GeneralOptions = new MBBindingList<GenericHostGameOptionDataVM>();
			this._optionComparer = new MPHostGameOptionsVM.OptionPreferredIndexComparer();
			this._customGameMode = customGameMode;
			this.InitializeDefaultOptionList();
		}

		// Token: 0x06000AB1 RID: 2737 RVA: 0x0002642E File Offset: 0x0002462E
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.GeneralOptions.ApplyActionOnAllItems(delegate(GenericHostGameOptionDataVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x06000AB2 RID: 2738 RVA: 0x00026460 File Offset: 0x00024660
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

		// Token: 0x06000AB3 RID: 2739 RVA: 0x00026528 File Offset: 0x00024728
		private void OnChangeSelected(MultipleSelectionHostGameOptionDataVM option)
		{
			this.IsRefreshed = false;
			if (option.OptionType == MultiplayerOptions.OptionType.GameType || option.OptionType == MultiplayerOptions.OptionType.PremadeMatchGameMode)
			{
				this.OnGameModeChanged(MultiplayerOptions.Instance.GetMultiplayerOptionsList(MultiplayerOptions.OptionType.GameType)[option.Selector.SelectedIndex]);
			}
			this.IsRefreshed = true;
		}

		// Token: 0x06000AB4 RID: 2740 RVA: 0x0002657C File Offset: 0x0002477C
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

		// Token: 0x06000AB5 RID: 2741 RVA: 0x00026600 File Offset: 0x00024800
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

		// Token: 0x06000AB6 RID: 2742 RVA: 0x000266C8 File Offset: 0x000248C8
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

		// Token: 0x06000AB7 RID: 2743 RVA: 0x00026768 File Offset: 0x00024968
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

		// Token: 0x06000AB8 RID: 2744 RVA: 0x000267E0 File Offset: 0x000249E0
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

		// Token: 0x17000353 RID: 851
		// (get) Token: 0x06000AB9 RID: 2745 RVA: 0x00026826 File Offset: 0x00024A26
		// (set) Token: 0x06000ABA RID: 2746 RVA: 0x0002682E File Offset: 0x00024A2E
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

		// Token: 0x17000354 RID: 852
		// (get) Token: 0x06000ABB RID: 2747 RVA: 0x0002684C File Offset: 0x00024A4C
		// (set) Token: 0x06000ABC RID: 2748 RVA: 0x00026854 File Offset: 0x00024A54
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

		// Token: 0x17000355 RID: 853
		// (get) Token: 0x06000ABD RID: 2749 RVA: 0x00026872 File Offset: 0x00024A72
		// (set) Token: 0x06000ABE RID: 2750 RVA: 0x0002687A File Offset: 0x00024A7A
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

		// Token: 0x04000530 RID: 1328
		private List<GenericHostGameOptionDataVM> _hostGameItemsForNextTick = new List<GenericHostGameOptionDataVM>();

		// Token: 0x04000531 RID: 1329
		private MPHostGameOptionsVM.OptionPreferredIndexComparer _optionComparer;

		// Token: 0x04000532 RID: 1330
		private MPCustomGameVM.CustomGameMode _customGameMode;

		// Token: 0x04000533 RID: 1331
		private bool _isRefreshed;

		// Token: 0x04000534 RID: 1332
		private bool _isInMission;

		// Token: 0x04000535 RID: 1333
		private MBBindingList<GenericHostGameOptionDataVM> _generalOptions;

		// Token: 0x0200019F RID: 415
		private class OptionPreferredIndexComparer : IComparer<GenericHostGameOptionDataVM>
		{
			// Token: 0x060019C6 RID: 6598 RVA: 0x00053500 File Offset: 0x00051700
			public int Compare(GenericHostGameOptionDataVM x, GenericHostGameOptionDataVM y)
			{
				return x.PreferredIndex.CompareTo(y.PreferredIndex);
			}
		}
	}
}
