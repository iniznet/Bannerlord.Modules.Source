using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.Core
{
	public static class FormationClassExtensions
	{
		public static string GetName(this FormationClass formationClass)
		{
			switch (formationClass)
			{
			case FormationClass.Infantry:
				return "Infantry";
			case FormationClass.Ranged:
				return "Ranged";
			case FormationClass.Cavalry:
				return "Cavalry";
			case FormationClass.HorseArcher:
				return "HorseArcher";
			case FormationClass.NumberOfDefaultFormations:
				return "Skirmisher";
			case FormationClass.HeavyInfantry:
				return "HeavyInfantry";
			case FormationClass.LightCavalry:
				return "LightCavalry";
			case FormationClass.HeavyCavalry:
				return "HeavyCavalry";
			case FormationClass.NumberOfRegularFormations:
				return "General";
			case FormationClass.Bodyguard:
				return "Bodyguard";
			default:
				return "Unset";
			}
		}

		public static TextObject GetLocalizedName(this FormationClass formationClass)
		{
			string text = "str_troop_group_name";
			int num = (int)formationClass;
			return GameTexts.FindText(text, num.ToString());
		}

		public static TroopUsageFlags GetTroopUsageFlags(this FormationClass troopClass)
		{
			switch (troopClass)
			{
			case FormationClass.Ranged:
				return TroopUsageFlags.OnFoot | TroopUsageFlags.Ranged | TroopUsageFlags.BowUser | TroopUsageFlags.ThrownUser | TroopUsageFlags.CrossbowUser;
			case FormationClass.Cavalry:
				return TroopUsageFlags.Mounted | TroopUsageFlags.Melee | TroopUsageFlags.OneHandedUser | TroopUsageFlags.ShieldUser | TroopUsageFlags.TwoHandedUser | TroopUsageFlags.PolearmUser;
			case FormationClass.HorseArcher:
				return TroopUsageFlags.Mounted | TroopUsageFlags.Ranged | TroopUsageFlags.BowUser | TroopUsageFlags.ThrownUser | TroopUsageFlags.CrossbowUser;
			}
			return TroopUsageFlags.OnFoot | TroopUsageFlags.Melee | TroopUsageFlags.OneHandedUser | TroopUsageFlags.ShieldUser | TroopUsageFlags.TwoHandedUser | TroopUsageFlags.PolearmUser;
		}

		public static TroopType GetTroopTypeForRegularFormation(this FormationClass formationClass)
		{
			TroopType troopType = TroopType.Invalid;
			switch (formationClass)
			{
			case FormationClass.Infantry:
			case FormationClass.HeavyInfantry:
				troopType = TroopType.Infantry;
				break;
			case FormationClass.Ranged:
			case FormationClass.NumberOfDefaultFormations:
				troopType = TroopType.Ranged;
				break;
			case FormationClass.Cavalry:
			case FormationClass.HorseArcher:
			case FormationClass.LightCavalry:
			case FormationClass.HeavyCavalry:
				troopType = TroopType.Cavalry;
				break;
			default:
				Debug.FailedAssert(string.Format("Undefined formation class {0} for TroopType!", formationClass), "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\FormationClass.cs", "GetTroopTypeForRegularFormation", 311);
				break;
			}
			return troopType;
		}

		public static bool IsDefaultFormationClass(this FormationClass formationClass)
		{
			return formationClass >= FormationClass.Infantry && formationClass < FormationClass.NumberOfDefaultFormations;
		}

		public static bool IsRegularFormationClass(this FormationClass formationClass)
		{
			return formationClass >= FormationClass.Infantry && formationClass < FormationClass.NumberOfRegularFormations;
		}

		public static FormationClass FallbackClass(this FormationClass formationClass)
		{
			if (formationClass == FormationClass.Ranged || formationClass == FormationClass.NumberOfDefaultFormations)
			{
				return FormationClass.Ranged;
			}
			if (formationClass == FormationClass.Cavalry || formationClass == FormationClass.HeavyCavalry)
			{
				return FormationClass.Cavalry;
			}
			if (formationClass == FormationClass.HorseArcher || formationClass == FormationClass.LightCavalry)
			{
				return FormationClass.HorseArcher;
			}
			return FormationClass.Infantry;
		}

		public const TroopUsageFlags DefaultInfantryTroopUsageFlags = TroopUsageFlags.OnFoot | TroopUsageFlags.Melee | TroopUsageFlags.OneHandedUser | TroopUsageFlags.ShieldUser | TroopUsageFlags.TwoHandedUser | TroopUsageFlags.PolearmUser;

		public const TroopUsageFlags DefaultRangedTroopUsageFlags = TroopUsageFlags.OnFoot | TroopUsageFlags.Ranged | TroopUsageFlags.BowUser | TroopUsageFlags.ThrownUser | TroopUsageFlags.CrossbowUser;

		public const TroopUsageFlags DefaultCavalryTroopUsageFlags = TroopUsageFlags.Mounted | TroopUsageFlags.Melee | TroopUsageFlags.OneHandedUser | TroopUsageFlags.ShieldUser | TroopUsageFlags.TwoHandedUser | TroopUsageFlags.PolearmUser;

		public const TroopUsageFlags DefaultHorseArcherTroopUsageFlags = TroopUsageFlags.Mounted | TroopUsageFlags.Ranged | TroopUsageFlags.BowUser | TroopUsageFlags.ThrownUser | TroopUsageFlags.CrossbowUser;

		public static FormationClass[] FormationClassValues = (FormationClass[])Enum.GetValues(typeof(FormationClass));
	}
}
