using System;
using System.Linq;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class SexistTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "SexistTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			bool flag = character.HeroObject.Clan.Heroes.Any((Hero x) => x.IsFemale && x.IsCommander);
			int num = character.GetTraitLevel(DefaultTraits.Calculating) + character.GetTraitLevel(DefaultTraits.Mercy);
			int num2 = character.GetTraitLevel(DefaultTraits.Valor) + character.GetTraitLevel(DefaultTraits.Generosity);
			return num < 0 && num2 <= 0 && !flag;
		}

		public const string Id = "SexistTag";
	}
}
