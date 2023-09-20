using System;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox
{
	public class Give5TroopsToPlayerCheat : GameplayCheatItem
	{
		public override void ExecuteCheat()
		{
			Settlement settlement = SettlementHelper.FindNearestFortification(null, null);
			if (Mission.Current == null && MobileParty.MainParty.MapEvent == null && MobileParty.MainParty.SiegeEvent == null && Campaign.Current.ConversationManager.OneToOneConversationCharacter == null && settlement != null)
			{
				CultureObject culture = settlement.Culture;
				Clan randomElementWithPredicate = Extensions.GetRandomElementWithPredicate<Clan>(Clan.All, (Clan x) => x.Culture != null && (culture == null || culture == x.Culture) && !x.IsMinorFaction && !x.IsBanditFaction);
				int num = PartyBase.MainParty.PartySizeLimit - PartyBase.MainParty.NumberOfAllMembers;
				num = MBMath.ClampInt(num, 0, num);
				int num2 = 5;
				num2 = MBMath.ClampInt(num2, 0, num);
				if (randomElementWithPredicate != null && num2 > 0)
				{
					CharacterObject characterObject = randomElementWithPredicate.Culture.BasicTroop;
					if (MBRandom.RandomFloat < 0.3f && randomElementWithPredicate.Culture.EliteBasicTroop != null)
					{
						characterObject = randomElementWithPredicate.Culture.EliteBasicTroop;
					}
					CharacterObject randomElementInefficiently = Extensions.GetRandomElementInefficiently<CharacterObject>(CharacterHelper.GetTroopTree(characterObject, 1f, float.MaxValue));
					MobileParty.MainParty.AddElementToMemberRoster(randomElementInefficiently, num2, false);
				}
			}
		}

		public override TextObject GetName()
		{
			return new TextObject("{=9FMvBKrV}Give 5 Troops", null);
		}
	}
}
