using System;
using Helpers;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.LinQuick;
using TaleWorlds.MountAndBlade;

namespace SandBox
{
	// Token: 0x0200000B RID: 11
	public class CampaignAgentComponent : AgentComponent
	{
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000076 RID: 118 RVA: 0x000050B8 File Offset: 0x000032B8
		// (set) Token: 0x06000077 RID: 119 RVA: 0x000050C0 File Offset: 0x000032C0
		public AgentNavigator AgentNavigator { get; private set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000078 RID: 120 RVA: 0x000050C9 File Offset: 0x000032C9
		public PartyBase OwnerParty
		{
			get
			{
				IAgentOriginBase origin = this.Agent.Origin;
				return (PartyBase)((origin != null) ? origin.BattleCombatant : null);
			}
		}

		// Token: 0x06000079 RID: 121 RVA: 0x000050E7 File Offset: 0x000032E7
		public CampaignAgentComponent(Agent agent)
			: base(agent)
		{
		}

		// Token: 0x0600007A RID: 122 RVA: 0x000050F0 File Offset: 0x000032F0
		public AgentNavigator CreateAgentNavigator(LocationCharacter locationCharacter)
		{
			this.AgentNavigator = new AgentNavigator(this.Agent, locationCharacter);
			return this.AgentNavigator;
		}

		// Token: 0x0600007B RID: 123 RVA: 0x0000510A File Offset: 0x0000330A
		public AgentNavigator CreateAgentNavigator()
		{
			this.AgentNavigator = new AgentNavigator(this.Agent);
			return this.AgentNavigator;
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00005123 File Offset: 0x00003323
		public void OnAgentRemoved(Agent agent)
		{
			AgentNavigator agentNavigator = this.AgentNavigator;
			if (agentNavigator == null)
			{
				return;
			}
			agentNavigator.OnAgentRemoved(agent);
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00005136 File Offset: 0x00003336
		public override void OnTickAsAI(float dt)
		{
			AgentNavigator agentNavigator = this.AgentNavigator;
			if (agentNavigator == null)
			{
				return;
			}
			agentNavigator.Tick(dt, false);
		}

		// Token: 0x0600007E RID: 126 RVA: 0x0000514C File Offset: 0x0000334C
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

		// Token: 0x0600007F RID: 127 RVA: 0x000051BC File Offset: 0x000033BC
		public override float GetMoraleAddition()
		{
			float num = 0f;
			PartyBase ownerParty = this.OwnerParty;
			if (((ownerParty != null) ? ownerParty.MapEvent : null) != null)
			{
				float num2;
				float num3;
				MapEventHelper.GetStrengthsRelativeToParty(this.OwnerParty.Side, this.OwnerParty.MapEvent, ref num2, ref num3);
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

		// Token: 0x06000080 RID: 128 RVA: 0x00005249 File Offset: 0x00003449
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
