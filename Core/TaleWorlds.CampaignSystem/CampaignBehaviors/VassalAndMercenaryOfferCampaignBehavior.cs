using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003DD RID: 989
	public class VassalAndMercenaryOfferCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x17000CEE RID: 3310
		// (get) Token: 0x06003BC6 RID: 15302 RVA: 0x0011B062 File Offset: 0x00119262
		private static TextObject DecisionPopUpTitleText
		{
			get
			{
				return new TextObject("{=ho5EndaV}Decision", null);
			}
		}

		// Token: 0x17000CEF RID: 3311
		// (get) Token: 0x06003BC7 RID: 15303 RVA: 0x0011B06F File Offset: 0x0011926F
		private static TextObject DecisionPopUpAffirmativeText
		{
			get
			{
				return new TextObject("{=Y94H6XnK}Accept", null);
			}
		}

		// Token: 0x17000CF0 RID: 3312
		// (get) Token: 0x06003BC8 RID: 15304 RVA: 0x0011B07C File Offset: 0x0011927C
		private static TextObject DecisionPopUpNegativeText
		{
			get
			{
				return new TextObject("{=cOgmdp9e}Decline", null);
			}
		}

		// Token: 0x06003BC9 RID: 15305 RVA: 0x0011B08C File Offset: 0x0011928C
		public override void RegisterEvents()
		{
			if (!this._stopOffers)
			{
				CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
				CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTick));
				CampaignEvents.HeroPrisonerTaken.AddNonSerializedListener(this, new Action<PartyBase, Hero>(this.OnHeroPrisonerTaken));
				CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangedKingdom));
				CampaignEvents.OnVassalOrMercenaryServiceOfferedToPlayerEvent.AddNonSerializedListener(this, new Action<Kingdom>(this.OnVassalOrMercenaryServiceOfferedToPlayer));
				CampaignEvents.OnVassalOrMercenaryServiceOfferCanceledEvent.AddNonSerializedListener(this, new Action<Kingdom>(this.OnVassalOrMercenaryServiceOfferCanceled));
				CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction, DeclareWarAction.DeclareWarDetail>(this.OnWarDeclared));
				CampaignEvents.HeroRelationChanged.AddNonSerializedListener(this, new Action<Hero, Hero, int, bool, ChangeRelationAction.ChangeRelationDetail, Hero, Hero>(this.OnHeroRelationChanged));
				CampaignEvents.KingdomDestroyedEvent.AddNonSerializedListener(this, new Action<Kingdom>(this.OnKingdomDestroyed));
				CampaignEvents.OnPlayerCharacterChangedEvent.AddNonSerializedListener(this, new Action<Hero, Hero, MobileParty, bool>(this.OnPlayerCharacterChanged));
			}
		}

		// Token: 0x06003BCA RID: 15306 RVA: 0x0011B18A File Offset: 0x0011938A
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Tuple<Kingdom, CampaignTime>>("_currentMercenaryOffer", ref this._currentMercenaryOffer);
			dataStore.SyncData<Dictionary<Kingdom, CampaignTime>>("_vassalOffers", ref this._vassalOffers);
			dataStore.SyncData<bool>("_stopOffers", ref this._stopOffers);
		}

		// Token: 0x06003BCB RID: 15307 RVA: 0x0011B1C2 File Offset: 0x001193C2
		private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			this.AddVassalDialogues(campaignGameStarter);
		}

		// Token: 0x06003BCC RID: 15308 RVA: 0x0011B1CC File Offset: 0x001193CC
		private void DailyTick()
		{
			if (!this._stopOffers && Clan.PlayerClan.Tier > Campaign.Current.Models.ClanTierModel.MinClanTier)
			{
				if (this._currentMercenaryOffer != null)
				{
					if (this._currentMercenaryOffer.Item2.ElapsedHoursUntilNow >= 48f || !this.MercenaryKingdomSelectionConditionsHold(this._currentMercenaryOffer.Item1))
					{
						CampaignEventDispatcher.Instance.OnVassalOrMercenaryServiceOfferCanceled(this._currentMercenaryOffer.Item1);
						return;
					}
				}
				else
				{
					float randomFloat = MBRandom.RandomFloat;
					if (randomFloat <= 0.02f && this.CanPlayerClanReceiveMercenaryOffer())
					{
						Kingdom randomElementWithPredicate = Kingdom.All.GetRandomElementWithPredicate(new Func<Kingdom, bool>(this.MercenaryKingdomSelectionConditionsHold));
						if (randomElementWithPredicate != null)
						{
							this.CreateMercenaryOffer(randomElementWithPredicate);
							return;
						}
					}
					else if (randomFloat <= 0.01f && this.CanPlayerClanReceiveVassalOffer())
					{
						Kingdom randomElementWithPredicate2 = Kingdom.All.GetRandomElementWithPredicate(new Func<Kingdom, bool>(this.VassalKingdomSelectionConditionsHold));
						if (randomElementWithPredicate2 != null)
						{
							this.CreateVassalOffer(randomElementWithPredicate2);
						}
					}
				}
			}
		}

		// Token: 0x06003BCD RID: 15309 RVA: 0x0011B2BC File Offset: 0x001194BC
		private bool VassalKingdomSelectionConditionsHold(Kingdom kingdom)
		{
			List<IFaction> list;
			List<IFaction> list2;
			return !this._vassalOffers.ContainsKey(kingdom) && FactionHelper.CanPlayerOfferVassalage(kingdom, out list, out list2);
		}

		// Token: 0x06003BCE RID: 15310 RVA: 0x0011B2E4 File Offset: 0x001194E4
		private bool MercenaryKingdomSelectionConditionsHold(Kingdom kingdom)
		{
			List<IFaction> list;
			List<IFaction> list2;
			return FactionHelper.CanPlayerOfferMercenaryService(kingdom, out list, out list2);
		}

		// Token: 0x06003BCF RID: 15311 RVA: 0x0011B2FB File Offset: 0x001194FB
		private void OnHeroPrisonerTaken(PartyBase captor, Hero prisoner)
		{
			if (prisoner == Hero.MainHero && this._currentMercenaryOffer != null)
			{
				CampaignEventDispatcher.Instance.OnVassalOrMercenaryServiceOfferCanceled(this._currentMercenaryOffer.Item1);
			}
		}

		// Token: 0x06003BD0 RID: 15312 RVA: 0x0011B324 File Offset: 0x00119524
		private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification = true)
		{
			if (clan == Clan.PlayerClan && newKingdom != null)
			{
				if (detail == ChangeKingdomAction.ChangeKingdomActionDetail.JoinAsMercenary && this._currentMercenaryOffer != null && this._currentMercenaryOffer.Item1 != newKingdom)
				{
					CampaignEventDispatcher.Instance.OnVassalOrMercenaryServiceOfferCanceled(this._currentMercenaryOffer.Item1);
					return;
				}
				if (detail == ChangeKingdomAction.ChangeKingdomActionDetail.JoinKingdom || detail == ChangeKingdomAction.ChangeKingdomActionDetail.CreateKingdom)
				{
					this._stopOffers = true;
					if (this._currentMercenaryOffer != null)
					{
						CampaignEventDispatcher.Instance.OnVassalOrMercenaryServiceOfferCanceled(this._currentMercenaryOffer.Item1);
					}
					foreach (KeyValuePair<Kingdom, CampaignTime> keyValuePair in this._vassalOffers.ToDictionary((KeyValuePair<Kingdom, CampaignTime> x) => x.Key, (KeyValuePair<Kingdom, CampaignTime> x) => x.Value))
					{
						CampaignEventDispatcher.Instance.OnVassalOrMercenaryServiceOfferCanceled(keyValuePair.Key);
					}
				}
			}
		}

		// Token: 0x06003BD1 RID: 15313 RVA: 0x0011B438 File Offset: 0x00119638
		private void OnVassalOrMercenaryServiceOfferedToPlayer(Kingdom kingdom)
		{
			if (this._currentMercenaryOffer != null && this._currentMercenaryOffer.Item1 == kingdom)
			{
				this.CreateMercenaryOfferDecisionPopUp(kingdom);
			}
		}

		// Token: 0x06003BD2 RID: 15314 RVA: 0x0011B457 File Offset: 0x00119657
		private void OnVassalOrMercenaryServiceOfferCanceled(Kingdom kingdom)
		{
			this.ClearKingdomOffer(kingdom);
		}

		// Token: 0x06003BD3 RID: 15315 RVA: 0x0011B460 File Offset: 0x00119660
		private void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail detail)
		{
			if ((faction1 == Clan.PlayerClan || faction2 == Clan.PlayerClan) && this._currentMercenaryOffer != null && !this.MercenaryKingdomSelectionConditionsHold(this._currentMercenaryOffer.Item1))
			{
				CampaignEventDispatcher.Instance.OnVassalOrMercenaryServiceOfferCanceled(this._currentMercenaryOffer.Item1);
			}
		}

		// Token: 0x06003BD4 RID: 15316 RVA: 0x0011B4B0 File Offset: 0x001196B0
		private void OnHeroRelationChanged(Hero effectiveHero, Hero effectiveHeroGainedRelationWith, int relationChange, bool showNotification, ChangeRelationAction.ChangeRelationDetail detail, Hero originalHero, Hero originalGainedRelationWith)
		{
			if ((effectiveHero == Hero.MainHero || effectiveHeroGainedRelationWith == Hero.MainHero) && this._currentMercenaryOffer != null && !this.MercenaryKingdomSelectionConditionsHold(this._currentMercenaryOffer.Item1))
			{
				CampaignEventDispatcher.Instance.OnVassalOrMercenaryServiceOfferCanceled(this._currentMercenaryOffer.Item1);
			}
		}

		// Token: 0x06003BD5 RID: 15317 RVA: 0x0011B4FD File Offset: 0x001196FD
		private void OnKingdomDestroyed(Kingdom destroyedKingdom)
		{
			if ((this._currentMercenaryOffer != null && this._currentMercenaryOffer.Item1 == destroyedKingdom) || this._vassalOffers.ContainsKey(destroyedKingdom))
			{
				CampaignEventDispatcher.Instance.OnVassalOrMercenaryServiceOfferCanceled(destroyedKingdom);
			}
		}

		// Token: 0x06003BD6 RID: 15318 RVA: 0x0011B530 File Offset: 0x00119730
		private void OnPlayerCharacterChanged(Hero oldPlayer, Hero newPlayer, MobileParty newMainParty, bool isMainPartyChanged)
		{
			if (this._currentMercenaryOffer != null)
			{
				CampaignEventDispatcher.Instance.OnVassalOrMercenaryServiceOfferCanceled(this._currentMercenaryOffer.Item1);
			}
			if (!this._vassalOffers.IsEmpty<KeyValuePair<Kingdom, CampaignTime>>())
			{
				foreach (Kingdom kingdom in Kingdom.All)
				{
					if (this._vassalOffers.ContainsKey(kingdom))
					{
						CampaignEventDispatcher.Instance.OnVassalOrMercenaryServiceOfferCanceled(kingdom);
					}
				}
			}
		}

		// Token: 0x06003BD7 RID: 15319 RVA: 0x0011B5C0 File Offset: 0x001197C0
		private void ClearKingdomOffer(Kingdom kingdom)
		{
			if (this._currentMercenaryOffer != null && this._currentMercenaryOffer.Item1 == kingdom)
			{
				this._currentMercenaryOffer = null;
				return;
			}
			if (this._vassalOffers.Count > 0)
			{
				this._vassalOffers.Clear();
			}
		}

		// Token: 0x06003BD8 RID: 15320 RVA: 0x0011B5F9 File Offset: 0x001197F9
		private bool CanPlayerClanReceiveMercenaryOffer()
		{
			return Clan.PlayerClan.Kingdom == null && Clan.PlayerClan.Tier == Campaign.Current.Models.ClanTierModel.MercenaryEligibleTier;
		}

		// Token: 0x06003BD9 RID: 15321 RVA: 0x0011B62C File Offset: 0x0011982C
		private void CreateMercenaryOffer(Kingdom kingdom)
		{
			this._currentMercenaryOffer = new Tuple<Kingdom, CampaignTime>(kingdom, CampaignTime.Now);
			VassalAndMercenaryOfferCampaignBehavior.MercenaryOfferPanelNotificationText.SetCharacterProperties("OFFERED_KINGDOM_LEADER", kingdom.Leader.CharacterObject, false);
			Campaign.Current.CampaignInformationManager.NewMapNoticeAdded(new MercenaryOfferMapNotification(kingdom, VassalAndMercenaryOfferCampaignBehavior.MercenaryOfferPanelNotificationText));
		}

		// Token: 0x06003BDA RID: 15322 RVA: 0x0011B680 File Offset: 0x00119880
		private void CreateMercenaryOfferDecisionPopUp(Kingdom kingdom)
		{
			Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
			int mercenaryAwardFactorToJoinKingdom = Campaign.Current.Models.MinorFactionsModel.GetMercenaryAwardFactorToJoinKingdom(Clan.PlayerClan, kingdom, true);
			VassalAndMercenaryOfferCampaignBehavior.MercenaryOfferDecisionPopUpExplanationText.SetTextVariable("OFFERED_KINGDOM_NAME", kingdom.Name);
			VassalAndMercenaryOfferCampaignBehavior.MercenaryOfferDecisionPopUpExplanationText.SetTextVariable("GOLD_AMOUNT", mercenaryAwardFactorToJoinKingdom);
			InformationManager.ShowInquiry(new InquiryData(VassalAndMercenaryOfferCampaignBehavior.DecisionPopUpTitleText.ToString(), VassalAndMercenaryOfferCampaignBehavior.MercenaryOfferDecisionPopUpExplanationText.ToString(), true, true, VassalAndMercenaryOfferCampaignBehavior.DecisionPopUpAffirmativeText.ToString(), VassalAndMercenaryOfferCampaignBehavior.DecisionPopUpNegativeText.ToString(), new Action(this.MercenaryOfferAccepted), new Action(this.MercenaryOfferDeclined), "", 0f, null, null, null), false, false);
		}

		// Token: 0x06003BDB RID: 15323 RVA: 0x0011B738 File Offset: 0x00119938
		private void MercenaryOfferAccepted()
		{
			Kingdom item = this._currentMercenaryOffer.Item1;
			this.ClearKingdomOffer(this._currentMercenaryOffer.Item1);
			int mercenaryAwardFactorToJoinKingdom = Campaign.Current.Models.MinorFactionsModel.GetMercenaryAwardFactorToJoinKingdom(Clan.PlayerClan, item, true);
			ChangeKingdomAction.ApplyByJoinFactionAsMercenary(Clan.PlayerClan, item, mercenaryAwardFactorToJoinKingdom, true);
		}

		// Token: 0x06003BDC RID: 15324 RVA: 0x0011B78B File Offset: 0x0011998B
		private void MercenaryOfferDeclined()
		{
			this.ClearKingdomOffer(this._currentMercenaryOffer.Item1);
		}

		// Token: 0x06003BDD RID: 15325 RVA: 0x0011B79E File Offset: 0x0011999E
		private bool CanPlayerClanReceiveVassalOffer()
		{
			return (Clan.PlayerClan.Kingdom == null || Clan.PlayerClan.IsUnderMercenaryService) && Clan.PlayerClan.Tier >= Campaign.Current.Models.ClanTierModel.VassalEligibleTier;
		}

		// Token: 0x06003BDE RID: 15326 RVA: 0x0011B7E0 File Offset: 0x001199E0
		private void CreateVassalOffer(Kingdom kingdom)
		{
			this._vassalOffers.Add(kingdom, CampaignTime.Now);
			VassalAndMercenaryOfferCampaignBehavior.VassalOfferPanelNotificationText.SetCharacterProperties("OFFERED_KINGDOM_LEADER", kingdom.Leader.CharacterObject, false);
			Campaign.Current.CampaignInformationManager.NewMapNoticeAdded(new VassalOfferMapNotification(kingdom, VassalAndMercenaryOfferCampaignBehavior.VassalOfferPanelNotificationText));
		}

		// Token: 0x06003BDF RID: 15327 RVA: 0x0011B834 File Offset: 0x00119A34
		private void AddVassalDialogues(CampaignGameStarter campaignGameStarter)
		{
			campaignGameStarter.AddDialogLine("valid_vassal_offer_start", "start", "valid_vassal_offer_player_response", "{=aDABE6Md}Greetings, {PLAYER.NAME}. I am glad that you received my message. Are you interested in my offer?", new ConversationSentence.OnConditionDelegate(this.valid_vassal_offer_start_condition), null, int.MaxValue, null);
			campaignGameStarter.AddPlayerLine("vassal_offer_player_accepts_response", "valid_vassal_offer_player_response", "vassal_offer_start_oath", "{=IHXqZSnt}Yes, I am ready to accept your offer.", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("vassal_offer_player_declines_response", "valid_vassal_offer_player_response", "vassal_offer_king_response_to_decline", "{=FAuoq2gT}While I am honored, I must decline your offer.", null, new ConversationSentence.OnConsequenceDelegate(this.vassal_conversation_end_consequence), 100, null, null);
			campaignGameStarter.AddDialogLine("vassal_offer_king_response_to_accept_continue", "vassal_offer_start_oath", "vassal_offer_king_response_to_accept_start_oath_1_response", "{=54PbMkNw}Good. Then repeat the words of the oath with me: {OATH_LINE_1}", new ConversationSentence.OnConditionDelegate(this.conversation_set_oath_phrases_on_condition), null, 100, null);
			campaignGameStarter.AddPlayerLine("vassal_offer_player_oath_1", "vassal_offer_king_response_to_accept_start_oath_1_response", "vassal_offer_king_response_to_accept_start_oath_2", "{=!}{OATH_LINE_1}", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("vassal_offer_player_oath_1_decline", "vassal_offer_king_response_to_accept_start_oath_1_response", "vassal_offer_king_response_to_accept_start_oath_decline", "{=8bLwh9yy}Excuse me, {?CONVERSATION_NPC.GENDER}my lady{?}sir{\\?}. But I feel I need to think about this.", null, null, 100, null, null);
			campaignGameStarter.AddDialogLine("vassal_offer_lord_oath_2", "vassal_offer_king_response_to_accept_start_oath_2", "vassal_offer_king_response_to_accept_start_oath_2_response", "{=!}{OATH_LINE_2}", null, null, 100, null);
			campaignGameStarter.AddPlayerLine("vassal_offer_player_oath_2", "vassal_offer_king_response_to_accept_start_oath_2_response", "vassal_offer_king_response_to_accept_start_oath_3", "{=!}{OATH_LINE_2}", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("vassal_offer_player_oath_2_decline", "vassal_offer_king_response_to_accept_start_oath_2_response", "vassal_offer_king_response_to_accept_start_oath_decline", "{=LKdrCaTO}{?CONVERSATION_NPC.GENDER}My lady{?}Sir{\\?}, may I ask for some time to think about this?", null, null, 100, null, null);
			campaignGameStarter.AddDialogLine("vassal_offer_lord_oath_3", "vassal_offer_king_response_to_accept_start_oath_3", "vassal_offer_king_response_to_accept_start_oath_3_response", "{=!}{OATH_LINE_3}", null, null, 100, null);
			campaignGameStarter.AddPlayerLine("vassal_offer_player_oath_3", "vassal_offer_king_response_to_accept_start_oath_3_response", "vassal_offer_king_response_to_accept_start_oath_4", "{=!}{OATH_LINE_3}", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("vassal_offer_player_oath_3_decline", "vassal_offer_king_response_to_accept_start_oath_3_response", "vassal_offer_king_response_to_accept_start_oath_decline", "{=aa5F4vP5}My {?CONVERSATION_NPC.GENDER}lady{?}lord{\\?}, please give me more time to think about this.", null, null, 100, null, null);
			campaignGameStarter.AddDialogLine("vassal_offer_lord_oath_4", "vassal_offer_king_response_to_accept_start_oath_4", "vassal_offer_king_response_to_accept_start_oath_4_response", "{=!}{OATH_LINE_4}", null, null, 100, null);
			campaignGameStarter.AddPlayerLine("vassal_offer_player_oath_4", "vassal_offer_king_response_to_accept_start_oath_4_response", "lord_give_oath_10", "{=!}{OATH_LINE_4}", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("vassal_offer_player_oath_4_decline", "vassal_offer_king_response_to_accept_start_oath_4_response", "vassal_offer_king_response_to_accept_start_oath_decline", "{=aupbQveh}{?CONVERSATION_NPC.GENDER}Madame{?}Sir{\\?}, I must have more time to consider this.", null, null, 100, null, null);
			campaignGameStarter.AddDialogLine("vassal_offer_king_response_to_decline_during_oath", "vassal_offer_king_response_to_accept_start_oath_decline", "lord_start", "{=vueZBBYB}Indeed. I am not sure why you didn't make up your mind before coming to speak with me.", null, new ConversationSentence.OnConsequenceDelegate(this.vassal_conversation_end_consequence), 100, null);
			campaignGameStarter.AddDialogLine("vassal_offer_king_response_to_decline_continue", "vassal_offer_king_response_to_decline", "lord_start", "{=Lo2kJuhK}I am sorry to hear that.", null, null, 100, null);
			campaignGameStarter.AddDialogLine("invalid_vassal_offer_start", "start", "invalid_vassal_offer_player_response", "{=!}{INVALID_REASON}[if:idle_angry][ib:closed]", new ConversationSentence.OnConditionDelegate(this.invalid_vassal_offer_start_condition), null, int.MaxValue, null);
			campaignGameStarter.AddPlayerLine("vassal_offer_player_accepts_response", "invalid_vassal_offer_player_response", "lord_start", "{=AmBEgOyq}I see...", null, new ConversationSentence.OnConsequenceDelegate(this.vassal_conversation_end_consequence), 100, null, null);
		}

		// Token: 0x06003BE0 RID: 15328 RVA: 0x0011BAF4 File Offset: 0x00119CF4
		private bool valid_vassal_offer_start_condition()
		{
			if (Hero.OneToOneConversationHero != null)
			{
				IFaction mapFaction = Hero.OneToOneConversationHero.MapFaction;
				if (mapFaction == null || mapFaction.IsKingdomFaction)
				{
					KeyValuePair<Kingdom, CampaignTime> keyValuePair = this._vassalOffers.FirstOrDefault(delegate(KeyValuePair<Kingdom, CampaignTime> o)
					{
						IFaction key = o.Key;
						Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
						return key == ((oneToOneConversationHero != null) ? oneToOneConversationHero.MapFaction : null);
					});
					List<IFaction> list;
					List<IFaction> list2;
					bool flag = Hero.OneToOneConversationHero != null && keyValuePair.Key != null && Hero.OneToOneConversationHero == keyValuePair.Key.Leader && FactionHelper.CanPlayerOfferVassalage((Kingdom)Hero.OneToOneConversationHero.MapFaction, out list, out list2);
					if (flag)
					{
						StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, null, false);
						Hero.OneToOneConversationHero.SetHasMet();
						float scoreOfKingdomToGetClan = Campaign.Current.Models.DiplomacyModel.GetScoreOfKingdomToGetClan((Kingdom)Hero.OneToOneConversationHero.MapFaction, Clan.PlayerClan);
						flag &= scoreOfKingdomToGetClan > 0f;
					}
					return flag;
				}
			}
			return false;
		}

		// Token: 0x06003BE1 RID: 15329 RVA: 0x0011BBE8 File Offset: 0x00119DE8
		private bool conversation_set_oath_phrases_on_condition()
		{
			Hero leader = Hero.OneToOneConversationHero.MapFaction.Leader;
			string stringId = Hero.OneToOneConversationHero.Culture.StringId;
			MBTextManager.SetTextVariable("FACTION_TITLE", leader.IsFemale ? Campaign.Current.ConversationManager.FindMatchingTextOrNull("str_liege_title_female", leader.CharacterObject) : Campaign.Current.ConversationManager.FindMatchingTextOrNull("str_liege_title", leader.CharacterObject), false);
			StringHelpers.SetCharacterProperties("LORD", CharacterObject.OneToOneConversationCharacter, null, false);
			if (stringId == "empire")
			{
				MBTextManager.SetTextVariable("OATH_LINE_1", "{=ya8VF98X}I swear by my ancestors that you are lawful {FACTION_TITLE}.", false);
			}
			else if (stringId == "khuzait")
			{
				MBTextManager.SetTextVariable("OATH_LINE_1", "{=PP8VeNiC}I swear that you are my {?LORD.GENDER}khatun{?}khan{\\?}, my {?LORD.GENDER}mother{?}father{\\?}, my protector...", false);
			}
			else
			{
				MBTextManager.SetTextVariable("OATH_LINE_1", "{=MqIg6Mh2}I swear homage to you as lawful {FACTION_TITLE}.", false);
			}
			if (stringId == "empire")
			{
				MBTextManager.SetTextVariable("OATH_LINE_2", "{=vuEyisBW}I affirm that you are executor of the will of the Senate and people...", false);
			}
			else if (stringId == "khuzait")
			{
				MBTextManager.SetTextVariable("OATH_LINE_2", "{=QSPMKz2R}You are the chosen of the Sky, and I shall follow your banner as long as my breath remains...", false);
			}
			else if (stringId == "battania")
			{
				MBTextManager.SetTextVariable("OATH_LINE_2", "{=OHJYAaW5}The powers of Heaven and of the Earth have entrusted to you the guardianship of this sacred land...", false);
			}
			else if (stringId == "aserai")
			{
				MBTextManager.SetTextVariable("OATH_LINE_2", "{=kc3tLqGy}You command the sons of Asera in war and govern them in peace...", false);
			}
			else if (stringId == "sturgia")
			{
				MBTextManager.SetTextVariable("OATH_LINE_2", "{=Qs7qs3b0}You are the shield of our people against the wolves of the forest, the steppe and the sea.", false);
			}
			else
			{
				MBTextManager.SetTextVariable("OATH_LINE_2", "{=PypPEj5Z}I will be your loyal {?PLAYER.GENDER}follower{?}man{\\?} as long as my breath remains...", false);
			}
			if (stringId == "empire")
			{
				MBTextManager.SetTextVariable("OATH_LINE_3", "{=LWFDXeQc}Furthermore, I accept induction into the army of Calradia, at the rank of archon.", false);
			}
			else if (stringId == "khuzait")
			{
				MBTextManager.SetTextVariable("OATH_LINE_3", "{=8lOCOcXw}Your word shall direct the strike of my sword and the flight of my arrow...", false);
			}
			else if (stringId == "aserai")
			{
				MBTextManager.SetTextVariable("OATH_LINE_3", "{=bue9AShm}I swear to fight your enemies and give shelter and water to your friends...", false);
			}
			else if (stringId == "sturgia")
			{
				MBTextManager.SetTextVariable("OATH_LINE_3", "{=U3u2D6Ze}I give you my word and bond, to stand by your banner in battle so long as my breath remains...", false);
			}
			else if (stringId == "battania")
			{
				MBTextManager.SetTextVariable("OATH_LINE_3", "{=UwbhGhGw}I shall stand by your side and not foresake you, and fight until my life leaves my body...", false);
			}
			else
			{
				MBTextManager.SetTextVariable("OATH_LINE_3", "{=2o7U1bNV}..and I will be at your side to fight your enemies should you need my sword.", false);
			}
			if (stringId == "empire")
			{
				MBTextManager.SetTextVariable("OATH_LINE_4", "{=EsF8sEaQ}And as such, that you are my commander, and I shall follow you wherever you lead.", false);
			}
			else if (stringId == "battania")
			{
				MBTextManager.SetTextVariable("OATH_LINE_4", "{=6KbDn1HS}I shall heed your judgements and pay you the tribute that is your due, so that this land may have a strong protector.", false);
			}
			else if (stringId == "khuzait")
			{
				MBTextManager.SetTextVariable("OATH_LINE_4", "{=xDzxaYed}Your word shall divide the spoils of victory and the bounties of peace.", false);
			}
			else if (stringId == "aserai")
			{
				MBTextManager.SetTextVariable("OATH_LINE_4", "{=qObicX7y}I swear to heed your judgements according to the laws of the Aserai, and ensure that my kinfolk heed them as well...", false);
			}
			else if (stringId == "sturgia")
			{
				MBTextManager.SetTextVariable("OATH_LINE_4", "{=HpWYfcgw}..and to uphold your rights under the laws of the Sturgians, and the rights of your kin, and to avenge their blood as thought it were my own.", false);
			}
			else
			{
				MBTextManager.SetTextVariable("OATH_LINE_4", "{=waoSd6tj}.. and I shall defend your rights and the rights of your legitimate heirs.", false);
			}
			StringHelpers.SetCharacterProperties("CONVERSATION_NPC", CharacterObject.OneToOneConversationCharacter, null, false);
			return true;
		}

		// Token: 0x06003BE2 RID: 15330 RVA: 0x0011BEDC File Offset: 0x0011A0DC
		private bool invalid_vassal_offer_start_condition()
		{
			if (Hero.OneToOneConversationHero != null)
			{
				IFaction mapFaction = Hero.OneToOneConversationHero.MapFaction;
				if ((mapFaction == null || mapFaction.IsKingdomFaction) && (PlayerEncounter.Current == null || PlayerEncounter.Current.EncounterState != PlayerEncounterState.CaptureHeroes))
				{
					Kingdom offerKingdom = (Kingdom)Hero.OneToOneConversationHero.MapFaction;
					KeyValuePair<Kingdom, CampaignTime> keyValuePair = this._vassalOffers.FirstOrDefault((KeyValuePair<Kingdom, CampaignTime> o) => o.Key == offerKingdom);
					List<IFaction> list = new List<IFaction>();
					List<IFaction> list2 = new List<IFaction>();
					bool flag = Hero.OneToOneConversationHero != null && keyValuePair.Key != null && Hero.OneToOneConversationHero == keyValuePair.Key.Leader && !FactionHelper.CanPlayerOfferVassalage(offerKingdom, out list, out list2);
					if (flag)
					{
						Hero.OneToOneConversationHero.SetHasMet();
						TextObject textObject = TextObject.Empty;
						if (offerKingdom.Leader.GetRelationWithPlayer() < (float)Campaign.Current.Models.DiplomacyModel.MinimumRelationWithConversationCharacterToJoinKingdom)
						{
							textObject = new TextObject("{=niWfuEeh}Well, {PLAYER.NAME}. Are you here about that offer I made? Seeing as what's happened between then and now, surely you realize that that offer no longer stands?", null);
						}
						else if (list.Contains(offerKingdom))
						{
							textObject = new TextObject("{=RACyH7N5}Greetings, {PLAYER.NAME}. I suppose that you're here because of that message I sent you. But we are at war now. I can no longer make that offer to you.", null);
						}
						else if (list2.Intersect(list).Count<IFaction>() != list.Count)
						{
							textObject = new TextObject("{=lynev8Lk}Greetings, {PLAYER.NAME}. I suppose that you're here because of that message I sent you. But the diplomatic situation has changed. You are at war with {WAR_KINGDOMS}, and we are at peace with them. Until that changes, I can no longer accept your fealty.", null);
							List<TextObject> list3 = new List<TextObject>();
							foreach (IFaction faction in list)
							{
								if (!list2.Contains(faction))
								{
									list3.Add(faction.Name);
								}
							}
							textObject.SetTextVariable("WAR_KINGDOMS", GameTexts.GameTextHelper.MergeTextObjectsWithComma(list3, true));
						}
						textObject.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, false);
						MBTextManager.SetTextVariable("INVALID_REASON", textObject, false);
					}
					return flag;
				}
			}
			return false;
		}

		// Token: 0x06003BE3 RID: 15331 RVA: 0x0011C0C0 File Offset: 0x0011A2C0
		private void vassal_conversation_end_consequence()
		{
			CampaignEventDispatcher.Instance.OnVassalOrMercenaryServiceOfferCanceled((Kingdom)Hero.OneToOneConversationHero.MapFaction);
		}

		// Token: 0x04001232 RID: 4658
		private const float MercenaryOfferCreationChance = 0.02f;

		// Token: 0x04001233 RID: 4659
		private const float VassalOfferCreationChance = 0.01f;

		// Token: 0x04001234 RID: 4660
		private const int MercenaryOfferCancelTimeInHours = 48;

		// Token: 0x04001235 RID: 4661
		private static readonly TextObject MercenaryOfferDecisionPopUpExplanationText = new TextObject("{=TENbJKpP}The {OFFERED_KINGDOM_NAME} is offering you work as a mercenary, paying {GOLD_AMOUNT}{GOLD_ICON} per influence point that you would gain from fighting on their behalf. Do you accept?", null);

		// Token: 0x04001236 RID: 4662
		private static readonly TextObject MercenaryOfferPanelNotificationText = new TextObject("{=FA2QZc7Q}A courier arrives, bearing a message from {OFFERED_KINGDOM_LEADER.NAME}. {?OFFERED_KINGDOM_LEADER.GENDER}She{?}He{\\?} is offering you a contract as a mercenary.", null);

		// Token: 0x04001237 RID: 4663
		private static readonly TextObject VassalOfferPanelNotificationText = new TextObject("{=7ouzFASf}A courier arrives, bearing a message from {OFFERED_KINGDOM_LEADER.NAME}. {?OFFERED_KINGDOM_LEADER.GENDER}She{?}He{\\?} remarks on your growing reputation, and asks if you would consider pledging yourself as a vassal of the {OFFERED_KINGDOM_LEADER.NAME}. You should speak in person if you are interested.", null);

		// Token: 0x04001238 RID: 4664
		private Tuple<Kingdom, CampaignTime> _currentMercenaryOffer;

		// Token: 0x04001239 RID: 4665
		private Dictionary<Kingdom, CampaignTime> _vassalOffers = new Dictionary<Kingdom, CampaignTime>();

		// Token: 0x0400123A RID: 4666
		private bool _stopOffers;
	}
}
