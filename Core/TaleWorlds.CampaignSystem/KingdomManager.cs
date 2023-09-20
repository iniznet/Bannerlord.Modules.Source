﻿using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem
{
	public class KingdomManager
	{
		internal static void AutoGeneratedStaticCollectObjectsKingdomManager(object o, List<object> collectedObjects)
		{
			((KingdomManager)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			collectedObjects.Add(this.PrisonerLordRansomOffered);
		}

		internal static object AutoGeneratedGetMemberValuePlayerMercenaryServiceNextRenewDay(object o)
		{
			return ((KingdomManager)o).PlayerMercenaryServiceNextRenewDay;
		}

		internal static object AutoGeneratedGetMemberValuePrisonerLordRansomOffered(object o)
		{
			return ((KingdomManager)o).PrisonerLordRansomOffered;
		}

		public void OnNewGameCreated()
		{
			foreach (Clan clan in Clan.All)
			{
				clan.UpdateStrength();
			}
		}

		internal void RegisterEvents()
		{
			CampaignEvents.SiegeCompletedEvent.AddNonSerializedListener(this, new Action<Settlement, MobileParty, bool, MapEvent.BattleTypes>(this.SiegeCompleted));
			CampaignEvents.RaidCompletedEvent.AddNonSerializedListener(this, new Action<BattleSideEnum, RaidEventComponent>(this.RaidCompleted));
			CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction, DeclareWarAction.DeclareWarDetail>(this.OnWarDeclared));
			CampaignEvents.DailyTickClanEvent.AddNonSerializedListener(this, new Action<Clan>(this.DailyTickClan));
			CampaignEvents.HourlyTickClanEvent.AddNonSerializedListener(this, new Action<Clan>(this.HourlyTickClan));
			this.QuarterHourlyTickEvent = CampaignPeriodicEventManager.CreatePeriodicEvent(CampaignTime.Hours(0.25f), CampaignTime.Zero);
			this.QuarterHourlyTickEvent.AddHandler(new MBCampaignEvent.CampaignEventDelegate(KingdomManager.QuarterHourlyTick));
		}

		private void OnWarDeclared(IFaction factionDeclaringWar, IFaction factionDeclaredWarAgainst, DeclareWarAction.DeclareWarDetail detail)
		{
			if (factionDeclaringWar is Clan)
			{
				(factionDeclaringWar as Clan).Aggressiveness += (100f - factionDeclaredWarAgainst.Aggressiveness) * 0.5f;
				return;
			}
			(factionDeclaringWar as Kingdom).Aggressiveness += (100f - factionDeclaredWarAgainst.Aggressiveness) * 0.5f;
		}

		private void HourlyTickClan(Clan clan)
		{
			clan.UpdateStrength();
		}

		private void DailyTickClan(Clan clan)
		{
			clan.Aggressiveness += -1f;
			if (clan.Kingdom != null && clan.Kingdom.RulingClan == clan)
			{
				this.DailyTickKingdom(clan.Kingdom);
			}
		}

		private void DailyTickKingdom(Kingdom kingdom)
		{
			kingdom.Aggressiveness += -1f;
		}

		public void CreateKingdom(TextObject kingdomName, TextObject informalName, CultureObject culture, Clan founderClan, MBReadOnlyList<PolicyObject> initialPolicies = null, TextObject encyclopediaText = null, TextObject encyclopediaTitle = null, TextObject encyclopediaRulerTitle = null)
		{
			Kingdom kingdom = Kingdom.CreateKingdom("new_kingdom");
			if (encyclopediaTitle == null)
			{
				encyclopediaTitle = new TextObject("{=ZOEamqUd}Kingdom of {NAME}", null);
				encyclopediaTitle.SetTextVariable("NAME", founderClan.Name);
			}
			if (encyclopediaText == null)
			{
				if (founderClan.IsRebelClan)
				{
					encyclopediaText = new TextObject("{=drZC1Frp}The {KINGDOM_NAME} was created in {CREATION_YEAR} by {RULER.NAME}, leader of a group of {CULTURE_ADJECTIVE} rebels.", null);
				}
				else
				{
					encyclopediaText = new TextObject("{=21yUheIy}The {KINGDOM_NAME} was created in {CREATION_YEAR} by {RULER.NAME}, a rising {CULTURE_ADJECTIVE} warlord.", null);
				}
				encyclopediaText.SetTextVariable("KINGDOM_NAME", encyclopediaTitle);
				encyclopediaText.SetTextVariable("CREATION_YEAR", CampaignTime.Now.GetYear);
				encyclopediaText.SetTextVariable("CULTURE_ADJECTIVE", FactionHelper.GetAdjectiveForFactionCulture(culture));
				StringHelpers.SetCharacterProperties("RULER", founderClan.Leader.CharacterObject, encyclopediaText, false);
			}
			if (encyclopediaRulerTitle == null)
			{
				Kingdom kingdom2 = Kingdom.All.FirstOrDefault((Kingdom x) => x.Culture == culture);
				encyclopediaRulerTitle = ((kingdom2 != null) ? kingdom2.EncyclopediaRulerTitle : TextObject.Empty);
			}
			kingdom.InitializeKingdom(kingdomName, informalName, culture, founderClan.Banner, founderClan.Color, founderClan.Color2, founderClan.HomeSettlement, encyclopediaText, encyclopediaTitle, encyclopediaRulerTitle);
			List<IFaction> list = new List<IFaction>(FactionManager.GetEnemyFactions(founderClan));
			ChangeKingdomAction.ApplyByCreateKingdom(founderClan, kingdom, false);
			foreach (IFaction faction in list)
			{
				DeclareWarAction.ApplyByKingdomCreation(kingdom, faction);
			}
			if (initialPolicies != null)
			{
				foreach (PolicyObject policyObject in initialPolicies)
				{
					kingdom.AddPolicy(policyObject);
				}
			}
			CampaignEventDispatcher.Instance.OnKingdomCreated(kingdom);
		}

		public void AbdicateTheThrone(Kingdom kingdom)
		{
			Clan rulingClan = kingdom.RulingClan;
			if (kingdom.Clans.Count((Clan x) => !x.IsUnderMercenaryService) > 1)
			{
				float num = float.MinValue;
				Clan clan = null;
				foreach (Clan clan2 in kingdom.Clans)
				{
					if (clan2 != rulingClan && clan2.Influence > num)
					{
						num = clan2.Influence;
						clan = clan2;
					}
				}
				ChangeRulingClanAction.Apply(kingdom, clan);
				kingdom.AddDecision(new KingSelectionKingdomDecision(rulingClan, rulingClan)
				{
					IsEnforced = true
				}, true);
				return;
			}
			ChangeKingdomAction.ApplyByLeaveKingdom(kingdom.RulingClan, true);
			foreach (Kingdom kingdom2 in Kingdom.All)
			{
				if (kingdom2.IsAtWarWith(kingdom))
				{
					if (!kingdom2.IsAtWarWith(rulingClan))
					{
						DeclareWarAction.ApplyByDefault(kingdom2, rulingClan);
					}
				}
				else if (kingdom2.IsAtWarWith(rulingClan))
				{
					Debug.FailedAssert("Deviation in peace states between ruling clan & kingdom in abdication", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\KingdomManager.cs", "AbdicateTheThrone", 198);
				}
			}
			DestroyKingdomAction.Apply(kingdom);
		}

		public void RaidCompleted(BattleSideEnum winnerSide, RaidEventComponent raidEvent)
		{
			MapEvent mapEvent = raidEvent.MapEvent;
			if (winnerSide != BattleSideEnum.Defender)
			{
				if (winnerSide != BattleSideEnum.Attacker)
				{
					return;
				}
				using (IEnumerator<PartyBase> enumerator = mapEvent.InvolvedParties.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						PartyBase partyBase = enumerator.Current;
						if (partyBase.MobileParty != null && partyBase.MobileParty.DefaultBehavior == AiBehavior.RaidSettlement && (partyBase.MobileParty.Army == null || partyBase.MobileParty.Army.LeaderParty == partyBase.MobileParty))
						{
							partyBase.MobileParty.Ai.SetMoveModeHold();
						}
					}
					return;
				}
			}
			foreach (PartyBase partyBase2 in mapEvent.InvolvedParties)
			{
				if (partyBase2.MobileParty != null && !partyBase2.MobileParty.IsMilitia && partyBase2.MobileParty.DefaultBehavior == AiBehavior.DefendSettlement && (partyBase2.MobileParty.Army == null || partyBase2.MobileParty.Army.LeaderParty == partyBase2.MobileParty))
				{
					partyBase2.MobileParty.Ai.SetMoveModeHold();
				}
			}
			ChangeVillageStateAction.ApplyBySettingToNormal(mapEvent.MapEventSettlement);
		}

		public void SiegeCompleted(Settlement settlement, MobileParty capturerParty, bool isWin, MapEvent.BattleTypes battleType)
		{
			if ((battleType == MapEvent.BattleTypes.SallyOut || battleType == MapEvent.BattleTypes.Siege) && isWin)
			{
				Kingdom kingdom = capturerParty.MapFaction as Kingdom;
				Hero hero = ((kingdom != null) ? kingdom.Leader : capturerParty.MapFaction.Leader);
				if (capturerParty.LeaderHero != null)
				{
					GainKingdomInfluenceAction.ApplyForCapturingEnemySettlement(capturerParty, (float)Campaign.Current.Models.DiplomacyModel.GetInfluenceAwardForSettlementCapturer(settlement));
				}
				List<MobileParty> list = new List<MobileParty>();
				foreach (PartyBase partyBase in settlement.SiegeEvent.BesiegerCamp.GetInvolvedPartiesForEventType(MapEvent.BattleTypes.Siege))
				{
					list.Add(partyBase.MobileParty);
				}
				settlement.SiegeEvent.BesiegerCamp.RemoveAllSiegeParties();
				foreach (MobileParty mobileParty in list)
				{
					if (mobileParty != MobileParty.MainParty && (mobileParty.Army == null || mobileParty.Army.LeaderParty == mobileParty))
					{
						mobileParty.Ai.DisableForHours(10);
						mobileParty.Ai.SetMoveGoToSettlement(settlement);
						mobileParty.Ai.RecalculateShortTermAi();
					}
				}
				settlement.Party.MemberRoster.Clear();
				ChangeOwnerOfSettlementAction.ApplyBySiege(hero, capturerParty.Party.Owner, settlement);
				Debug.Print(string.Concat(new object[] { capturerParty.Name, " has captured ", settlement, " successfully.\n" }), 0, Debug.DebugColor.Green, 64UL);
			}
		}

		private static void UpdateLordPartyVariablesRelatedToSettlements()
		{
			TWParallel.For(0, Settlement.All.Count, delegate(int startInclusive, int endExclusive)
			{
				for (int i = startInclusive; i < endExclusive; i++)
				{
					Settlement.All[i].NumberOfLordPartiesTargeting = 0;
				}
			}, 16);
			foreach (MobileParty mobileParty in MobileParty.AllLordParties)
			{
				if ((mobileParty.Army == null || mobileParty.Army.LeaderParty == mobileParty) && mobileParty.TargetSettlement != null && mobileParty.CurrentSettlement != mobileParty.TargetSettlement)
				{
					mobileParty.TargetSettlement.NumberOfLordPartiesTargeting += ((mobileParty.Army == null) ? 1 : (1 + mobileParty.Army.LeaderParty.AttachedParties.Count));
				}
			}
		}

		public void RelinquishSettlementOwnership(Settlement settlement)
		{
			SettlementClaimantDecision settlementClaimantDecision = new SettlementClaimantDecision(settlement.OwnerClan, settlement, null, settlement.OwnerClan);
			settlement.OwnerClan.Kingdom.AddDecision(settlementClaimantDecision, true);
		}

		public void GiftSettlementOwnership(Settlement settlement, Clan receiverClan)
		{
			int num = (settlement.IsTown ? Campaign.Current.Models.DiplomacyModel.GiftingTownRelationshipBonus : Campaign.Current.Models.DiplomacyModel.GiftingCastleRelationshipBonus);
			ChangeRelationAction.ApplyRelationChangeBetweenHeroes(settlement.OwnerClan.Leader, receiverClan.Leader, num, true);
			ChangeOwnerOfSettlementAction.ApplyByGift(settlement, receiverClan.Leader);
		}

		public IEnumerable<Clan> GetEligibleClansForSettlementOwnershipGift(Settlement settlement)
		{
			Kingdom kingdom = settlement.OwnerClan.Kingdom;
			foreach (Clan clan in kingdom.Clans)
			{
				if (clan != kingdom.RulingClan)
				{
					yield return clan;
				}
			}
			List<Clan>.Enumerator enumerator = default(List<Clan>.Enumerator);
			yield break;
			yield break;
		}

		private static void QuarterHourlyTick(MBCampaignEvent campaignevent, object[] delegateparams)
		{
			KingdomManager.UpdateLordPartyVariablesRelatedToSettlements();
		}

		public int GetMercenaryWageAmount(Hero hero)
		{
			return (int)hero.Clan.Influence * hero.Clan.MercenaryAwardMultiplier;
		}

		[SaveableField(2)]
		public float PlayerMercenaryServiceNextRenewDay = -1f;

		[SaveableField(3)]
		public Hero PrisonerLordRansomOffered;

		public MBCampaignEvent QuarterHourlyTickEvent;
	}
}
