using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.BarterSystem.Barterables;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003CA RID: 970
	public class RansomOfferCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x17000CE7 RID: 3303
		// (get) Token: 0x06003A23 RID: 14883 RVA: 0x0010BFB4 File Offset: 0x0010A1B4
		private static TextObject RansomPanelTitleText
		{
			get
			{
				return new TextObject("{=ho5EndaV}Decision", null);
			}
		}

		// Token: 0x17000CE8 RID: 3304
		// (get) Token: 0x06003A24 RID: 14884 RVA: 0x0010BFC1 File Offset: 0x0010A1C1
		private static TextObject RansomPanelAffirmativeText
		{
			get
			{
				return new TextObject("{=Y94H6XnK}Accept", null);
			}
		}

		// Token: 0x17000CE9 RID: 3305
		// (get) Token: 0x06003A25 RID: 14885 RVA: 0x0010BFCE File Offset: 0x0010A1CE
		private static TextObject RansomPanelNegativeText
		{
			get
			{
				return new TextObject("{=cOgmdp9e}Decline", null);
			}
		}

		// Token: 0x06003A26 RID: 14886 RVA: 0x0010BFDC File Offset: 0x0010A1DC
		public override void RegisterEvents()
		{
			CampaignEvents.DailyTickHeroEvent.AddNonSerializedListener(this, new Action<Hero>(this.DailyTickHero));
			CampaignEvents.OnRansomOfferedToPlayerEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnRansomOffered));
			CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.OnHeroKilled));
			CampaignEvents.HeroPrisonerReleased.AddNonSerializedListener(this, new Action<Hero, PartyBase, IFaction, EndCaptivityDetail>(this.OnHeroReleased));
			CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.HourlyTick));
			CampaignEvents.PrisonersChangeInSettlement.AddNonSerializedListener(this, new Action<Settlement, FlattenedTroopRoster, Hero, bool>(this.OnPrisonersChangeInSettlement));
			CampaignEvents.HeroPrisonerTaken.AddNonSerializedListener(this, new Action<PartyBase, Hero>(this.OnHeroPrisonerTaken));
		}

		// Token: 0x06003A27 RID: 14887 RVA: 0x0010C08A File Offset: 0x0010A28A
		private void OnHeroPrisonerTaken(PartyBase party, Hero hero)
		{
			this.HandleDeclineRansomOffer(hero);
		}

		// Token: 0x06003A28 RID: 14888 RVA: 0x0010C094 File Offset: 0x0010A294
		private void DailyTickHero(Hero hero)
		{
			if (hero.IsPrisoner && hero.Clan != null && hero.PartyBelongedToAsPrisoner != null && !hero.PartyBelongedToAsPrisoner.MapFaction.IsBanditFaction && hero.PartyBelongedToAsPrisoner.MapFaction != null && hero != Hero.MainHero && hero.Clan.Lords.Count > 1)
			{
				this.ConsiderRansomPrisoner(hero);
			}
		}

		// Token: 0x06003A29 RID: 14889 RVA: 0x0010C0FC File Offset: 0x0010A2FC
		private void ConsiderRansomPrisoner(Hero hero)
		{
			Clan captorClanOfPrisoner = this.GetCaptorClanOfPrisoner(hero);
			if (captorClanOfPrisoner != null)
			{
				Hero hero2 = ((hero.Clan.Leader != hero) ? hero.Clan.Leader : hero.Clan.Lords.Where((Hero t) => t != hero.Clan.Leader).GetRandomElementInefficiently<Hero>());
				if (hero2 != Hero.MainHero || !hero2.IsPrisoner)
				{
					if (captorClanOfPrisoner == Clan.PlayerClan || hero.Clan == Clan.PlayerClan)
					{
						if (this._currentRansomHero == null)
						{
							float num = ((!this._heroesWithDeclinedRansomOffers.Contains(hero)) ? 0.2f : 0.12f);
							if (MBRandom.RandomFloat < num)
							{
								float num2 = (float)new SetPrisonerFreeBarterable(hero, captorClanOfPrisoner.Leader, hero.PartyBelongedToAsPrisoner, hero2).GetUnitValueForFaction(hero.Clan) * 1.1f;
								if (MBRandom.RandomFloat < num && (float)(hero2.Gold + 1000) >= num2)
								{
									this.SetCurrentRansomHero(hero, hero2);
									StringHelpers.SetCharacterProperties("CAPTIVE_HERO", hero.CharacterObject, RansomOfferCampaignBehavior.RansomOfferDescriptionText, false);
									Campaign.Current.CampaignInformationManager.NewMapNoticeAdded(new RansomOfferMapNotification(hero, RansomOfferCampaignBehavior.RansomOfferDescriptionText));
									return;
								}
							}
						}
					}
					else if (MBRandom.RandomFloat < 0.1f)
					{
						SetPrisonerFreeBarterable setPrisonerFreeBarterable = new SetPrisonerFreeBarterable(hero, captorClanOfPrisoner.Leader, hero.PartyBelongedToAsPrisoner, hero2);
						if (setPrisonerFreeBarterable.GetValueForFaction(captorClanOfPrisoner) + setPrisonerFreeBarterable.GetValueForFaction(hero.Clan) > 0)
						{
							Campaign.Current.BarterManager.ExecuteAiBarter(captorClanOfPrisoner, hero.Clan, captorClanOfPrisoner.Leader, hero2, setPrisonerFreeBarterable);
						}
					}
				}
			}
		}

		// Token: 0x06003A2A RID: 14890 RVA: 0x0010C2EC File Offset: 0x0010A4EC
		private Clan GetCaptorClanOfPrisoner(Hero hero)
		{
			Clan clan;
			if (hero.PartyBelongedToAsPrisoner.IsMobile)
			{
				if ((hero.PartyBelongedToAsPrisoner.MobileParty.IsMilitia || hero.PartyBelongedToAsPrisoner.MobileParty.IsGarrison || hero.PartyBelongedToAsPrisoner.MobileParty.IsCaravan || hero.PartyBelongedToAsPrisoner.MobileParty.IsVillager) && hero.PartyBelongedToAsPrisoner.Owner != null)
				{
					if (hero.PartyBelongedToAsPrisoner.Owner.IsNotable)
					{
						clan = hero.PartyBelongedToAsPrisoner.Owner.CurrentSettlement.OwnerClan;
					}
					else
					{
						clan = hero.PartyBelongedToAsPrisoner.Owner.Clan;
					}
				}
				else
				{
					clan = hero.PartyBelongedToAsPrisoner.MobileParty.ActualClan;
				}
			}
			else
			{
				clan = hero.PartyBelongedToAsPrisoner.Settlement.OwnerClan;
			}
			return clan;
		}

		// Token: 0x06003A2B RID: 14891 RVA: 0x0010C3C2 File Offset: 0x0010A5C2
		private void SetCurrentRansomHero(Hero hero, Hero ransomPayer = null)
		{
			this._currentRansomHero = hero;
			this._currentRansomPayer = ransomPayer;
			this._currentRansomOfferDate = ((hero != null) ? CampaignTime.Now : CampaignTime.Never);
		}

		// Token: 0x06003A2C RID: 14892 RVA: 0x0010C3E8 File Offset: 0x0010A5E8
		private void OnRansomOffered(Hero captiveHero)
		{
			Clan captorClanOfPrisoner = this.GetCaptorClanOfPrisoner(captiveHero);
			Clan clan = ((captiveHero.Clan == Clan.PlayerClan) ? captorClanOfPrisoner : captiveHero.Clan);
			Hero hero = ((captiveHero.Clan.Leader != captiveHero) ? captiveHero.Clan.Leader : captiveHero.Clan.Lords.Where((Hero t) => t != captiveHero.Clan.Leader).GetRandomElementInefficiently<Hero>());
			int ransomPrice = (int)Math.Min((float)hero.Gold, (float)new SetPrisonerFreeBarterable(captiveHero, captorClanOfPrisoner.Leader, captiveHero.PartyBelongedToAsPrisoner, hero).GetUnitValueForFaction(captiveHero.Clan) * 1.1f);
			TextObject textObject = ((captorClanOfPrisoner == Clan.PlayerClan) ? RansomOfferCampaignBehavior.RansomPanelDescriptionPlayerHeldPrisonerText : RansomOfferCampaignBehavior.RansomPanelDescriptionNpcHeldPrisonerText);
			textObject.SetTextVariable("CLAN_NAME", clan.Name);
			textObject.SetTextVariable("GOLD_AMOUNT", ransomPrice);
			textObject.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
			StringHelpers.SetCharacterProperties("CAPTIVE_HERO", captiveHero.CharacterObject, textObject, false);
			InformationManager.ShowInquiry(new InquiryData(RansomOfferCampaignBehavior.RansomPanelTitleText.ToString(), textObject.ToString(), true, true, RansomOfferCampaignBehavior.RansomPanelAffirmativeText.ToString(), RansomOfferCampaignBehavior.RansomPanelNegativeText.ToString(), delegate
			{
				this.AcceptRansomOffer(ransomPrice);
			}, new Action(this.DeclineRansomOffer), "", 0f, null, null, null), true, false);
		}

		// Token: 0x06003A2D RID: 14893 RVA: 0x0010C590 File Offset: 0x0010A790
		private void AcceptRansomOffer(int ransomPrice)
		{
			if (this._heroesWithDeclinedRansomOffers.Contains(this._currentRansomHero))
			{
				this._heroesWithDeclinedRansomOffers.Remove(this._currentRansomHero);
			}
			GiveGoldAction.ApplyBetweenCharacters(this._currentRansomPayer, this.GetCaptorClanOfPrisoner(this._currentRansomHero).Leader, ransomPrice, false);
			EndCaptivityAction.ApplyByRansom(this._currentRansomHero, this._currentRansomHero.Clan.Leader);
			IStatisticsCampaignBehavior behavior = Campaign.Current.CampaignBehaviorManager.GetBehavior<IStatisticsCampaignBehavior>();
			if (behavior != null)
			{
				behavior.OnPlayerAcceptedRansomOffer(ransomPrice);
			}
		}

		// Token: 0x06003A2E RID: 14894 RVA: 0x0010C618 File Offset: 0x0010A818
		private void DeclineRansomOffer()
		{
			if (this._currentRansomHero.IsPrisoner && this._currentRansomHero.IsAlive && !this._heroesWithDeclinedRansomOffers.Contains(this._currentRansomHero))
			{
				this._heroesWithDeclinedRansomOffers.Add(this._currentRansomHero);
			}
			this.SetCurrentRansomHero(null, null);
		}

		// Token: 0x06003A2F RID: 14895 RVA: 0x0010C66B File Offset: 0x0010A86B
		private void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
		{
			this.HandleDeclineRansomOffer(victim);
		}

		// Token: 0x06003A30 RID: 14896 RVA: 0x0010C674 File Offset: 0x0010A874
		private void HandleDeclineRansomOffer(Hero victim)
		{
			if (this._currentRansomHero != null && (victim == this._currentRansomHero || victim == Hero.MainHero))
			{
				CampaignEventDispatcher.Instance.OnRansomOfferCancelled(this._currentRansomHero);
				this.DeclineRansomOffer();
			}
		}

		// Token: 0x06003A31 RID: 14897 RVA: 0x0010C6A8 File Offset: 0x0010A8A8
		private void OnPrisonersChangeInSettlement(Settlement settlement, FlattenedTroopRoster roster, Hero prisoner, bool takenFromDungeon)
		{
			if (!takenFromDungeon && this._currentRansomHero != null)
			{
				if (prisoner == this._currentRansomHero)
				{
					CampaignEventDispatcher.Instance.OnRansomOfferCancelled(this._currentRansomHero);
					this.DeclineRansomOffer();
					return;
				}
				if (roster != null)
				{
					foreach (FlattenedTroopRosterElement flattenedTroopRosterElement in roster)
					{
						if (flattenedTroopRosterElement.Troop.IsHero && flattenedTroopRosterElement.Troop.HeroObject == this._currentRansomHero)
						{
							CampaignEventDispatcher.Instance.OnRansomOfferCancelled(this._currentRansomHero);
							this.DeclineRansomOffer();
							break;
						}
					}
				}
			}
		}

		// Token: 0x06003A32 RID: 14898 RVA: 0x0010C758 File Offset: 0x0010A958
		private void OnHeroReleased(Hero prisoner, PartyBase party, IFaction capturerFaction, EndCaptivityDetail detail)
		{
			this.HandleDeclineRansomOffer(prisoner);
		}

		// Token: 0x06003A33 RID: 14899 RVA: 0x0010C761 File Offset: 0x0010A961
		private void HourlyTick()
		{
			if (this._currentRansomHero != null && this._currentRansomOfferDate.ElapsedHoursUntilNow >= 48f)
			{
				CampaignEventDispatcher.Instance.OnRansomOfferCancelled(this._currentRansomHero);
				this.DeclineRansomOffer();
			}
		}

		// Token: 0x06003A34 RID: 14900 RVA: 0x0010C794 File Offset: 0x0010A994
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<List<Hero>>("_heroesWithDeclinedRansomOffers", ref this._heroesWithDeclinedRansomOffers);
			dataStore.SyncData<Hero>("_currentRansomHero", ref this._currentRansomHero);
			dataStore.SyncData<Hero>("_currentRansomPayer", ref this._currentRansomPayer);
			dataStore.SyncData<CampaignTime>("_currentRansomOfferDate", ref this._currentRansomOfferDate);
		}

		// Token: 0x06003A35 RID: 14901 RVA: 0x0010C7EC File Offset: 0x0010A9EC
		[CommandLineFunctionality.CommandLineArgumentFunction("trigger_ransom_offer", "campaign")]
		public static string TriggerRansomOffer(List<string> strings)
		{
			Hero randomElementInefficiently = Hero.AllAliveHeroes.Where((Hero t) => t.IsLord && !t.IsPrisoner).GetRandomElementInefficiently<Hero>();
			TakePrisonerAction.Apply(PartyBase.MainParty, randomElementInefficiently);
			Campaign.Current.CampaignBehaviorManager.GetBehavior<RansomOfferCampaignBehavior>().SetCurrentRansomHero(randomElementInefficiently, randomElementInefficiently.Clan.Leader);
			CampaignEventDispatcher.Instance.OnRansomOfferedToPlayer(randomElementInefficiently);
			return "success";
		}

		// Token: 0x06003A36 RID: 14902 RVA: 0x0010C864 File Offset: 0x0010AA64
		[CommandLineFunctionality.CommandLineArgumentFunction("establish_ransom_offer_conditions", "campaign")]
		public static string EstablishRansomOfferConditions(List<string> strings)
		{
			Hero randomElementInefficiently = Hero.AllAliveHeroes.Where((Hero t) => t.IsLord && !t.IsPrisoner).GetRandomElementInefficiently<Hero>();
			TakePrisonerAction.Apply(PartyBase.MainParty, randomElementInefficiently);
			return "success";
		}

		// Token: 0x06003A37 RID: 14903 RVA: 0x0010C8B0 File Offset: 0x0010AAB0
		[CommandLineFunctionality.CommandLineArgumentFunction("establish_ransom_offer_conditions_for_clan_member", "campaign")]
		public static string EstablishRansomOfferConditionsForPlayerClanMember(List<string> strings)
		{
			Hero randomElementInefficiently = Hero.AllAliveHeroes.Where((Hero t) => t.IsLord && !t.IsPrisoner && t.Clan != Clan.PlayerClan && t.Clan.Leader != t).GetRandomElementInefficiently<Hero>();
			randomElementInefficiently.Clan = Clan.PlayerClan;
			TakePrisonerAction.Apply(Clan.All.Where((Clan t) => t != Clan.PlayerClan).GetRandomElementInefficiently<Clan>().WarPartyComponents.GetRandomElement<WarPartyComponent>().Party, randomElementInefficiently);
			return "success";
		}

		// Token: 0x040011E2 RID: 4578
		private const float RansomOfferInitialChance = 0.2f;

		// Token: 0x040011E3 RID: 4579
		private const float RansomOfferChanceAfterRefusal = 0.12f;

		// Token: 0x040011E4 RID: 4580
		private const float RansomOfferChanceForPrisonersKeptByAI = 0.1f;

		// Token: 0x040011E5 RID: 4581
		private const float MapNotificationAutoDeclineDurationInHours = 48f;

		// Token: 0x040011E6 RID: 4582
		private const int AmountOfGoldLeftAfterRansom = 1000;

		// Token: 0x040011E7 RID: 4583
		private static TextObject RansomOfferDescriptionText = new TextObject("{=ZqJ92UN4}A courier with a ransom offer for the freedom of {CAPTIVE_HERO.NAME} has arrived", null);

		// Token: 0x040011E8 RID: 4584
		private static TextObject RansomPanelDescriptionNpcHeldPrisonerText = new TextObject("{=4fXpOe4N}A courier arrives from the {CLAN_NAME}. They hold {CAPTIVE_HERO.NAME} and are demanding {GOLD_AMOUNT}{GOLD_ICON} in ransom.", null);

		// Token: 0x040011E9 RID: 4585
		private static TextObject RansomPanelDescriptionPlayerHeldPrisonerText = new TextObject("{=PutoRsWp}A courier arrives from the {CLAN_NAME}. They offer you {GOLD_AMOUNT}{GOLD_ICON} in ransom if you will free {CAPTIVE_HERO.NAME}.", null);

		// Token: 0x040011EA RID: 4586
		private List<Hero> _heroesWithDeclinedRansomOffers = new List<Hero>();

		// Token: 0x040011EB RID: 4587
		private Hero _currentRansomHero;

		// Token: 0x040011EC RID: 4588
		private Hero _currentRansomPayer;

		// Token: 0x040011ED RID: 4589
		private CampaignTime _currentRansomOfferDate;
	}
}
