﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Siege
{
	// Token: 0x02000288 RID: 648
	public class BesiegerCamp : ISiegeEventSide
	{
		// Token: 0x06002214 RID: 8724 RVA: 0x00090EDF File Offset: 0x0008F0DF
		internal static void AutoGeneratedStaticCollectObjectsBesiegerCamp(object o, List<object> collectedObjects)
		{
			((BesiegerCamp)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x06002215 RID: 8725 RVA: 0x00090EED File Offset: 0x0008F0ED
		protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			collectedObjects.Add(this._besiegerParties);
			collectedObjects.Add(this._siegeEngineMissiles);
			collectedObjects.Add(this.SiegeEvent);
			collectedObjects.Add(this.SiegeEngines);
			collectedObjects.Add(this.SiegeStrategy);
		}

		// Token: 0x06002216 RID: 8726 RVA: 0x00090F2B File Offset: 0x0008F12B
		internal static object AutoGeneratedGetMemberValueSiegeEvent(object o)
		{
			return ((BesiegerCamp)o).SiegeEvent;
		}

		// Token: 0x06002217 RID: 8727 RVA: 0x00090F38 File Offset: 0x0008F138
		internal static object AutoGeneratedGetMemberValueSiegeEngines(object o)
		{
			return ((BesiegerCamp)o).SiegeEngines;
		}

		// Token: 0x06002218 RID: 8728 RVA: 0x00090F45 File Offset: 0x0008F145
		internal static object AutoGeneratedGetMemberValueSiegeStrategy(object o)
		{
			return ((BesiegerCamp)o).SiegeStrategy;
		}

		// Token: 0x06002219 RID: 8729 RVA: 0x00090F52 File Offset: 0x0008F152
		internal static object AutoGeneratedGetMemberValueNumberOfTroopsKilledOnSide(object o)
		{
			return ((BesiegerCamp)o).NumberOfTroopsKilledOnSide;
		}

		// Token: 0x0600221A RID: 8730 RVA: 0x00090F64 File Offset: 0x0008F164
		internal static object AutoGeneratedGetMemberValueIsAttackersVulnerable(object o)
		{
			return ((BesiegerCamp)o).IsAttackersVulnerable;
		}

		// Token: 0x0600221B RID: 8731 RVA: 0x00090F76 File Offset: 0x0008F176
		internal static object AutoGeneratedGetMemberValue_besiegerParties(object o)
		{
			return ((BesiegerCamp)o)._besiegerParties;
		}

		// Token: 0x0600221C RID: 8732 RVA: 0x00090F83 File Offset: 0x0008F183
		internal static object AutoGeneratedGetMemberValue_siegeEngineMissiles(object o)
		{
			return ((BesiegerCamp)o)._siegeEngineMissiles;
		}

		// Token: 0x0600221D RID: 8733 RVA: 0x00090F90 File Offset: 0x0008F190
		internal static object AutoGeneratedGetMemberValue_attackerVulnerableDurationLeft(object o)
		{
			return ((BesiegerCamp)o)._attackerVulnerableDurationLeft;
		}

		// Token: 0x0600221E RID: 8734 RVA: 0x00090FA2 File Offset: 0x0008F1A2
		internal static object AutoGeneratedGetMemberValue_attackerVulnerableDayCount(object o)
		{
			return ((BesiegerCamp)o)._attackerVulnerableDayCount;
		}

		// Token: 0x0600221F RID: 8735 RVA: 0x00090FB4 File Offset: 0x0008F1B4
		internal static object AutoGeneratedGetMemberValue_attackerVulnerableDueTime(object o)
		{
			return ((BesiegerCamp)o)._attackerVulnerableDueTime;
		}

		// Token: 0x17000873 RID: 2163
		// (get) Token: 0x06002220 RID: 8736 RVA: 0x00090FC6 File Offset: 0x0008F1C6
		// (set) Token: 0x06002221 RID: 8737 RVA: 0x00090FCE File Offset: 0x0008F1CE
		[SaveableProperty(6)]
		public SiegeEvent SiegeEvent { get; private set; }

		// Token: 0x17000874 RID: 2164
		// (get) Token: 0x06002222 RID: 8738 RVA: 0x00090FD7 File Offset: 0x0008F1D7
		// (set) Token: 0x06002223 RID: 8739 RVA: 0x00090FDF File Offset: 0x0008F1DF
		[SaveableProperty(7)]
		public SiegeEvent.SiegeEnginesContainer SiegeEngines { get; private set; }

		// Token: 0x17000875 RID: 2165
		// (get) Token: 0x06002224 RID: 8740 RVA: 0x00090FE8 File Offset: 0x0008F1E8
		public MobileParty BesiegerParty
		{
			get
			{
				if (this._besiegerParties.Count <= 0)
				{
					return null;
				}
				return this._besiegerParties[0];
			}
		}

		// Token: 0x06002225 RID: 8741 RVA: 0x00091006 File Offset: 0x0008F206
		public IEnumerable<PartyBase> GetInvolvedPartiesForEventType(MapEvent.BattleTypes mapEventType = MapEvent.BattleTypes.Siege)
		{
			foreach (MobileParty mobileParty in this._besiegerParties)
			{
				yield return mobileParty.Party;
			}
			List<MobileParty>.Enumerator enumerator = default(List<MobileParty>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x06002226 RID: 8742 RVA: 0x00091016 File Offset: 0x0008F216
		public PartyBase GetNextInvolvedPartyForEventType(ref int partyIndex, MapEvent.BattleTypes mapEventType = MapEvent.BattleTypes.Siege)
		{
			partyIndex++;
			if (partyIndex >= this._besiegerParties.Count)
			{
				return null;
			}
			return this._besiegerParties[partyIndex].Party;
		}

		// Token: 0x06002227 RID: 8743 RVA: 0x00091044 File Offset: 0x0008F244
		public bool HasInvolvedPartyForEventType(PartyBase party, MapEvent.BattleTypes mapEventType = MapEvent.BattleTypes.Siege)
		{
			using (List<MobileParty>.Enumerator enumerator = this._besiegerParties.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Party == party)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x17000876 RID: 2166
		// (get) Token: 0x06002228 RID: 8744 RVA: 0x000910A0 File Offset: 0x0008F2A0
		public BattleSideEnum BattleSide
		{
			get
			{
				return BattleSideEnum.Attacker;
			}
		}

		// Token: 0x17000877 RID: 2167
		// (get) Token: 0x06002229 RID: 8745 RVA: 0x000910A3 File Offset: 0x0008F2A3
		public MBReadOnlyList<SiegeEvent.SiegeEngineMissile> SiegeEngineMissiles
		{
			get
			{
				return this._siegeEngineMissiles;
			}
		}

		// Token: 0x17000878 RID: 2168
		// (get) Token: 0x0600222A RID: 8746 RVA: 0x000910AB File Offset: 0x0008F2AB
		// (set) Token: 0x0600222B RID: 8747 RVA: 0x000910B3 File Offset: 0x0008F2B3
		[SaveableProperty(10)]
		public SiegeStrategy SiegeStrategy { get; private set; }

		// Token: 0x17000879 RID: 2169
		// (get) Token: 0x0600222C RID: 8748 RVA: 0x000910BC File Offset: 0x0008F2BC
		// (set) Token: 0x0600222D RID: 8749 RVA: 0x000910C4 File Offset: 0x0008F2C4
		[SaveableProperty(11)]
		public int NumberOfTroopsKilledOnSide { get; private set; }

		// Token: 0x1700087A RID: 2170
		// (get) Token: 0x0600222E RID: 8750 RVA: 0x000910CD File Offset: 0x0008F2CD
		// (set) Token: 0x0600222F RID: 8751 RVA: 0x000910D5 File Offset: 0x0008F2D5
		[SaveableProperty(13)]
		public bool IsAttackersVulnerable { get; private set; }

		// Token: 0x06002230 RID: 8752 RVA: 0x000910DE File Offset: 0x0008F2DE
		public BesiegerCamp(SiegeEvent siegeEvent)
		{
			this._besiegerParties = new List<MobileParty>();
			this.SiegeEvent = siegeEvent;
		}

		// Token: 0x06002231 RID: 8753 RVA: 0x000910F8 File Offset: 0x0008F2F8
		public bool IsBesiegerSideParty(MobileParty mobileParty)
		{
			Func<MobileParty, bool> <>9__1;
			return this.GetInvolvedPartiesForEventType(MapEvent.BattleTypes.Siege).Any(delegate(PartyBase t)
			{
				if (t != mobileParty.Party)
				{
					IEnumerable<MobileParty> attachedParties = t.MobileParty.AttachedParties;
					Func<MobileParty, bool> func;
					if ((func = <>9__1) == null)
					{
						func = (<>9__1 = (MobileParty k) => k == mobileParty);
					}
					return attachedParties.Any(func);
				}
				return true;
			});
		}

		// Token: 0x1700087B RID: 2171
		// (get) Token: 0x06002232 RID: 8754 RVA: 0x0009112C File Offset: 0x0008F32C
		private float PreparationProgress
		{
			get
			{
				SiegeEvent.SiegeEnginesContainer siegeEngines = this.SiegeEngines;
				float? num;
				if (siegeEngines == null)
				{
					num = null;
				}
				else
				{
					SiegeEvent.SiegeEngineConstructionProgress siegePreparations = siegeEngines.SiegePreparations;
					num = ((siegePreparations != null) ? new float?(siegePreparations.Progress) : null);
				}
				float? num2 = num;
				if (num2 == null)
				{
					return 0f;
				}
				return num2.GetValueOrDefault();
			}
		}

		// Token: 0x1700087C RID: 2172
		// (get) Token: 0x06002233 RID: 8755 RVA: 0x00091183 File Offset: 0x0008F383
		public bool IsPreparationComplete
		{
			get
			{
				return this.PreparationProgress >= 1f;
			}
		}

		// Token: 0x1700087D RID: 2173
		// (get) Token: 0x06002234 RID: 8756 RVA: 0x00091195 File Offset: 0x0008F395
		public bool IsReadyToBesiege
		{
			get
			{
				return this.IsPreparationComplete && this.StartingAssaultOnBesiegedSettlementIsLogical();
			}
		}

		// Token: 0x06002235 RID: 8757 RVA: 0x000911A8 File Offset: 0x0008F3A8
		public void InitializeSiegeEventSide()
		{
			this.SiegeStrategy = DefaultSiegeStrategies.Custom;
			this.NumberOfTroopsKilledOnSide = 0;
			SiegeEvent.SiegeEngineConstructionProgress siegeEngineConstructionProgress = new SiegeEvent.SiegeEngineConstructionProgress(DefaultSiegeEngineTypes.Preparations, this.PreparationProgress, (float)DefaultSiegeEngineTypes.Preparations.BaseHitPoints);
			siegeEngineConstructionProgress.Activate(true);
			this.SiegeEngines = new SiegeEvent.SiegeEnginesContainer(BattleSideEnum.Attacker, siegeEngineConstructionProgress);
			this._siegeEngineMissiles = new MBList<SiegeEvent.SiegeEngineMissile>();
			this.SetPrebuiltSiegeEngines();
		}

		// Token: 0x06002236 RID: 8758 RVA: 0x0009120A File Offset: 0x0008F40A
		public void OnTroopsKilledOnSide(int killCount)
		{
			this.NumberOfTroopsKilledOnSide += killCount;
		}

		// Token: 0x06002237 RID: 8759 RVA: 0x0009121A File Offset: 0x0008F41A
		public void SetSiegeStrategy(SiegeStrategy strategy)
		{
			this.SiegeStrategy = strategy;
		}

		// Token: 0x06002238 RID: 8760 RVA: 0x00091223 File Offset: 0x0008F423
		internal void AddSiegePartyInternal(MobileParty mobileParty)
		{
			this._besiegerParties.Add(mobileParty);
			if (this._besiegerParties.Count == 1)
			{
				this.SiegeEvent.BesiegedSettlement.LastAttackerParty = mobileParty;
			}
		}

		// Token: 0x06002239 RID: 8761 RVA: 0x00091250 File Offset: 0x0008F450
		internal void RemoveSiegePartyInternal(MobileParty mobileParty)
		{
			this._besiegerParties.Remove(mobileParty);
			if (mobileParty != null)
			{
				this.OnPartyLeftSiege(mobileParty);
			}
			if (this._besiegerParties.Count == 0)
			{
				this.SiegeEvent.FinalizeSiegeEvent();
			}
		}

		// Token: 0x0600223A RID: 8762 RVA: 0x00091284 File Offset: 0x0008F484
		public void RemoveAllSiegeParties()
		{
			MapEvent mapEvent = this.SiegeEvent.BesiegedSettlement.Party.MapEvent;
			if (mapEvent != null && mapEvent.IsSiegeAssault && !mapEvent.IsFinished)
			{
				Debug.FailedAssert("RemoveAllParties called before mapEvent is cleared", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Siege\\BesiegerCamp.cs", "RemoveAllSiegeParties", 162);
			}
			while (this._besiegerParties.Count > 0)
			{
				this._besiegerParties[0].BesiegerCamp = null;
			}
		}

		// Token: 0x0600223B RID: 8763 RVA: 0x000912F8 File Offset: 0x0008F4F8
		public void Tick(float dt)
		{
			bool flag = false;
			if (this._attackerVulnerableDurationLeft > 0f)
			{
				this._attackerVulnerableDurationLeft -= dt;
				flag = true;
			}
			else if (this._attackerVulnerableDueTime > 0f)
			{
				this._attackerVulnerableDueTime -= dt;
				if (this._attackerVulnerableDueTime <= 0f)
				{
					this._attackerVulnerableDurationLeft = Campaign.Current.Models.PartyImpairmentModel.GetVulnerabilityStateDuration(this.BesiegerParty.Party);
					flag = true;
				}
			}
			else if (this._attackerVulnerableDayCount < this.SiegeEvent.SiegeStartTime.ElapsedDaysUntilNow)
			{
				this._attackerVulnerableDayCount = this.SiegeEvent.SiegeStartTime.ElapsedDaysUntilNow;
				this._attackerVulnerableDueTime = Campaign.Current.Models.PartyImpairmentModel.GetSiegeExpectedVulnerabilityTime();
			}
			this.IsAttackersVulnerable = flag;
		}

		// Token: 0x0600223C RID: 8764 RVA: 0x000913D0 File Offset: 0x0008F5D0
		public void OnPartyLeftSiege(MobileParty mobileParty)
		{
			if (mobileParty == MobileParty.MainParty && PlayerSiege.PlayerSiegeEvent != null)
			{
				PlayerSiege.ClosePlayerSiege();
			}
			if (Campaign.Current.Models.PartyImpairmentModel.CanGetDisorganized(mobileParty.Party))
			{
				mobileParty.SetDisorganized(true);
			}
			if (this._besiegerParties.Contains(MobileParty.MainParty) && (MobileParty.MainParty.MapEvent == null || !MobileParty.MainParty.MapEvent.HasWinner))
			{
				Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
				MenuContext currentMenuContext = Campaign.Current.CurrentMenuContext;
				if (currentMenuContext == null)
				{
					return;
				}
				currentMenuContext.Refresh();
			}
		}

		// Token: 0x0600223D RID: 8765 RVA: 0x00091464 File Offset: 0x0008F664
		internal void SetSiegeCampPartyPosition(MobileParty mobileParty)
		{
			ReadOnlyCollection<MatrixFrame> siegeCamp1GlobalFrames = this.SiegeEvent.BesiegedSettlement.Party.Visuals.GetSiegeCamp1GlobalFrames();
			ReadOnlyCollection<MatrixFrame> siegeCamp2GlobalFrames = this.SiegeEvent.BesiegedSettlement.Party.Visuals.GetSiegeCamp2GlobalFrames();
			int num = 0;
			Vec2 vec2;
			bool flag;
			do
			{
				int num2 = MBRandom.RandomInt(siegeCamp1GlobalFrames.Count);
				float randomFloat = MBRandom.RandomFloat;
				float randomFloat2 = MBRandom.RandomFloat;
				MatrixFrame matrixFrame = siegeCamp1GlobalFrames[num2];
				Vec2 asVec = matrixFrame.origin.AsVec2;
				float num3 = randomFloat;
				matrixFrame = siegeCamp1GlobalFrames[num2];
				Vec2 vec = asVec + num3 * matrixFrame.rotation.s.AsVec2;
				float num4 = randomFloat2;
				matrixFrame = siegeCamp1GlobalFrames[num2];
				vec2 = vec + num4 * matrixFrame.rotation.f.AsVec2;
				flag = false;
				foreach (PartyBase partyBase in this.SiegeEvent.BesiegerCamp.GetInvolvedPartiesForEventType(MapEvent.BattleTypes.Siege))
				{
					if ((vec2 - partyBase.MobileParty.VisualPosition2DWithoutError).LengthSquared < 0.25f)
					{
						flag = true;
						break;
					}
				}
				num++;
			}
			while (flag && num < 20);
			if (num == 20 && siegeCamp2GlobalFrames != null && siegeCamp2GlobalFrames.Count > 0)
			{
				num = 0;
				do
				{
					int num5 = MBRandom.RandomInt(siegeCamp2GlobalFrames.Count);
					float randomFloat3 = MBRandom.RandomFloat;
					float randomFloat4 = MBRandom.RandomFloat;
					MatrixFrame matrixFrame = siegeCamp2GlobalFrames[num5];
					Vec2 asVec2 = matrixFrame.origin.AsVec2;
					float num6 = randomFloat3;
					matrixFrame = siegeCamp2GlobalFrames[num5];
					Vec2 vec3 = asVec2 + num6 * matrixFrame.rotation.s.AsVec2;
					float num7 = randomFloat4;
					matrixFrame = siegeCamp2GlobalFrames[num5];
					vec2 = vec3 + num7 * matrixFrame.rotation.f.AsVec2;
					flag = false;
					foreach (PartyBase partyBase2 in this.SiegeEvent.BesiegerCamp.GetInvolvedPartiesForEventType(MapEvent.BattleTypes.Siege))
					{
						if ((vec2 - partyBase2.MobileParty.VisualPosition2DWithoutError).LengthSquared < 0.25f)
						{
							flag = true;
							break;
						}
					}
					num++;
				}
				while (flag && num < 20);
			}
			mobileParty.Position2D = vec2;
			mobileParty.Party.Visuals.SetMapIconAsDirty();
		}

		// Token: 0x0600223E RID: 8766 RVA: 0x000916E0 File Offset: 0x0008F8E0
		public void AddSiegeEngineMissile(SiegeEvent.SiegeEngineMissile missile)
		{
			this._siegeEngineMissiles.Add(missile);
		}

		// Token: 0x0600223F RID: 8767 RVA: 0x000916EE File Offset: 0x0008F8EE
		public void RemoveDeprecatedMissiles()
		{
			this._siegeEngineMissiles.RemoveAll((SiegeEvent.SiegeEngineMissile missile) => missile.CollisionTime.IsPast);
		}

		// Token: 0x06002240 RID: 8768 RVA: 0x0009171C File Offset: 0x0008F91C
		private bool StartingAssaultOnBesiegedSettlementIsLogical()
		{
			float num = 0f;
			float num2 = ((MobileParty.MainParty.CurrentSettlement == this.SiegeEvent.BesiegedSettlement) ? 0.5f : 1f);
			foreach (PartyBase partyBase in this.SiegeEvent.BesiegedSettlement.GetInvolvedPartiesForEventType(MapEvent.BattleTypes.Siege))
			{
				if (partyBase.IsMobile && partyBase.MobileParty.CurrentSettlement == this.SiegeEvent.BesiegedSettlement && (partyBase.MobileParty.Aggressiveness > 0.01f || partyBase.MobileParty.IsMilitia || partyBase.MobileParty.IsGarrison))
				{
					num += num2 * partyBase.TotalStrength;
				}
			}
			float num3 = 0f;
			LocatableSearchData<MobileParty> locatableSearchData = Campaign.Current.MobilePartyLocator.StartFindingLocatablesAroundPosition(this.SiegeEvent.BesiegedSettlement.Position2D, 10f);
			for (MobileParty mobileParty = Campaign.Current.MobilePartyLocator.FindNextLocatable(ref locatableSearchData); mobileParty != null; mobileParty = Campaign.Current.MobilePartyLocator.FindNextLocatable(ref locatableSearchData))
			{
				if (mobileParty.Aggressiveness > 0f && mobileParty.MapFaction == this.SiegeEvent.BesiegedSettlement.MapFaction && mobileParty.CurrentSettlement == null)
				{
					num3 += mobileParty.Party.TotalStrength;
				}
			}
			float num4 = 0f;
			foreach (PartyBase partyBase2 in this.SiegeEvent.BesiegerCamp.GetInvolvedPartiesForEventType(MapEvent.BattleTypes.Siege))
			{
				num4 += partyBase2.TotalStrength;
			}
			float settlementAdvantage = Campaign.Current.Models.CombatSimulationModel.GetSettlementAdvantage(this.SiegeEvent.BesiegedSettlement);
			float maximumSiegeEquipmentProgress = Campaign.Current.Models.CombatSimulationModel.GetMaximumSiegeEquipmentProgress(this.SiegeEvent.BesiegedSettlement);
			float randomFloat = MBRandom.RandomFloat;
			float num5 = num4 / (num * MathF.Pow(settlementAdvantage, 0.67f));
			bool flag = false;
			float num6 = 0.9f;
			if (num5 > num6)
			{
				float num7 = 0f;
				float num8 = 0f;
				foreach (PartyBase partyBase3 in this.SiegeEvent.BesiegerCamp.GetInvolvedPartiesForEventType(MapEvent.BattleTypes.Siege))
				{
					num7 += partyBase3.MobileParty.Food;
					num8 += -partyBase3.MobileParty.FoodChange;
				}
				float num9 = MathF.Max(0f, num7) / num8;
				float num10 = MathF.Min(1f, num3 / num4);
				float num11 = ((num9 < 3f) ? ((1f + (3f - num9) * (3f - num9)) * 0.2f) : 0f);
				float num12 = (MathF.Min(4f, MathF.Max(num5, 1f)) - 1f) * 0.2f;
				float totalStrength = this.SiegeEvent.BesiegerCamp.BesiegerParty.MapFaction.TotalStrength;
				float totalStrength2 = this.SiegeEvent.BesiegedSettlement.MapFaction.TotalStrength;
				float num13 = ((totalStrength < totalStrength2) ? ((1f - (totalStrength + 0.01f) / (totalStrength2 + 0.01f)) * 0.6f) : 0f);
				float num14 = MathF.Max(0.5f, 3f - num12 - num10 - num11 - num13);
				float num15 = MathF.Pow(settlementAdvantage, num14);
				int numberOfEquipmentsBuilt = Campaign.Current.Models.CombatSimulationModel.GetNumberOfEquipmentsBuilt(this.SiegeEvent.BesiegedSettlement);
				float num16 = (MathF.Min(9f, num5) - num6) / num15;
				num16 *= ((float)MathF.Min(2, numberOfEquipmentsBuilt) + 1f) / 3f;
				num16 *= 1f - 0.66f * (maximumSiegeEquipmentProgress * maximumSiegeEquipmentProgress);
				flag = num == 0f || randomFloat < num16;
			}
			return flag;
		}

		// Token: 0x06002241 RID: 8769 RVA: 0x00091B44 File Offset: 0x0008FD44
		private void SetPrebuiltSiegeEngines()
		{
			foreach (SiegeEngineType siegeEngineType in Campaign.Current.Models.SiegeEventModel.GetPrebuiltSiegeEnginesOfSiegeCamp(this))
			{
				float siegeEngineHitPoints = Campaign.Current.Models.SiegeEventModel.GetSiegeEngineHitPoints(this.SiegeEvent, siegeEngineType, BattleSideEnum.Attacker);
				SiegeEvent.SiegeEngineConstructionProgress siegeEngineConstructionProgress = new SiegeEvent.SiegeEngineConstructionProgress(siegeEngineType, 1f, siegeEngineHitPoints);
				this.SiegeEngines.AddPrebuiltEngineToReserve(siegeEngineConstructionProgress);
				this.SiegeEvent.CreateSiegeObject(siegeEngineConstructionProgress, this.SiegeEvent.GetSiegeEventSide(BattleSideEnum.Defender));
			}
		}

		// Token: 0x06002242 RID: 8770 RVA: 0x00091BE8 File Offset: 0x0008FDE8
		private float GetDistanceBetweenRangedEngineAndWall(int rangedEngine, int wallIndex)
		{
			float num = (float)rangedEngine * 1f;
			float num2 = 0.5f + 2f * (float)wallIndex;
			return MathF.Abs(num - num2) + 2f;
		}

		// Token: 0x06002243 RID: 8771 RVA: 0x00091C19 File Offset: 0x0008FE19
		private float PriorityCalculationForWalls(float distance)
		{
			return 5f - distance;
		}

		// Token: 0x06002244 RID: 8772 RVA: 0x00091C24 File Offset: 0x0008FE24
		private void FindAttackableWallSectionWithHighestPriority(int attackerSlotIndex, SiegeEngineType siegeEngine, out int targetIndex, out float targetPriority)
		{
			targetIndex = -1;
			targetPriority = 0f;
			if (siegeEngine.IsAntiPersonnel)
			{
				return;
			}
			float num = 9999f;
			MBReadOnlyList<float> settlementWallSectionHitPointsRatioList = this.SiegeEvent.BesiegedSettlement.SettlementWallSectionHitPointsRatioList;
			for (int i = 0; i < settlementWallSectionHitPointsRatioList.Count; i++)
			{
				if (settlementWallSectionHitPointsRatioList[i] > 0f)
				{
					float distanceBetweenRangedEngineAndWall = this.GetDistanceBetweenRangedEngineAndWall(attackerSlotIndex, i);
					float num2 = this.PriorityCalculationForWalls(distanceBetweenRangedEngineAndWall);
					if (num2 > targetPriority || (MathF.Abs(num2 - targetPriority) < 0.0001f && num > distanceBetweenRangedEngineAndWall))
					{
						targetIndex = i;
						targetPriority = num2;
						num = distanceBetweenRangedEngineAndWall;
					}
				}
			}
		}

		// Token: 0x06002245 RID: 8773 RVA: 0x00091CB8 File Offset: 0x0008FEB8
		public void BombardHitWalls(SiegeEngineType attackerEngineType, int wallIndex)
		{
			MBReadOnlyList<float> settlementWallSectionHitPointsRatioList = this.SiegeEvent.BesiegedSettlement.SettlementWallSectionHitPointsRatioList;
			if (settlementWallSectionHitPointsRatioList[wallIndex] > 0f)
			{
				float siegeEngineDamage = Campaign.Current.Models.SiegeEventModel.GetSiegeEngineDamage(this.SiegeEvent, BattleSideEnum.Attacker, attackerEngineType, SiegeBombardTargets.Wall);
				float num = siegeEngineDamage / this.SiegeEvent.BesiegedSettlement.MaxHitPointsOfOneWallSection;
				this.SiegeEvent.BesiegedSettlement.SetWallSectionHitPointsRatioAtIndex(wallIndex, MBMath.ClampFloat(settlementWallSectionHitPointsRatioList[wallIndex] - num, 0f, 1f));
				bool flag = settlementWallSectionHitPointsRatioList[wallIndex] <= 0f;
				if (flag)
				{
					this.SiegeEvent.BesiegedSettlement.Party.Visuals.SetMapIconAsDirty();
				}
				CampaignEventDispatcher.Instance.OnSiegeBombardmentWallHit(this.BesiegerParty, this.SiegeEvent.BesiegedSettlement, BattleSideEnum.Attacker, attackerEngineType, flag);
				Debug.Print(string.Concat(new object[]
				{
					this.SiegeEvent.BesiegedSettlement.Name,
					" - ",
					BattleSideEnum.Attacker.ToString(),
					" ",
					attackerEngineType.Name,
					" hit the walls for ",
					siegeEngineDamage
				}), 0, Debug.DebugColor.Yellow, 137438953472UL);
			}
		}

		// Token: 0x06002246 RID: 8774 RVA: 0x00091DFC File Offset: 0x0008FFFC
		public void GetAttackTarget(ISiegeEventSide siegeEventSide, SiegeEngineType siegeEngine, int siegeEngineSlot, out SiegeBombardTargets targetType, out int targetIndex)
		{
			targetType = SiegeBombardTargets.None;
			targetIndex = -1;
			int num;
			float num2;
			siegeEventSide.SiegeEvent.FindAttackableRangedEngineWithHighestPriority(this, siegeEngineSlot, out num, out num2);
			int num3;
			float num4;
			this.FindAttackableWallSectionWithHighestPriority(siegeEngineSlot, siegeEngine, out num3, out num4);
			if (num == -1 && num3 == -1)
			{
				return;
			}
			if (num == -1)
			{
				targetIndex = num3;
				targetType = SiegeBombardTargets.Wall;
				return;
			}
			if (num3 == -1)
			{
				targetIndex = num;
				targetType = SiegeBombardTargets.RangedEngines;
				return;
			}
			float num5 = num2 + num4;
			if (MBRandom.RandomFloat * num5 < num2)
			{
				targetIndex = num;
				targetType = SiegeBombardTargets.RangedEngines;
				return;
			}
			targetIndex = num3;
			targetType = SiegeBombardTargets.Wall;
		}

		// Token: 0x06002247 RID: 8775 RVA: 0x00091E72 File Offset: 0x00090072
		public void FinalizeSiegeEvent()
		{
			if (!this.GetInvolvedPartiesForEventType(MapEvent.BattleTypes.Siege).IsEmpty<PartyBase>())
			{
				this.RemoveAllSiegeParties();
			}
		}

		// Token: 0x04000A94 RID: 2708
		[SaveableField(8)]
		private readonly List<MobileParty> _besiegerParties;

		// Token: 0x04000A95 RID: 2709
		[SaveableField(1)]
		private MBList<SiegeEvent.SiegeEngineMissile> _siegeEngineMissiles;

		// Token: 0x04000A99 RID: 2713
		[SaveableField(3)]
		private float _attackerVulnerableDurationLeft;

		// Token: 0x04000A9A RID: 2714
		[SaveableField(4)]
		private float _attackerVulnerableDayCount;

		// Token: 0x04000A9B RID: 2715
		[SaveableField(5)]
		private float _attackerVulnerableDueTime;
	}
}
