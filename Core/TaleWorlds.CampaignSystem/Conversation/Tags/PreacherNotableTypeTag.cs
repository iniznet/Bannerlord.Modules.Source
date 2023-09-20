using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000227 RID: 551
	public class PreacherNotableTypeTag : ConversationTag
	{
		// Token: 0x170007AB RID: 1963
		// (get) Token: 0x06001E7B RID: 7803 RVA: 0x00087812 File Offset: 0x00085A12
		public override string StringId
		{
			get
			{
				return "PreacherNotableTypeTag";
			}
		}

		// Token: 0x06001E7C RID: 7804 RVA: 0x00087819 File Offset: 0x00085A19
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.Occupation == Occupation.Preacher;
		}

		// Token: 0x040009AA RID: 2474
		public const string Id = "PreacherNotableTypeTag";
	}
}
