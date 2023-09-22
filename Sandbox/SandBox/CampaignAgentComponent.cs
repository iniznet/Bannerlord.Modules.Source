using System;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.LinQuick;
using TaleWorlds.MountAndBlade;

namespace SandBox
{
	public class CampaignAgentComponent : AgentComponent
	{
		public AgentNavigator AgentNavigator { get; private set; }

		public PartyBase OwnerParty
		{
			get
			{
				IAgentOriginBase origin = this.Agent.Origin;
				return (PartyBase)((origin != null) ? origin.BattleCombatant : null);
			}
		}

		public CampaignAgentComponent(Agent agent)
			: base(agent)
		{
		}

		public AgentNavigator CreateAgentNavigator(LocationCharacter locationCharacter)
		{
			this.AgentNavigator = new AgentNavigator(this.Agent, locationCharacter);
			return this.AgentNavigator;
		}

		public AgentNavigator CreateAgentNavigator()
		{
			this.AgentNavigator = new AgentNavigator(this.Agent);
			return this.AgentNavigator;
		}

		public void OnAgentRemoved(Agent agent)
		{
			AgentNavigator agentNavigator = this.AgentNavigator;
			if (agentNavigator == null)
			{
				return;
			}
			agentNavigator.OnAgentRemoved(agent);
		}

		public override void OnTickAsAI(float dt)
		{
			AgentNavigator agentNavigator = this.AgentNavigator;
			if (agentNavigator == null)
			{
				return;
			}
			agentNavigator.Tick(dt, false);
		}

		public override float GetMoraleDecreaseConstant()
		{
			PartyBase ownerParty = this.OwnerParty;
			if (((ownerParty != null) ? ownerParty.MapEvent : null) == null || !this.OwnerParty.MapEvent.IsSiegeAssault)
			{
				return 1f;
			}
			if (LinQuick.FindIndexQ<MapEventParty>(this.OwnerParty.MapEvent.AttackerSide.Parties, (MapEventParty p) => p.Party == this.OwnerParty) < 0)
			{
				return 0.5f;
			}
			return 0.33f;
		}

		public override float GetMoraleAddition()
		{
			float num = 0f;
			PartyBase ownerParty = this.OwnerParty;
			if (((ownerParty != null) ? ownerParty.MapEvent : null) != null)
			{
				float num2;
				float num3;
				this.OwnerParty.MapEvent.GetStrengthsRelativeToParty(this.OwnerParty.Side, ref num2, ref num3);
				if (this.OwnerParty.IsMobile)
				{
					float num4 = (this.OwnerParty.MobileParty.Morale - 50f) / 2f;
					num += num4;
				}
				float num5 = num2 / (num2 + num3) * 10f - 5f;
				num += num5;
			}
			return num;
		}

		public override void OnStopUsingGameObject()
		{
			if (this.Agent.IsAIControlled)
			{
				AgentNavigator agentNavigator = this.AgentNavigator;
				if (agentNavigator == null)
				{
					return;
				}
				agentNavigator.OnStopUsingGameObject();
			}
		}
	}
}
