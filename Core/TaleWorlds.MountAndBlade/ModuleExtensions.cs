using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public static class ModuleExtensions
	{
		public static bool IsRanged(this FormationClass formationClass)
		{
			return formationClass == FormationClass.Ranged || formationClass == FormationClass.HorseArcher;
		}

		public static bool IsCavalry(this FormationClass formationClass)
		{
			return formationClass == FormationClass.Cavalry || formationClass == FormationClass.LightCavalry || formationClass == FormationClass.HeavyCavalry;
		}

		public static bool IsInfantry(this FormationClass formationClass)
		{
			return formationClass == FormationClass.Infantry || formationClass == FormationClass.HeavyInfantry || formationClass == FormationClass.NumberOfDefaultFormations;
		}

		public static bool IsMounted(this FormationClass formationClass)
		{
			return formationClass == FormationClass.Cavalry || formationClass == FormationClass.LightCavalry || formationClass == FormationClass.HeavyCavalry || formationClass == FormationClass.HorseArcher;
		}

		public static bool IsDefaultFormationClass(this FormationClass formationClass)
		{
			return formationClass >= FormationClass.Infantry && formationClass < FormationClass.NumberOfDefaultFormations;
		}

		public static FormationClass FallbackClass(this FormationClass formationClass)
		{
			if (formationClass == FormationClass.Infantry || formationClass == FormationClass.HeavyInfantry || formationClass == FormationClass.Bodyguard || formationClass == FormationClass.NumberOfRegularFormations)
			{
				return FormationClass.Infantry;
			}
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

		public static FormationClass AlternativeClass(this FormationClass fallbackClass)
		{
			if (fallbackClass == FormationClass.Infantry)
			{
				return FormationClass.Ranged;
			}
			if (fallbackClass == FormationClass.Cavalry)
			{
				return FormationClass.HorseArcher;
			}
			if (fallbackClass == FormationClass.HorseArcher)
			{
				return FormationClass.Cavalry;
			}
			return FormationClass.Infantry;
		}

		public static FormationClass SiegeClass(this FormationClass fallbackClass)
		{
			if (fallbackClass == FormationClass.Cavalry)
			{
				return FormationClass.Infantry;
			}
			if (fallbackClass == FormationClass.HorseArcher)
			{
				return FormationClass.Ranged;
			}
			return fallbackClass;
		}

		public static bool IsRanged(this Formation formation)
		{
			return formation.PrimaryClass.IsRanged();
		}

		public static bool IsCavalry(this Formation formation)
		{
			return formation.PrimaryClass.IsCavalry();
		}

		public static bool IsInfantry(this Formation formation)
		{
			return formation.PrimaryClass.IsInfantry();
		}

		public static bool IsMounted(this Formation formation)
		{
			return formation.PrimaryClass.IsMounted();
		}

		public static IEnumerable<UsableMachine> GetUsedMachines(this Formation formation)
		{
			return from d in formation.Detachments
				select d as UsableMachine into u
				where u != null
				select u;
		}

		public static void StartUsingMachine(this Formation formation, UsableMachine usable, bool isPlayerOrder = false)
		{
			if (isPlayerOrder || (formation.IsAIControlled && !Mission.Current.IsMissionEnding))
			{
				formation.JoinDetachment(usable);
			}
		}

		public static void StopUsingMachine(this Formation formation, UsableMachine usable, bool isPlayerOrder = false)
		{
			if (isPlayerOrder || formation.IsAIControlled)
			{
				formation.LeaveDetachment(usable);
			}
		}

		public static WorldPosition ToWorldPosition(this Vec3 rawPosition)
		{
			return new WorldPosition(Mission.Current.Scene, rawPosition);
		}
	}
}
