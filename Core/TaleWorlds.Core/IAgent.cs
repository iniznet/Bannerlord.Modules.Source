using System;

namespace TaleWorlds.Core
{
	public interface IAgent
	{
		BasicCharacterObject Character { get; }

		bool IsEnemyOf(IAgent agent);

		bool IsFriendOf(IAgent agent);

		AgentState State { get; }

		IMissionTeam Team { get; }

		IAgentOriginBase Origin { get; }

		float Age { get; }

		bool IsActive();

		void SetAsConversationAgent(bool set);
	}
}
