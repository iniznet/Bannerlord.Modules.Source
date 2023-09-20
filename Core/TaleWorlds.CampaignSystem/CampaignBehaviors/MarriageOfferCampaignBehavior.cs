using System;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class MarriageOfferCampaignBehavior : CampaignBehaviorBase
	{
		private static TextObject DecisionPopUpTitleText
		{
			get
			{
				return new TextObject("{=ho5EndaV}Decision", null);
			}
		}

		private static TextObject DecisionPopUpAffirmativeText
		{
			get
			{
				return new TextObject("{=Y94H6XnK}Accept", null);
			}
		}

		private static TextObject DecisionPopUpNegativeText
		{
			get
			{
				return new TextObject("{=cOgmdp9e}Decline", null);
			}
		}

		private bool _isThereActiveMarriageOffer
		{
			get
			{
				return this._currentOfferedPlayerClanHero != null && this._currentOfferedOtherClanHero != null;
			}
		}

		public override void RegisterEvents()
		{
			CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, new Action(this.OnGameLoadFinished));
			CampaignEvents.DailyTickClanEvent.AddNonSerializedListener(this, new Action<Clan>(this.DailyTickClan));
			CampaignEvents.OnMarriageOfferedToPlayerEvent.AddNonSerializedListener(this, new Action<Hero, Hero>(this.OnMarriageOfferedToPlayer));
			CampaignEvents.OnMarriageOfferCanceledEvent.AddNonSerializedListener(this, new Action<Hero, Hero>(this.OnMarriageOfferCanceled));
			CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.HourlyTick));
			CampaignEvents.HeroPrisonerTaken.AddNonSerializedListener(this, new Action<PartyBase, Hero>(this.OnHeroPrisonerTaken));
			CampaignEvents.HeroesMarried.AddNonSerializedListener(this, new Action<Hero, Hero, bool>(this.OnHeroesMarried));
			CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.OnHeroKilled));
			CampaignEvents.ArmyCreated.AddNonSerializedListener(this, new Action<Army>(this.OnArmyCreated));
			CampaignEvents.MapEventStarted.AddNonSerializedListener(this, new Action<MapEvent, PartyBase, PartyBase>(this.OnMapEventStarted));
			CampaignEvents.CharacterBecameFugitive.AddNonSerializedListener(this, new Action<Hero>(this.CharacterBecameFugitive));
			CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction, DeclareWarAction.DeclareWarDetail>(this.OnWarDeclared));
			CampaignEvents.HeroRelationChanged.AddNonSerializedListener(this, new Action<Hero, Hero, int, bool, ChangeRelationAction.ChangeRelationDetail, Hero, Hero>(this.OnHeroRelationChanged));
			CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangedKingdom));
		}

		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Hero>("_currentOfferedPlayerClanHero", ref this._currentOfferedPlayerClanHero);
			dataStore.SyncData<Hero>("_currentOfferedOtherClanHero", ref this._currentOfferedOtherClanHero);
			dataStore.SyncData<CampaignTime>("_lastMarriageOfferTime", ref this._lastMarriageOfferTime);
		}

		private void OnGameLoadFinished()
		{
			if (this._isThereActiveMarriageOffer && !Campaign.Current.Models.MarriageModel.IsCoupleSuitableForMarriage(this._currentOfferedPlayerClanHero, this._currentOfferedOtherClanHero))
			{
				CampaignEventDispatcher.Instance.OnMarriageOfferCanceled(this._currentOfferedPlayerClanHero.IsFemale ? this._currentOfferedOtherClanHero : this._currentOfferedPlayerClanHero, this._currentOfferedPlayerClanHero.IsFemale ? this._currentOfferedPlayerClanHero : this._currentOfferedOtherClanHero);
			}
		}

		private void DailyTickClan(Clan consideringClan)
		{
			if (this.CanOfferMarriageForClan(consideringClan))
			{
				float distance = Campaign.Current.Models.MapDistanceModel.GetDistance(Clan.PlayerClan.FactionMidSettlement, consideringClan.FactionMidSettlement);
				if (MBRandom.RandomFloat >= distance / Campaign.MaximumDistanceBetweenTwoSettlements - 0.5f)
				{
					foreach (Hero hero in Clan.PlayerClan.Heroes)
					{
						if (hero != Hero.MainHero && hero.CanMarry() && this.ConsiderMarriageForPlayerClanMember(hero, consideringClan))
						{
							break;
						}
					}
				}
			}
		}

		private void OnMarriageOfferedToPlayer(Hero suitor, Hero maiden)
		{
			this.CreateMarriageOfferDecisionPopUp();
		}

		private void OnMarriageOfferCanceled(Hero suitor, Hero maiden)
		{
			this.FinalizeMarriageOffer();
		}

		private void HourlyTick()
		{
			if (this._isThereActiveMarriageOffer && this._lastMarriageOfferTime.ElapsedHoursUntilNow >= 48f)
			{
				CampaignEventDispatcher.Instance.OnMarriageOfferCanceled(this._currentOfferedPlayerClanHero.IsFemale ? this._currentOfferedOtherClanHero : this._currentOfferedPlayerClanHero, this._currentOfferedPlayerClanHero.IsFemale ? this._currentOfferedPlayerClanHero : this._currentOfferedOtherClanHero);
			}
		}

		private void OnHeroPrisonerTaken(PartyBase capturer, Hero prisoner)
		{
			if (this._isThereActiveMarriageOffer && (prisoner == Hero.MainHero || prisoner == this._currentOfferedPlayerClanHero || prisoner == this._currentOfferedOtherClanHero))
			{
				CampaignEventDispatcher.Instance.OnMarriageOfferCanceled(this._currentOfferedPlayerClanHero.IsFemale ? this._currentOfferedOtherClanHero : this._currentOfferedPlayerClanHero, this._currentOfferedPlayerClanHero.IsFemale ? this._currentOfferedPlayerClanHero : this._currentOfferedOtherClanHero);
			}
		}

		private void OnHeroesMarried(Hero hero1, Hero hero2, bool showNotification = true)
		{
			if (this._isThereActiveMarriageOffer && ((hero1 == this._currentOfferedPlayerClanHero && hero2 == this._currentOfferedOtherClanHero) || (hero1 == this._currentOfferedOtherClanHero && hero2 == this._currentOfferedPlayerClanHero)))
			{
				CampaignEventDispatcher.Instance.OnMarriageOfferCanceled(this._currentOfferedPlayerClanHero.IsFemale ? this._currentOfferedOtherClanHero : this._currentOfferedPlayerClanHero, this._currentOfferedPlayerClanHero.IsFemale ? this._currentOfferedPlayerClanHero : this._currentOfferedOtherClanHero);
			}
		}

		private void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
		{
			if (this._isThereActiveMarriageOffer && (victim == Hero.MainHero || victim == this._currentOfferedPlayerClanHero || victim == this._currentOfferedOtherClanHero))
			{
				CampaignEventDispatcher.Instance.OnMarriageOfferCanceled(this._currentOfferedPlayerClanHero.IsFemale ? this._currentOfferedOtherClanHero : this._currentOfferedPlayerClanHero, this._currentOfferedPlayerClanHero.IsFemale ? this._currentOfferedPlayerClanHero : this._currentOfferedOtherClanHero);
			}
		}

		private void OnArmyCreated(Army army)
		{
			if (this._isThereActiveMarriageOffer)
			{
				MobileParty partyBelongedTo = this._currentOfferedPlayerClanHero.PartyBelongedTo;
				if (((partyBelongedTo != null) ? partyBelongedTo.Army : null) == null)
				{
					MobileParty partyBelongedTo2 = this._currentOfferedOtherClanHero.PartyBelongedTo;
					if (((partyBelongedTo2 != null) ? partyBelongedTo2.Army : null) == null)
					{
						return;
					}
				}
				CampaignEventDispatcher.Instance.OnMarriageOfferCanceled(this._currentOfferedPlayerClanHero.IsFemale ? this._currentOfferedOtherClanHero : this._currentOfferedPlayerClanHero, this._currentOfferedPlayerClanHero.IsFemale ? this._currentOfferedPlayerClanHero : this._currentOfferedOtherClanHero);
			}
		}

		private void OnMapEventStarted(MapEvent mapEvent, PartyBase attackerParty, PartyBase defenderParty)
		{
			if (this._isThereActiveMarriageOffer)
			{
				MobileParty partyBelongedTo = this._currentOfferedPlayerClanHero.PartyBelongedTo;
				if (((partyBelongedTo != null) ? partyBelongedTo.MapEvent : null) == null)
				{
					MobileParty partyBelongedTo2 = this._currentOfferedOtherClanHero.PartyBelongedTo;
					if (((partyBelongedTo2 != null) ? partyBelongedTo2.MapEvent : null) == null)
					{
						return;
					}
				}
				CampaignEventDispatcher.Instance.OnMarriageOfferCanceled(this._currentOfferedPlayerClanHero.IsFemale ? this._currentOfferedOtherClanHero : this._currentOfferedPlayerClanHero, this._currentOfferedPlayerClanHero.IsFemale ? this._currentOfferedPlayerClanHero : this._currentOfferedOtherClanHero);
			}
		}

		private void CharacterBecameFugitive(Hero hero)
		{
			if (this._isThereActiveMarriageOffer && (!this._currentOfferedPlayerClanHero.IsActive || !this._currentOfferedOtherClanHero.IsActive))
			{
				CampaignEventDispatcher.Instance.OnMarriageOfferCanceled(this._currentOfferedPlayerClanHero.IsFemale ? this._currentOfferedOtherClanHero : this._currentOfferedPlayerClanHero, this._currentOfferedPlayerClanHero.IsFemale ? this._currentOfferedPlayerClanHero : this._currentOfferedOtherClanHero);
			}
		}

		private void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail declareWarDetail)
		{
			if (this._isThereActiveMarriageOffer && (!Campaign.Current.Models.MarriageModel.IsCoupleSuitableForMarriage(this._currentOfferedPlayerClanHero, this._currentOfferedOtherClanHero) || !Campaign.Current.Models.MarriageModel.ShouldNpcMarriageBetweenClansBeAllowed(Clan.PlayerClan, this._currentOfferedOtherClanHero.Clan)))
			{
				CampaignEventDispatcher.Instance.OnMarriageOfferCanceled(this._currentOfferedPlayerClanHero.IsFemale ? this._currentOfferedOtherClanHero : this._currentOfferedPlayerClanHero, this._currentOfferedPlayerClanHero.IsFemale ? this._currentOfferedPlayerClanHero : this._currentOfferedOtherClanHero);
			}
		}

		private void OnHeroRelationChanged(Hero effectiveHero, Hero effectiveHeroGainedRelationWith, int relationChange, bool showNotification, ChangeRelationAction.ChangeRelationDetail detail, Hero originalHero, Hero originalGainedRelationWith)
		{
			if (this._isThereActiveMarriageOffer && (effectiveHero.Clan == this._currentOfferedPlayerClanHero.Clan || effectiveHero.Clan == this._currentOfferedOtherClanHero.Clan) && (effectiveHeroGainedRelationWith.Clan == this._currentOfferedPlayerClanHero.Clan || effectiveHeroGainedRelationWith.Clan == this._currentOfferedOtherClanHero.Clan) && !Campaign.Current.Models.MarriageModel.ShouldNpcMarriageBetweenClansBeAllowed(this._currentOfferedPlayerClanHero.Clan, this._currentOfferedOtherClanHero.Clan))
			{
				CampaignEventDispatcher.Instance.OnMarriageOfferCanceled(this._currentOfferedPlayerClanHero.IsFemale ? this._currentOfferedOtherClanHero : this._currentOfferedPlayerClanHero, this._currentOfferedPlayerClanHero.IsFemale ? this._currentOfferedPlayerClanHero : this._currentOfferedOtherClanHero);
			}
		}

		private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification = true)
		{
			if (this._isThereActiveMarriageOffer && (this._currentOfferedPlayerClanHero.Clan == clan || this._currentOfferedOtherClanHero.Clan == clan) && !Campaign.Current.Models.MarriageModel.ShouldNpcMarriageBetweenClansBeAllowed(this._currentOfferedPlayerClanHero.Clan, this._currentOfferedOtherClanHero.Clan))
			{
				CampaignEventDispatcher.Instance.OnMarriageOfferCanceled(this._currentOfferedPlayerClanHero.IsFemale ? this._currentOfferedOtherClanHero : this._currentOfferedPlayerClanHero, this._currentOfferedPlayerClanHero.IsFemale ? this._currentOfferedPlayerClanHero : this._currentOfferedOtherClanHero);
			}
		}

		private bool CanOfferMarriageForClan(Clan consideringClan)
		{
			return !this._isThereActiveMarriageOffer && this._lastMarriageOfferTime.ElapsedWeeksUntilNow >= 1f && !Hero.MainHero.IsPrisoner && consideringClan != Clan.PlayerClan && Campaign.Current.Models.MarriageModel.IsClanSuitableForMarriage(consideringClan) && Campaign.Current.Models.MarriageModel.ShouldNpcMarriageBetweenClansBeAllowed(Clan.PlayerClan, consideringClan);
		}

		private bool ConsiderMarriageForPlayerClanMember(Hero playerClanHero, Clan consideringClan)
		{
			MarriageModel marriageModel = Campaign.Current.Models.MarriageModel;
			foreach (Hero hero in consideringClan.Heroes)
			{
				float num = marriageModel.NpcCoupleMarriageChance(playerClanHero, hero);
				if (num > 0f && MBRandom.RandomFloat < num)
				{
					foreach (Romance.RomanticState romanticState in Romance.RomanticStateList)
					{
						if (romanticState.Level >= Romance.RomanceLevelEnum.MatchMadeByFamily && (romanticState.Person1 == playerClanHero || romanticState.Person2 == playerClanHero || romanticState.Person1 == hero || romanticState.Person2 == hero))
						{
							return false;
						}
					}
					this.CreateMarriageOffer(playerClanHero, hero);
					return true;
				}
			}
			return false;
		}

		private void CreateMarriageOffer(Hero currentOfferedPlayerClanHero, Hero currentOfferedOtherClanHero)
		{
			this._currentOfferedPlayerClanHero = currentOfferedPlayerClanHero;
			this._currentOfferedOtherClanHero = currentOfferedOtherClanHero;
			this._lastMarriageOfferTime = CampaignTime.Now;
			MarriageOfferCampaignBehavior.MarriageOfferPanelExplanationText.SetCharacterProperties("CLAN_MEMBER", this._currentOfferedPlayerClanHero.CharacterObject, false);
			MarriageOfferCampaignBehavior.MarriageOfferPanelExplanationText.SetCharacterProperties("OFFERED_HERO", this._currentOfferedOtherClanHero.CharacterObject, false);
			Campaign.Current.CampaignInformationManager.NewMapNoticeAdded(new MarriageOfferMapNotification(this._currentOfferedPlayerClanHero, this._currentOfferedOtherClanHero, MarriageOfferCampaignBehavior.MarriageOfferPanelExplanationText));
		}

		private void CreateMarriageOfferDecisionPopUp()
		{
			TextObject textObject;
			if (this._currentOfferedOtherClanHero == this._currentOfferedPlayerClanHero.Clan.Leader)
			{
				textObject = MarriageOfferCampaignBehavior.DecisionPopUpExplanationTextForLeaderOffer;
			}
			else
			{
				textObject = MarriageOfferCampaignBehavior.DecisionPopUpExplanationTextForRegularOffer;
				textObject.SetCharacterProperties("OFFERED_HERO", this._currentOfferedOtherClanHero.CharacterObject, false);
			}
			textObject.SetTextVariable("CLAN_NAME", this._currentOfferedOtherClanHero.Clan.Name);
			textObject.SetCharacterProperties("CLAN_LEADER", this._currentOfferedOtherClanHero.Clan.Leader.CharacterObject, false);
			textObject.SetCharacterProperties("PLAYER_CLAN_HERO", this._currentOfferedPlayerClanHero.CharacterObject, false);
			InformationManager.ShowInquiry(new InquiryData(MarriageOfferCampaignBehavior.DecisionPopUpTitleText.ToString(), textObject.ToString(), true, true, MarriageOfferCampaignBehavior.DecisionPopUpAffirmativeText.ToString(), MarriageOfferCampaignBehavior.DecisionPopUpNegativeText.ToString(), new Action(this.OnMarriageOfferAcceptedOnPopUp), new Action(this.OnMarriageOfferDeclinedOnPopUp), "", 0f, null, null, null), false, false);
		}

		private void OnMarriageOfferAcceptedOnPopUp()
		{
			Hero hero = (this._currentOfferedPlayerClanHero.IsFemale ? this._currentOfferedOtherClanHero : this._currentOfferedPlayerClanHero);
			Hero hero2 = (this._currentOfferedPlayerClanHero.IsFemale ? this._currentOfferedPlayerClanHero : this._currentOfferedOtherClanHero);
			MBInformationManager.ShowSceneNotification(new MarriageSceneNotificationItem(hero, hero2, SceneNotificationData.RelevantContextType.Any));
			ChangeRelationAction.ApplyPlayerRelation(this._currentOfferedOtherClanHero.Clan.Leader, 10, true, true);
			MarriageAction.Apply(this._currentOfferedPlayerClanHero, this._currentOfferedOtherClanHero, true);
			this.FinalizeMarriageOffer();
		}

		private void OnMarriageOfferDeclinedOnPopUp()
		{
			CampaignEventDispatcher.Instance.OnMarriageOfferCanceled(this._currentOfferedPlayerClanHero.IsFemale ? this._currentOfferedOtherClanHero : this._currentOfferedPlayerClanHero, this._currentOfferedPlayerClanHero.IsFemale ? this._currentOfferedPlayerClanHero : this._currentOfferedOtherClanHero);
		}

		private void FinalizeMarriageOffer()
		{
			this._currentOfferedPlayerClanHero = null;
			this._currentOfferedOtherClanHero = null;
		}

		private const int MarriageOfferCooldownDurationAsWeeks = 1;

		private const int OfferRelationGainAmountWithTheMarriageClan = 10;

		private const float MapNotificationAutoDeclineDurationInHours = 48f;

		private static TextObject DecisionPopUpExplanationTextForLeaderOffer = new TextObject("{=EQSIg1W3}A courier arrives from {CLAN_LEADER.NAME}, head of the {CLAN_NAME}. {?CLAN_LEADER.GENDER}She{?}He{\\?} proposes that {?CLAN_LEADER.GENDER}herself{?}himself{\\?} marry {PLAYER_CLAN_HERO.NAME}, from your clan. The couple appear to be compatible. Do you accept?", null);

		private static TextObject DecisionPopUpExplanationTextForRegularOffer = new TextObject("{=bipaJ1c4}A courier arrives from {CLAN_LEADER.NAME}, head of the {CLAN_NAME}. {?CLAN_LEADER.GENDER}She{?}He{\\?} proposes that {?CLAN_LEADER.GENDER}her{?}his{\\?} {?OFFERED_HERO.GENDER}kinswoman{?}kinsman{\\?} {OFFERED_HERO.NAME} marry {PLAYER_CLAN_HERO.NAME}, from your clan. The couple appear to be compatible. Do you accept?", null);

		private static TextObject MarriageOfferPanelExplanationText = new TextObject("{=CZwrlJMJ}A courier with a marriage offer for {CLAN_MEMBER.NAME} from {OFFERED_HERO.NAME} has arrived.", null);

		private Hero _currentOfferedPlayerClanHero;

		private Hero _currentOfferedOtherClanHero;

		private CampaignTime _lastMarriageOfferTime = CampaignTime.Zero;
	}
}
