﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace SandBox
{
	// Token: 0x02000007 RID: 7
	public class SandboxBattleBannerBearersModel : BattleBannerBearersModel
	{
		// Token: 0x0600000F RID: 15 RVA: 0x000034F8 File Offset: 0x000016F8
		public override int GetMinimumFormationTroopCountToBearBanners()
		{
			return 3;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000034FB File Offset: 0x000016FB
		public override float GetBannerInteractionDistance(Agent interactingAgent)
		{
			if (!interactingAgent.HasMount)
			{
				return 1.5f;
			}
			return 3f;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00003510 File Offset: 0x00001710
		public override bool CanBannerBearerProvideEffectToFormation(Agent agent, Formation formation)
		{
			return agent.Formation == formation || (agent.IsPlayerControlled && agent.Team == formation.Team);
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00003538 File Offset: 0x00001738
		public override bool CanAgentPickUpAnyBanner(Agent agent)
		{
			return agent.IsHuman && agent.Banner == null && agent.CanBeAssignedForScriptedMovement() && (agent.CommonAIComponent == null || !agent.CommonAIComponent.IsPanicked) && (agent.HumanAIComponent == null || !agent.HumanAIComponent.IsInImportantCombatAction());
		}

		// Token: 0x06000013 RID: 19 RVA: 0x0000358C File Offset: 0x0000178C
		public override bool CanAgentBecomeBannerBearer(Agent agent)
		{
			CharacterObject characterObject;
			return agent.IsHuman && !agent.IsMainAgent && agent.IsAIControlled && (characterObject = agent.Character as CharacterObject) != null && !characterObject.IsHero;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000035CC File Offset: 0x000017CC
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
			int num = 0;
			CharacterObject characterObject;
			if ((characterObject = agent.Character as CharacterObject) != null)
			{
				int num2 = Math.Min(characterObject.Tier, SandboxBattleBannerBearersModel.BannerBearerPriorityPerTier.Length - 1);
				num += SandboxBattleBannerBearersModel.BannerBearerPriorityPerTier[num2];
			}
			return num;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00003644 File Offset: 0x00001844
		public override bool CanFormationDeployBannerBearers(Formation formation)
		{
			BannerBearerLogic bannerBearerLogic = base.BannerBearerLogic;
			return bannerBearerLogic != null && formation.CountOfUnits >= this.GetMinimumFormationTroopCountToBearBanners() && bannerBearerLogic.GetFormationBanner(formation) != null && formation.UnitsWithoutLooseDetachedOnes.Count(delegate(IFormationUnit unit)
			{
				Agent agent;
				return (agent = unit as Agent) != null && this.CanAgentBecomeBannerBearer(agent);
			}) > 0;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x0000368E File Offset: 0x0000188E
		public override int GetDesiredNumberOfBannerBearersForFormation(Formation formation)
		{
			if (!this.CanFormationDeployBannerBearers(formation))
			{
				return 0;
			}
			return 1;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x0000369C File Offset: 0x0000189C
		public override ItemObject GetBannerBearerReplacementWeapon(BasicCharacterObject agentCharacter)
		{
			CharacterObject characterObject;
			CultureObject cultureObject;
			if ((characterObject = agentCharacter as CharacterObject) != null && (cultureObject = agentCharacter.Culture as CultureObject) != null && !Extensions.IsEmpty<ItemObject>(cultureObject.BannerBearerReplacementWeapons))
			{
				List<ValueTuple<int, ItemObject>> list = new List<ValueTuple<int, ItemObject>>();
				int minTierDifference = int.MaxValue;
				foreach (ItemObject itemObject in cultureObject.BannerBearerReplacementWeapons)
				{
					int num = MathF.Abs(itemObject.Tier + 1 - characterObject.Tier);
					if (num < minTierDifference)
					{
						minTierDifference = num;
					}
					list.Add(new ValueTuple<int, ItemObject>(num, itemObject));
				}
				return Extensions.GetRandomElementInefficiently<ValueTuple<int, ItemObject>>(list.Where(([TupleElementNames(new string[] { "TierDifference", "Weapon" })] ValueTuple<int, ItemObject> tuple) => tuple.Item1 == minTierDifference)).Item2;
			}
			return null;
		}

		// Token: 0x04000027 RID: 39
		private static readonly int[] BannerBearerPriorityPerTier = new int[] { 0, 1, 3, 5, 6, 4, 2 };
	}
}
