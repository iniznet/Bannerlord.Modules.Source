using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ComponentInterfaces;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001F1 RID: 497
	public class CustomBattleBannerBearersModel : BattleBannerBearersModel
	{
		// Token: 0x06001BA0 RID: 7072 RVA: 0x00061FD4 File Offset: 0x000601D4
		public override int GetMinimumFormationTroopCountToBearBanners()
		{
			return 3;
		}

		// Token: 0x06001BA1 RID: 7073 RVA: 0x00061FD7 File Offset: 0x000601D7
		public override float GetBannerInteractionDistance(Agent interactingAgent)
		{
			if (!interactingAgent.HasMount)
			{
				return 1.5f;
			}
			return 3f;
		}

		// Token: 0x06001BA2 RID: 7074 RVA: 0x00061FEC File Offset: 0x000601EC
		public override bool CanBannerBearerProvideEffectToFormation(Agent agent, Formation formation)
		{
			return agent.Formation == formation || (agent.IsPlayerControlled && agent.Team == formation.Team);
		}

		// Token: 0x06001BA3 RID: 7075 RVA: 0x00062014 File Offset: 0x00060214
		public override bool CanAgentPickUpAnyBanner(Agent agent)
		{
			return agent.IsHuman && agent.Banner == null && agent.CanBeAssignedForScriptedMovement() && (agent.CommonAIComponent == null || !agent.CommonAIComponent.IsPanicked) && (agent.HumanAIComponent == null || !agent.HumanAIComponent.IsInImportantCombatAction());
		}

		// Token: 0x06001BA4 RID: 7076 RVA: 0x00062068 File Offset: 0x00060268
		public override bool CanAgentBecomeBannerBearer(Agent agent)
		{
			if (CustomBattleBannerBearersModel._missionSpawnLogic == null)
			{
				CustomBattleBannerBearersModel._missionSpawnLogic = Mission.Current.GetMissionBehavior<MissionAgentSpawnLogic>();
			}
			if (CustomBattleBannerBearersModel._missionSpawnLogic != null)
			{
				Formation formation = agent.Formation;
				Team team = ((formation != null) ? formation.Team : null);
				if (team != null)
				{
					BasicCharacterObject generalCharacterOfSide = CustomBattleBannerBearersModel._missionSpawnLogic.GetGeneralCharacterOfSide(team.Side);
					return agent.IsHuman && !agent.IsMainAgent && agent.IsAIControlled && agent.Character != generalCharacterOfSide;
				}
			}
			return false;
		}

		// Token: 0x06001BA5 RID: 7077 RVA: 0x000620E4 File Offset: 0x000602E4
		public override int GetAgentBannerBearingPriority(Agent agent)
		{
			if (!this.CanAgentBecomeBannerBearer(agent))
			{
				return 0;
			}
			if (agent.Formation != null)
			{
				bool calculateHasSignificantNumberOfMounted = agent.Formation.CalculateHasSignificantNumberOfMounted;
				if ((calculateHasSignificantNumberOfMounted && !agent.HasMount) || (!calculateHasSignificantNumberOfMounted && agent.HasMount))
				{
					return 0;
				}
			}
			int num = Math.Min(agent.Character.Level / 4 + 1, CustomBattleBannerBearersModel.BannerBearerPriorityPerTier.Length - 1);
			return CustomBattleBannerBearersModel.BannerBearerPriorityPerTier[num];
		}

		// Token: 0x06001BA6 RID: 7078 RVA: 0x00062150 File Offset: 0x00060350
		public override bool CanFormationDeployBannerBearers(Formation formation)
		{
			BannerBearerLogic bannerBearerLogic = base.BannerBearerLogic;
			return bannerBearerLogic != null && formation.CountOfUnits >= this.GetMinimumFormationTroopCountToBearBanners() && bannerBearerLogic.GetFormationBanner(formation) != null && formation.UnitsWithoutLooseDetachedOnes.Count(delegate(IFormationUnit unit)
			{
				Agent agent;
				return (agent = unit as Agent) != null && this.CanAgentBecomeBannerBearer(agent);
			}) > 0;
		}

		// Token: 0x06001BA7 RID: 7079 RVA: 0x0006219A File Offset: 0x0006039A
		public override int GetDesiredNumberOfBannerBearersForFormation(Formation formation)
		{
			if (!this.CanFormationDeployBannerBearers(formation))
			{
				return 0;
			}
			return 1;
		}

		// Token: 0x06001BA8 RID: 7080 RVA: 0x000621A8 File Offset: 0x000603A8
		public override ItemObject GetBannerBearerReplacementWeapon(BasicCharacterObject agentCharacter)
		{
			if (CustomBattleBannerBearersModel.ReplacementWeapons == null)
			{
				CustomBattleBannerBearersModel.ReplacementWeapons = MBObjectManager.Instance.GetObjectTypeList<ItemObject>().Where(delegate(ItemObject item)
				{
					if (item.PrimaryWeapon != null)
					{
						WeaponComponentData primaryWeapon = item.PrimaryWeapon;
						return primaryWeapon.WeaponClass == WeaponClass.OneHandedSword;
					}
					return false;
				}).ToList<ItemObject>();
			}
			if (CustomBattleBannerBearersModel.ReplacementWeapons.IsEmpty<ItemObject>())
			{
				return null;
			}
			IEnumerable<ItemObject> enumerable = CustomBattleBannerBearersModel.ReplacementWeapons.Where((ItemObject item) => item.Culture != null && item.Culture == agentCharacter.Culture);
			List<ValueTuple<int, ItemObject>> list = new List<ValueTuple<int, ItemObject>>();
			int minTierDifference = int.MaxValue;
			foreach (ItemObject itemObject in enumerable)
			{
				int num = MathF.Ceiling(((float)agentCharacter.Level - 5f) / 5f);
				num = MathF.Min(MathF.Max(num, 0), 7);
				int num2 = MathF.Abs(itemObject.Tier - (ItemObject.ItemTiers)num);
				if (num2 < minTierDifference)
				{
					minTierDifference = num2;
				}
				list.Add(new ValueTuple<int, ItemObject>(num2, itemObject));
			}
			return list.Where(([TupleElementNames(new string[] { "TierDifference", "Weapon" })] ValueTuple<int, ItemObject> tuple) => tuple.Item1 == minTierDifference).GetRandomElementInefficiently<ValueTuple<int, ItemObject>>().Item2;
		}

		// Token: 0x040008E9 RID: 2281
		private static readonly int[] BannerBearerPriorityPerTier = new int[] { 0, 1, 3, 5, 6, 4, 2 };

		// Token: 0x040008EA RID: 2282
		private static List<ItemObject> ReplacementWeapons = null;

		// Token: 0x040008EB RID: 2283
		private static MissionAgentSpawnLogic _missionSpawnLogic;
	}
}
