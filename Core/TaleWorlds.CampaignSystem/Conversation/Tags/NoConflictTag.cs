using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000215 RID: 533
	public class NoConflictTag : ConversationTag
	{
		// Token: 0x17000799 RID: 1945
		// (get) Token: 0x06001E45 RID: 7749 RVA: 0x00087360 File Offset: 0x00085560
		public override string StringId
		{
			get
			{
				return "NoConflictTag";
			}
		}

		// Token: 0x06001E46 RID: 7750 RVA: 0x00087368 File Offset: 0x00085568
		public override bool IsApplicableTo(CharacterObject character)
		{
			bool flag = new HostileRelationshipTag().IsApplicableTo(character);
			bool flag2 = new PlayerIsEnemyTag().IsApplicableTo(character);
			return !flag && !flag2;
		}

		// Token: 0x04000997 RID: 2455
		public const string Id = "NoConflictTag";
	}
}
