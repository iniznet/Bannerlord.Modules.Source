using System;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003BB RID: 955
	public class PeaceOfferCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x17000CD8 RID: 3288
		// (get) Token: 0x060038C7 RID: 14535 RVA: 0x00103264 File Offset: 0x00101464
		private static TextObject PeacePanelTitleText
		{
			get
			{
				return new TextObject("{=ho5EndaV}Decision", null);
			}
		}

		// Token: 0x17000CD9 RID: 3289
		// (get) Token: 0x060038C8 RID: 14536 RVA: 0x00103271 File Offset: 0x00101471
		private static TextObject PeacePanelOkText
		{
			get
			{
				return new TextObject("{=oHaWR73d}Ok", null);
			}
		}

		// Token: 0x17000CDA RID: 3290
		// (get) Token: 0x060038C9 RID: 14537 RVA: 0x0010327E File Offset: 0x0010147E
		private static TextObject PeacePanelAffirmativeText
		{
			get
			{
				return new TextObject("{=Y94H6XnK}Accept", null);
			}
		}

		// Token: 0x17000CDB RID: 3291
		// (get) Token: 0x060038CA RID: 14538 RVA: 0x0010328B File Offset: 0x0010148B
		private static TextObject PeacePanelNegativeText
		{
			get
			{
				return new TextObject("{=cOgmdp9e}Decline", null);
			}
		}

		// Token: 0x060038CB RID: 14539 RVA: 0x00103298 File Offset: 0x00101498
		public override void RegisterEvents()
		{
			CampaignEvents.OnPeaceOfferedToPlayerEvent.AddNonSerializedListener(this, new Action<IFaction, int>(this.OnPeaceOffered));
			CampaignEvents.MakePeace.AddNonSerializedListener(this, new Action<IFaction, IFaction, MakePeaceAction.MakePeaceDetail>(this.OnMakePeace));
			CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.HourlyTick));
			CampaignEvents.OnPeaceOfferCancelledEvent.AddNonSerializedListener(this, new Action<IFaction>(this.OnPeaceOfferCancelled));
		}

		// Token: 0x060038CC RID: 14540 RVA: 0x00103301 File Offset: 0x00101501
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<int>("_currentPeaceOfferTributeAmount", ref this._currentPeaceOfferTributeAmount);
			dataStore.SyncData<IFaction>("_opponentFaction", ref this._opponentFaction);
		}

		// Token: 0x060038CD RID: 14541 RVA: 0x00103327 File Offset: 0x00101527
		public void SetCurrentTributeAmount(int tributeAmount)
		{
			this._currentPeaceOfferTributeAmount = tributeAmount;
		}

		// Token: 0x060038CE RID: 14542 RVA: 0x00103330 File Offset: 0x00101530
		private void OnPeaceOffered(IFaction opponentFaction, int tributeAmount)
		{
			if (this._opponentFaction == null)
			{
				this._opponentFaction = opponentFaction;
				this._currentPeaceOfferTributeAmount = tributeAmount;
				TextObject textObject = ((tributeAmount > 0) ? ((Hero.MainHero.MapFaction.Leader == Hero.MainHero) ? PeaceOfferCampaignBehavior.PeaceOfferTributePaidPanelDescriptionText : PeaceOfferCampaignBehavior.PeaceOfferTributePaidPanelPlayerIsVassalDescriptionText) : ((tributeAmount < 0) ? ((Hero.MainHero.MapFaction.Leader == Hero.MainHero) ? PeaceOfferCampaignBehavior.PeaceOfferTributeWantedPanelDescriptionText : PeaceOfferCampaignBehavior.PeaceOfferTributeWantedPanelPlayerIsVassalDescriptionText) : ((Hero.MainHero.MapFaction.Leader == Hero.MainHero) ? PeaceOfferCampaignBehavior.PeaceOfferDefaultPanelDescriptionText : PeaceOfferCampaignBehavior.PeaceOfferDefaultPanelPlayerIsVassalDescriptionText)));
				textObject.SetTextVariable("MAP_FACTION_NAME", opponentFaction.InformalName);
				textObject.SetTextVariable("GOLD_AMOUNT", MathF.Abs(this._currentPeaceOfferTributeAmount));
				textObject.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
				TextObject peacePanelNegativeText = PeaceOfferCampaignBehavior.PeacePanelNegativeText;
				this._influenceCostOfDecline = 0;
				if (Hero.MainHero.MapFaction.Leader == Hero.MainHero)
				{
					InformationManager.ShowInquiry(new InquiryData(PeaceOfferCampaignBehavior.PeacePanelTitleText.ToString(), textObject.ToString(), true, (float)this._influenceCostOfDecline <= 0.1f || Hero.MainHero.Clan.Influence >= (float)this._influenceCostOfDecline, PeaceOfferCampaignBehavior.PeacePanelAffirmativeText.ToString(), peacePanelNegativeText.ToString(), new Action(this.AcceptPeaceOffer), new Action(this.DeclinePeaceOffer), "", 0f, null, null, null), true, false);
					this._hourCounter = 0;
					return;
				}
				InformationManager.ShowInquiry(new InquiryData(PeaceOfferCampaignBehavior.PeacePanelTitleText.ToString(), textObject.ToString(), false, true, PeaceOfferCampaignBehavior.PeacePanelOkText.ToString(), PeaceOfferCampaignBehavior.PeacePanelOkText.ToString(), new Action(this.OkPeaceOffer), new Action(this.OkPeaceOffer), "", 0f, null, null, null), true, false);
				this._hourCounter = 0;
			}
		}

		// Token: 0x060038CF RID: 14543 RVA: 0x0010350D File Offset: 0x0010170D
		private void OnPeaceOfferCancelled(IFaction opponentFaction)
		{
			if (Hero.MainHero.MapFaction.Leader != Hero.MainHero)
			{
				this._opponentFaction = opponentFaction;
				this.OkPeaceOffer();
			}
		}

		// Token: 0x060038D0 RID: 14544 RVA: 0x00103534 File Offset: 0x00101734
		public void HourlyTick()
		{
			if (this._opponentFaction != null)
			{
				this._hourCounter++;
				if (this._hourCounter == 24)
				{
					if (Hero.MainHero.MapFaction.Leader == Hero.MainHero)
					{
						CampaignEventDispatcher.Instance.OnPeaceOfferCancelled(this._opponentFaction);
						return;
					}
					CampaignEventDispatcher.Instance.OnPeaceOfferCancelled(this._opponentFaction);
				}
			}
		}

		// Token: 0x060038D1 RID: 14545 RVA: 0x00103598 File Offset: 0x00101798
		private void OnMakePeace(IFaction side1Faction, IFaction side2Faction, MakePeaceAction.MakePeaceDetail detail)
		{
			if ((side1Faction == Hero.MainHero.MapFaction && side2Faction == this._opponentFaction) || (side2Faction == Hero.MainHero.MapFaction && side1Faction == this._opponentFaction))
			{
				this.DeclinePeaceOffer();
			}
		}

		// Token: 0x060038D2 RID: 14546 RVA: 0x001035CC File Offset: 0x001017CC
		private void OkPeaceOffer()
		{
			if (Clan.PlayerClan.IsUnderMercenaryService)
			{
				this.AcceptPeaceOffer();
				return;
			}
			MakePeaceKingdomDecision makePeaceKingdomDecision = new MakePeaceKingdomDecision(Hero.MainHero.MapFaction.Leader.Clan, this._opponentFaction, -this._currentPeaceOfferTributeAmount, true);
			((Kingdom)Hero.MainHero.MapFaction).AddDecision(makePeaceKingdomDecision, false);
			this._opponentFaction = null;
		}

		// Token: 0x060038D3 RID: 14547 RVA: 0x00103631 File Offset: 0x00101831
		private void AcceptPeaceOffer()
		{
			MakePeaceAction.Apply(this._opponentFaction, Hero.MainHero.MapFaction, this._currentPeaceOfferTributeAmount);
			this._opponentFaction = null;
		}

		// Token: 0x060038D4 RID: 14548 RVA: 0x00103655 File Offset: 0x00101855
		private void DeclinePeaceOffer()
		{
			CampaignEventDispatcher.Instance.OnPeaceOfferCancelled(this._opponentFaction);
			this._opponentFaction = null;
			ChangeClanInfluenceAction.Apply(Clan.PlayerClan, (float)(-(float)this._influenceCostOfDecline));
		}

		// Token: 0x040011B3 RID: 4531
		private static TextObject PeaceOfferDefaultPanelDescriptionText = new TextObject("{=IB1xsVEr}A courier has arrived from the {MAP_FACTION_NAME}. They offer you a white peace. Your vassals have left the decision with you.", null);

		// Token: 0x040011B4 RID: 4532
		private static TextObject PeaceOfferTributePaidPanelDescriptionText = new TextObject("{=JJQ0Hp4m}A courier has arrived from the {MAP_FACTION_NAME}. The {MAP_FACTION_NAME} will pay {GOLD_AMOUNT} {GOLD_ICON} in tribute each day to end the war between your realms. Your vassals have left the decision with you.", null);

		// Token: 0x040011B5 RID: 4533
		private static TextObject PeaceOfferTributeWantedPanelDescriptionText = new TextObject("{=Nd0Vhkxn}A courier has arrived from the {MAP_FACTION_NAME}. They offer you peace if you agree to pay a {GOLD_AMOUNT} {GOLD_ICON} daily tribute. Your vassals have left the decision with you.", null);

		// Token: 0x040011B6 RID: 4534
		private static TextObject PeaceOfferDefaultPanelPlayerIsVassalDescriptionText = new TextObject("{=gNf0ALKw}A courier has arrived from the {MAP_FACTION_NAME}. They offer you a white peace. Your kingdom will vote whether to accept the offer.", null);

		// Token: 0x040011B7 RID: 4535
		private static TextObject PeaceOfferTributePaidPanelPlayerIsVassalDescriptionText = new TextObject("{=SR9FC5jH}A courier has arrived from the {MAP_FACTION_NAME} bearing a peace offer. The {MAP_FACTION_NAME} will pay {GOLD_AMOUNT} {GOLD_ICON} in tribute each day to end the war between your realms. Your kingdom will vote whether to accept the offer.", null);

		// Token: 0x040011B8 RID: 4536
		private static TextObject PeaceOfferTributeWantedPanelPlayerIsVassalDescriptionText = new TextObject("{=sbFboHmV}A courier has arrived from the {MAP_FACTION_NAME}. They offer you peace if you agree to pay a {GOLD_AMOUNT} {GOLD_ICON} daily tribute. Your kingdom will vote whether to accept the offer.", null);

		// Token: 0x040011B9 RID: 4537
		private IFaction _opponentFaction;

		// Token: 0x040011BA RID: 4538
		private int _currentPeaceOfferTributeAmount;

		// Token: 0x040011BB RID: 4539
		private int _influenceCostOfDecline;

		// Token: 0x040011BC RID: 4540
		private int _hourCounter;
	}
}
