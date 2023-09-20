using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.ClassLoadout
{
	// Token: 0x020000C9 RID: 201
	public class HeroClassVM : ViewModel
	{
		// Token: 0x17000621 RID: 1569
		// (get) Token: 0x060012BB RID: 4795 RVA: 0x0003D95F File Offset: 0x0003BB5F
		// (set) Token: 0x060012BC RID: 4796 RVA: 0x0003D967 File Offset: 0x0003BB67
		public List<IReadOnlyPerkObject> SelectedPerks { get; private set; }

		// Token: 0x060012BD RID: 4797 RVA: 0x0003D970 File Offset: 0x0003BB70
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

		// Token: 0x060012BE RID: 4798 RVA: 0x0003DAA8 File Offset: 0x0003BCA8
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this.HeroClass.HeroName.ToString();
			this.Perks.ApplyActionOnAllItems(delegate(HeroPerkVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x060012BF RID: 4799 RVA: 0x0003DAFC File Offset: 0x0003BCFC
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

		// Token: 0x060012C0 RID: 4800 RVA: 0x0003DC6C File Offset: 0x0003BE6C
		public void UpdateEnabled()
		{
			this.IsEnabled = this._gameMode.IsInWarmup || !this._gameMode.IsGameModeUsingGold || this._gameMode.GetGoldAmount() >= this.Gold;
		}

		// Token: 0x060012C1 RID: 4801 RVA: 0x0003DCA7 File Offset: 0x0003BEA7
		[UsedImplicitly]
		public void OnSelect()
		{
			this._onSelect(this);
		}

		// Token: 0x17000622 RID: 1570
		// (get) Token: 0x060012C2 RID: 4802 RVA: 0x0003DCB5 File Offset: 0x0003BEB5
		// (set) Token: 0x060012C3 RID: 4803 RVA: 0x0003DCBD File Offset: 0x0003BEBD
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

		// Token: 0x17000623 RID: 1571
		// (get) Token: 0x060012C4 RID: 4804 RVA: 0x0003DCDB File Offset: 0x0003BEDB
		// (set) Token: 0x060012C5 RID: 4805 RVA: 0x0003DCE3 File Offset: 0x0003BEE3
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

		// Token: 0x17000624 RID: 1572
		// (get) Token: 0x060012C6 RID: 4806 RVA: 0x0003DD01 File Offset: 0x0003BF01
		// (set) Token: 0x060012C7 RID: 4807 RVA: 0x0003DD09 File Offset: 0x0003BF09
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

		// Token: 0x17000625 RID: 1573
		// (get) Token: 0x060012C8 RID: 4808 RVA: 0x0003DD27 File Offset: 0x0003BF27
		// (set) Token: 0x060012C9 RID: 4809 RVA: 0x0003DD2F File Offset: 0x0003BF2F
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

		// Token: 0x17000626 RID: 1574
		// (get) Token: 0x060012CA RID: 4810 RVA: 0x0003DD52 File Offset: 0x0003BF52
		// (set) Token: 0x060012CB RID: 4811 RVA: 0x0003DD5A File Offset: 0x0003BF5A
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

		// Token: 0x17000627 RID: 1575
		// (get) Token: 0x060012CC RID: 4812 RVA: 0x0003DD7D File Offset: 0x0003BF7D
		// (set) Token: 0x060012CD RID: 4813 RVA: 0x0003DD85 File Offset: 0x0003BF85
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

		// Token: 0x17000628 RID: 1576
		// (get) Token: 0x060012CE RID: 4814 RVA: 0x0003DDA3 File Offset: 0x0003BFA3
		// (set) Token: 0x060012CF RID: 4815 RVA: 0x0003DDAB File Offset: 0x0003BFAB
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

		// Token: 0x17000629 RID: 1577
		// (get) Token: 0x060012D0 RID: 4816 RVA: 0x0003DDCE File Offset: 0x0003BFCE
		// (set) Token: 0x060012D1 RID: 4817 RVA: 0x0003DDD6 File Offset: 0x0003BFD6
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

		// Token: 0x1700062A RID: 1578
		// (get) Token: 0x060012D2 RID: 4818 RVA: 0x0003DDF9 File Offset: 0x0003BFF9
		// (set) Token: 0x060012D3 RID: 4819 RVA: 0x0003DE01 File Offset: 0x0003C001
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

		// Token: 0x1700062B RID: 1579
		// (get) Token: 0x060012D4 RID: 4820 RVA: 0x0003DE1F File Offset: 0x0003C01F
		// (set) Token: 0x060012D5 RID: 4821 RVA: 0x0003DE27 File Offset: 0x0003C027
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

		// Token: 0x1700062C RID: 1580
		// (get) Token: 0x060012D6 RID: 4822 RVA: 0x0003DE45 File Offset: 0x0003C045
		// (set) Token: 0x060012D7 RID: 4823 RVA: 0x0003DE4D File Offset: 0x0003C04D
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

		// Token: 0x1700062D RID: 1581
		// (get) Token: 0x060012D8 RID: 4824 RVA: 0x0003DE6B File Offset: 0x0003C06B
		// (set) Token: 0x060012D9 RID: 4825 RVA: 0x0003DE73 File Offset: 0x0003C073
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

		// Token: 0x1700062E RID: 1582
		// (get) Token: 0x060012DA RID: 4826 RVA: 0x0003DE91 File Offset: 0x0003C091
		[DataSourceProperty]
		public HeroPerkVM FirstPerk
		{
			get
			{
				return this.Perks.ElementAtOrDefault(0);
			}
		}

		// Token: 0x1700062F RID: 1583
		// (get) Token: 0x060012DB RID: 4827 RVA: 0x0003DE9F File Offset: 0x0003C09F
		[DataSourceProperty]
		public HeroPerkVM SecondPerk
		{
			get
			{
				return this.Perks.ElementAtOrDefault(1);
			}
		}

		// Token: 0x17000630 RID: 1584
		// (get) Token: 0x060012DC RID: 4828 RVA: 0x0003DEAD File Offset: 0x0003C0AD
		[DataSourceProperty]
		public HeroPerkVM ThirdPerk
		{
			get
			{
				return this.Perks.ElementAtOrDefault(2);
			}
		}

		// Token: 0x040008FD RID: 2301
		private readonly MissionMultiplayerGameModeBaseClient _gameMode;

		// Token: 0x040008FE RID: 2302
		public readonly MultiplayerClassDivisions.MPHeroClass HeroClass;

		// Token: 0x040008FF RID: 2303
		private readonly Action<HeroClassVM> _onSelect;

		// Token: 0x04000900 RID: 2304
		private Action<HeroPerkVM, MPPerkVM> _onPerkSelect;

		// Token: 0x04000902 RID: 2306
		private bool _isSelected;

		// Token: 0x04000903 RID: 2307
		private string _name;

		// Token: 0x04000904 RID: 2308
		private string _iconType;

		// Token: 0x04000905 RID: 2309
		private int _gold;

		// Token: 0x04000906 RID: 2310
		private int _numOfTroops;

		// Token: 0x04000907 RID: 2311
		private bool _isEnabled;

		// Token: 0x04000908 RID: 2312
		private bool _isGoldEnabled;

		// Token: 0x04000909 RID: 2313
		private bool _isNumOfTroopsEnabled;

		// Token: 0x0400090A RID: 2314
		private bool _useSecondary;

		// Token: 0x0400090B RID: 2315
		private string _cultureId;

		// Token: 0x0400090C RID: 2316
		private string _troopTypeId;

		// Token: 0x0400090D RID: 2317
		private MBBindingList<HeroPerkVM> _perks;
	}
}
