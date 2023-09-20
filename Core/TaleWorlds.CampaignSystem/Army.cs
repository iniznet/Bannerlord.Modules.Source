using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem
{
	public class Army
	{
		public MBReadOnlyList<MobileParty> Parties
		{
			get
			{
				return this._parties;
			}
		}

		public TextObject EncyclopediaLinkWithName
		{
			get
			{
				return this.ArmyOwner.EncyclopediaLinkWithName;
			}
		}

		[SaveableProperty(2)]
		public Army.AIBehaviorFlags AIBehavior { get; set; }

		[SaveableProperty(3)]
		public Army.ArmyTypes ArmyType { get; set; }

		[SaveableProperty(4)]
		public Hero ArmyOwner { get; set; }

		[SaveableProperty(5)]
		public float Cohesion { get; set; }

		public float DailyCohesionChange
		{
			get
			{
				return Campaign.Current.Models.ArmyManagementCalculationModel.CalculateDailyCohesionChange(this, false).ResultNumber;
			}
		}

		public ExplainedNumber DailyCohesionChangeExplanation
		{
			get
			{
				return Campaign.Current.Models.ArmyManagementCalculationModel.CalculateDailyCohesionChange(this, true);
			}
		}

		public int CohesionThresholdForDispersion
		{
			get
			{
				return Campaign.Current.Models.ArmyManagementCalculationModel.CohesionThresholdForDispersion;
			}
		}

		[SaveableProperty(13)]
		public float Morale { get; private set; }

		[SaveableProperty(14)]
		public MobileParty LeaderParty { get; private set; }

		public int LeaderPartyAndAttachedPartiesCount
		{
			get
			{
				return this.LeaderParty.AttachedParties.Count + 1;
			}
		}

		public override string ToString()
		{
			return this.Name.ToString();
		}

		public float TotalStrength
		{
			get
			{
				float num = this.LeaderParty.Party.TotalStrength;
				foreach (MobileParty mobileParty in this.LeaderParty.AttachedParties)
				{
					num += mobileParty.Party.TotalStrength;
				}
				return num;
			}
		}

		public Kingdom Kingdom
		{
			get
			{
				return this._kingdom;
			}
			set
			{
				if (value != this._kingdom)
				{
					Kingdom kingdom = this._kingdom;
					if (kingdom != null)
					{
						kingdom.RemoveArmyInternal(this);
					}
					this._kingdom = value;
					Kingdom kingdom2 = this._kingdom;
					if (kingdom2 == null)
					{
						return;
					}
					kingdom2.AddArmyInternal(this);
				}
			}
		}

		public IMapPoint AiBehaviorObject
		{
			get
			{
				return this._aiBehaviorObject;
			}
			set
			{
				if (value != this._aiBehaviorObject && this.Parties.Contains(MobileParty.MainParty) && this.LeaderParty != MobileParty.MainParty)
				{
					this.StopTrackingTargetSettlement();
					this.StartTrackingTargetSettlement(value);
				}
				if (value == null)
				{
					this.AIBehavior = Army.AIBehaviorFlags.Unassigned;
				}
				this._aiBehaviorObject = value;
			}
		}

		[SaveableProperty(17)]
		public TextObject Name { get; private set; }

		public int TotalHealthyMembers
		{
			get
			{
				return this.LeaderParty.Party.NumberOfHealthyMembers + this.LeaderParty.AttachedParties.Sum((MobileParty mobileParty) => mobileParty.Party.NumberOfHealthyMembers);
			}
		}

		public int TotalManCount
		{
			get
			{
				return this.LeaderParty.Party.MemberRoster.TotalManCount + this.LeaderParty.AttachedParties.Sum((MobileParty mobileParty) => mobileParty.Party.MemberRoster.TotalManCount);
			}
		}

		public int TotalRegularCount
		{
			get
			{
				return this.LeaderParty.Party.MemberRoster.TotalRegulars + this.LeaderParty.AttachedParties.Sum((MobileParty mobileParty) => mobileParty.Party.MemberRoster.TotalRegulars);
			}
		}

		public Army(Kingdom kingdom, MobileParty leaderParty, Army.ArmyTypes armyType)
		{
			this.Kingdom = kingdom;
			this._parties = new MBList<MobileParty>();
			this._armyGatheringTime = 0f;
			this._creationTime = CampaignTime.Now;
			this.LeaderParty = leaderParty;
			this.LeaderParty.Army = this;
			this.ArmyOwner = this.LeaderParty.LeaderHero;
			this.UpdateName();
			this.ArmyType = armyType;
			this.AddEventHandlers();
			this.Cohesion = 100f;
		}

		public void UpdateName()
		{
			this.Name = new TextObject("{=nbmctMLk}{LEADER_NAME}{.o} Army", null);
			this.Name.SetTextVariable("LEADER_NAME", (this.ArmyOwner != null) ? this.ArmyOwner.Name : ((this.LeaderParty.Owner != null) ? this.LeaderParty.Owner.Name : TextObject.Empty));
		}

		private void AddEventHandlers()
		{
			if (this._creationTime == default(CampaignTime))
			{
				this._creationTime = CampaignTime.HoursFromNow(MBRandom.RandomFloat - 2f);
			}
			CampaignTime campaignTime = CampaignTime.Now - this._creationTime;
			CampaignTime campaignTime2 = CampaignTime.Hours(1f + (float)((int)campaignTime.ToHours)) - campaignTime;
			this._hourlyTickEvent = CampaignPeriodicEventManager.CreatePeriodicEvent(CampaignTime.Hours(1f), campaignTime2);
			this._hourlyTickEvent.AddHandler(new MBCampaignEvent.CampaignEventDelegate(this.HourlyTick));
			this._tickEvent = CampaignPeriodicEventManager.CreatePeriodicEvent(CampaignTime.Hours(0.1f), CampaignTime.Hours(1f));
			this._tickEvent.AddHandler(new MBCampaignEvent.CampaignEventDelegate(this.Tick));
		}

		internal void OnAfterLoad()
		{
			this.AddEventHandlers();
		}

		[LoadInitializationCallback]
		private void OnLoad(MetaData metaData)
		{
			if (this.AiBehaviorObject == null)
			{
				this.AIBehavior = Army.AIBehaviorFlags.Unassigned;
			}
		}

		public bool DoesLeaderPartyAndAttachedPartiesContain(MobileParty party)
		{
			return this.LeaderParty == party || this.LeaderParty.AttachedParties.IndexOf(party) >= 0;
		}

		public void BoostCohesionWithInfluence(float cohesionToGain, int cost)
		{
			if (this.LeaderParty.LeaderHero.Clan.Influence >= (float)cost)
			{
				ChangeClanInfluenceAction.Apply(this.LeaderParty.LeaderHero.Clan, (float)(-(float)cost));
				this.Cohesion += cohesionToGain;
				this._numberOfBoosts++;
			}
		}

		private void ThinkAboutCohesionBoost()
		{
			float num = 0f;
			foreach (MobileParty mobileParty in this.Parties)
			{
				float partySizeRatio = mobileParty.PartySizeRatio;
				num += partySizeRatio;
			}
			float num2 = num / (float)this.Parties.Count;
			float num3 = MathF.Min(1f, num2);
			float num4 = Campaign.Current.Models.TargetScoreCalculatingModel.CurrentObjectiveValue(this.LeaderParty);
			if (num4 > 0.01f)
			{
				num4 *= num3;
				num4 *= ((this._numberOfBoosts == 0) ? 1f : (1f / MathF.Pow(1f + (float)this._numberOfBoosts, 0.7f)));
				ArmyManagementCalculationModel armyManagementCalculationModel = Campaign.Current.Models.ArmyManagementCalculationModel;
				float num5 = MathF.Min(100f, 100f - this.Cohesion);
				int num6 = armyManagementCalculationModel.CalculateTotalInfluenceCost(this, num5);
				if (this.LeaderParty.Party.Owner.Clan.Influence > (float)num6)
				{
					float num7 = MathF.Min(9f, MathF.Sqrt(this.LeaderParty.Party.Owner.Clan.Influence / (float)num6));
					float num8 = ((this.LeaderParty.BesiegedSettlement != null) ? 2f : 1f);
					if (this.LeaderParty.BesiegedSettlement == null && this.LeaderParty.DefaultBehavior == AiBehavior.BesiegeSettlement)
					{
						float num9 = this.LeaderParty.Position2D.Distance(this.LeaderParty.TargetSettlement.Position2D);
						if (num9 < 125f)
						{
							num8 += (1f - num9 / 125f) * (1f - num9 / 125f);
						}
					}
					float num10 = num4 * num8 * 0.25f * num7;
					if (MBRandom.RandomFloat < num10)
					{
						this.BoostCohesionWithInfluence(num5, num6);
					}
				}
			}
		}

		public void RecalculateArmyMorale()
		{
			float num = 0f;
			foreach (MobileParty mobileParty in this.Parties)
			{
				num += mobileParty.Morale;
			}
			this.Morale = num / (float)this.Parties.Count;
		}

		private void HourlyTick(MBCampaignEvent campaignEvent, object[] delegateParams)
		{
			bool flag = this.LeaderParty.CurrentSettlement != null && this.LeaderParty.CurrentSettlement.SiegeEvent != null;
			if (this.LeaderParty.MapEvent != null || flag)
			{
				return;
			}
			this.RecalculateArmyMorale();
			this.Cohesion += this.DailyCohesionChange / 24f;
			if (this.LeaderParty == MobileParty.MainParty)
			{
				this.CheckMainPartyGathering();
				this.CheckMainPartyTravelingToAssignment();
			}
			else
			{
				this.MoveLeaderToGatheringLocationIfNeeded();
				if (this.Cohesion < 50f)
				{
					this.ThinkAboutCohesionBoost();
					if (this.Cohesion < 30f && this.LeaderParty.MapEvent == null && this.LeaderParty.SiegeEvent == null)
					{
						DisbandArmyAction.ApplyByCohesionDepleted(this);
						return;
					}
				}
				switch (this.AIBehavior)
				{
				case Army.AIBehaviorFlags.Gathering:
					this.ThinkAboutConcludingArmyGathering();
					break;
				case Army.AIBehaviorFlags.WaitingForArmyMembers:
					this.ThinkAboutTravelingToAssignment();
					break;
				case Army.AIBehaviorFlags.TravellingToAssignment:
					if (this.ArmyType == Army.ArmyTypes.Besieger)
					{
						this.IsAtSiegeLocation();
					}
					break;
				case Army.AIBehaviorFlags.Defending:
					switch (this.ArmyType)
					{
					case Army.ArmyTypes.Besieger:
						if (this.AnyoneBesiegingTarget())
						{
							this.FinishArmyObjective();
						}
						else
						{
							this.IsAtSiegeLocation();
						}
						break;
					case Army.ArmyTypes.Raider:
					case Army.ArmyTypes.Defender:
					case Army.ArmyTypes.Patrolling:
					case Army.ArmyTypes.NumberOfArmyTypes:
						break;
					default:
						throw new ArgumentOutOfRangeException();
					}
					break;
				}
			}
			this.CheckArmyDispersion();
			this.CallArmyMembersToArmyIfNeeded();
			this.ApplyHostileActionInfluenceAwards();
		}

		private void Tick(MBCampaignEvent campaignevent, object[] delegateparams)
		{
			foreach (MobileParty mobileParty in this._parties)
			{
				if (mobileParty.AttachedTo == null && mobileParty.Army != null && mobileParty.ShortTermTargetParty == this.LeaderParty && mobileParty.MapEvent == null && (mobileParty.Position2D - this.LeaderParty.Position2D).LengthSquared < Campaign.Current.Models.EncounterModel.NeededMaximumDistanceForEncounteringMobileParty)
				{
					this.AddPartyToMergedParties(mobileParty);
					if (mobileParty.IsMainParty)
					{
						Campaign.Current.CameraFollowParty = this.LeaderParty.Party;
					}
					CampaignEventDispatcher.Instance.OnArmyOverlaySetDirty();
				}
			}
		}

		private void CheckArmyDispersion()
		{
			if (this.LeaderParty == MobileParty.MainParty)
			{
				if (this.Cohesion <= 0.1f)
				{
					DisbandArmyAction.ApplyByCohesionDepleted(this);
					GameMenu.ActivateGameMenu("army_dispersed");
					MBTextManager.SetTextVariable("ARMY_DISPERSE_REASON", new TextObject("{=rJBgDaxe}Your army has disbanded due to lack of cohesion.", null), false);
					return;
				}
			}
			else
			{
				int num = (this.LeaderParty.Party.IsStarving ? 1 : 0);
				using (List<MobileParty>.Enumerator enumerator = this.LeaderParty.AttachedParties.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.Party.IsStarving)
						{
							num++;
						}
					}
				}
				if ((float)num / (float)this.LeaderPartyAndAttachedPartiesCount > 0.5f)
				{
					DisbandArmyAction.ApplyByFoodProblem(this);
					return;
				}
				if (MBRandom.RandomFloat < 0.25f)
				{
					if (!FactionManager.GetEnemyFactions(this.LeaderParty.MapFaction as Kingdom).AnyQ((IFaction x) => x.Fiefs.Any<Town>()))
					{
						DisbandArmyAction.ApplyByNoActiveWar(this);
						return;
					}
				}
				if (this.Cohesion <= 0.1f)
				{
					DisbandArmyAction.ApplyByCohesionDepleted(this);
					return;
				}
				if (!this.LeaderParty.IsActive)
				{
					DisbandArmyAction.ApplyByUnknownReason(this);
					return;
				}
			}
		}

		private void MoveLeaderToGatheringLocationIfNeeded()
		{
			if (this.AiBehaviorObject != null && (this.AIBehavior == Army.AIBehaviorFlags.Gathering || this.AIBehavior == Army.AIBehaviorFlags.WaitingForArmyMembers) && this.LeaderParty.MapEvent == null && this.LeaderParty.ShortTermBehavior == AiBehavior.Hold)
			{
				Settlement settlement = this.AiBehaviorObject as Settlement;
				Vec2 vec = (settlement.IsFortification ? settlement.GatePosition : settlement.Position2D);
				if (!settlement.IsUnderSiege && !settlement.IsUnderRaid)
				{
					this.LeaderParty.SendPartyToReachablePointAroundPosition(vec, 6f, 3f);
				}
			}
		}

		private void CheckMainPartyTravelingToAssignment()
		{
			float num;
			if (this.AIBehavior == Army.AIBehaviorFlags.Gathering && this.AiBehaviorObject != null && !Campaign.Current.Models.MapDistanceModel.GetDistance(this.AiBehaviorObject, MobileParty.MainParty, 3.5f, out num))
			{
				this.AIBehavior = Army.AIBehaviorFlags.TravellingToAssignment;
			}
		}

		private void CallArmyMembersToArmyIfNeeded()
		{
			for (int i = this.Parties.Count - 1; i >= 0; i--)
			{
				MobileParty mobileParty = this.Parties[i];
				if (mobileParty != this.LeaderParty && !this.DoesLeaderPartyAndAttachedPartiesContain(mobileParty) && mobileParty != MobileParty.MainParty)
				{
					if (mobileParty.MapEvent == null && mobileParty.TargetParty != this.LeaderParty && (mobileParty.CurrentSettlement == null || !mobileParty.CurrentSettlement.IsUnderSiege))
					{
						mobileParty.Ai.SetMoveEscortParty(this.LeaderParty);
					}
					if (mobileParty.Party.IsStarving)
					{
						mobileParty.Army = null;
					}
				}
			}
		}

		private void ApplyHostileActionInfluenceAwards()
		{
			if (this.LeaderParty.LeaderHero != null && this.LeaderParty.MapEvent != null)
			{
				if (this.LeaderParty.MapEvent.IsRaid && this.LeaderParty.MapEvent.DefenderSide.TroopCount == 0)
				{
					float hourlyInfluenceAwardForRaidingEnemyVillage = Campaign.Current.Models.DiplomacyModel.GetHourlyInfluenceAwardForRaidingEnemyVillage(this.LeaderParty);
					GainKingdomInfluenceAction.ApplyForRaidingEnemyVillage(this.LeaderParty, hourlyInfluenceAwardForRaidingEnemyVillage);
					return;
				}
				if (this.LeaderParty.BesiegedSettlement != null && this.LeaderParty.MapFaction.IsAtWarWith(this.LeaderParty.BesiegedSettlement.MapFaction))
				{
					float hourlyInfluenceAwardForBesiegingEnemyFortification = Campaign.Current.Models.DiplomacyModel.GetHourlyInfluenceAwardForBesiegingEnemyFortification(this.LeaderParty);
					GainKingdomInfluenceAction.ApplyForBesiegingEnemySettlement(this.LeaderParty, hourlyInfluenceAwardForBesiegingEnemyFortification);
				}
			}
		}

		private void CheckMainPartyGathering()
		{
			float num;
			if (this.AIBehavior == Army.AIBehaviorFlags.PreGathering && this.AiBehaviorObject != null && Campaign.Current.Models.MapDistanceModel.GetDistance(this.AiBehaviorObject, MobileParty.MainParty, 3.5f, out num))
			{
				this.AIBehavior = Army.AIBehaviorFlags.Gathering;
			}
		}

		private Army.MainPartyCurrentAction GetMainPartyCurrentAction()
		{
			if (PlayerEncounter.EncounterSettlement == null)
			{
				return Army.MainPartyCurrentAction.PatrolAroundSettlement;
			}
			Settlement encounterSettlement = PlayerEncounter.EncounterSettlement;
			if (MobileParty.MainParty.IsActive)
			{
				if (encounterSettlement.IsUnderSiege)
				{
					if (encounterSettlement.MapFaction.IsAtWarWith(MobileParty.MainParty.MapFaction))
					{
						return Army.MainPartyCurrentAction.BesiegeSettlement;
					}
					return Army.MainPartyCurrentAction.DefendingSettlement;
				}
				else if (encounterSettlement.IsUnderRaid)
				{
					if (encounterSettlement.MapFaction.IsAtWarWith(MobileParty.MainParty.MapFaction))
					{
						return Army.MainPartyCurrentAction.RaidSettlement;
					}
					return Army.MainPartyCurrentAction.DefendingSettlement;
				}
			}
			return Army.MainPartyCurrentAction.GoToSettlement;
		}

		public static Army.ArmyLeaderThinkReason GetBehaviorChangeExplanation(Army.AIBehaviorFlags previousBehavior, Army.AIBehaviorFlags currentBehavior)
		{
			switch (previousBehavior)
			{
			case Army.AIBehaviorFlags.Unassigned:
				if (currentBehavior == Army.AIBehaviorFlags.TravellingToAssignment)
				{
					return Army.ArmyLeaderThinkReason.FromUnassignedToTravelling;
				}
				if (currentBehavior == Army.AIBehaviorFlags.Patrolling)
				{
					return Army.ArmyLeaderThinkReason.FromUnassignedToPatrolling;
				}
				break;
			case Army.AIBehaviorFlags.Gathering:
				if (currentBehavior == Army.AIBehaviorFlags.WaitingForArmyMembers)
				{
					return Army.ArmyLeaderThinkReason.FromGatheringToWaiting;
				}
				break;
			case Army.AIBehaviorFlags.WaitingForArmyMembers:
				if (currentBehavior == Army.AIBehaviorFlags.TravellingToAssignment)
				{
					return Army.ArmyLeaderThinkReason.FromWaitingToTravelling;
				}
				break;
			case Army.AIBehaviorFlags.TravellingToAssignment:
				switch (currentBehavior)
				{
				case Army.AIBehaviorFlags.TravellingToAssignment:
					return Army.ArmyLeaderThinkReason.ChangingTarget;
				case Army.AIBehaviorFlags.Besieging:
					return Army.ArmyLeaderThinkReason.FromTravellingToBesieging;
				case Army.AIBehaviorFlags.Raiding:
					return Army.ArmyLeaderThinkReason.FromTravellingToRaiding;
				case Army.AIBehaviorFlags.Defending:
					return Army.ArmyLeaderThinkReason.FromTravellingToDefending;
				}
				break;
			case Army.AIBehaviorFlags.Besieging:
				if (currentBehavior == Army.AIBehaviorFlags.TravellingToAssignment)
				{
					return Army.ArmyLeaderThinkReason.FromBesiegingToTravelling;
				}
				if (currentBehavior == Army.AIBehaviorFlags.Defending)
				{
					return Army.ArmyLeaderThinkReason.FromBesiegingToDefending;
				}
				break;
			case Army.AIBehaviorFlags.Raiding:
				if (currentBehavior == Army.AIBehaviorFlags.TravellingToAssignment)
				{
					return Army.ArmyLeaderThinkReason.FromRaidingToTravelling;
				}
				break;
			case Army.AIBehaviorFlags.Defending:
				if (currentBehavior == Army.AIBehaviorFlags.TravellingToAssignment)
				{
					return Army.ArmyLeaderThinkReason.FromDefendingToTravelling;
				}
				if (currentBehavior == Army.AIBehaviorFlags.Besieging)
				{
					return Army.ArmyLeaderThinkReason.FromDefendingToBesieging;
				}
				if (currentBehavior == Army.AIBehaviorFlags.Patrolling)
				{
					return Army.ArmyLeaderThinkReason.FromDefendingToPatrolling;
				}
				break;
			case Army.AIBehaviorFlags.Patrolling:
				if (currentBehavior == Army.AIBehaviorFlags.Defending)
				{
					return Army.ArmyLeaderThinkReason.FromPatrollingToDefending;
				}
				if (currentBehavior == Army.AIBehaviorFlags.Patrolling)
				{
					return Army.ArmyLeaderThinkReason.ChangingTarget;
				}
				break;
			}
			return Army.ArmyLeaderThinkReason.Unknown;
		}

		public TextObject GetNotificationText()
		{
			if (this.LeaderParty != MobileParty.MainParty)
			{
				TextObject textObject = GameTexts.FindText("str_army_gather", null);
				StringHelpers.SetCharacterProperties("ARMY_LEADER", this.LeaderParty.LeaderHero.CharacterObject, textObject, false);
				textObject.SetTextVariable("SETTLEMENT_NAME", this.AiBehaviorObject.Name);
				return textObject;
			}
			return null;
		}

		public TextObject GetBehaviorText(bool setWithLink = false)
		{
			if (this.LeaderParty == MobileParty.MainParty)
			{
				Army.MainPartyCurrentAction mainPartyCurrentAction = this.GetMainPartyCurrentAction();
				TextObject textObject;
				if (!setWithLink)
				{
					Settlement encounterSettlement = PlayerEncounter.EncounterSettlement;
					textObject = ((encounterSettlement != null) ? encounterSettlement.Name : null);
				}
				else
				{
					Settlement encounterSettlement2 = PlayerEncounter.EncounterSettlement;
					textObject = ((encounterSettlement2 != null) ? encounterSettlement2.EncyclopediaLinkWithName : null);
				}
				TextObject textObject2 = textObject;
				TextObject textObject3;
				switch (mainPartyCurrentAction)
				{
				case Army.MainPartyCurrentAction.Idle:
					return new TextObject("{=sBahcJcl}Idle.", null);
				case Army.MainPartyCurrentAction.GatherAroundHero:
					textObject3 = GameTexts.FindText("str_army_gathering_around_hero", null);
					textObject3.SetTextVariable("PARTY_NAME", MobileParty.MainParty.Name);
					break;
				case Army.MainPartyCurrentAction.GatherAroundSettlement:
					textObject3 = GameTexts.FindText("str_army_gathering", null);
					break;
				case Army.MainPartyCurrentAction.GoToSettlement:
					textObject3 = ((Settlement.CurrentSettlement == null) ? GameTexts.FindText("str_army_going_to_settlement", null) : GameTexts.FindText("str_army_waiting_in_settlement", null));
					break;
				case Army.MainPartyCurrentAction.RaidSettlement:
				{
					textObject3 = GameTexts.FindText("str_army_raiding_settlement", null);
					Settlement encounterSettlement3 = PlayerEncounter.EncounterSettlement;
					float? num = ((encounterSettlement3 != null) ? new float?(encounterSettlement3.SettlementHitPoints) : null);
					TextObject textObject4 = textObject3;
					string text = "RAIDING_PROCESS";
					float num2 = 100f;
					float? num3 = (float)1 - num;
					textObject4.SetTextVariable(text, (int)(num2 * num3).Value);
					break;
				}
				case Army.MainPartyCurrentAction.BesiegeSettlement:
					textObject3 = GameTexts.FindText("str_army_besieging_settlement", null);
					break;
				case Army.MainPartyCurrentAction.PatrolAroundSettlement:
				{
					Settlement settlement = null;
					float num4 = Campaign.MapDiagonalSquared;
					foreach (Settlement settlement2 in Settlement.All)
					{
						if (!settlement2.IsHideout)
						{
							float distance = Campaign.Current.Models.MapDistanceModel.GetDistance(MobileParty.MainParty, settlement2);
							if (distance < num4)
							{
								num4 = distance;
								settlement = settlement2;
							}
						}
					}
					textObject2 = (setWithLink ? settlement.EncyclopediaLinkWithName : settlement.Name);
					textObject3 = GameTexts.FindText("str_army_patrolling_travelling", null);
					break;
				}
				case Army.MainPartyCurrentAction.DefendingSettlement:
					textObject3 = GameTexts.FindText("str_army_defending", null);
					textObject3.SetTextVariable("SETTLEMENT_NAME", textObject2);
					break;
				default:
					return new TextObject("{=av14a64q}Thinking", null);
				}
				textObject3.SetTextVariable("SETTLEMENT_NAME", textObject2);
				return textObject3;
			}
			switch (this.AIBehavior)
			{
			case Army.AIBehaviorFlags.PreGathering:
			case Army.AIBehaviorFlags.Gathering:
			case Army.AIBehaviorFlags.WaitingForArmyMembers:
			{
				TextObject textObject3;
				if (this.LeaderParty != MobileParty.MainParty)
				{
					textObject3 = GameTexts.FindText("str_army_gathering", null);
					textObject3.SetTextVariable("SETTLEMENT_NAME", setWithLink ? ((Settlement)this.AiBehaviorObject).EncyclopediaLinkWithName : this.AiBehaviorObject.Name);
				}
				else
				{
					textObject3 = GameTexts.FindText("str_army_gathering_around_hero", null);
					textObject3.SetTextVariable("PARTY_NAME", MobileParty.MainParty.Name);
				}
				return textObject3;
			}
			case Army.AIBehaviorFlags.TravellingToAssignment:
			{
				TextObject textObject3;
				if (this.LeaderParty.MapEvent != null && this.LeaderParty.MapEvent.MapEventSettlement != null && this.AiBehaviorObject != null && this.LeaderParty.MapEvent.MapEventSettlement == this.AiBehaviorObject)
				{
					switch (this.ArmyType)
					{
					case Army.ArmyTypes.Besieger:
						textObject3 = GameTexts.FindText("str_army_besieging_settlement", null);
						break;
					case Army.ArmyTypes.Raider:
					{
						Settlement settlement3 = (Settlement)this.AiBehaviorObject;
						textObject3 = GameTexts.FindText("str_army_raiding_settlement", null);
						textObject3.SetTextVariable("RAIDING_PROCESS", (int)(100f * (1f - settlement3.SettlementHitPoints)));
						break;
					}
					case Army.ArmyTypes.Defender:
						textObject3 = GameTexts.FindText("str_army_defending_travelling", null);
						break;
					case Army.ArmyTypes.Patrolling:
						textObject3 = GameTexts.FindText("str_army_patrolling_travelling", null);
						break;
					default:
						return new TextObject("{=av14a64q}Thinking", null);
					}
					textObject3.SetTextVariable("SETTLEMENT_NAME", setWithLink ? ((Settlement)this.AiBehaviorObject).EncyclopediaLinkWithName : this.AiBehaviorObject.Name);
					return textObject3;
				}
				switch (this.ArmyType)
				{
				case Army.ArmyTypes.Besieger:
					textObject3 = GameTexts.FindText("str_army_besieging_travelling", null);
					break;
				case Army.ArmyTypes.Raider:
					textObject3 = GameTexts.FindText("str_army_raiding_travelling", null);
					break;
				case Army.ArmyTypes.Defender:
					textObject3 = GameTexts.FindText("str_army_defending_travelling", null);
					break;
				case Army.ArmyTypes.Patrolling:
					textObject3 = GameTexts.FindText("str_army_patrolling_travelling", null);
					break;
				default:
					return new TextObject("{=av14a64q}Thinking", null);
				}
				textObject3.SetTextVariable("SETTLEMENT_NAME", setWithLink ? ((Settlement)this.AiBehaviorObject).EncyclopediaLinkWithName : this.AiBehaviorObject.Name);
				return textObject3;
			}
			case Army.AIBehaviorFlags.Besieging:
			{
				float num2;
				TextObject textObject3 = ((!Campaign.Current.Models.MapDistanceModel.GetDistance(this.AiBehaviorObject, MobileParty.MainParty, 15f, out num2)) ? GameTexts.FindText("str_army_besieging_travelling", null) : GameTexts.FindText("str_army_besieging", null));
				Settlement settlement4 = (Settlement)this.AiBehaviorObject;
				if (settlement4.IsVillage)
				{
					textObject3 = GameTexts.FindText("str_army_patrolling_travelling", null);
				}
				textObject3.SetTextVariable("SETTLEMENT_NAME", setWithLink ? settlement4.EncyclopediaLinkWithName : this.AiBehaviorObject.Name);
				return textObject3;
			}
			case Army.AIBehaviorFlags.Raiding:
			{
				float num2;
				TextObject textObject3 = ((!Campaign.Current.Models.MapDistanceModel.GetDistance(this.AiBehaviorObject, MobileParty.MainParty, 15f, out num2)) ? GameTexts.FindText("str_army_raiding_travelling", null) : GameTexts.FindText("str_army_raiding", null));
				textObject3.SetTextVariable("SETTLEMENT_NAME", setWithLink ? ((Settlement)this.AiBehaviorObject).EncyclopediaLinkWithName : this.AiBehaviorObject.Name);
				return textObject3;
			}
			case Army.AIBehaviorFlags.Defending:
			{
				float num2;
				TextObject textObject3 = ((!Campaign.Current.Models.MapDistanceModel.GetDistance(this.AiBehaviorObject, MobileParty.MainParty, 15f, out num2)) ? GameTexts.FindText("str_army_defending_travelling", null) : GameTexts.FindText("str_army_defending", null));
				textObject3.SetTextVariable("SETTLEMENT_NAME", setWithLink ? ((Settlement)this.AiBehaviorObject).EncyclopediaLinkWithName : this.AiBehaviorObject.Name);
				return textObject3;
			}
			case Army.AIBehaviorFlags.Patrolling:
			{
				TextObject textObject3 = GameTexts.FindText("str_army_patrolling_travelling", null);
				textObject3.SetTextVariable("SETTLEMENT_NAME", setWithLink ? ((Settlement)this.AiBehaviorObject).EncyclopediaLinkWithName : this.AiBehaviorObject.Name);
				return textObject3;
			}
			case Army.AIBehaviorFlags.GoToSettlement:
			{
				TextObject textObject3 = ((this.LeaderParty.CurrentSettlement == null) ? GameTexts.FindText("str_army_going_to_settlement", null) : GameTexts.FindText("str_army_waiting_in_settlement", null));
				textObject3.SetTextVariable("SETTLEMENT_NAME", (setWithLink && this.AiBehaviorObject is Settlement) ? ((Settlement)this.AiBehaviorObject).EncyclopediaLinkWithName : (this.AiBehaviorObject.Name ?? this.LeaderParty.Ai.AiBehaviorPartyBase.Name));
				return textObject3;
			}
			}
			return new TextObject("{=av14a64q}Thinking", null);
		}

		public void Gather(Settlement initialHostileSettlement)
		{
			this._armyGatheringTime = Campaign.CurrentTime;
			if (this.LeaderParty != MobileParty.MainParty)
			{
				Settlement settlement = this.FindBestInitialGatheringSettlement(initialHostileSettlement);
				this.AiBehaviorObject = settlement;
				Vec2 vec = (settlement.IsFortification ? settlement.GatePosition : settlement.Position2D);
				this.LeaderParty.SendPartyToReachablePointAroundPosition(vec, 6f, 3f);
				this.CallPartiesToArmy();
			}
			else
			{
				this.AiBehaviorObject = SettlementHelper.FindNearestSettlement((Settlement x) => x.IsFortification || x.IsVillage, null);
			}
			GatherArmyAction.Apply(this.LeaderParty, (Settlement)this.AiBehaviorObject);
		}

		private Settlement FindBestInitialGatheringSettlement(Settlement initialHostileTargetSettlement)
		{
			Settlement settlement = null;
			Hero leaderHero = this.LeaderParty.LeaderHero;
			float num = 0f;
			if (leaderHero != null && leaderHero.IsActive)
			{
				using (List<Settlement>.Enumerator enumerator = this.Kingdom.Settlements.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Settlement settlement2 = enumerator.Current;
						if (settlement2.IsVillage || settlement2.IsFortification)
						{
							float distance = Campaign.Current.Models.MapDistanceModel.GetDistance(initialHostileTargetSettlement, settlement2);
							if (distance > 40f)
							{
								float num2 = 0f;
								if (settlement == null)
								{
									num2 += 0.001f;
								}
								if (settlement2 != initialHostileTargetSettlement && settlement2.Party.MapEvent == null)
								{
									if (settlement2.MapFaction == this.Kingdom)
									{
										num2 += 10f;
									}
									else if (!FactionManager.IsAtWarAgainstFaction(settlement2.MapFaction, this.Kingdom))
									{
										num2 += 2f;
									}
									bool flag = false;
									foreach (Army army in this.Kingdom.Armies)
									{
										if (army != this && army.AiBehaviorObject == settlement2)
										{
											flag = true;
										}
									}
									if (!flag)
									{
										num2 += 10f;
									}
									float num3 = distance / (Campaign.MapDiagonal * 0.1f);
									float num4 = 20f * (1f - num3);
									float num5 = (settlement2.Position2D - this.LeaderParty.Position2D).Length / (Campaign.MapDiagonal * 0.1f);
									float num6 = 5f * (1f - num5);
									float num7 = num2 + num4 * 0.5f + num6 * 0.1f;
									if (num7 < 0f)
									{
										num7 = 0f;
									}
									if (num7 > num)
									{
										num = num7;
										settlement = settlement2;
									}
								}
							}
						}
					}
					goto IL_1F5;
				}
			}
			settlement = (Settlement)this.AiBehaviorObject;
			IL_1F5:
			if (settlement == null)
			{
				settlement = this.Kingdom.Settlements.FirstOrDefault<Settlement>() ?? this.LeaderParty.HomeSettlement;
			}
			return settlement;
		}

		private void CallPartiesToArmy()
		{
			foreach (MobileParty mobileParty in Campaign.Current.Models.ArmyManagementCalculationModel.GetMobilePartiesToCallToArmy(this.LeaderParty))
			{
				SetPartyAiAction.GetActionForEscortingParty(mobileParty, this.LeaderParty);
			}
		}

		public void ThinkAboutConcludingArmyGathering()
		{
			float currentTime = Campaign.CurrentTime;
			float num = 0f;
			float num2 = ((this.ArmyType == Army.ArmyTypes.Defender) ? 1f : 2f);
			float num3 = currentTime - this._armyGatheringTime;
			if (num3 > 24f)
			{
				num = 1f * ((num3 - 24f) / (num2 * 24f));
			}
			else if (num3 > (num2 + 1f) * 24f)
			{
				num = 1f;
			}
			if (MBRandom.RandomFloat < num)
			{
				this._waitTimeStart = Campaign.CurrentTime;
				this.AIBehavior = Army.AIBehaviorFlags.WaitingForArmyMembers;
				if (this.Parties.Count <= 1)
				{
					DisbandArmyAction.ApplyByNotEnoughParty(this);
				}
			}
		}

		public void ThinkAboutTravelingToAssignment()
		{
			bool flag = false;
			if (Campaign.CurrentTime - this._waitTimeStart < 72f)
			{
				if (this.LeaderParty.Position2D.DistanceSquared(this.AiBehaviorObject.Position2D) < 100f)
				{
					flag = this.TotalStrength / this.Parties.SumQ((MobileParty x) => x.Party.TotalStrength) > 0.7f;
				}
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				this.AIBehavior = Army.AIBehaviorFlags.TravellingToAssignment;
			}
		}

		private bool AnyoneBesiegingTarget()
		{
			Settlement settlement = (Settlement)this.AiBehaviorObject;
			return this.ArmyType == Army.ArmyTypes.Besieger && settlement.IsUnderSiege && !settlement.SiegeEvent.BesiegerCamp.IsBesiegerSideParty(this.LeaderParty);
		}

		private void IsAtSiegeLocation()
		{
			if (this.LeaderParty.Position2D.DistanceSquared(this.AiBehaviorObject.Position2D) < 100f && this.AIBehavior != Army.AIBehaviorFlags.Besieging)
			{
				if (this.LeaderParty.Army.Parties.ContainsQ(MobileParty.MainParty))
				{
					Debug.Print(string.Concat(new object[]
					{
						this.LeaderParty.LeaderHero.StringId,
						": ",
						this.LeaderParty.LeaderHero.Name,
						" is besieging ",
						this.AiBehaviorObject.Name,
						" of ",
						this.AiBehaviorObject.MapFaction.StringId,
						": ",
						this.AiBehaviorObject.MapFaction.Name,
						"\n"
					}), 0, Debug.DebugColor.Cyan, 17592186044416UL);
				}
				this.AIBehavior = Army.AIBehaviorFlags.Besieging;
			}
		}

		public void FinishArmyObjective()
		{
			this.AIBehavior = Army.AIBehaviorFlags.Unassigned;
			this.AiBehaviorObject = null;
		}

		internal void DisperseInternal(Army.ArmyDispersionReason reason = Army.ArmyDispersionReason.Unknown)
		{
			if (this._armyIsDispersing)
			{
				return;
			}
			CampaignEventDispatcher.Instance.OnArmyDispersed(this, reason, this.Parties.Contains(MobileParty.MainParty));
			this._armyIsDispersing = true;
			int num = 0;
			for (int i = this.Parties.Count - 1; i >= num; i--)
			{
				MobileParty mobileParty = this.Parties[i];
				bool flag = mobileParty.AttachedTo == this.LeaderParty;
				mobileParty.Army = null;
				if (flag && mobileParty.CurrentSettlement == null && mobileParty.IsActive)
				{
					mobileParty.Position2D = MobilePartyHelper.FindReachablePointAroundPosition(this.LeaderParty.Position2D, 1f, 0f);
				}
			}
			this._parties.Clear();
			this.Kingdom = null;
			if (this.LeaderParty == MobileParty.MainParty)
			{
				MapState mapState = Game.Current.GameStateManager.ActiveState as MapState;
				if (mapState != null)
				{
					mapState.OnDispersePlayerLeadedArmy();
				}
			}
			this._hourlyTickEvent.DeletePeriodicEvent();
			this._tickEvent.DeletePeriodicEvent();
			this._armyIsDispersing = false;
		}

		public Vec2 GetRelativePositionForParty(MobileParty mobileParty, Vec2 armyFacing)
		{
			float num = 0.5f;
			float num2 = (float)MathF.Ceiling(-1f + MathF.Sqrt(1f + 8f * (float)(this.LeaderParty.AttachedParties.Count - 1))) / 4f * num * 0.5f + num;
			int num3 = -1;
			for (int i = 0; i < this.LeaderParty.AttachedParties.Count; i++)
			{
				if (this.LeaderParty.AttachedParties[i] == mobileParty)
				{
					num3 = i;
					break;
				}
			}
			int num4 = MathF.Ceiling((-1f + MathF.Sqrt(1f + 8f * (float)(num3 + 2))) / 2f) - 1;
			int num5 = num3 + 1 - num4 * (num4 + 1) / 2;
			bool flag = (num4 & 1) != 0;
			num5 = ((((num5 & 1) != 0) ? (-1 - num5) : num5) >> 1) * (flag ? (-1) : 1);
			float num6 = 1.25f;
			Vec2 vec = this.LeaderParty.VisualPosition2DWithoutError + -armyFacing * 0.1f * (float)this.LeaderParty.AttachedParties.Count;
			Vec2 vec2 = vec - (float)MathF.Sign((float)num5 - (((num4 & 1) != 0) ? 0.5f : 0f)) * armyFacing.LeftVec() * num2;
			PathFaceRecord faceIndex = Campaign.Current.MapSceneWrapper.GetFaceIndex(vec);
			if (vec != vec2)
			{
				Vec2 lastPointOnNavigationMeshFromPositionToDestination = Campaign.Current.MapSceneWrapper.GetLastPointOnNavigationMeshFromPositionToDestination(faceIndex, vec, vec2);
				if ((vec2 - lastPointOnNavigationMeshFromPositionToDestination).LengthSquared > 2.25E-06f)
				{
					num = num * (vec - lastPointOnNavigationMeshFromPositionToDestination).Length / num2;
					num6 = num6 * (vec - lastPointOnNavigationMeshFromPositionToDestination).Length / (num2 / 1.5f);
				}
			}
			return new Vec2((flag ? (-num * 0.5f) : 0f) + (float)num5 * num + mobileParty.Party.RandomFloat(-0.25f, 0.25f) * 0.6f * num, ((float)(-(float)num4) + mobileParty.Party.RandomFloatWithSeed(1U, -0.25f, 0.25f)) * num6 * 0.3f);
		}

		public void AddPartyToMergedParties(MobileParty mobileParty)
		{
			mobileParty.AttachedTo = this.LeaderParty;
			if (mobileParty.IsMainParty)
			{
				MapState mapState = GameStateManager.Current.ActiveState as MapState;
				if (mapState != null)
				{
					mapState.OnJoinArmy();
				}
				Hero leaderHero = this.LeaderParty.LeaderHero;
				if (leaderHero != null && leaderHero != Hero.MainHero && !leaderHero.HasMet)
				{
					leaderHero.SetHasMet();
				}
			}
		}

		internal void OnRemovePartyInternal(MobileParty mobileParty)
		{
			mobileParty.Ai.SetInitiative(1f, 1f, 24f);
			this._parties.Remove(mobileParty);
			CampaignEventDispatcher.Instance.OnPartyRemovedFromArmy(mobileParty);
			if (this == MobileParty.MainParty.Army)
			{
				CampaignEventDispatcher.Instance.OnArmyOverlaySetDirty();
			}
			mobileParty.AttachedTo = null;
			if (this.LeaderParty == mobileParty && !this._armyIsDispersing)
			{
				DisbandArmyAction.ApplyByLeaderPartyRemoved(this);
			}
			mobileParty.OnRemovedFromArmyInternal();
			if (mobileParty == MobileParty.MainParty)
			{
				Campaign.Current.CameraFollowParty = MobileParty.MainParty.Party;
				this.StopTrackingTargetSettlement();
			}
			Army army = mobileParty.Army;
			if (((army != null) ? army.LeaderParty : null) == mobileParty)
			{
				this.FinishArmyObjective();
				if (!this._armyIsDispersing)
				{
					Army army2 = mobileParty.Army;
					if (((army2 != null) ? army2.LeaderParty.LeaderHero : null) == null)
					{
						DisbandArmyAction.ApplyByArmyLeaderIsDead(mobileParty.Army);
					}
					else
					{
						DisbandArmyAction.ApplyByObjectiveFinished(mobileParty.Army);
					}
				}
			}
			else if (this.Parties.Count == 0 && !this._armyIsDispersing)
			{
				if (mobileParty.Army != null && MobileParty.MainParty.Army != null && mobileParty.Army == MobileParty.MainParty.Army && Hero.MainHero.IsPrisoner)
				{
					DisbandArmyAction.ApplyByPlayerTakenPrisoner(this);
				}
				else
				{
					DisbandArmyAction.ApplyByNotEnoughParty(this);
				}
			}
			mobileParty.Party.SetVisualAsDirty();
			mobileParty.Party.UpdateVisibilityAndInspected(0f);
		}

		internal void OnAddPartyInternal(MobileParty mobileParty)
		{
			this._parties.Add(mobileParty);
			CampaignEventDispatcher.Instance.OnPartyJoinedArmy(mobileParty);
			if (this == MobileParty.MainParty.Army && this.LeaderParty != MobileParty.MainParty)
			{
				this.StartTrackingTargetSettlement(this.AiBehaviorObject);
				CampaignEventDispatcher.Instance.OnArmyOverlaySetDirty();
			}
			if (mobileParty != MobileParty.MainParty && this.LeaderParty != MobileParty.MainParty && this.LeaderParty.LeaderHero != null)
			{
				int num = -Campaign.Current.Models.ArmyManagementCalculationModel.CalculatePartyInfluenceCost(this.LeaderParty, mobileParty);
				ChangeClanInfluenceAction.Apply(this.LeaderParty.LeaderHero.Clan, (float)num);
			}
		}

		private void StartTrackingTargetSettlement(IMapPoint targetObject)
		{
			Settlement settlement = targetObject as Settlement;
			if (settlement != null)
			{
				Campaign.Current.VisualTrackerManager.RegisterObject(settlement);
			}
		}

		private void StopTrackingTargetSettlement()
		{
			Settlement settlement = this.AiBehaviorObject as Settlement;
			if (settlement != null)
			{
				Campaign.Current.VisualTrackerManager.RemoveTrackedObject(settlement, false);
			}
		}

		internal static void AutoGeneratedStaticCollectObjectsArmy(object o, List<object> collectedObjects)
		{
			((Army)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			collectedObjects.Add(this._parties);
			CampaignTime.AutoGeneratedStaticCollectObjectsCampaignTime(this._creationTime, collectedObjects);
			collectedObjects.Add(this._kingdom);
			collectedObjects.Add(this._aiBehaviorObject);
			collectedObjects.Add(this.ArmyOwner);
			collectedObjects.Add(this.LeaderParty);
			collectedObjects.Add(this.Name);
		}

		internal static object AutoGeneratedGetMemberValueAIBehavior(object o)
		{
			return ((Army)o).AIBehavior;
		}

		internal static object AutoGeneratedGetMemberValueArmyType(object o)
		{
			return ((Army)o).ArmyType;
		}

		internal static object AutoGeneratedGetMemberValueArmyOwner(object o)
		{
			return ((Army)o).ArmyOwner;
		}

		internal static object AutoGeneratedGetMemberValueCohesion(object o)
		{
			return ((Army)o).Cohesion;
		}

		internal static object AutoGeneratedGetMemberValueMorale(object o)
		{
			return ((Army)o).Morale;
		}

		internal static object AutoGeneratedGetMemberValueLeaderParty(object o)
		{
			return ((Army)o).LeaderParty;
		}

		internal static object AutoGeneratedGetMemberValueName(object o)
		{
			return ((Army)o).Name;
		}

		internal static object AutoGeneratedGetMemberValue_parties(object o)
		{
			return ((Army)o)._parties;
		}

		internal static object AutoGeneratedGetMemberValue_creationTime(object o)
		{
			return ((Army)o)._creationTime;
		}

		internal static object AutoGeneratedGetMemberValue_armyGatheringTime(object o)
		{
			return ((Army)o)._armyGatheringTime;
		}

		internal static object AutoGeneratedGetMemberValue_waitTimeStart(object o)
		{
			return ((Army)o)._waitTimeStart;
		}

		internal static object AutoGeneratedGetMemberValue_armyIsDispersing(object o)
		{
			return ((Army)o)._armyIsDispersing;
		}

		internal static object AutoGeneratedGetMemberValue_numberOfBoosts(object o)
		{
			return ((Army)o)._numberOfBoosts;
		}

		internal static object AutoGeneratedGetMemberValue_kingdom(object o)
		{
			return ((Army)o)._kingdom;
		}

		internal static object AutoGeneratedGetMemberValue_aiBehaviorObject(object o)
		{
			return ((Army)o)._aiBehaviorObject;
		}

		private const float MaximumWaitTime = 72f;

		private const float ArmyGatheringConcludingTickFrequency = 1f;

		private const float GatheringDistance = 3.5f;

		private const float DefaultGatheringWaitTime = 24f;

		private const float MinimumDistanceWhileGatheringAsAttackerArmy = 40f;

		private const float CheckingForBoostingCohesionThreshold = 50f;

		private const float DisbandCohesionThreshold = 30f;

		private const float StrengthThresholdRatioForGathering = 0.7f;

		[SaveableField(1)]
		private readonly MBList<MobileParty> _parties;

		[SaveableField(19)]
		private CampaignTime _creationTime;

		[SaveableField(7)]
		private float _armyGatheringTime;

		[SaveableField(9)]
		private float _waitTimeStart;

		[SaveableField(10)]
		private bool _armyIsDispersing;

		[SaveableField(11)]
		private int _numberOfBoosts;

		[SaveableField(15)]
		private Kingdom _kingdom;

		[SaveableField(16)]
		private IMapPoint _aiBehaviorObject;

		[CachedData]
		private MBCampaignEvent _hourlyTickEvent;

		[CachedData]
		private MBCampaignEvent _tickEvent;

		public enum AIBehaviorFlags
		{
			Unassigned,
			PreGathering,
			Gathering,
			WaitingForArmyMembers,
			TravellingToAssignment,
			Besieging,
			AssaultingTown,
			Raiding,
			Defending,
			Patrolling,
			GoToSettlement,
			NumberOfAIBehaviorFlags
		}

		public enum ArmyTypes
		{
			Besieger,
			Raider,
			Defender,
			Patrolling,
			NumberOfArmyTypes
		}

		private enum MainPartyCurrentAction
		{
			Idle,
			GatherAroundHero,
			GatherAroundSettlement,
			GoToSettlement,
			RaidSettlement,
			BesiegeSettlement,
			PatrolAroundSettlement,
			DefendingSettlement
		}

		public enum ArmyDispersionReason
		{
			Unknown,
			DismissalRequestedWithInfluence,
			NotEnoughParty,
			KingdomChanged,
			CohesionDepleted,
			ObjectiveFinished,
			LeaderPartyRemoved,
			PlayerTakenPrisoner,
			CannotElectNewLeader,
			LeaderCannotArrivePointOnTime,
			ArmyLeaderIsDead,
			FoodProblem,
			NotEnoughTroop,
			NoActiveWar
		}

		public enum ArmyLeaderThinkReason
		{
			Unknown,
			FromGatheringToWaiting,
			FromTravellingToBesieging,
			FromWaitingToTravelling,
			ChangingTarget,
			FromTravellingToRaiding,
			FromTravellingToDefending,
			FromRaidingToTravelling,
			FromBesiegingToTravelling,
			FromDefendingToTravelling,
			FromPatrollingToDefending,
			FromBesiegingToDefending,
			FromDefendingToBesieging,
			FromDefendingToPatrolling,
			FromUnassignedToPatrolling,
			FromUnassignedToTravelling
		}
	}
}
