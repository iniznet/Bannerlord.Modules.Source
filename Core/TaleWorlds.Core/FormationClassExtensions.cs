using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.Core
{
	// Token: 0x02000060 RID: 96
	public static class FormationClassExtensions
	{
		// Token: 0x0600065A RID: 1626 RVA: 0x000170BC File Offset: 0x000152BC
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

		// Token: 0x0600065B RID: 1627 RVA: 0x0001713C File Offset: 0x0001533C
		public static TextObject GetLocalizedName(this FormationClass formationClass)
		{
			string text = "str_troop_group_name";
			int num = (int)formationClass;
			return GameTexts.FindText(text, num.ToString());
		}

		// Token: 0x0600065C RID: 1628 RVA: 0x0001715C File Offset: 0x0001535C
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
				Debug.FailedAssert(string.Format("Undefined formation class {0} for TroopType!", formationClass), "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\FormationClass.cs", "GetTroopTypeForRegularFormation", 143);
				break;
			}
			return troopType;
		}

		// Token: 0x04000374 RID: 884
		public static FormationClass[] FormationClassValues = (FormationClass[])Enum.GetValues(typeof(FormationClass));
	}
}
