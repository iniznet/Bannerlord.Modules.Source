using System;
using System.Collections.Generic;
using SandBox.Conversation.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace SandBox.Conversation
{
	public static class ConversationMission
	{
		public static Agent OneToOneConversationAgent
		{
			get
			{
				return Campaign.Current.ConversationManager.OneToOneConversationAgent as Agent;
			}
		}

		public static CharacterObject OneToOneConversationCharacter
		{
			get
			{
				return Campaign.Current.ConversationManager.OneToOneConversationCharacter;
			}
		}

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
