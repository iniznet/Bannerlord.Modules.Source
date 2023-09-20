using System;

namespace TaleWorlds.Core
{
	public static class TroopClassExtensions
	{
		public static bool IsRanged(this FormationClass troopClass)
		{
			FormationClass formationClass = troopClass.DefaultClass();
			return formationClass == FormationClass.Ranged || formationClass == FormationClass.HorseArcher;
		}

		public static bool IsMounted(this FormationClass troopClass)
		{
			troopClass.DefaultClass();
			return troopClass == FormationClass.Cavalry || troopClass == FormationClass.HorseArcher;
		}

		public static bool IsMeleeInfantry(this FormationClass troopClass)
		{
			troopClass.DefaultClass();
			return troopClass == FormationClass.Infantry;
		}

		public static bool IsMeleeCavalry(this FormationClass troopClass)
		{
			return troopClass.DefaultClass() == FormationClass.Cavalry;
		}

		public static FormationClass DefaultClass(this FormationClass troopClass)
		{
			if (troopClass.IsRegularFormationClass())
			{
				FormationClass formationClass = troopClass;
				switch (troopClass)
				{
				case FormationClass.NumberOfDefaultFormations:
					formationClass = FormationClass.Ranged;
					break;
				case FormationClass.HeavyInfantry:
					formationClass = FormationClass.Infantry;
					break;
				case FormationClass.LightCavalry:
					formationClass = FormationClass.HorseArcher;
					break;
				case FormationClass.HeavyCavalry:
					formationClass = FormationClass.Cavalry;
					break;
				}
				return formationClass;
			}
			return FormationClass.Infantry;
		}

		public static FormationClass AlternativeClass(this FormationClass troopClass)
		{
			switch (troopClass)
			{
			case FormationClass.Infantry:
				return FormationClass.Ranged;
			case FormationClass.Ranged:
				return FormationClass.Infantry;
			case FormationClass.Cavalry:
				return FormationClass.HorseArcher;
			case FormationClass.HorseArcher:
				return FormationClass.Cavalry;
			case FormationClass.NumberOfDefaultFormations:
				return FormationClass.HeavyInfantry;
			case FormationClass.HeavyInfantry:
				return FormationClass.NumberOfDefaultFormations;
			case FormationClass.LightCavalry:
				return FormationClass.HeavyCavalry;
			case FormationClass.HeavyCavalry:
				return FormationClass.LightCavalry;
			default:
				return troopClass;
			}
		}

		public static FormationClass DismountedClass(this FormationClass troopClass)
		{
			FormationClass formationClass = troopClass;
			switch (troopClass)
			{
			case FormationClass.Cavalry:
				formationClass = FormationClass.Infantry;
				break;
			case FormationClass.HorseArcher:
				formationClass = FormationClass.Ranged;
				break;
			case FormationClass.LightCavalry:
				formationClass = FormationClass.NumberOfDefaultFormations;
				break;
			case FormationClass.HeavyCavalry:
				formationClass = FormationClass.HeavyInfantry;
				break;
			}
			return formationClass;
		}

		public static bool IsDefaultTroopClass(this FormationClass troopClass)
		{
			return troopClass >= FormationClass.Infantry && troopClass < FormationClass.NumberOfDefaultFormations;
		}

		public static bool IsRegularTroopClass(this FormationClass troopClass)
		{
			return troopClass >= FormationClass.Infantry && troopClass < FormationClass.NumberOfRegularFormations;
		}
	}
}
