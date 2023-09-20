using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	public static class OrderOfBattleFormationExtensions
	{
		public unsafe static void Refresh(this Formation formation)
		{
			if (formation != null)
			{
				formation.SetMovementOrder(*formation.GetReadonlyMovementOrderReference());
			}
		}

		public static DeploymentFormationClass GetOrderOfBattleFormationClass(this FormationClass formationClass)
		{
			switch (formationClass)
			{
			case FormationClass.Infantry:
			case FormationClass.NumberOfDefaultFormations:
			case FormationClass.HeavyInfantry:
				return DeploymentFormationClass.Infantry;
			case FormationClass.Ranged:
				return DeploymentFormationClass.Ranged;
			case FormationClass.Cavalry:
			case FormationClass.LightCavalry:
			case FormationClass.HeavyCavalry:
				return DeploymentFormationClass.Cavalry;
			case FormationClass.HorseArcher:
				return DeploymentFormationClass.HorseArcher;
			default:
				return DeploymentFormationClass.Unset;
			}
		}

		public static List<FormationClass> GetFormationClasses(this DeploymentFormationClass orderOfBattleFormationClass)
		{
			List<FormationClass> list = new List<FormationClass>();
			switch (orderOfBattleFormationClass)
			{
			case DeploymentFormationClass.Infantry:
				list.Add(FormationClass.Infantry);
				break;
			case DeploymentFormationClass.Ranged:
				list.Add(FormationClass.Ranged);
				break;
			case DeploymentFormationClass.Cavalry:
				list.Add(FormationClass.Cavalry);
				break;
			case DeploymentFormationClass.HorseArcher:
				list.Add(FormationClass.HorseArcher);
				break;
			case DeploymentFormationClass.InfantryAndRanged:
				list.Add(FormationClass.Infantry);
				list.Add(FormationClass.Ranged);
				break;
			case DeploymentFormationClass.CavalryAndHorseArcher:
				list.Add(FormationClass.Cavalry);
				list.Add(FormationClass.HorseArcher);
				break;
			}
			return list;
		}

		public static TextObject GetFilterName(this FormationFilterType filterType)
		{
			switch (filterType)
			{
			case FormationFilterType.Shield:
				return new TextObject("{=PSN8IaIg}Shields", null);
			case FormationFilterType.Spear:
				return new TextObject("{=f83FU4X6}Polearms", null);
			case FormationFilterType.Thrown:
				return new TextObject("{=Ea3K1PVR}Thrown Weapons", null);
			case FormationFilterType.Heavy:
				return new TextObject("{=Jw0GMgzv}Heavy Armors", null);
			case FormationFilterType.HighTier:
				return new TextObject("{=DzAkCzwd}High Tier", null);
			case FormationFilterType.LowTier:
				return new TextObject("{=qaPgbwZv}Low Tier", null);
			default:
				return new TextObject("{=w7Yrbi5t}Unset", null);
			}
		}

		public static TextObject GetFilterDescription(this FormationFilterType filterType)
		{
			switch (filterType)
			{
			case FormationFilterType.Unset:
				return new TextObject("{=Q1Ga032B}Don't give preference to any type of troop.", null);
			case FormationFilterType.Shield:
				return new TextObject("{=MVOPbhNj}Give preference to troops with Shields", null);
			case FormationFilterType.Spear:
				return new TextObject("{=K3Cr70PY}Give preference to troops with Polearms", null);
			case FormationFilterType.Thrown:
				return new TextObject("{=DWWa3aIb}Give preference to troops with Thrown Weapons", null);
			case FormationFilterType.Heavy:
				return new TextObject("{=ush8OHIw}Give preference to troops with Heavy Armors", null);
			case FormationFilterType.HighTier:
				return new TextObject("{=DRNDtkP2}Give preference to troops at higher tiers", null);
			case FormationFilterType.LowTier:
				return new TextObject("{=zbpCRmuJ}Give preference to troops at lower tiers", null);
			default:
				return new TextObject("{=w7Yrbi5t}Unset", null);
			}
		}

		public static TextObject GetClassName(this DeploymentFormationClass formationClass)
		{
			switch (formationClass)
			{
			case DeploymentFormationClass.Infantry:
				return GameTexts.FindText("str_troop_type_name", "Infantry");
			case DeploymentFormationClass.Ranged:
				return GameTexts.FindText("str_troop_type_name", "Ranged");
			case DeploymentFormationClass.Cavalry:
				return GameTexts.FindText("str_troop_type_name", "Cavalry");
			case DeploymentFormationClass.HorseArcher:
				return GameTexts.FindText("str_troop_type_name", "HorseArcher");
			case DeploymentFormationClass.InfantryAndRanged:
				return new TextObject("{=mBDj5uG5}Infantry and Ranged", null);
			case DeploymentFormationClass.CavalryAndHorseArcher:
				return new TextObject("{=FNLfNWH3}Cavalry and Horse Archer", null);
			default:
				return new TextObject("{=w7Yrbi5t}Unset", null);
			}
		}

		public static List<Agent> GetHeroAgents(this Team team)
		{
			return team.ActiveAgents.Where((Agent a) => a.IsHero).ToList<Agent>();
		}
	}
}
