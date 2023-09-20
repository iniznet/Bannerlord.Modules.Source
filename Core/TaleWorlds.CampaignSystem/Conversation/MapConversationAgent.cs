using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.Conversation
{
	// Token: 0x020001E9 RID: 489
	public class MapConversationAgent : IAgent
	{
		// Token: 0x06001D2E RID: 7470 RVA: 0x000837F3 File Offset: 0x000819F3
		public MapConversationAgent(CharacterObject characterObject)
		{
			this._characterObject = characterObject;
		}

		// Token: 0x17000751 RID: 1873
		// (get) Token: 0x06001D2F RID: 7471 RVA: 0x00083802 File Offset: 0x00081A02
		public BasicCharacterObject Character
		{
			get
			{
				return this._characterObject;
			}
		}

		// Token: 0x06001D30 RID: 7472 RVA: 0x0008380A File Offset: 0x00081A0A
		public bool IsEnemyOf(IAgent agent)
		{
			return false;
		}

		// Token: 0x06001D31 RID: 7473 RVA: 0x0008380D File Offset: 0x00081A0D
		public bool IsFriendOf(IAgent agent)
		{
			return true;
		}

		// Token: 0x17000752 RID: 1874
		// (get) Token: 0x06001D32 RID: 7474 RVA: 0x00083810 File Offset: 0x00081A10
		public AgentState State
		{
			get
			{
				return AgentState.Active;
			}
		}

		// Token: 0x17000753 RID: 1875
		// (get) Token: 0x06001D33 RID: 7475 RVA: 0x00083813 File Offset: 0x00081A13
		public IMissionTeam Team
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000754 RID: 1876
		// (get) Token: 0x06001D34 RID: 7476 RVA: 0x00083816 File Offset: 0x00081A16
		public IAgentOriginBase Origin
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000755 RID: 1877
		// (get) Token: 0x06001D35 RID: 7477 RVA: 0x00083819 File Offset: 0x00081A19
		public float Age
		{
			get
			{
				return this.Character.Age;
			}
		}

		// Token: 0x06001D36 RID: 7478 RVA: 0x00083826 File Offset: 0x00081A26
		public bool IsActive()
		{
			return true;
		}

		// Token: 0x06001D37 RID: 7479 RVA: 0x00083829 File Offset: 0x00081A29
		public void SetAsConversationAgent(bool set)
		{
		}

		// Token: 0x0400090E RID: 2318
		private CharacterObject _characterObject;

		// Token: 0x0400090F RID: 2319
		public bool DeliveredLine;
	}
}
