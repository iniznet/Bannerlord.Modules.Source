using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.Conversation
{
	public class MapConversationAgent : IAgent
	{
		public MapConversationAgent(CharacterObject characterObject)
		{
			this._characterObject = characterObject;
		}

		public BasicCharacterObject Character
		{
			get
			{
				return this._characterObject;
			}
		}

		public bool IsEnemyOf(IAgent agent)
		{
			return false;
		}

		public bool IsFriendOf(IAgent agent)
		{
			return true;
		}

		public AgentState State
		{
			get
			{
				return AgentState.Active;
			}
		}

		public IMissionTeam Team
		{
			get
			{
				return null;
			}
		}

		public IAgentOriginBase Origin
		{
			get
			{
				return null;
			}
		}

		public float Age
		{
			get
			{
				return this.Character.Age;
			}
		}

		public bool IsActive()
		{
			return true;
		}

		public void SetAsConversationAgent(bool set)
		{
		}

		private CharacterObject _characterObject;

		public bool DeliveredLine;
	}
}
