using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.BarterSystem.Barterables;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003A9 RID: 937
	public class KingdomDecisionProposalBehavior : CampaignBehaviorBase
	{
		// Token: 0x060037D5 RID: 14293 RVA: 0x000FB644 File Offset: 0x000F9844
		public override void RegisterEvents()
		{
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.SessionLaunched));
			CampaignEvents.DailyTickClanEvent.AddNonSerializedListener(this, new Action<Clan>(this.DailyTickClan));
			CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.HourlyTick));
			CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTick));
			CampaignEvents.MakePeace.AddNonSerializedListener(this, new Action<IFaction, IFaction, MakePeaceAction.MakePeaceDetail>(this.OnPeaceMade));
			CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction, DeclareWarAction.DeclareWarDetail>(this.OnWarDeclared));
		}

		// Token: 0x060037D6 RID: 14294 RVA: 0x000FB6DC File Offset: 0x000F98DC
		private void DailyTickClan(Clan clan)
		{
			if ((float)((int)Campaign.Current.CampaignStartTime.ElapsedDaysUntilNow) < 5f)
			{
				return;
			}
			if (clan.IsEliminated)
			{
				return;
			}
			if (clan == Clan.PlayerClan || clan.TotalStrength <= 0f)
			{
				return;
			}
			if (clan.IsBanditFaction)
			{
				return;
			}
			if (clan.Kingdom == null)
			{
				return;
			}
			if (clan.Influence < 100f)
			{
				return;
			}
			KingdomDecision kingdomDecision = null;
			float randomFloat = MBRandom.RandomFloat;
			int num = ((Kingdom)clan.MapFaction).Clans.Count((Clan x) => x.Influence > 100f);
			float num2 = MathF.Min(0.33f, 1f / ((float)num + 2f));
			num2 *= ((clan.Kingdom == Hero.MainHero.MapFaction && !Hero.MainHero.Clan.IsUnderMercenaryService) ? ((clan.Kingdom.Leader == Hero.MainHero) ? 0.5f : 0.75f) : 1f);
			DiplomacyModel diplomacyModel = Campaign.Current.Models.DiplomacyModel;
			if (randomFloat < num2 && clan.Influence > (float)diplomacyModel.GetInfluenceCostOfProposingPeace())
			{
				kingdomDecision = this.GetRandomPeaceDecision(clan);
			}
			else if (randomFloat < num2 * 2f && clan.Influence > (float)diplomacyModel.GetInfluenceCostOfProposingWar(clan.Kingdom))
			{
				kingdomDecision = this.GetRandomWarDecision(clan);
			}
			else if (randomFloat < num2 * 2.5f && clan.Influence > (float)(diplomacyModel.GetInfluenceCostOfPolicyProposalAndDisavowal() * 4))
			{
				kingdomDecision = this.GetRandomPolicyDecision(clan);
			}
			else if (randomFloat < num2 * 3f && clan.Influence > 700f)
			{
				kingdomDecision = this.GetRandomAnnexationDecision(clan);
			}
			if (kingdomDecision != null)
			{
				if (this._kingdomDecisionsList == null)
				{
					this._kingdomDecisionsList = new List<KingdomDecision>();
				}
				bool flag = false;
				if (kingdomDecision is MakePeaceKingdomDecision && ((MakePeaceKingdomDecision)kingdomDecision).FactionToMakePeaceWith == Hero.MainHero.MapFaction)
				{
					foreach (KingdomDecision kingdomDecision2 in this._kingdomDecisionsList)
					{
						if (kingdomDecision2 is MakePeaceKingdomDecision && kingdomDecision2.Kingdom == Hero.MainHero.MapFaction && ((MakePeaceKingdomDecision)kingdomDecision2).FactionToMakePeaceWith == clan.Kingdom && kingdomDecision2.TriggerTime.IsFuture)
						{
							flag = true;
						}
						if (kingdomDecision2 is MakePeaceKingdomDecision && kingdomDecision2.Kingdom == clan.Kingdom && ((MakePeaceKingdomDecision)kingdomDecision2).FactionToMakePeaceWith == Hero.MainHero.MapFaction && kingdomDecision2.TriggerTime.IsFuture)
						{
							flag = true;
						}
					}
				}
				if (!flag)
				{
					bool flag2 = false;
					foreach (KingdomDecision kingdomDecision3 in this._kingdomDecisionsList)
					{
						DeclareWarDecision declareWarDecision;
						DeclareWarDecision declareWarDecision2;
						MakePeaceKingdomDecision makePeaceKingdomDecision;
						MakePeaceKingdomDecision makePeaceKingdomDecision2;
						if ((declareWarDecision = kingdomDecision3 as DeclareWarDecision) != null && (declareWarDecision2 = kingdomDecision as DeclareWarDecision) != null && declareWarDecision.FactionToDeclareWarOn == declareWarDecision2.FactionToDeclareWarOn && declareWarDecision.ProposerClan.MapFaction == declareWarDecision2.ProposerClan.MapFaction)
						{
							flag2 = true;
						}
						else if ((makePeaceKingdomDecision = kingdomDecision3 as MakePeaceKingdomDecision) != null && (makePeaceKingdomDecision2 = kingdomDecision as MakePeaceKingdomDecision) != null && makePeaceKingdomDecision.FactionToMakePeaceWith == makePeaceKingdomDecision2.FactionToMakePeaceWith && makePeaceKingdomDecision.ProposerClan.MapFaction == makePeaceKingdomDecision2.ProposerClan.MapFaction)
						{
							flag2 = true;
						}
					}
					if (!flag2)
					{
						this._kingdomDecisionsList.Add(kingdomDecision);
						new KingdomElection(kingdomDecision);
						clan.Kingdom.AddDecision(kingdomDecision, false);
						return;
					}
				}
			}
			else
			{
				this.UpdateKingdomDecisions(clan.Kingdom);
			}
		}

		// Token: 0x060037D7 RID: 14295 RVA: 0x000FBAA4 File Offset: 0x000F9CA4
		private void HourlyTick()
		{
			if (Clan.PlayerClan.Kingdom != null)
			{
				this.UpdateKingdomDecisions(Clan.PlayerClan.Kingdom);
			}
		}

		// Token: 0x060037D8 RID: 14296 RVA: 0x000FBAC4 File Offset: 0x000F9CC4
		private void DailyTick()
		{
			if (this._kingdomDecisionsList != null)
			{
				int count = this._kingdomDecisionsList.Count;
				int num = 0;
				for (int i = 0; i < count; i++)
				{
					if (this._kingdomDecisionsList[i - num].TriggerTime.ElapsedDaysUntilNow > 15f)
					{
						this._kingdomDecisionsList.RemoveAt(i - num);
						num++;
					}
				}
			}
		}

		// Token: 0x060037D9 RID: 14297 RVA: 0x000FBB28 File Offset: 0x000F9D28
		public void UpdateKingdomDecisions(Kingdom kingdom)
		{
			List<KingdomDecision> list = new List<KingdomDecision>();
			List<KingdomDecision> list2 = new List<KingdomDecision>();
			foreach (KingdomDecision kingdomDecision in kingdom.UnresolvedDecisions)
			{
				if (kingdomDecision.ShouldBeCancelled())
				{
					list.Add(kingdomDecision);
				}
				else if (kingdomDecision.TriggerTime.IsPast && !kingdomDecision.NeedsPlayerResolution)
				{
					list2.Add(kingdomDecision);
				}
			}
			foreach (KingdomDecision kingdomDecision2 in list)
			{
				kingdom.RemoveDecision(kingdomDecision2);
				bool flag;
				if (!kingdomDecision2.DetermineChooser().Leader.IsHumanPlayerCharacter)
				{
					flag = kingdomDecision2.DetermineSupporters().Any((Supporter x) => x.IsPlayer);
				}
				else
				{
					flag = true;
				}
				bool flag2 = flag;
				CampaignEventDispatcher.Instance.OnKingdomDecisionCancelled(kingdomDecision2, flag2);
			}
			foreach (KingdomDecision kingdomDecision3 in list2)
			{
				new KingdomElection(kingdomDecision3).StartElectionWithoutPlayer();
			}
		}

		// Token: 0x060037DA RID: 14298 RVA: 0x000FBC80 File Offset: 0x000F9E80
		private void OnPeaceMade(IFaction side1Faction, IFaction side2Faction, MakePeaceAction.MakePeaceDetail detail)
		{
			this.HandleDiplomaticChangeBetweenFactions(side1Faction, side2Faction);
		}

		// Token: 0x060037DB RID: 14299 RVA: 0x000FBC8A File Offset: 0x000F9E8A
		private void OnWarDeclared(IFaction side1Faction, IFaction side2Faction, DeclareWarAction.DeclareWarDetail detail)
		{
			this.HandleDiplomaticChangeBetweenFactions(side1Faction, side2Faction);
		}

		// Token: 0x060037DC RID: 14300 RVA: 0x000FBC94 File Offset: 0x000F9E94
		private void HandleDiplomaticChangeBetweenFactions(IFaction side1Faction, IFaction side2Faction)
		{
			if (side1Faction.IsKingdomFaction && side2Faction.IsKingdomFaction)
			{
				this.UpdateKingdomDecisions((Kingdom)side1Faction);
				this.UpdateKingdomDecisions((Kingdom)side2Faction);
			}
		}

		// Token: 0x060037DD RID: 14301 RVA: 0x000FBCC0 File Offset: 0x000F9EC0
		private KingdomDecision GetRandomWarDecision(Clan clan)
		{
			KingdomDecision kingdomDecision = null;
			Kingdom kingdom = clan.Kingdom;
			if (kingdom.UnresolvedDecisions.FirstOrDefault((KingdomDecision x) => x is DeclareWarDecision) != null)
			{
				return null;
			}
			Kingdom randomElementWithPredicate = Kingdom.All.GetRandomElementWithPredicate((Kingdom x) => x != kingdom && !x.IsAtWarWith(kingdom) && x.GetStanceWith(kingdom).PeaceDeclarationDate.ElapsedDaysUntilNow > 20f);
			if (randomElementWithPredicate != null && this.ConsiderWar(clan, kingdom, randomElementWithPredicate))
			{
				kingdomDecision = new DeclareWarDecision(clan, randomElementWithPredicate);
			}
			return kingdomDecision;
		}

		// Token: 0x060037DE RID: 14302 RVA: 0x000FBD48 File Offset: 0x000F9F48
		private KingdomDecision GetRandomPeaceDecision(Clan clan)
		{
			KingdomDecision kingdomDecision = null;
			Kingdom kingdom = clan.Kingdom;
			if (kingdom.UnresolvedDecisions.FirstOrDefault((KingdomDecision x) => x is MakePeaceKingdomDecision) != null)
			{
				return null;
			}
			Kingdom randomElementWithPredicate = Kingdom.All.GetRandomElementWithPredicate((Kingdom x) => x.IsAtWarWith(kingdom));
			MakePeaceKingdomDecision makePeaceKingdomDecision;
			if (randomElementWithPredicate != null && this.ConsiderPeace(clan, randomElementWithPredicate.RulingClan, kingdom, randomElementWithPredicate, out makePeaceKingdomDecision))
			{
				kingdomDecision = makePeaceKingdomDecision;
			}
			return kingdomDecision;
		}

		// Token: 0x060037DF RID: 14303 RVA: 0x000FBDD4 File Offset: 0x000F9FD4
		private bool ConsiderWar(Clan clan, Kingdom kingdom, IFaction otherFaction)
		{
			int num = Campaign.Current.Models.DiplomacyModel.GetInfluenceCostOfProposingWar(kingdom) / 2;
			if (clan.Influence < (float)num)
			{
				return false;
			}
			DeclareWarDecision declareWarDecision = new DeclareWarDecision(clan, otherFaction);
			if (declareWarDecision.CalculateSupport(clan) > 50f)
			{
				float kingdomSupportForDecision = this.GetKingdomSupportForDecision(declareWarDecision);
				if (MBRandom.RandomFloat < 1.4f * kingdomSupportForDecision - 0.55f)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060037E0 RID: 14304 RVA: 0x000FBE3A File Offset: 0x000FA03A
		private float GetKingdomSupportForWar(Clan clan, Kingdom kingdom, IFaction otherFaction)
		{
			return new KingdomElection(new DeclareWarDecision(clan, otherFaction)).GetLikelihoodForSponsor(clan);
		}

		// Token: 0x060037E1 RID: 14305 RVA: 0x000FBE50 File Offset: 0x000FA050
		private bool ConsiderPeace(Clan clan, Clan otherClan, Kingdom kingdom, IFaction otherFaction, out MakePeaceKingdomDecision decision)
		{
			decision = null;
			int influenceCostOfProposingPeace = Campaign.Current.Models.DiplomacyModel.GetInfluenceCostOfProposingPeace();
			if (clan.Influence < (float)influenceCostOfProposingPeace)
			{
				return false;
			}
			int num = new PeaceBarterable(clan.Leader, kingdom, otherFaction, CampaignTime.Years(1f)).GetValueForFaction(otherFaction);
			int num2 = -num;
			if (clan.MapFaction == Hero.MainHero.MapFaction && otherFaction is Kingdom)
			{
				foreach (Clan clan2 in ((Kingdom)otherFaction).Clans)
				{
					if (clan2.Leader != clan2.MapFaction.Leader)
					{
						int valueForFaction = new PeaceBarterable(clan2.Leader, kingdom, otherFaction, CampaignTime.Years(1f)).GetValueForFaction(clan2);
						if (valueForFaction < num)
						{
							num = valueForFaction;
						}
					}
				}
				num2 = -num;
			}
			else
			{
				num2 += 30000;
			}
			if (otherFaction is Clan && num2 < 0)
			{
				num2 = 0;
			}
			float num3 = 0.5f;
			if (otherFaction == Hero.MainHero.MapFaction)
			{
				PeaceBarterable peaceBarterable = new PeaceBarterable(clan.MapFaction.Leader, kingdom, otherFaction, CampaignTime.Years(1f));
				int num4 = peaceBarterable.GetValueForFaction(clan.MapFaction);
				int num5 = 0;
				int num6 = 1;
				if (clan.MapFaction is Kingdom)
				{
					foreach (Clan clan3 in ((Kingdom)clan.MapFaction).Clans)
					{
						if (clan3.Leader != clan3.MapFaction.Leader)
						{
							int valueForFaction2 = peaceBarterable.GetValueForFaction(clan3);
							if (valueForFaction2 < num4)
							{
								num4 = valueForFaction2;
							}
							num5 += valueForFaction2;
							num6++;
						}
					}
				}
				float num7 = (float)num5 / (float)num6;
				int num8 = (int)(0.65f * num7 + 0.35f * (float)num4);
				if (num8 > num2)
				{
					num2 = num8;
					num3 = 0.2f;
				}
			}
			int num9 = num2;
			if (num2 > -5000 && num2 < 5000)
			{
				num2 = 0;
			}
			int dailyTributeForValue = Campaign.Current.Models.DiplomacyModel.GetDailyTributeForValue(num2);
			decision = new MakePeaceKingdomDecision(clan, otherFaction, dailyTributeForValue, true);
			if (decision.CalculateSupport(clan) > 5f)
			{
				float kingdomSupportForDecision = this.GetKingdomSupportForDecision(decision);
				if (MBRandom.RandomFloat < 2f * (kingdomSupportForDecision - num3))
				{
					if (otherFaction == Hero.MainHero.MapFaction)
					{
						num2 = num9 + 15000;
						if (num2 > -5000 && num2 < 5000)
						{
							num2 = 0;
						}
						int dailyTributeForValue2 = Campaign.Current.Models.DiplomacyModel.GetDailyTributeForValue(num2);
						decision = new MakePeaceKingdomDecision(clan, otherFaction, dailyTributeForValue2, true);
					}
					return true;
				}
			}
			return false;
		}

		// Token: 0x060037E2 RID: 14306 RVA: 0x000FC120 File Offset: 0x000FA320
		private float GetKingdomSupportForPeace(Clan clan, Clan otherClan, Kingdom kingdom, IFaction otherFaction)
		{
			int num = Campaign.Current.Models.DiplomacyModel.GetInfluenceCostOfProposingPeace() / 2;
			int num2 = -new PeaceBarterable(clan.Leader, kingdom, otherFaction, CampaignTime.Years(1f)).GetValueForFaction(otherFaction);
			if (otherFaction is Clan && num2 < 0)
			{
				num2 = 0;
			}
			if (num2 > -5000 && num2 < 5000)
			{
				num2 = 0;
			}
			int dailyTributeForValue = Campaign.Current.Models.DiplomacyModel.GetDailyTributeForValue(num2);
			return new KingdomElection(new MakePeaceKingdomDecision(clan, otherFaction, dailyTributeForValue, true)).GetLikelihoodForSponsor(clan);
		}

		// Token: 0x060037E3 RID: 14307 RVA: 0x000FC1B4 File Offset: 0x000FA3B4
		private KingdomDecision GetRandomPolicyDecision(Clan clan)
		{
			KingdomDecision kingdomDecision = null;
			Kingdom kingdom = clan.Kingdom;
			if (kingdom.UnresolvedDecisions.FirstOrDefault((KingdomDecision x) => x is KingdomPolicyDecision) != null)
			{
				return null;
			}
			if (clan.Influence < 200f)
			{
				return null;
			}
			PolicyObject randomElement = PolicyObject.All.GetRandomElement<PolicyObject>();
			bool flag = kingdom.ActivePolicies.Contains(randomElement);
			if (this.ConsiderPolicy(clan, kingdom, randomElement, flag))
			{
				kingdomDecision = new KingdomPolicyDecision(clan, randomElement, flag);
			}
			return kingdomDecision;
		}

		// Token: 0x060037E4 RID: 14308 RVA: 0x000FC238 File Offset: 0x000FA438
		private bool ConsiderPolicy(Clan clan, Kingdom kingdom, PolicyObject policy, bool invert)
		{
			int influenceCostOfPolicyProposalAndDisavowal = Campaign.Current.Models.DiplomacyModel.GetInfluenceCostOfPolicyProposalAndDisavowal();
			if (clan.Influence < (float)influenceCostOfPolicyProposalAndDisavowal)
			{
				return false;
			}
			KingdomPolicyDecision kingdomPolicyDecision = new KingdomPolicyDecision(clan, policy, invert);
			if (kingdomPolicyDecision.CalculateSupport(clan) > 50f)
			{
				float kingdomSupportForDecision = this.GetKingdomSupportForDecision(kingdomPolicyDecision);
				if ((double)MBRandom.RandomFloat < (double)kingdomSupportForDecision - 0.55)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060037E5 RID: 14309 RVA: 0x000FC29D File Offset: 0x000FA49D
		private float GetKingdomSupportForPolicy(Clan clan, Kingdom kingdom, PolicyObject policy, bool invert)
		{
			Campaign.Current.Models.DiplomacyModel.GetInfluenceCostOfPolicyProposalAndDisavowal();
			return new KingdomElection(new KingdomPolicyDecision(clan, policy, invert)).GetLikelihoodForSponsor(clan);
		}

		// Token: 0x060037E6 RID: 14310 RVA: 0x000FC2C8 File Offset: 0x000FA4C8
		private KingdomDecision GetRandomAnnexationDecision(Clan clan)
		{
			KingdomDecision kingdomDecision = null;
			Kingdom kingdom = clan.Kingdom;
			if (kingdom.UnresolvedDecisions.FirstOrDefault((KingdomDecision x) => x is KingdomPolicyDecision) != null)
			{
				return null;
			}
			if (clan.Influence < 300f)
			{
				return null;
			}
			Clan randomElement = kingdom.Clans.GetRandomElement<Clan>();
			if (randomElement != null && randomElement != clan && randomElement.GetRelationWithClan(clan) < -25)
			{
				if (randomElement.Fiefs.Count == 0)
				{
					return null;
				}
				Town randomElement2 = randomElement.Fiefs.GetRandomElement<Town>();
				if (this.ConsiderAnnex(clan, kingdom, randomElement, randomElement2))
				{
					kingdomDecision = new SettlementClaimantPreliminaryDecision(clan, randomElement2.Settlement);
				}
			}
			return kingdomDecision;
		}

		// Token: 0x060037E7 RID: 14311 RVA: 0x000FC370 File Offset: 0x000FA570
		private bool ConsiderAnnex(Clan clan, Kingdom kingdom, Clan targetClan, Town targetSettlement)
		{
			int influenceCostOfAnnexation = Campaign.Current.Models.DiplomacyModel.GetInfluenceCostOfAnnexation(kingdom);
			if (clan.Influence < (float)influenceCostOfAnnexation)
			{
				return false;
			}
			SettlementClaimantPreliminaryDecision settlementClaimantPreliminaryDecision = new SettlementClaimantPreliminaryDecision(clan, targetSettlement.Settlement);
			if (settlementClaimantPreliminaryDecision.CalculateSupport(clan) > 50f)
			{
				float kingdomSupportForDecision = this.GetKingdomSupportForDecision(settlementClaimantPreliminaryDecision);
				if ((double)MBRandom.RandomFloat < (double)kingdomSupportForDecision - 0.6)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060037E8 RID: 14312 RVA: 0x000FC3DA File Offset: 0x000FA5DA
		private float GetKingdomSupportForDecision(KingdomDecision decision)
		{
			return new KingdomElection(decision).GetLikelihoodForOutcome(0);
		}

		// Token: 0x060037E9 RID: 14313 RVA: 0x000FC3E8 File Offset: 0x000FA5E8
		private void SessionLaunched(CampaignGameStarter starter)
		{
		}

		// Token: 0x060037EA RID: 14314 RVA: 0x000FC3EA File Offset: 0x000FA5EA
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<List<KingdomDecision>>("_kingdomDecisionsList", ref this._kingdomDecisionsList);
		}

		// Token: 0x0400118A RID: 4490
		private const int KingdomDecisionProposalCooldownInDays = 1;

		// Token: 0x0400118B RID: 4491
		private const float ClanInterestModifier = 1f;

		// Token: 0x0400118C RID: 4492
		private const float DecisionSuccessChanceModifier = 1f;

		// Token: 0x0400118D RID: 4493
		private List<KingdomDecision> _kingdomDecisionsList;

		// Token: 0x020006FD RID: 1789
		// (Invoke) Token: 0x06005549 RID: 21833
		private delegate KingdomDecision KingdomDecisionCreatorDelegate(Clan sponsorClan);
	}
}
