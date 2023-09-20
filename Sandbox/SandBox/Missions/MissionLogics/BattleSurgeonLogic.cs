using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	// Token: 0x02000036 RID: 54
	public class BattleSurgeonLogic : MissionLogic
	{
		// Token: 0x0600027F RID: 639 RVA: 0x00010ED4 File Offset: 0x0000F0D4
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

		// Token: 0x06000280 RID: 640 RVA: 0x00010F30 File Offset: 0x0000F130
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

		// Token: 0x04000145 RID: 325
		private Dictionary<string, Agent> _surgeonAgents = new Dictionary<string, Agent>();
	}
}
