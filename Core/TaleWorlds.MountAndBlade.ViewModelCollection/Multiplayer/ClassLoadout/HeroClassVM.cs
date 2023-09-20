using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.ClassLoadout
{
	public class HeroClassVM : ViewModel
	{
		public List<IReadOnlyPerkObject> SelectedPerks { get; private set; }

		public HeroClassVM(Action<HeroClassVM> onSelect, Action<HeroPerkVM, MPPerkVM> onPerkSelect, MultiplayerClassDivisions.MPHeroClass heroClass, bool useSecondary)
		{
			this.HeroClass = heroClass;
			this._onSelect = onSelect;
			this._onPerkSelect = onPerkSelect;
			this.CultureId = heroClass.Culture.StringId;
			this.IconType = heroClass.IconType.ToString();
			this.TroopTypeId = heroClass.ClassGroup.StringId;
			this.UseSecondary = useSecondary;
			this._gameMode = Mission.Current.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
			this.Gold = (this._gameMode.IsGameModeUsingCasualGold ? this.HeroClass.TroopCasualCost : ((this._gameMode.GameType == MissionLobbyComponent.MultiplayerGameType.Battle) ? this.HeroClass.TroopBattleCost : this.HeroClass.TroopCost));
			this.InitPerksList();
			this.IsNumOfTroopsEnabled = !this._gameMode.IsInWarmup && MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) > 0;
			if (this.IsNumOfTroopsEnabled)
			{
				this.NumOfTroops = MPPerkObject.GetTroopCount(heroClass, MPPerkObject.GetOnSpawnPerkHandler(this._perks.Select((HeroPerkVM p) => p.SelectedPerk)));
			}
			this.UpdateEnabled();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this.HeroClass.HeroName.ToString();
			this.Perks.ApplyActionOnAllItems(delegate(HeroPerkVM x)
			{
				x.RefreshValues();
			});
		}

		private void InitPerksList()
		{
			List<List<IReadOnlyPerkObject>> allPerksForHeroClass = MultiplayerClassDivisions.GetAllPerksForHeroClass(this.HeroClass, null);
			if (this.SelectedPerks == null)
			{
				this.SelectedPerks = new List<IReadOnlyPerkObject>();
			}
			else
			{
				this.SelectedPerks.Clear();
			}
			for (int i = 0; i < allPerksForHeroClass.Count; i++)
			{
				if (allPerksForHeroClass[i].Count > 0)
				{
					this.SelectedPerks.Add(allPerksForHeroClass[i][0]);
				}
			}
			if (GameNetwork.IsMyPeerReady)
			{
				MissionPeer component = GameNetwork.MyPeer.GetComponent<MissionPeer>();
				int num = MultiplayerClassDivisions.GetMPHeroClasses(this.HeroClass.Culture).ToList<MultiplayerClassDivisions.MPHeroClass>().IndexOf(this.HeroClass);
				component.NextSelectedTroopIndex = num;
				for (int j = 0; j < allPerksForHeroClass.Count; j++)
				{
					if (allPerksForHeroClass[j].Count > 0)
					{
						int num2 = component.GetSelectedPerkIndexWithPerkListIndex(num, j);
						if (num2 >= allPerksForHeroClass[j].Count)
						{
							num2 = 0;
						}
						IReadOnlyPerkObject readOnlyPerkObject = allPerksForHeroClass[j][num2];
						this.SelectedPerks[j] = readOnlyPerkObject;
					}
				}
			}
			MBBindingList<HeroPerkVM> mbbindingList = new MBBindingList<HeroPerkVM>();
			for (int k = 0; k < allPerksForHeroClass.Count; k++)
			{
				if (allPerksForHeroClass[k].Count > 0)
				{
					mbbindingList.Add(new HeroPerkVM(this._onPerkSelect, this.SelectedPerks[k], allPerksForHeroClass[k], k));
				}
			}
			this.Perks = mbbindingList;
		}

		public void UpdateEnabled()
		{
			this.IsEnabled = this._gameMode.IsInWarmup || !this._gameMode.IsGameModeUsingGold || this._gameMode.GetGoldAmount() >= this.Gold;
		}

		[UsedImplicitly]
		public void OnSelect()
		{
			this._onSelect(this);
		}

		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		[DataSourceProperty]
		public bool UseSecondary
		{
			get
			{
				return this._useSecondary;
			}
			set
			{
				if (value != this._useSecondary)
				{
					this._useSecondary = value;
					base.OnPropertyChangedWithValue(value, "UseSecondary");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<HeroPerkVM> Perks
		{
			get
			{
				return this._perks;
			}
			set
			{
				if (value != this._perks)
				{
					this._perks = value;
					base.OnPropertyChangedWithValue<MBBindingList<HeroPerkVM>>(value, "Perks");
				}
			}
		}

		[DataSourceProperty]
		public string CultureId
		{
			get
			{
				return this._cultureId;
			}
			set
			{
				if (value != this._cultureId)
				{
					this._cultureId = value;
					base.OnPropertyChangedWithValue<string>(value, "CultureId");
				}
			}
		}

		[DataSourceProperty]
		public string TroopTypeId
		{
			get
			{
				return this._troopTypeId;
			}
			set
			{
				if (value != this._troopTypeId)
				{
					this._troopTypeId = value;
					base.OnPropertyChangedWithValue<string>(value, "TroopTypeId");
				}
			}
		}

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
					base.OnPropertyChangedWithValue(value, "IsSelected");
				}
			}
		}

		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		[DataSourceProperty]
		public string IconType
		{
			get
			{
				return this._iconType;
			}
			set
			{
				if (value != this._iconType)
				{
					this._iconType = value;
					base.OnPropertyChangedWithValue<string>(value, "IconType");
				}
			}
		}

		[DataSourceProperty]
		public int Gold
		{
			get
			{
				return this._gold;
			}
			set
			{
				if (value != this._gold)
				{
					this._gold = value;
					base.OnPropertyChangedWithValue(value, "Gold");
				}
			}
		}

		[DataSourceProperty]
		public int NumOfTroops
		{
			get
			{
				return this._numOfTroops;
			}
			set
			{
				if (value != this._numOfTroops)
				{
					this._numOfTroops = value;
					base.OnPropertyChangedWithValue(value, "NumOfTroops");
				}
			}
		}

		[DataSourceProperty]
		public bool IsGoldEnabled
		{
			get
			{
				return this._isGoldEnabled;
			}
			set
			{
				if (value != this._isGoldEnabled)
				{
					this._isGoldEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsGoldEnabled");
				}
			}
		}

		[DataSourceProperty]
		public bool IsNumOfTroopsEnabled
		{
			get
			{
				return this._isNumOfTroopsEnabled;
			}
			set
			{
				if (value != this._isNumOfTroopsEnabled)
				{
					this._isNumOfTroopsEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsNumOfTroopsEnabled");
				}
			}
		}

		[DataSourceProperty]
		public HeroPerkVM FirstPerk
		{
			get
			{
				return this.Perks.ElementAtOrDefault(0);
			}
		}

		[DataSourceProperty]
		public HeroPerkVM SecondPerk
		{
			get
			{
				return this.Perks.ElementAtOrDefault(1);
			}
		}

		[DataSourceProperty]
		public HeroPerkVM ThirdPerk
		{
			get
			{
				return this.Perks.ElementAtOrDefault(2);
			}
		}

		private readonly MissionMultiplayerGameModeBaseClient _gameMode;

		public readonly MultiplayerClassDivisions.MPHeroClass HeroClass;

		private readonly Action<HeroClassVM> _onSelect;

		private Action<HeroPerkVM, MPPerkVM> _onPerkSelect;

		private bool _isSelected;

		private string _name;

		private string _iconType;

		private int _gold;

		private int _numOfTroops;

		private bool _isEnabled;

		private bool _isGoldEnabled;

		private bool _isNumOfTroopsEnabled;

		private bool _useSecondary;

		private string _cultureId;

		private string _troopTypeId;

		private MBBindingList<HeroPerkVM> _perks;
	}
}
