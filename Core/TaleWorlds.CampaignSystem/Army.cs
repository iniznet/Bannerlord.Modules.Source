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
	// Token: 0x0200001F RID: 31
	public class Army
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x060000F6 RID: 246 RVA: 0x0000C443 File Offset: 0x0000A643
		public MBReadOnlyList<MobileParty> Parties
		{
			get
			{
				return this._parties;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x060000F7 RID: 247 RVA: 0x0000C44B File Offset: 0x0000A64B
		public TextObject EncyclopediaLinkWithName
		{
			get
			{
				return this.ArmyOwner.EncyclopediaLinkWithName;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x060000F8 RID: 248 RVA: 0x0000C458 File Offset: 0x0000A658
		// (set) Token: 0x060000F9 RID: 249 RVA: 0x0000C460 File Offset: 0x0000A660
		[SaveableProperty(2)]
		public Army.AIBehaviorFlags AIBehavior { get; set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x060000FA RID: 250 RVA: 0x0000C469 File Offset: 0x0000A669
		// (set) Token: 0x060000FB RID: 251 RVA: 0x0000C471 File Offset: 0x0000A671
		[SaveableProperty(3)]
		public Army.ArmyTypes ArmyType { get; set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x060000FC RID: 252 RVA: 0x0000C47A File Offset: 0x0000A67A
		// (set) Token: 0x060000FD RID: 253 RVA: 0x0000C482 File Offset: 0x0000A682
		[SaveableProperty(4)]
		public Hero ArmyOwner { get; set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x060000FE RID: 254 RVA: 0x0000C48B File Offset: 0x0000A68B
		// (set) Token: 0x060000FF RID: 255 RVA: 0x0000C493 File Offset: 0x0000A693
		[SaveableProperty(5)]
		public float Cohesion { get; set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000100 RID: 256 RVA: 0x0000C49C File Offset: 0x0000A69C
		public float DailyCohesionChange
		{
			get
			{
				return Campaign.Current.Models.ArmyManagementCalculationModel.CalculateDailyCohesionChange(this, false).ResultNumber;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000101 RID: 257 RVA: 0x0000C4C7 File Offset: 0x0000A6C7
		public ExplainedNumber DailyCohesionChangeExplanation
		{
			get
			{
				return Campaign.Current.Models.ArmyManagementCalculationModel.CalculateDailyCohesionChange(this, true);
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000102 RID: 258 RVA: 0x0000C4DF File Offset: 0x0000A6DF
		public int CohesionThresholdForDispersion
		{
			get
			{
				return Campaign.Current.Models.ArmyManagementCalculationModel.CohesionThresholdForDispersion;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000103 RID: 259 RVA: 0x0000C4F5 File Offset: 0x0000A6F5
		// (set) Token: 0x06000104 RID: 260 RVA: 0x0000C4FD File Offset: 0x0000A6FD
		[SaveableProperty(13)]
		public float Morale { get; private set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000105 RID: 261 RVA: 0x0000C506 File Offset: 0x0000A706
		// (set) Token: 0x06000106 RID: 262 RVA: 0x0000C50E File Offset: 0x0000A70E
		[SaveableProperty(14)]
		public MobileParty LeaderParty { get; private set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000107 RID: 263 RVA: 0x0000C517 File Offset: 0x0000A717
		public int LeaderPartyAndAttachedPartiesCount
		{
			get
			{
				return this.LeaderParty.AttachedParties.Count + 1;
			}
		}

		// Token: 0x06000108 RID: 264 RVA: 0x0000C52B File Offset: 0x0000A72B
		public override string ToString()
		{
			return this.Name.ToString();
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000109 RID: 265 RVA: 0x0000C538 File Offset: 0x0000A738
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

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600010A RID: 266 RVA: 0x0000C5AC File Offset: 0x0000A7AC
		// (set) Token: 0x0600010B RID: 267 RVA: 0x0000C5B4 File Offset: 0x0000A7B4
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

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600010C RID: 268 RVA: 0x0000C5E9 File Offset: 0x0000A7E9
		public Kingdom MapFaction
		{
			get
			{
				return this.Kingdom;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600010D RID: 269 RVA: 0x0000C5F1 File Offset: 0x0000A7F1
		// (set) Token: 0x0600010E RID: 270 RVA: 0x0000C5FC File Offset: 0x0000A7FC
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

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600010F RID: 271 RVA: 0x0000C64F File Offset: 0x0000A84F
		// (set) Token: 0x06000110 RID: 272 RVA: 0x0000C657 File Offset: 0x0000A857
		[SaveableProperty(17)]
		public TextObject Name { get; private set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000111 RID: 273 RVA: 0x0000C660 File Offset: 0x0000A860
		public int TotalHealthyMembers
		{
			get
			{
				return this.LeaderParty.Party.NumberOfHealthyMembers + this.LeaderParty.AttachedParties.Sum((MobileParty mobileParty) => mobileParty.Party.NumberOfHealthyMembers);
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000112 RID: 274 RVA: 0x0000C6B0 File Offset: 0x0000A8B0
		public int TotalManCount
		{
			get
			{
				return this.LeaderParty.Party.MemberRoster.TotalManCount + this.LeaderParty.AttachedParties.Sum((MobileParty mobileParty) => mobileParty.Party.MemberRoster.TotalManCount);
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000113 RID: 275 RVA: 0x0000C704 File Offset: 0x0000A904
		public int TotalRegularCount
		{
			get
			{
				return this.LeaderParty.Party.MemberRoster.TotalRegulars + this.LeaderParty.AttachedParties.Sum((MobileParty mobileParty) => mobileParty.Party.MemberRoster.TotalRegulars);
			}
		}

		// Token: 0x06000114 RID: 276 RVA: 0x0000C758 File Offset: 0x0000A958
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

		// Token: 0x06000115 RID: 277 RVA: 0x0000C7D8 File Offset: 0x0000A9D8
		public void UpdateName()
		{
			this.Name = new TextObject("{=nbmctMLk}{LEADER_NAME}{.o} Army", null);
			this.Name.SetTextVariable("LEADER_NAME", (this.ArmyOwner != null) ? this.ArmyOwner.Name : ((this.LeaderParty.PartyComponent.PartyOwner != null) ? this.LeaderParty.PartyComponent.PartyOwner.Name : TextObject.Empty));
		}

		// Token: 0x06000116 RID: 278 RVA: 0x0000C84C File Offset: 0x0000AA4C
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

		// Token: 0x06000117 RID: 279 RVA: 0x0000C914 File Offset: 0x0000AB14
		internal void OnAfterLoad()
		{
			this.AddEventHandlers();
		}

		// Token: 0x06000118 RID: 280 RVA: 0x0000C91C File Offset: 0x0000AB1C
		[LoadInitializationCallback]
		private void OnLoad(MetaData metaData)
		{
			if (this.AiBehaviorObject == null)
			{
				this.AIBehavior = Army.AIBehaviorFlags.Unassigned;
			}
		}

		// Token: 0x06000119 RID: 281 RVA: 0x0000C92D File Offset: 0x0000AB2D
		public bool DoesLeaderPartyAndAttachedPartiesContain(MobileParty party)
		{
			return this.LeaderParty == party || this.LeaderParty.AttachedParties.IndexOf(party) >= 0;
		}

		// Token: 0x0600011A RID: 282 RVA: 0x0000C954 File Offset: 0x0000AB54
		public void BoostCohesionWithInfluence(float cohesionToGain, int cost)
		{
			if (this.LeaderParty.LeaderHero.Clan.Influence >= (float)cost)
			{
				ChangeClanInfluenceAction.Apply(this.LeaderParty.LeaderHero.Clan, (float)(-(float)cost));
				this.Cohesion += cohesionToGain;
				this._numberOfBoosts++;
			}
		}

		// Token: 0x0600011B RID: 283 RVA: 0x0000C9B0 File Offset: 0x0000ABB0
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

		// Token: 0x0600011C RID: 284 RVA: 0x0000CBB4 File Offset: 0x0000ADB4
		public void RecalculateArmyMorale()
		{
			float num = 0f;
			foreach (MobileParty mobileParty in this.Parties)
			{
				num += mobileParty.Morale;
			}
			this.Morale = num / (float)this.Parties.Count;
		}

		// Token: 0x0600011D RID: 285 RVA: 0x0000CC24 File Offset: 0x0000AE24
		private void HourlyTick(MBCampaignEvent campaignEvent, object[] delegateParams)
		{
			if (this.LeaderParty.MapEvent != null)
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

		// Token: 0x0600011E RID: 286 RVA: 0x0000CD64 File Offset: 0x0000AF64
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

		// Token: 0x0600011F RID: 287 RVA: 0x0000CE44 File Offset: 0x0000B044
		private void CheckArmyDispersion()
		{
			if (this.LeaderParty == MobileParty.MainParty)
			{
				if (this.Cohesion <= 0.1f)
				{
					DisbandArmyAction.ApplyByCohesionDepleted(this);
					GameMenu.SwitchToMenu("army_dispersed");
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

		// Token: 0x06000120 RID: 288 RVA: 0x0000CF90 File Offset: 0x0000B190
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

		// Token: 0x06000121 RID: 289 RVA: 0x0000D01C File Offset: 0x0000B21C
		private void CheckMainPartyTravelingToAssignment()
		{
			float num;
			if (this.AIBehavior == Army.AIBehaviorFlags.Gathering && this.AiBehaviorObject != null && !Campaign.Current.Models.MapDistanceModel.GetDistance(this.AiBehaviorObject, MobileParty.MainParty, 3.5f, out num))
			{
				this.AIBehavior = Army.AIBehaviorFlags.TravellingToAssignment;
			}
		}

		// Token: 0x06000122 RID: 290 RVA: 0x0000D06C File Offset: 0x0000B26C
		private void CallArmyMembersToArmyIfNeeded()
		{
			for (int i = this.Parties.Count - 1; i >= 0; i--)
			{
				MobileParty mobileParty = this.Parties[i];
				if (mobileParty != this.LeaderParty && mobileParty.MapEvent == null && mobileParty.TargetParty != this.LeaderParty && mobileParty != MobileParty.MainParty)
				{
					mobileParty.Ai.SetMoveEscortParty(this.LeaderParty);
				}
				if (mobileParty.Party.IsStarving)
				{
					mobileParty.Army = null;
				}
			}
		}

		// Token: 0x06000123 RID: 291 RVA: 0x0000D0EC File Offset: 0x0000B2EC
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

		// Token: 0x06000124 RID: 292 RVA: 0x0000D1C0 File Offset: 0x0000B3C0
		private void CheckMainPartyGathering()
		{
			float num;
			if (this.AIBehavior == Army.AIBehaviorFlags.PreGathering && this.AiBehaviorObject != null && Campaign.Current.Models.MapDistanceModel.GetDistance(this.AiBehaviorObject, MobileParty.MainParty, 3.5f, out num))
			{
				this.AIBehavior = Army.AIBehaviorFlags.Gathering;
			}
		}

		// Token: 0x06000125 RID: 293 RVA: 0x0000D210 File Offset: 0x0000B410
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

		// Token: 0x06000126 RID: 294 RVA: 0x0000D280 File Offset: 0x0000B480
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

		// Token: 0x06000127 RID: 295 RVA: 0x0000D33C File Offset: 0x0000B53C
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

		// Token: 0x06000128 RID: 296 RVA: 0x0000D39C File Offset: 0x0000B59C
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
				textObject3.SetTextVariable("SETTLEMENT_NAME", setWithLink ? ((Settlement)this.AiBehaviorObject).EncyclopediaLinkWithName : this.AiBehaviorObject.Name);
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

		// Token: 0x06000129 RID: 297 RVA: 0x0000DA58 File Offset: 0x0000BC58
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

		// Token: 0x0600012A RID: 298 RVA: 0x0000DB04 File Offset: 0x0000BD04
		private Settlement FindBestInitialGatheringSettlement(Settlement initialHostileTargetSettlement)
		{
			Settlement settlement = null;
			Hero leaderHero = this.LeaderParty.LeaderHero;
			float num = 0f;
			if (leaderHero != null && leaderHero.IsActive)
			{
				using (List<Settlement>.Enumerator enumerator = this.MapFaction.Settlements.GetEnumerator())
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
									if (settlement2.MapFaction == this.MapFaction)
									{
										num2 += 10f;
									}
									else if (!FactionManager.IsAtWarAgainstFaction(settlement2.MapFaction, this.MapFaction))
									{
										num2 += 2f;
									}
									bool flag = false;
									foreach (Army army in this.MapFaction.Armies)
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
				settlement = this.MapFaction.Settlements.FirstOrDefault<Settlement>() ?? this.LeaderParty.HomeSettlement;
			}
			return settlement;
		}

		// Token: 0x0600012B RID: 299 RVA: 0x0000DD60 File Offset: 0x0000BF60
		private void CallPartiesToArmy()
		{
			foreach (MobileParty mobileParty in Campaign.Current.Models.ArmyManagementCalculationModel.GetMobilePartiesToCallToArmy(this.LeaderParty))
			{
				if (this.LeaderParty.LeaderHero != null)
				{
					int num = Campaign.Current.Models.ArmyManagementCalculationModel.CalculatePartyInfluenceCost(this.LeaderParty, mobileParty);
					ChangeClanInfluenceAction.Apply(this.LeaderParty.LeaderHero.Clan, (float)(-(float)num));
				}
				SetPartyAiAction.GetActionForEscortingParty(mobileParty, this.LeaderParty);
			}
		}

		// Token: 0x0600012C RID: 300 RVA: 0x0000DE10 File Offset: 0x0000C010
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

		// Token: 0x0600012D RID: 301 RVA: 0x0000DEAC File Offset: 0x0000C0AC
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

		// Token: 0x0600012E RID: 302 RVA: 0x0000DF3C File Offset: 0x0000C13C
		private bool AnyoneBesiegingTarget()
		{
			Settlement settlement = (Settlement)this.AiBehaviorObject;
			return this.ArmyType == Army.ArmyTypes.Besieger && settlement.IsUnderSiege && !settlement.SiegeEvent.BesiegerCamp.IsBesiegerSideParty(this.LeaderParty);
		}

		// Token: 0x0600012F RID: 303 RVA: 0x0000DF80 File Offset: 0x0000C180
		private void IsAtSiegeLocation()
		{
			if (this.LeaderParty.Position2D.DistanceSquared(this.AiBehaviorObject.Position2D) < 100f && this.AIBehavior != Army.AIBehaviorFlags.Besieging)
			{
				Debug.Print(string.Concat(new object[]
				{
					"[DEBUG] ",
					this.LeaderParty.LeaderHero.Name,
					" is besieging ",
					this.AiBehaviorObject.Name,
					" of ",
					this.AiBehaviorObject.MapFaction.Name,
					"\n"
				}), 0, Debug.DebugColor.Cyan, 17592186044416UL);
				this.AIBehavior = Army.AIBehaviorFlags.Besieging;
			}
		}

		// Token: 0x06000130 RID: 304 RVA: 0x0000E031 File Offset: 0x0000C231
		public void FinishArmyObjective()
		{
			this.AIBehavior = Army.AIBehaviorFlags.Unassigned;
			this.AiBehaviorObject = null;
		}

		// Token: 0x06000131 RID: 305 RVA: 0x0000E044 File Offset: 0x0000C244
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

		// Token: 0x06000132 RID: 306 RVA: 0x0000E148 File Offset: 0x0000C348
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
			float num7 = (float)this.LeaderPartyAndAttachedPartiesCount * 0.1f;
			Vec2 vec = new Vec2(num7, num7);
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

		// Token: 0x06000133 RID: 307 RVA: 0x0000E36C File Offset: 0x0000C56C
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

		// Token: 0x06000134 RID: 308 RVA: 0x0000E3CC File Offset: 0x0000C5CC
		internal void OnRemovePartyInternal(MobileParty mobileParty)
		{
			mobileParty.Ai.SetInitiative(1f, 1f, 24f);
			this._parties.Remove(mobileParty);
			if (this.Parties.Count > 0)
			{
				CampaignEventDispatcher.Instance.OnPartyRemovedFromArmy(mobileParty);
				if (this == MobileParty.MainParty.Army)
				{
					CampaignEventDispatcher.Instance.OnArmyOverlaySetDirty();
				}
			}
			mobileParty.AttachedTo = null;
			if (this.LeaderParty == mobileParty)
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
			IPartyVisual visuals = mobileParty.Party.Visuals;
			if (visuals != null)
			{
				visuals.SetMapIconAsDirty();
			}
			mobileParty.Party.UpdateVisibilityAndInspected(0f, false);
		}

		// Token: 0x06000135 RID: 309 RVA: 0x0000E544 File Offset: 0x0000C744
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
				ChangeClanInfluenceAction.Apply(this.LeaderParty.LeaderHero.Clan, (float)(-(float)Campaign.Current.Models.ArmyManagementCalculationModel.CalculatePartyInfluenceCost(this.LeaderParty, mobileParty)));
			}
		}

		// Token: 0x06000136 RID: 310 RVA: 0x0000E5EC File Offset: 0x0000C7EC
		private void StartTrackingTargetSettlement(IMapPoint targetObject)
		{
			Settlement settlement = targetObject as Settlement;
			if (settlement != null)
			{
				Campaign.Current.VisualTrackerManager.RegisterObject(settlement);
			}
		}

		// Token: 0x06000137 RID: 311 RVA: 0x0000E614 File Offset: 0x0000C814
		private void StopTrackingTargetSettlement()
		{
			Settlement settlement = this.AiBehaviorObject as Settlement;
			if (settlement != null)
			{
				Campaign.Current.VisualTrackerManager.RemoveTrackedObject(settlement, false);
			}
		}

		// Token: 0x06000138 RID: 312 RVA: 0x0000E641 File Offset: 0x0000C841
		internal static void AutoGeneratedStaticCollectObjectsArmy(object o, List<object> collectedObjects)
		{
			((Army)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x06000139 RID: 313 RVA: 0x0000E650 File Offset: 0x0000C850
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

		// Token: 0x0600013A RID: 314 RVA: 0x0000E6B6 File Offset: 0x0000C8B6
		internal static object AutoGeneratedGetMemberValueAIBehavior(object o)
		{
			return ((Army)o).AIBehavior;
		}

		// Token: 0x0600013B RID: 315 RVA: 0x0000E6C8 File Offset: 0x0000C8C8
		internal static object AutoGeneratedGetMemberValueArmyType(object o)
		{
			return ((Army)o).ArmyType;
		}

		// Token: 0x0600013C RID: 316 RVA: 0x0000E6DA File Offset: 0x0000C8DA
		internal static object AutoGeneratedGetMemberValueArmyOwner(object o)
		{
			return ((Army)o).ArmyOwner;
		}

		// Token: 0x0600013D RID: 317 RVA: 0x0000E6E7 File Offset: 0x0000C8E7
		internal static object AutoGeneratedGetMemberValueCohesion(object o)
		{
			return ((Army)o).Cohesion;
		}

		// Token: 0x0600013E RID: 318 RVA: 0x0000E6F9 File Offset: 0x0000C8F9
		internal static object AutoGeneratedGetMemberValueMorale(object o)
		{
			return ((Army)o).Morale;
		}

		// Token: 0x0600013F RID: 319 RVA: 0x0000E70B File Offset: 0x0000C90B
		internal static object AutoGeneratedGetMemberValueLeaderParty(object o)
		{
			return ((Army)o).LeaderParty;
		}

		// Token: 0x06000140 RID: 320 RVA: 0x0000E718 File Offset: 0x0000C918
		internal static object AutoGeneratedGetMemberValueName(object o)
		{
			return ((Army)o).Name;
		}

		// Token: 0x06000141 RID: 321 RVA: 0x0000E725 File Offset: 0x0000C925
		internal static object AutoGeneratedGetMemberValue_parties(object o)
		{
			return ((Army)o)._parties;
		}

		// Token: 0x06000142 RID: 322 RVA: 0x0000E732 File Offset: 0x0000C932
		internal static object AutoGeneratedGetMemberValue_creationTime(object o)
		{
			return ((Army)o)._creationTime;
		}

		// Token: 0x06000143 RID: 323 RVA: 0x0000E744 File Offset: 0x0000C944
		internal static object AutoGeneratedGetMemberValue_armyGatheringTime(object o)
		{
			return ((Army)o)._armyGatheringTime;
		}

		// Token: 0x06000144 RID: 324 RVA: 0x0000E756 File Offset: 0x0000C956
		internal static object AutoGeneratedGetMemberValue_waitTimeStart(object o)
		{
			return ((Army)o)._waitTimeStart;
		}

		// Token: 0x06000145 RID: 325 RVA: 0x0000E768 File Offset: 0x0000C968
		internal static object AutoGeneratedGetMemberValue_armyIsDispersing(object o)
		{
			return ((Army)o)._armyIsDispersing;
		}

		// Token: 0x06000146 RID: 326 RVA: 0x0000E77A File Offset: 0x0000C97A
		internal static object AutoGeneratedGetMemberValue_numberOfBoosts(object o)
		{
			return ((Army)o)._numberOfBoosts;
		}

		// Token: 0x06000147 RID: 327 RVA: 0x0000E78C File Offset: 0x0000C98C
		internal static object AutoGeneratedGetMemberValue_kingdom(object o)
		{
			return ((Army)o)._kingdom;
		}

		// Token: 0x06000148 RID: 328 RVA: 0x0000E799 File Offset: 0x0000C999
		internal static object AutoGeneratedGetMemberValue_aiBehaviorObject(object o)
		{
			return ((Army)o)._aiBehaviorObject;
		}

		// Token: 0x04000019 RID: 25
		private const float MaximumWaitTime = 72f;

		// Token: 0x0400001A RID: 26
		private const float ArmyGatheringConcludingTickFrequency = 1f;

		// Token: 0x0400001B RID: 27
		private const float GatheringDistance = 3.5f;

		// Token: 0x0400001C RID: 28
		private const float DefaultGatheringWaitTime = 24f;

		// Token: 0x0400001D RID: 29
		private const float MinimumDistanceWhileGatheringAsAttackerArmy = 40f;

		// Token: 0x0400001E RID: 30
		private const float CheckingForBoostingCohesionThreshold = 50f;

		// Token: 0x0400001F RID: 31
		private const float DisbandCohesionThreshold = 30f;

		// Token: 0x04000020 RID: 32
		private const float StrengthThresholdRatioForGathering = 0.7f;

		// Token: 0x04000021 RID: 33
		[SaveableField(1)]
		private readonly MBList<MobileParty> _parties;

		// Token: 0x04000026 RID: 38
		[SaveableField(19)]
		private CampaignTime _creationTime;

		// Token: 0x04000027 RID: 39
		[SaveableField(7)]
		private float _armyGatheringTime;

		// Token: 0x04000028 RID: 40
		[SaveableField(9)]
		private float _waitTimeStart;

		// Token: 0x04000029 RID: 41
		[SaveableField(10)]
		private bool _armyIsDispersing;

		// Token: 0x0400002A RID: 42
		[SaveableField(11)]
		private int _numberOfBoosts;

		// Token: 0x0400002D RID: 45
		[SaveableField(15)]
		private Kingdom _kingdom;

		// Token: 0x0400002E RID: 46
		[SaveableField(16)]
		private IMapPoint _aiBehaviorObject;

		// Token: 0x04000030 RID: 48
		[CachedData]
		private MBCampaignEvent _hourlyTickEvent;

		// Token: 0x04000031 RID: 49
		[CachedData]
		private MBCampaignEvent _tickEvent;

		// Token: 0x02000472 RID: 1138
		public enum AIBehaviorFlags
		{
			// Token: 0x0400132F RID: 4911
			Unassigned,
			// Token: 0x04001330 RID: 4912
			PreGathering,
			// Token: 0x04001331 RID: 4913
			Gathering,
			// Token: 0x04001332 RID: 4914
			WaitingForArmyMembers,
			// Token: 0x04001333 RID: 4915
			TravellingToAssignment,
			// Token: 0x04001334 RID: 4916
			Besieging,
			// Token: 0x04001335 RID: 4917
			AssaultingTown,
			// Token: 0x04001336 RID: 4918
			Raiding,
			// Token: 0x04001337 RID: 4919
			Defending,
			// Token: 0x04001338 RID: 4920
			Patrolling,
			// Token: 0x04001339 RID: 4921
			GoToSettlement,
			// Token: 0x0400133A RID: 4922
			NumberOfAIBehaviorFlags
		}

		// Token: 0x02000473 RID: 1139
		public enum ArmyTypes
		{
			// Token: 0x0400133C RID: 4924
			Besieger,
			// Token: 0x0400133D RID: 4925
			Raider,
			// Token: 0x0400133E RID: 4926
			Defender,
			// Token: 0x0400133F RID: 4927
			Patrolling,
			// Token: 0x04001340 RID: 4928
			NumberOfArmyTypes
		}

		// Token: 0x02000474 RID: 1140
		private enum MainPartyCurrentAction
		{
			// Token: 0x04001342 RID: 4930
			Idle,
			// Token: 0x04001343 RID: 4931
			GatherAroundHero,
			// Token: 0x04001344 RID: 4932
			GatherAroundSettlement,
			// Token: 0x04001345 RID: 4933
			GoToSettlement,
			// Token: 0x04001346 RID: 4934
			RaidSettlement,
			// Token: 0x04001347 RID: 4935
			BesiegeSettlement,
			// Token: 0x04001348 RID: 4936
			PatrolAroundSettlement,
			// Token: 0x04001349 RID: 4937
			DefendingSettlement
		}

		// Token: 0x02000475 RID: 1141
		public enum ArmyDispersionReason
		{
			// Token: 0x0400134B RID: 4939
			Unknown,
			// Token: 0x0400134C RID: 4940
			DismissalRequestedWithInfluence,
			// Token: 0x0400134D RID: 4941
			NotEnoughParty,
			// Token: 0x0400134E RID: 4942
			KingdomChanged,
			// Token: 0x0400134F RID: 4943
			CohesionDepleted,
			// Token: 0x04001350 RID: 4944
			ObjectiveFinished,
			// Token: 0x04001351 RID: 4945
			LeaderPartyRemoved,
			// Token: 0x04001352 RID: 4946
			PlayerTakenPrisoner,
			// Token: 0x04001353 RID: 4947
			CannotElectNewLeader,
			// Token: 0x04001354 RID: 4948
			LeaderCannotArrivePointOnTime,
			// Token: 0x04001355 RID: 4949
			ArmyLeaderIsDead,
			// Token: 0x04001356 RID: 4950
			FoodProblem,
			// Token: 0x04001357 RID: 4951
			NotEnoughTroop,
			// Token: 0x04001358 RID: 4952
			NoActiveWar
		}

		// Token: 0x02000476 RID: 1142
		public enum ArmyLeaderThinkReason
		{
			// Token: 0x0400135A RID: 4954
			Unknown,
			// Token: 0x0400135B RID: 4955
			FromGatheringToWaiting,
			// Token: 0x0400135C RID: 4956
			FromTravellingToBesieging,
			// Token: 0x0400135D RID: 4957
			FromWaitingToTravelling,
			// Token: 0x0400135E RID: 4958
			ChangingTarget,
			// Token: 0x0400135F RID: 4959
			FromTravellingToRaiding,
			// Token: 0x04001360 RID: 4960
			FromTravellingToDefending,
			// Token: 0x04001361 RID: 4961
			FromRaidingToTravelling,
			// Token: 0x04001362 RID: 4962
			FromBesiegingToTravelling,
			// Token: 0x04001363 RID: 4963
			FromDefendingToTravelling,
			// Token: 0x04001364 RID: 4964
			FromPatrollingToDefending,
			// Token: 0x04001365 RID: 4965
			FromBesiegingToDefending,
			// Token: 0x04001366 RID: 4966
			FromDefendingToBesieging,
			// Token: 0x04001367 RID: 4967
			FromDefendingToPatrolling,
			// Token: 0x04001368 RID: 4968
			FromUnassignedToPatrolling,
			// Token: 0x04001369 RID: 4969
			FromUnassignedToTravelling
		}
	}
}
