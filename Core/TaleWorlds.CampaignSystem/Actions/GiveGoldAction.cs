using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x02000444 RID: 1092
	public static class GiveGoldAction
	{
		// Token: 0x06003F0C RID: 16140 RVA: 0x0012D0B0 File Offset: 0x0012B2B0
		private static void ApplyInternal(Hero giverHero, PartyBase giverParty, Hero recipientHero, PartyBase recipientParty, int goldAmount, bool showQuickInformation, string transactionStringId = "")
		{
			if (giverHero != null)
			{
				goldAmount = MathF.Min(giverHero.Gold, goldAmount);
				giverHero.ChangeHeroGold(-goldAmount);
			}
			else if (giverParty != null && giverParty.IsMobile)
			{
				goldAmount = MathF.Min(giverParty.MobileParty.PartyTradeGold, goldAmount);
				giverParty.MobileParty.PartyTradeGold -= goldAmount;
			}
			else if (giverParty != null && giverParty.IsSettlement)
			{
				SettlementComponent settlementComponent = giverParty.Settlement.SettlementComponent;
				goldAmount = MathF.Min(settlementComponent.Gold, goldAmount);
				settlementComponent.ChangeGold(-goldAmount);
			}
			if (recipientHero != null)
			{
				recipientHero.ChangeHeroGold(goldAmount);
			}
			else if (recipientParty != null && recipientParty.IsMobile)
			{
				recipientParty.MobileParty.PartyTradeGold += goldAmount;
			}
			else if (recipientParty != null && recipientParty.IsSettlement)
			{
				recipientParty.Settlement.SettlementComponent.ChangeGold(goldAmount);
			}
			CampaignEventDispatcher.Instance.OnHeroOrPartyTradedGold(new ValueTuple<Hero, PartyBase>(giverHero, giverParty), new ValueTuple<Hero, PartyBase>(recipientHero, recipientParty), new ValueTuple<int, string>(goldAmount, transactionStringId), showQuickInformation);
		}

		// Token: 0x06003F0D RID: 16141 RVA: 0x0012D1AA File Offset: 0x0012B3AA
		public static void ApplyForQuestBetweenCharacters(Hero giverHero, Hero recipientHero, int amount, bool disableNotification = false)
		{
			GiveGoldAction.ApplyInternal(giverHero, null, recipientHero, null, amount, !disableNotification && (giverHero == Hero.MainHero || recipientHero == Hero.MainHero), "");
		}

		// Token: 0x06003F0E RID: 16142 RVA: 0x0012D1D4 File Offset: 0x0012B3D4
		public static void ApplyBetweenCharacters(Hero giverHero, Hero recipientHero, int amount, bool disableNotification = false)
		{
			GiveGoldAction.ApplyInternal(giverHero, null, recipientHero, null, amount, !disableNotification && (giverHero == Hero.MainHero || recipientHero == Hero.MainHero), "");
		}

		// Token: 0x06003F0F RID: 16143 RVA: 0x0012D1FE File Offset: 0x0012B3FE
		public static void ApplyForCharacterToSettlement(Hero giverHero, Settlement settlement, int amount, bool disableNotification = false)
		{
			GiveGoldAction.ApplyInternal(giverHero, null, null, settlement.Party, amount, !disableNotification && giverHero == Hero.MainHero, "");
		}

		// Token: 0x06003F10 RID: 16144 RVA: 0x0012D222 File Offset: 0x0012B422
		public static void ApplyForSettlementToCharacter(Settlement giverSettlement, Hero recipientHero, int amount, bool disableNotification = false)
		{
			GiveGoldAction.ApplyInternal(recipientHero, null, null, giverSettlement.Party, -amount, !disableNotification && recipientHero == Hero.MainHero, "");
		}

		// Token: 0x06003F11 RID: 16145 RVA: 0x0012D247 File Offset: 0x0012B447
		public static void ApplyForSettlementToParty(Settlement giverSettlement, PartyBase recipientParty, int amount, bool disableNotification = false)
		{
			GiveGoldAction.ApplyInternal(null, giverSettlement.Party, null, recipientParty, amount, !disableNotification && recipientParty.LeaderHero == Hero.MainHero, "");
		}

		// Token: 0x06003F12 RID: 16146 RVA: 0x0012D270 File Offset: 0x0012B470
		public static void ApplyForPartyToSettlement(PartyBase giverParty, Settlement settlement, int amount, bool disableNotification = false)
		{
			GiveGoldAction.ApplyInternal(null, giverParty, null, settlement.Party, amount, !disableNotification && ((giverParty != null) ? giverParty.LeaderHero : null) == Hero.MainHero, "");
		}

		// Token: 0x06003F13 RID: 16147 RVA: 0x0012D29F File Offset: 0x0012B49F
		public static void ApplyForPartyToCharacter(PartyBase giverParty, Hero recipientHero, int amount, bool disableNotification = false)
		{
			GiveGoldAction.ApplyInternal(null, giverParty, recipientHero, null, amount, !disableNotification && giverParty != null && (giverParty.LeaderHero == Hero.MainHero || recipientHero == Hero.MainHero), "");
		}

		// Token: 0x06003F14 RID: 16148 RVA: 0x0012D2D1 File Offset: 0x0012B4D1
		public static void ApplyForCharacterToParty(Hero giverHero, PartyBase receipentParty, int amount, bool disableNotification = false)
		{
			GiveGoldAction.ApplyInternal(giverHero, null, null, receipentParty, amount, !disableNotification && (giverHero == Hero.MainHero || receipentParty.LeaderHero == Hero.MainHero), "");
		}

		// Token: 0x06003F15 RID: 16149 RVA: 0x0012D300 File Offset: 0x0012B500
		public static void ApplyForPartyToParty(PartyBase giverParty, PartyBase receipentParty, int amount, bool disableNotification = false)
		{
			GiveGoldAction.ApplyInternal(null, giverParty, null, receipentParty, amount, !disableNotification && (giverParty.LeaderHero == Hero.MainHero || receipentParty.LeaderHero == Hero.MainHero), "");
		}
	}
}
