using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x020001F6 RID: 502
	public class NpcIsLiegeTag : ConversationTag
	{
		// Token: 0x1700077A RID: 1914
		// (get) Token: 0x06001DE8 RID: 7656 RVA: 0x00086B86 File Offset: 0x00084D86
		public override string StringId
		{
			get
			{
				return "NpcIsLiegeTag";
			}
		}

		// Token: 0x06001DE9 RID: 7657 RVA: 0x00086B8D File Offset: 0x00084D8D
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.IsFactionLeader;
		}

		// Token: 0x04000978 RID: 2424
		public const string Id = "NpcIsLiegeTag";
	}
}
