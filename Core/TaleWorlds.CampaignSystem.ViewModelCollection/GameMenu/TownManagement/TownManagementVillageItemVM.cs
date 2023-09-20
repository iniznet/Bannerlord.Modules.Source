using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TownManagement
{
	public class TownManagementVillageItemVM : ViewModel
	{
		public TownManagementVillageItemVM(Village village)
		{
			this._village = village;
			this.Background = village.Settlement.SettlementComponent.BackgroundMeshName + "_t";
			this.VillageType = (int)this.DetermineVillageType(village.VillageType);
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this._village.Name.ToString();
			this.ProductionName = this._village.VillageType.PrimaryProduction.Name.ToString();
		}

		public void ExecuteShowTooltip()
		{
			InformationManager.ShowTooltip(typeof(Settlement), new object[]
			{
				this._village.Settlement,
				true
			});
		}

		public void ExecuteHideTooltip()
		{
			MBInformationManager.HideInformations();
		}

		private TownManagementVillageItemVM.VillageTypes DetermineVillageType(VillageType village)
		{
			if (village == DefaultVillageTypes.EuropeHorseRanch)
			{
				return TownManagementVillageItemVM.VillageTypes.EuropeHorseRanch;
			}
			if (village == DefaultVillageTypes.BattanianHorseRanch)
			{
				return TownManagementVillageItemVM.VillageTypes.BattanianHorseRanch;
			}
			if (village == DefaultVillageTypes.SteppeHorseRanch)
			{
				return TownManagementVillageItemVM.VillageTypes.SteppeHorseRanch;
			}
			if (village == DefaultVillageTypes.DesertHorseRanch)
			{
				return TownManagementVillageItemVM.VillageTypes.DesertHorseRanch;
			}
			if (village == DefaultVillageTypes.WheatFarm)
			{
				return TownManagementVillageItemVM.VillageTypes.WheatFarm;
			}
			if (village == DefaultVillageTypes.Lumberjack)
			{
				return TownManagementVillageItemVM.VillageTypes.Lumberjack;
			}
			if (village == DefaultVillageTypes.ClayMine)
			{
				return TownManagementVillageItemVM.VillageTypes.ClayMine;
			}
			if (village == DefaultVillageTypes.SaltMine)
			{
				return TownManagementVillageItemVM.VillageTypes.SaltMine;
			}
			if (village == DefaultVillageTypes.IronMine)
			{
				return TownManagementVillageItemVM.VillageTypes.IronMine;
			}
			if (village == DefaultVillageTypes.Fisherman)
			{
				return TownManagementVillageItemVM.VillageTypes.Fisherman;
			}
			if (village == DefaultVillageTypes.CattleRange)
			{
				return TownManagementVillageItemVM.VillageTypes.CattleRange;
			}
			if (village == DefaultVillageTypes.SheepFarm)
			{
				return TownManagementVillageItemVM.VillageTypes.SheepFarm;
			}
			if (village == DefaultVillageTypes.VineYard)
			{
				return TownManagementVillageItemVM.VillageTypes.VineYard;
			}
			if (village == DefaultVillageTypes.FlaxPlant)
			{
				return TownManagementVillageItemVM.VillageTypes.FlaxPlant;
			}
			if (village == DefaultVillageTypes.DateFarm)
			{
				return TownManagementVillageItemVM.VillageTypes.DateFarm;
			}
			if (village == DefaultVillageTypes.OliveTrees)
			{
				return TownManagementVillageItemVM.VillageTypes.OliveTrees;
			}
			if (village == DefaultVillageTypes.SilkPlant)
			{
				return TownManagementVillageItemVM.VillageTypes.SilkPlant;
			}
			if (village == DefaultVillageTypes.SilverMine)
			{
				return TownManagementVillageItemVM.VillageTypes.SilverMine;
			}
			return TownManagementVillageItemVM.VillageTypes.None;
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
		public string ProductionName
		{
			get
			{
				return this._productionName;
			}
			set
			{
				if (value != this._productionName)
				{
					this._productionName = value;
					base.OnPropertyChangedWithValue<string>(value, "ProductionName");
				}
			}
		}

		[DataSourceProperty]
		public string Background
		{
			get
			{
				return this._background;
			}
			set
			{
				if (value != this._background)
				{
					this._background = value;
					base.OnPropertyChangedWithValue<string>(value, "Background");
				}
			}
		}

		[DataSourceProperty]
		public int VillageType
		{
			get
			{
				return this._villageType;
			}
			set
			{
				if (value != this._villageType)
				{
					this._villageType = value;
					base.OnPropertyChangedWithValue(value, "VillageType");
				}
			}
		}

		private readonly Village _village;

		private string _name;

		private string _background;

		private string _productionName;

		private int _villageType;

		private enum VillageTypes
		{
			None,
			EuropeHorseRanch,
			BattanianHorseRanch,
			SteppeHorseRanch,
			DesertHorseRanch,
			WheatFarm,
			Lumberjack,
			ClayMine,
			SaltMine,
			IronMine,
			Fisherman,
			CattleRange,
			SheepFarm,
			VineYard,
			FlaxPlant,
			DateFarm,
			OliveTrees,
			SilkPlant,
			SilverMine
		}
	}
}
