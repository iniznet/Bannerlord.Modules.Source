using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class AlliedLordTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "PlayerIsAlliedTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && FactionManager.IsAlliedWithFaction(character.HeroObject.MapFaction, Hero.MainHero.MapFaction);
		}

		public const string Id = "PlayerIsAlliedTag";
	}
}
