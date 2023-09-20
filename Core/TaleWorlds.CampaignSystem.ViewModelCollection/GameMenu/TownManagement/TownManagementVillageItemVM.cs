using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TownManagement
{
	// Token: 0x02000095 RID: 149
	public class TownManagementVillageItemVM : ViewModel
	{
		// Token: 0x06000E57 RID: 3671 RVA: 0x000390BC File Offset: 0x000372BC
		public TownManagementVillageItemVM(Village village)
		{
			this._village = village;
			this.Background = village.Settlement.SettlementComponent.BackgroundMeshName + "_t";
			this.VillageType = (int)this.DetermineVillageType(village.VillageType);
			this.RefreshValues();
		}

		// Token: 0x06000E58 RID: 3672 RVA: 0x0003910E File Offset: 0x0003730E
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this._village.Name.ToString();
			this.ProductionName = this._village.VillageType.PrimaryProduction.Name.ToString();
		}

		// Token: 0x06000E59 RID: 3673 RVA: 0x0003914C File Offset: 0x0003734C
		public void ExecuteShowTooltip()
		{
			InformationManager.ShowTooltip(typeof(Settlement), new object[]
			{
				this._village.Settlement,
				true
			});
		}

		// Token: 0x06000E5A RID: 3674 RVA: 0x0003917A File Offset: 0x0003737A
		public void ExecuteHideTooltip()
		{
			MBInformationManager.HideInformations();
		}

		// Token: 0x06000E5B RID: 3675 RVA: 0x00039184 File Offset: 0x00037384
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

		// Token: 0x170004AA RID: 1194
		// (get) Token: 0x06000E5C RID: 3676 RVA: 0x00039250 File Offset: 0x00037450
		// (set) Token: 0x06000E5D RID: 3677 RVA: 0x00039258 File Offset: 0x00037458
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

		// Token: 0x170004AB RID: 1195
		// (get) Token: 0x06000E5E RID: 3678 RVA: 0x0003927B File Offset: 0x0003747B
		// (set) Token: 0x06000E5F RID: 3679 RVA: 0x00039283 File Offset: 0x00037483
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

		// Token: 0x170004AC RID: 1196
		// (get) Token: 0x06000E60 RID: 3680 RVA: 0x000392A6 File Offset: 0x000374A6
		// (set) Token: 0x06000E61 RID: 3681 RVA: 0x000392AE File Offset: 0x000374AE
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

		// Token: 0x170004AD RID: 1197
		// (get) Token: 0x06000E62 RID: 3682 RVA: 0x000392D1 File Offset: 0x000374D1
		// (set) Token: 0x06000E63 RID: 3683 RVA: 0x000392D9 File Offset: 0x000374D9
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

		// Token: 0x040006AA RID: 1706
		private readonly Village _village;

		// Token: 0x040006AB RID: 1707
		private string _name;

		// Token: 0x040006AC RID: 1708
		private string _background;

		// Token: 0x040006AD RID: 1709
		private string _productionName;

		// Token: 0x040006AE RID: 1710
		private int _villageType;

		// Token: 0x020001CF RID: 463
		private enum VillageTypes
		{
			// Token: 0x04000FC0 RID: 4032
			None,
			// Token: 0x04000FC1 RID: 4033
			EuropeHorseRanch,
			// Token: 0x04000FC2 RID: 4034
			BattanianHorseRanch,
			// Token: 0x04000FC3 RID: 4035
			SteppeHorseRanch,
			// Token: 0x04000FC4 RID: 4036
			DesertHorseRanch,
			// Token: 0x04000FC5 RID: 4037
			WheatFarm,
			// Token: 0x04000FC6 RID: 4038
			Lumberjack,
			// Token: 0x04000FC7 RID: 4039
			ClayMine,
			// Token: 0x04000FC8 RID: 4040
			SaltMine,
			// Token: 0x04000FC9 RID: 4041
			IronMine,
			// Token: 0x04000FCA RID: 4042
			Fisherman,
			// Token: 0x04000FCB RID: 4043
			CattleRange,
			// Token: 0x04000FCC RID: 4044
			SheepFarm,
			// Token: 0x04000FCD RID: 4045
			VineYard,
			// Token: 0x04000FCE RID: 4046
			FlaxPlant,
			// Token: 0x04000FCF RID: 4047
			DateFarm,
			// Token: 0x04000FD0 RID: 4048
			OliveTrees,
			// Token: 0x04000FD1 RID: 4049
			SilkPlant,
			// Token: 0x04000FD2 RID: 4050
			SilverMine
		}
	}
}
