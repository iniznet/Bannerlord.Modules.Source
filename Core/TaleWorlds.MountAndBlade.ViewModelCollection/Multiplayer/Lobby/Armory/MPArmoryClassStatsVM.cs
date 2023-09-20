using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.ClassLoadout;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Armory
{
	// Token: 0x020000A4 RID: 164
	public class MPArmoryClassStatsVM : ViewModel
	{
		// Token: 0x06000FB1 RID: 4017 RVA: 0x00034080 File Offset: 0x00032280
		public MPArmoryClassStatsVM()
		{
			this._dummyPerkList = new List<IReadOnlyPerkObject>();
			this.FactionDescription = new TextObject("{=5Pea977J}Faction: ", null).ToString();
			this.HeroInformation = new HeroInformationVM();
		}

		// Token: 0x06000FB2 RID: 4018 RVA: 0x000340B4 File Offset: 0x000322B4
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.CostHint = new HintViewModel(GameTexts.FindText("str_armory_troop_cost", null), null);
			this.HeroInformation.RefreshValues();
		}

		// Token: 0x06000FB3 RID: 4019 RVA: 0x000340E0 File Offset: 0x000322E0
		public void RefreshWith(MultiplayerClassDivisions.MPHeroClass heroClass)
		{
			this.FactionName = heroClass.Culture.Name.ToString();
			this.FlavorText = GameTexts.FindText("str_troop_description", heroClass.StringId).ToString();
			this.HeroInformation.RefreshWith(heroClass, this._dummyPerkList);
			this.Cost = heroClass.TroopCost;
		}

		// Token: 0x17000508 RID: 1288
		// (get) Token: 0x06000FB4 RID: 4020 RVA: 0x0003413C File Offset: 0x0003233C
		// (set) Token: 0x06000FB5 RID: 4021 RVA: 0x00034144 File Offset: 0x00032344
		[DataSourceProperty]
		public string FactionDescription
		{
			get
			{
				return this._factionDescription;
			}
			set
			{
				if (value != this._factionDescription)
				{
					this._factionDescription = value;
					base.OnPropertyChangedWithValue<string>(value, "FactionDescription");
				}
			}
		}

		// Token: 0x17000509 RID: 1289
		// (get) Token: 0x06000FB6 RID: 4022 RVA: 0x00034167 File Offset: 0x00032367
		// (set) Token: 0x06000FB7 RID: 4023 RVA: 0x0003416F File Offset: 0x0003236F
		[DataSourceProperty]
		public string FactionName
		{
			get
			{
				return this._factionName;
			}
			set
			{
				if (value != this._factionName)
				{
					this._factionName = value;
					base.OnPropertyChangedWithValue<string>(value, "FactionName");
				}
			}
		}

		// Token: 0x1700050A RID: 1290
		// (get) Token: 0x06000FB8 RID: 4024 RVA: 0x00034192 File Offset: 0x00032392
		// (set) Token: 0x06000FB9 RID: 4025 RVA: 0x0003419A File Offset: 0x0003239A
		[DataSourceProperty]
		public string FlavorText
		{
			get
			{
				return this._flavorText;
			}
			set
			{
				if (value != this._flavorText)
				{
					this._flavorText = value;
					base.OnPropertyChangedWithValue<string>(value, "FlavorText");
				}
			}
		}

		// Token: 0x1700050B RID: 1291
		// (get) Token: 0x06000FBA RID: 4026 RVA: 0x000341BD File Offset: 0x000323BD
		// (set) Token: 0x06000FBB RID: 4027 RVA: 0x000341C5 File Offset: 0x000323C5
		[DataSourceProperty]
		public int Cost
		{
			get
			{
				return this._cost;
			}
			set
			{
				if (value != this._cost)
				{
					this._cost = value;
					base.OnPropertyChangedWithValue(value, "Cost");
				}
			}
		}

		// Token: 0x1700050C RID: 1292
		// (get) Token: 0x06000FBC RID: 4028 RVA: 0x000341E3 File Offset: 0x000323E3
		// (set) Token: 0x06000FBD RID: 4029 RVA: 0x000341EB File Offset: 0x000323EB
		[DataSourceProperty]
		public HintViewModel CostHint
		{
			get
			{
				return this._costHint;
			}
			set
			{
				if (value != this._costHint)
				{
					this._costHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "CostHint");
				}
			}
		}

		// Token: 0x1700050D RID: 1293
		// (get) Token: 0x06000FBE RID: 4030 RVA: 0x00034209 File Offset: 0x00032409
		// (set) Token: 0x06000FBF RID: 4031 RVA: 0x00034211 File Offset: 0x00032411
		[DataSourceProperty]
		public HeroInformationVM HeroInformation
		{
			get
			{
				return this._heroInformation;
			}
			set
			{
				if (value != this._heroInformation)
				{
					this._heroInformation = value;
					base.OnPropertyChangedWithValue<HeroInformationVM>(value, "HeroInformation");
				}
			}
		}

		// Token: 0x04000765 RID: 1893
		private readonly List<IReadOnlyPerkObject> _dummyPerkList;

		// Token: 0x04000766 RID: 1894
		private string _factionDescription;

		// Token: 0x04000767 RID: 1895
		private string _factionName;

		// Token: 0x04000768 RID: 1896
		private string _flavorText;

		// Token: 0x04000769 RID: 1897
		private int _cost;

		// Token: 0x0400076A RID: 1898
		private HintViewModel _costHint;

		// Token: 0x0400076B RID: 1899
		private HeroInformationVM _heroInformation;
	}
}
