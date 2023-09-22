using System;
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
	public class SandboxBattleBannerBearersModel : BattleBannerBearersModel
	{
		public override int GetMinimumFormationTroopCountToBearBanners()
		{
			return 2;
		}

		public override float GetBannerInteractionDistance(Agent interactingAgent)
		{
			if (!interactingAgent.HasMount)
			{
				return 1.5f;
			}
			return 3f;
		}

		public override bool CanBannerBearerProvideEffectToFormation(Agent agent, Formation formation)
		{
			return agent.Formation == formation || (agent.IsPlayerControlled && agent.Team == formation.Team);
		}

		public override bool CanAgentPickUpAnyBanner(Agent agent)
		{
			return agent.IsHuman && agent.Banner == null && agent.CanBeAssignedForScriptedMovement() && (agent.CommonAIComponent == null || !agent.CommonAIComponent.IsPanicked) && (agent.HumanAIComponent == null || !agent.HumanAIComponent.IsInImportantCombatAction());
		}

		public override bool CanAgentBecomeBannerBearer(Agent agent)
		{
			CharacterObject characterObject;
			return agent.IsHuman && !agent.IsMainAgent && agent.IsAIControlled && (characterObject = agent.Character as CharacterObject) != null && !characterObject.IsHero;
		}

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

		public override bool CanFormationDeployBannerBearers(Formation formation)
		{
			BannerBearerLogic bannerBearerLogic = base.BannerBearerLogic;
			return bannerBearerLogic != null && formation.CountOfUnits >= this.GetMinimumFormationTroopCountToBearBanners() && bannerBearerLogic.GetFormationBanner(formation) != null && formation.UnitsWithoutLooseDetachedOnes.Count(delegate(IFormationUnit unit)
			{
				Agent agent;
				return (agent = unit as Agent) != null && this.CanAgentBecomeBannerBearer(agent);
			}) > 0;
		}

		public override int GetDesiredNumberOfBannerBearersForFormation(Formation formation)
		{
			if (!this.CanFormationDeployBannerBearers(formation))
			{
				return 0;
			}
			return 1;
		}

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

		private static readonly int[] BannerBearerPriorityPerTier = new int[] { 0, 1, 3, 5, 6, 4, 2 };
	}
}
