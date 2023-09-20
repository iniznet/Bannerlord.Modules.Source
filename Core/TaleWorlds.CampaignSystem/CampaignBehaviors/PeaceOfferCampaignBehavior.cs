using System;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class PeaceOfferCampaignBehavior : CampaignBehaviorBase
	{
		private static TextObject PeacePanelTitleText
		{
			get
			{
				return new TextObject("{=ho5EndaV}Decision", null);
			}
		}

		private static TextObject PeacePanelOkText
		{
			get
			{
				return new TextObject("{=oHaWR73d}Ok", null);
			}
		}

		private static TextObject PeacePanelAffirmativeText
		{
			get
			{
				return new TextObject("{=Y94H6XnK}Accept", null);
			}
		}

		private static TextObject PeacePanelNegativeText
		{
			get
			{
				return new TextObject("{=cOgmdp9e}Decline", null);
			}
		}

		public override void RegisterEvents()
		{
			CampaignEvents.OnPeaceOfferedToPlayerEvent.AddNonSerializedListener(this, new Action<IFaction, int>(this.OnPeaceOffered));
			CampaignEvents.MakePeace.AddNonSerializedListener(this, new Action<IFaction, IFaction, MakePeaceAction.MakePeaceDetail>(this.OnMakePeace));
			CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.HourlyTick));
			CampaignEvents.OnPeaceOfferCancelledEvent.AddNonSerializedListener(this, new Action<IFaction>(this.OnPeaceOfferCancelled));
		}

		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<int>("_currentPeaceOfferTributeAmount", ref this._currentPeaceOfferTributeAmount);
			dataStore.SyncData<IFaction>("_opponentFaction", ref this._opponentFaction);
		}

		public void SetCurrentTributeAmount(int tributeAmount)
		{
			this._currentPeaceOfferTributeAmount = tributeAmount;
		}

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

		private void OnPeaceOfferCancelled(IFaction opponentFaction)
		{
			if (Hero.MainHero.MapFaction.Leader != Hero.MainHero)
			{
				this._opponentFaction = opponentFaction;
				this.OkPeaceOffer();
			}
		}

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

		private void OnMakePeace(IFaction side1Faction, IFaction side2Faction, MakePeaceAction.MakePeaceDetail detail)
		{
			if ((side1Faction == Hero.MainHero.MapFaction && side2Faction == this._opponentFaction) || (side2Faction == Hero.MainHero.MapFaction && side1Faction == this._opponentFaction))
			{
				this.DeclinePeaceOffer();
			}
		}

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

		private void AcceptPeaceOffer()
		{
			MakePeaceAction.Apply(this._opponentFaction, Hero.MainHero.MapFaction, this._currentPeaceOfferTributeAmount);
			this._opponentFaction = null;
		}

		private void DeclinePeaceOffer()
		{
			CampaignEventDispatcher.Instance.OnPeaceOfferCancelled(this._opponentFaction);
			this._opponentFaction = null;
			ChangeClanInfluenceAction.Apply(Clan.PlayerClan, (float)(-(float)this._influenceCostOfDecline));
		}

		private static TextObject PeaceOfferDefaultPanelDescriptionText = new TextObject("{=IB1xsVEr}A courier has arrived from the {MAP_FACTION_NAME}. They offer you a white peace. Your vassals have left the decision with you.", null);

		private static TextObject PeaceOfferTributePaidPanelDescriptionText = new TextObject("{=JJQ0Hp4m}A courier has arrived from the {MAP_FACTION_NAME}. The {MAP_FACTION_NAME} will pay {GOLD_AMOUNT} {GOLD_ICON} in tribute each day to end the war between your realms. Your vassals have left the decision with you.", null);

		private static TextObject PeaceOfferTributeWantedPanelDescriptionText = new TextObject("{=Nd0Vhkxn}A courier has arrived from the {MAP_FACTION_NAME}. They offer you peace if you agree to pay a {GOLD_AMOUNT} {GOLD_ICON} daily tribute. Your vassals have left the decision with you.", null);

		private static TextObject PeaceOfferDefaultPanelPlayerIsVassalDescriptionText = new TextObject("{=gNf0ALKw}A courier has arrived from the {MAP_FACTION_NAME}. They offer you a white peace. Your kingdom will vote whether to accept the offer.", null);

		private static TextObject PeaceOfferTributePaidPanelPlayerIsVassalDescriptionText = new TextObject("{=SR9FC5jH}A courier has arrived from the {MAP_FACTION_NAME} bearing a peace offer. The {MAP_FACTION_NAME} will pay {GOLD_AMOUNT} {GOLD_ICON} in tribute each day to end the war between your realms. Your kingdom will vote whether to accept the offer.", null);

		private static TextObject PeaceOfferTributeWantedPanelPlayerIsVassalDescriptionText = new TextObject("{=sbFboHmV}A courier has arrived from the {MAP_FACTION_NAME}. They offer you peace if you agree to pay a {GOLD_AMOUNT} {GOLD_ICON} daily tribute. Your kingdom will vote whether to accept the offer.", null);

		private IFaction _opponentFaction;

		private int _currentPeaceOfferTributeAmount;

		private int _influenceCostOfDecline;

		private int _hourCounter;
	}
}
