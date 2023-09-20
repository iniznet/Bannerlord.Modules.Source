using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace SandBox.GameComponents
{
	// Token: 0x02000086 RID: 134
	public class SandboxAgentDecideKilledOrUnconsciousModel : AgentDecideKilledOrUnconsciousModel
	{
		// Token: 0x060005AB RID: 1451 RVA: 0x00028A88 File Offset: 0x00026C88
		public override float GetAgentStateProbability(Agent affectorAgent, Agent effectedAgent, DamageTypes damageType, out float useSurgeryProbability)
		{
			useSurgeryProbability = 1f;
			if (effectedAgent.IsHuman)
			{
				CharacterObject characterObject = (CharacterObject)effectedAgent.Character;
				if (Campaign.Current != null)
				{
					if (characterObject.IsHero && !characterObject.HeroObject.CanDie(4))
					{
						return 0f;
					}
					CampaignAgentComponent component = effectedAgent.GetComponent<CampaignAgentComponent>();
					PartyBase partyBase = ((component != null) ? component.OwnerParty : null);
					if (affectorAgent.IsHuman)
					{
						CampaignAgentComponent component2 = affectorAgent.GetComponent<CampaignAgentComponent>();
						PartyBase partyBase2 = ((component2 != null) ? component2.OwnerParty : null);
						return 1f - Campaign.Current.Models.PartyHealingModel.GetSurvivalChance(partyBase, characterObject, damageType, partyBase2);
					}
					return 1f - Campaign.Current.Models.PartyHealingModel.GetSurvivalChance(partyBase, characterObject, damageType, null);
				}
			}
			return 1f;
		}
	}
}
