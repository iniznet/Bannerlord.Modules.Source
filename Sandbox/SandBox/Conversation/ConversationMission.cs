using System;
using System.Collections.Generic;
using SandBox.Conversation.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace SandBox.Conversation
{
	// Token: 0x0200008E RID: 142
	public static class ConversationMission
	{
		// Token: 0x1700006B RID: 107
		// (get) Token: 0x060005E0 RID: 1504 RVA: 0x0002CA95 File Offset: 0x0002AC95
		public static Agent OneToOneConversationAgent
		{
			get
			{
				return Campaign.Current.ConversationManager.OneToOneConversationAgent as Agent;
			}
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x060005E1 RID: 1505 RVA: 0x0002CAAB File Offset: 0x0002ACAB
		public static CharacterObject OneToOneConversationCharacter
		{
			get
			{
				return Campaign.Current.ConversationManager.OneToOneConversationCharacter;
			}
		}

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x060005E2 RID: 1506 RVA: 0x0002CABC File Offset: 0x0002ACBC
		public static IEnumerable<Agent> ConversationAgents
		{
			get
			{
				foreach (IAgent agent in Campaign.Current.ConversationManager.ConversationAgents)
				{
					yield return agent as Agent;
				}
				IEnumerator<IAgent> enumerator = null;
				yield break;
				yield break;
			}
		}

		// Token: 0x060005E3 RID: 1507 RVA: 0x0002CAC5 File Offset: 0x0002ACC5
		public static void StartConversationWithAgent(Agent agent)
		{
			MissionConversationLogic missionBehavior = Mission.Current.GetMissionBehavior<MissionConversationLogic>();
			if (missionBehavior == null)
			{
				return;
			}
			missionBehavior.StartConversation(agent, true, false);
		}
	}
}
