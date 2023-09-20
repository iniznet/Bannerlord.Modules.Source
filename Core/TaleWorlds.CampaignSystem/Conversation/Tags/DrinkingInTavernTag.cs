using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x020001FA RID: 506
	public class DrinkingInTavernTag : ConversationTag
	{
		// Token: 0x1700077E RID: 1918
		// (get) Token: 0x06001DF4 RID: 7668 RVA: 0x00086C7A File Offset: 0x00084E7A
		public override string StringId
		{
			get
			{
				return "DrinkingInTavernTag";
			}
		}

		// Token: 0x06001DF5 RID: 7669 RVA: 0x00086C84 File Offset: 0x00084E84
		public override bool IsApplicableTo(CharacterObject character)
		{
			if (LocationComplex.Current != null && character.IsHero)
			{
				Location locationOfCharacter = LocationComplex.Current.GetLocationOfCharacter(character.HeroObject);
				Location locationWithId = LocationComplex.Current.GetLocationWithId("tavern");
				if (character.HeroObject.IsWanderer && Settlement.CurrentSettlement != null && locationWithId == locationOfCharacter)
				{
					return true;
				}
			}
			else if (character.HeroObject == null && LocationComplex.Current != null && Settlement.CurrentSettlement != null && LocationComplex.Current.GetLocationWithId("tavern") == CampaignMission.Current.Location)
			{
				return true;
			}
			return false;
		}

		// Token: 0x0400097C RID: 2428
		public const string Id = "DrinkingInTavernTag";
	}
}
