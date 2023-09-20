using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class NpcIsNobleTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "NpcIsNobleTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			Hero heroObject = character.HeroObject;
			if (heroObject == null)
			{
				return false;
			}
			Clan clan = heroObject.Clan;
			bool? flag = ((clan != null) ? new bool?(clan.IsNoble) : null);
			bool flag2 = true;
			return (flag.GetValueOrDefault() == flag2) & (flag != null);
		}

		public const string Id = "NpcIsNobleTag";
	}
}
