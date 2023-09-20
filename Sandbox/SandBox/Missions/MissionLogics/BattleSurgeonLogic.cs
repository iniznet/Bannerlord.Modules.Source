using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	public class BattleSurgeonLogic : MissionLogic
	{
		protected override void OnGetAgentState(Agent agent, bool usedSurgery)
		{
			if (usedSurgery)
			{
				PartyBase ownerParty = agent.GetComponent<CampaignAgentComponent>().OwnerParty;
				Agent agent2;
				if (ownerParty != null && this._surgeonAgents.TryGetValue(ownerParty.Id, out agent2) && agent2.State == 1)
				{
					SkillLevelingManager.OnSurgeryApplied(ownerParty.MobileParty, true, ((CharacterObject)agent.Character).Tier);
				}
			}
		}

		public override void OnAgentCreated(Agent agent)
		{
			base.OnAgentCreated(agent);
			CharacterObject characterObject = (CharacterObject)agent.Character;
			bool flag;
			if (characterObject == null)
			{
				flag = null != null;
			}
			else
			{
				Hero heroObject = characterObject.HeroObject;
				flag = ((heroObject != null) ? heroObject.PartyBelongedTo : null) != null;
			}
			if (flag && characterObject.HeroObject == characterObject.HeroObject.PartyBelongedTo.EffectiveSurgeon)
			{
				string id = characterObject.HeroObject.PartyBelongedTo.Party.Id;
				if (this._surgeonAgents.ContainsKey(id))
				{
					this._surgeonAgents.Remove(id);
				}
				this._surgeonAgents.Add(id, agent);
			}
		}

		private Dictionary<string, Agent> _surgeonAgents = new Dictionary<string, Agent>();
	}
}
