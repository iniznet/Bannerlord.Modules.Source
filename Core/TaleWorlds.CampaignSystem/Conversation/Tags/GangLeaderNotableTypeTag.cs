using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000229 RID: 553
	public class GangLeaderNotableTypeTag : ConversationTag
	{
		// Token: 0x170007AD RID: 1965
		// (get) Token: 0x06001E81 RID: 7809 RVA: 0x0008785C File Offset: 0x00085A5C
		public override string StringId
		{
			get
			{
				return "GangLeaderNotableTypeTag";
			}
		}

		// Token: 0x06001E82 RID: 7810 RVA: 0x00087863 File Offset: 0x00085A63
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.Occupation == Occupation.GangLeader;
		}

		// Token: 0x040009AC RID: 2476
		public const string Id = "GangLeaderNotableTypeTag";
	}
}
