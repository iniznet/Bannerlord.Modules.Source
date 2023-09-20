using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle
{
	// Token: 0x02000030 RID: 48
	internal static class OrderOfBattleUIHelper
	{
		// Token: 0x060003BE RID: 958 RVA: 0x000101AC File Offset: 0x0000E3AC
		internal static Team.TroopFilter GetTroopFilterForClass(params FormationClass[] formationClasses)
		{
			Team.TroopFilter troopFilter = Team.TroopFilter.Melee;
			if (formationClasses.Length == 1)
			{
				FormationClass formationClass = formationClasses[0];
				if (formationClass == FormationClass.Infantry)
				{
					troopFilter = Team.TroopFilter.Melee;
				}
				if (formationClass == FormationClass.Ranged)
				{
					troopFilter = Team.TroopFilter.Ranged;
				}
				if (formationClass == FormationClass.Cavalry)
				{
					troopFilter = Team.TroopFilter.Mount | Team.TroopFilter.Melee;
				}
				if (formationClass == FormationClass.HorseArcher)
				{
					troopFilter = Team.TroopFilter.Mount | Team.TroopFilter.Ranged;
				}
			}
			else if (formationClasses.Length == 2)
			{
				if (formationClasses[0] == FormationClass.Infantry && formationClasses[1] == FormationClass.Ranged)
				{
					troopFilter = Team.TroopFilter.Ranged | Team.TroopFilter.Melee;
				}
				if (formationClasses[0] == FormationClass.Cavalry && formationClasses[1] == FormationClass.HorseArcher)
				{
					troopFilter = Team.TroopFilter.Mount | Team.TroopFilter.Ranged | Team.TroopFilter.Melee;
				}
			}
			return troopFilter;
		}

		// Token: 0x060003BF RID: 959 RVA: 0x00010208 File Offset: 0x0000E408
		internal static Team.TroopFilter GetTroopFilterForFormationFilter(params FormationFilterType[] filterTypes)
		{
			if (filterTypes.Length == 0)
			{
				return (Team.TroopFilter)0;
			}
			Team.TroopFilter troopFilter = (Team.TroopFilter)0;
			if (filterTypes.Any((FormationFilterType f) => f == FormationFilterType.Heavy))
			{
				troopFilter |= Team.TroopFilter.Armor;
			}
			if (filterTypes.Any((FormationFilterType f) => f == FormationFilterType.Shield))
			{
				troopFilter |= Team.TroopFilter.Shield;
			}
			if (filterTypes.Any((FormationFilterType f) => f == FormationFilterType.Thrown))
			{
				troopFilter |= Team.TroopFilter.Thrown;
			}
			if (filterTypes.Any((FormationFilterType f) => f == FormationFilterType.Spear))
			{
				troopFilter |= Team.TroopFilter.Spear;
			}
			if (filterTypes.Any((FormationFilterType f) => f == FormationFilterType.HighTier))
			{
				troopFilter |= Team.TroopFilter.HighTier;
			}
			if (filterTypes.Any((FormationFilterType f) => f == FormationFilterType.LowTier))
			{
				troopFilter |= Team.TroopFilter.LowTier;
			}
			return troopFilter;
		}

		// Token: 0x060003C0 RID: 960 RVA: 0x00010328 File Offset: 0x0000E528
		internal static List<Agent> GetExcludedAgentsForTransfer(OrderOfBattleFormationItemVM formationVM, FormationClass formationClass)
		{
			List<Agent> list = new List<Agent>();
			if (formationVM.HasCommander)
			{
				list.Add(formationVM.Commander.Agent);
			}
			if (formationVM.HeroTroops.Count > 0)
			{
				list.AddRange(formationVM.HeroTroops.Select((OrderOfBattleHeroItemVM t) => t.Agent));
			}
			MBList<IFormationUnit> allUnits = formationVM.Formation.Arrangement.GetAllUnits();
			allUnits.AddRange(formationVM.Formation.DetachedUnits);
			for (int i = 0; i < allUnits.Count; i++)
			{
				Agent agent = (Agent)allUnits[i];
				if (agent.IsHero || agent.Banner != null || !OrderOfBattleUIHelper.IsAgentInFormationClass(agent, formationClass))
				{
					list.Add(agent);
				}
			}
			return list.Distinct<Agent>().ToList<Agent>();
		}

		// Token: 0x060003C1 RID: 961 RVA: 0x00010400 File Offset: 0x0000E600
		internal static Tuple<Formation, int, Team.TroopFilter, List<Agent>> CreateMassTransferData(OrderOfBattleFormationClassVM affectedClass, FormationClass formationClass, Team.TroopFilter filter, int unitCount)
		{
			List<Agent> excludedAgentsForTransfer = OrderOfBattleUIHelper.GetExcludedAgentsForTransfer(affectedClass.BelongedFormationItem, formationClass);
			return new Tuple<Formation, int, Team.TroopFilter, List<Agent>>(affectedClass.BelongedFormationItem.Formation, unitCount, filter, excludedAgentsForTransfer);
		}

		// Token: 0x060003C2 RID: 962 RVA: 0x00010430 File Offset: 0x0000E630
		internal static Tuple<Formation, int, Team.TroopFilter, List<Agent>> CreateMassTransferData(OrderOfBattleFormationItemVM affectedFormation, FormationClass formationClass, Team.TroopFilter filter, int unitCount)
		{
			List<Agent> excludedAgentsForTransfer = OrderOfBattleUIHelper.GetExcludedAgentsForTransfer(affectedFormation, formationClass);
			return new Tuple<Formation, int, Team.TroopFilter, List<Agent>>(affectedFormation.Formation, unitCount, filter, excludedAgentsForTransfer);
		}

		// Token: 0x060003C3 RID: 963 RVA: 0x00010454 File Offset: 0x0000E654
		internal static ValueTuple<int, bool, bool> GetRelevantTroopTransferParameters(OrderOfBattleFormationClassVM classVM)
		{
			if (classVM == null)
			{
				return new ValueTuple<int, bool, bool>(0, false, false);
			}
			DeploymentFormationClass orderOfBattleFormationClass = classVM.Class.GetOrderOfBattleFormationClass();
			bool flag = orderOfBattleFormationClass == DeploymentFormationClass.Ranged || orderOfBattleFormationClass == DeploymentFormationClass.HorseArcher;
			bool flag2 = orderOfBattleFormationClass == DeploymentFormationClass.Cavalry || orderOfBattleFormationClass == DeploymentFormationClass.HorseArcher;
			return new ValueTuple<int, bool, bool>(OrderOfBattleUIHelper.GetTotalCountOfUnitsInClass(classVM.BelongedFormationItem.Formation, classVM.Class), flag, flag2);
		}

		// Token: 0x060003C4 RID: 964 RVA: 0x000104B0 File Offset: 0x0000E6B0
		internal static OrderOfBattleFormationClassVM GetFormationClassWithExtremumWeight(List<OrderOfBattleFormationClassVM> classes, bool isMinimum)
		{
			if (classes.Count == 0)
			{
				return null;
			}
			if (classes.Count == 1)
			{
				return classes[0];
			}
			OrderOfBattleFormationClassVM orderOfBattleFormationClassVM = classes[0];
			for (int i = 1; i < classes.Count; i++)
			{
				if ((isMinimum && classes[i].Weight < orderOfBattleFormationClassVM.Weight) || (!isMinimum && classes[i].Weight > orderOfBattleFormationClassVM.Weight))
				{
					orderOfBattleFormationClassVM = classes[i];
				}
			}
			return orderOfBattleFormationClassVM;
		}

		// Token: 0x060003C5 RID: 965 RVA: 0x00010528 File Offset: 0x0000E728
		internal static List<OrderOfBattleFormationClassVM> GetMatchingClasses(List<OrderOfBattleFormationItemVM> formationList, OrderOfBattleFormationClassVM formationClass, Func<OrderOfBattleFormationClassVM, bool> predicate = null)
		{
			List<OrderOfBattleFormationClassVM> list = new List<OrderOfBattleFormationClassVM>();
			for (int i = 0; i < formationList.Count; i++)
			{
				OrderOfBattleFormationItemVM orderOfBattleFormationItemVM = formationList[i];
				for (int j = 0; j < orderOfBattleFormationItemVM.Classes.Count; j++)
				{
					OrderOfBattleFormationClassVM orderOfBattleFormationClassVM = orderOfBattleFormationItemVM.Classes[j];
					if (orderOfBattleFormationClassVM.Class == formationClass.Class && (predicate == null || predicate(orderOfBattleFormationClassVM)))
					{
						list.Add(orderOfBattleFormationClassVM);
					}
				}
			}
			return list;
		}

		// Token: 0x060003C6 RID: 966 RVA: 0x0001059D File Offset: 0x0000E79D
		internal static bool IsAgentInFormationClass(Agent agent, FormationClass fc)
		{
			switch (fc)
			{
			case FormationClass.Infantry:
				return QueryLibrary.IsInfantry(agent);
			case FormationClass.Ranged:
				return QueryLibrary.IsRanged(agent);
			case FormationClass.Cavalry:
				return QueryLibrary.IsCavalry(agent);
			case FormationClass.HorseArcher:
				return QueryLibrary.IsRangedCavalry(agent);
			default:
				return false;
			}
		}

		// Token: 0x060003C7 RID: 967 RVA: 0x000105D4 File Offset: 0x0000E7D4
		private static List<Agent> GetBannerBearersOfFormation(Formation formation)
		{
			Mission mission = Mission.Current;
			List<Agent> list;
			if (mission == null)
			{
				list = null;
			}
			else
			{
				BannerBearerLogic missionBehavior = mission.GetMissionBehavior<BannerBearerLogic>();
				list = ((missionBehavior != null) ? missionBehavior.GetFormationBannerBearers(formation) : null);
			}
			List<Agent> list2 = list;
			if (list2 != null)
			{
				return list2;
			}
			return new List<Agent>();
		}

		// Token: 0x060003C8 RID: 968 RVA: 0x0001060C File Offset: 0x0000E80C
		private static int GetCountOfUnitsInClass(OrderOfBattleFormationClassVM classVM, bool includeHeroes, bool includeBannerBearers)
		{
			Formation formation = classVM.BelongedFormationItem.Formation;
			FormationClass fc = classVM.Class;
			return formation.GetCountOfUnitsWithCondition((Agent agent) => (includeHeroes || !agent.IsHero) && (includeBannerBearers || agent.Banner == null) && OrderOfBattleUIHelper.IsAgentInFormationClass(agent, fc));
		}

		// Token: 0x060003C9 RID: 969 RVA: 0x00010658 File Offset: 0x0000E858
		internal static int GetTotalCountOfUnitsInClass(Formation formation, FormationClass fc)
		{
			switch (fc)
			{
			case FormationClass.Infantry:
				return MathF.Round(formation.QuerySystem.InfantryUnitRatio * (float)formation.CountOfUnits);
			case FormationClass.Ranged:
				return MathF.Round(formation.QuerySystem.RangedUnitRatio * (float)formation.CountOfUnits);
			case FormationClass.Cavalry:
				return MathF.Round(formation.QuerySystem.CavalryUnitRatio * (float)formation.CountOfUnits);
			case FormationClass.HorseArcher:
				return MathF.Round(formation.QuerySystem.RangedCavalryUnitRatio * (float)formation.CountOfUnits);
			default:
				return 0;
			}
		}

		// Token: 0x060003CA RID: 970 RVA: 0x000106E2 File Offset: 0x0000E8E2
		internal static int GetCountOfRealUnitsInClass(OrderOfBattleFormationClassVM classVM)
		{
			return OrderOfBattleUIHelper.GetTotalCountOfUnitsInClass(classVM.BelongedFormationItem.Formation, classVM.Class);
		}

		// Token: 0x060003CB RID: 971 RVA: 0x000106FC File Offset: 0x0000E8FC
		internal static int GetVisibleCountOfUnitsInClass(OrderOfBattleFormationClassVM classVM)
		{
			OrderOfBattleFormationItemVM belongedFormationItem = classVM.BelongedFormationItem;
			Formation formation = classVM.BelongedFormationItem.Formation;
			if (belongedFormationItem.Classes.Where((OrderOfBattleFormationClassVM c) => !c.IsUnset).ToList<OrderOfBattleFormationClassVM>().Count == 1)
			{
				return classVM.BelongedFormationItem.Formation.CountOfUnits;
			}
			int num = OrderOfBattleUIHelper.GetCountOfUnitsInClass(classVM, true, false);
			if (classVM.Class == FormationClass.Infantry || classVM.Class == FormationClass.Cavalry)
			{
				num += OrderOfBattleUIHelper.GetBannerBearersOfFormation(formation).Count;
			}
			return num;
		}
	}
}
