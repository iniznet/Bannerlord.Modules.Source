using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class AttractedToPlayerTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "AttractedToPlayerTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			Hero heroObject = character.HeroObject;
			return heroObject != null && Hero.MainHero.IsFemale != heroObject.IsFemale && !FactionManager.IsAtWarAgainstFaction(heroObject.MapFaction, Hero.MainHero.MapFaction) && Campaign.Current.Models.RomanceModel.GetAttractionValuePercentage(heroObject, Hero.MainHero) > 70 && heroObject.Spouse == null && Hero.MainHero.Spouse == null;
		}

		public const string Id = "AttractedToPlayerTag";

		private const int MinimumFlirtPercentageForComment = 70;
	}
}
