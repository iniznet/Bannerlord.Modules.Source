using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class PreacherNotableTypeTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "PreacherNotableTypeTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.Occupation == Occupation.Preacher;
		}

		public const string Id = "PreacherNotableTypeTag";
	}
}
