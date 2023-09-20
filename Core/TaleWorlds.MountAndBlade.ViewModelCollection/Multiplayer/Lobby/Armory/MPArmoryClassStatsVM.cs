using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.ClassLoadout;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Armory
{
	public class MPArmoryClassStatsVM : ViewModel
	{
		public MPArmoryClassStatsVM()
		{
			this._dummyPerkList = new List<IReadOnlyPerkObject>();
			this.FactionDescription = new TextObject("{=5Pea977J}Faction: ", null).ToString();
			this.HeroInformation = new HeroInformationVM();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.CostHint = new HintViewModel(GameTexts.FindText("str_armory_troop_cost", null), null);
			this.HeroInformation.RefreshValues();
		}

		public void RefreshWith(MultiplayerClassDivisions.MPHeroClass heroClass)
		{
			this.FactionName = heroClass.Culture.Name.ToString();
			this.FlavorText = GameTexts.FindText("str_troop_description", heroClass.StringId).ToString();
			this.HeroInformation.RefreshWith(heroClass, this._dummyPerkList);
			this.Cost = heroClass.TroopCost;
		}

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

		private readonly List<IReadOnlyPerkObject> _dummyPerkList;

		private string _factionDescription;

		private string _factionName;

		private string _flavorText;

		private int _cost;

		private HintViewModel _costHint;

		private HeroInformationVM _heroInformation;
	}
}
