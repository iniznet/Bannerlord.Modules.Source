using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000209 RID: 521
	public static class ModuleExtensions
	{
		// Token: 0x06001D90 RID: 7568 RVA: 0x0006A791 File Offset: 0x00068991
		public static bool IsRanged(this FormationClass formationClass)
		{
			return formationClass == FormationClass.Ranged || formationClass == FormationClass.HorseArcher;
		}

		// Token: 0x06001D91 RID: 7569 RVA: 0x0006A79D File Offset: 0x0006899D
		public static bool IsCavalry(this FormationClass formationClass)
		{
			return formationClass == FormationClass.Cavalry || formationClass == FormationClass.LightCavalry || formationClass == FormationClass.HeavyCavalry;
		}

		// Token: 0x06001D92 RID: 7570 RVA: 0x0006A7AD File Offset: 0x000689AD
		public static bool IsInfantry(this FormationClass formationClass)
		{
			return formationClass == FormationClass.Infantry || formationClass == FormationClass.HeavyInfantry || formationClass == FormationClass.NumberOfDefaultFormations;
		}

		// Token: 0x06001D93 RID: 7571 RVA: 0x0006A7BC File Offset: 0x000689BC
		public static bool IsMounted(this FormationClass formationClass)
		{
			return formationClass == FormationClass.Cavalry || formationClass == FormationClass.LightCavalry || formationClass == FormationClass.HeavyCavalry || formationClass == FormationClass.HorseArcher;
		}

		// Token: 0x06001D94 RID: 7572 RVA: 0x0006A7D0 File Offset: 0x000689D0
		public static bool IsDefaultFormationClass(this FormationClass formationClass)
		{
			return formationClass >= FormationClass.Infantry && formationClass < FormationClass.NumberOfDefaultFormations;
		}

		// Token: 0x06001D95 RID: 7573 RVA: 0x0006A7E9 File Offset: 0x000689E9
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

		// Token: 0x06001D96 RID: 7574 RVA: 0x0006A81C File Offset: 0x00068A1C
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

		// Token: 0x06001D97 RID: 7575 RVA: 0x0006A832 File Offset: 0x00068A32
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

		// Token: 0x06001D98 RID: 7576 RVA: 0x0006A843 File Offset: 0x00068A43
		public static bool IsRanged(this Formation formation)
		{
			return formation.PrimaryClass.IsRanged();
		}

		// Token: 0x06001D99 RID: 7577 RVA: 0x0006A850 File Offset: 0x00068A50
		public static bool IsCavalry(this Formation formation)
		{
			return formation.PrimaryClass.IsCavalry();
		}

		// Token: 0x06001D9A RID: 7578 RVA: 0x0006A85D File Offset: 0x00068A5D
		public static bool IsInfantry(this Formation formation)
		{
			return formation.PrimaryClass.IsInfantry();
		}

		// Token: 0x06001D9B RID: 7579 RVA: 0x0006A86A File Offset: 0x00068A6A
		public static bool IsMounted(this Formation formation)
		{
			return formation.PrimaryClass.IsMounted();
		}

		// Token: 0x06001D9C RID: 7580 RVA: 0x0006A878 File Offset: 0x00068A78
		public static IEnumerable<UsableMachine> GetUsedMachines(this Formation formation)
		{
			return from d in formation.Detachments
				select d as UsableMachine into u
				where u != null
				select u;
		}

		// Token: 0x06001D9D RID: 7581 RVA: 0x0006A8D3 File Offset: 0x00068AD3
		public static void StartUsingMachine(this Formation formation, UsableMachine usable, bool isPlayerOrder = false)
		{
			if (isPlayerOrder || (formation.IsAIControlled && !Mission.Current.IsMissionEnding))
			{
				formation.JoinDetachment(usable);
			}
		}

		// Token: 0x06001D9E RID: 7582 RVA: 0x0006A8F3 File Offset: 0x00068AF3
		public static void StopUsingMachine(this Formation formation, UsableMachine usable, bool isPlayerOrder = false)
		{
			if (isPlayerOrder || formation.IsAIControlled)
			{
				formation.LeaveDetachment(usable);
			}
		}

		// Token: 0x06001D9F RID: 7583 RVA: 0x0006A907 File Offset: 0x00068B07
		public static WorldPosition ToWorldPosition(this Vec3 rawPosition)
		{
			return new WorldPosition(Mission.Current.Scene, rawPosition);
		}
	}
}
