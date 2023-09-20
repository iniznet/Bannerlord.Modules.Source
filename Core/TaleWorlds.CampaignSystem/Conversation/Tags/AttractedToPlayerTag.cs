using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x0200021F RID: 543
	public class AttractedToPlayerTag : ConversationTag
	{
		// Token: 0x170007A3 RID: 1955
		// (get) Token: 0x06001E63 RID: 7779 RVA: 0x000875ED File Offset: 0x000857ED
		public override string StringId
		{
			get
			{
				return "AttractedToPlayerTag";
			}
		}

		// Token: 0x06001E64 RID: 7780 RVA: 0x000875F4 File Offset: 0x000857F4
		public override bool IsApplicableTo(CharacterObject character)
		{
			Hero heroObject = character.HeroObject;
			return heroObject != null && Hero.MainHero.IsFemale != heroObject.IsFemale && !FactionManager.IsAtWarAgainstFaction(heroObject.MapFaction, Hero.MainHero.MapFaction) && Campaign.Current.Models.RomanceModel.GetAttractionValuePercentage(heroObject, Hero.MainHero) > 70 && heroObject.Spouse == null && Hero.MainHero.Spouse == null;
		}

		// Token: 0x040009A1 RID: 2465
		public const string Id = "AttractedToPlayerTag";

		// Token: 0x040009A2 RID: 2466
		private const int MinimumFlirtPercentageForComment = 70;
	}
}
