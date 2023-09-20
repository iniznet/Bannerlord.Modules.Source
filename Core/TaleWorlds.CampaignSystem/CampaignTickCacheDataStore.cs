using System;
using System.Collections.Generic;
using System.Threading;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x0200006F RID: 111
	public class CampaignTickCacheDataStore
	{
		// Token: 0x06000EA2 RID: 3746 RVA: 0x0004416C File Offset: 0x0004236C
		internal CampaignTickCacheDataStore()
		{
			this._fadingPartiesSet = new HashSet<IPartyVisual>();
			this._fadingPartiesFlatten = new List<IPartyVisual>();
			this._mobilePartyComparer = new CampaignTickCacheDataStore.MobilePartyComparer();
			this._parallelInitializeCachedPartyVariablesPredicate = new TWParallel.ParallelForAuxPredicate(this.ParallelInitializeCachedPartyVariables);
			this._parallelCacheTargetPartyVariablesAtFrameStartPredicate = new TWParallel.ParallelForAuxPredicate(this.ParallelCacheTargetPartyVariablesAtFrameStart);
			this._parallelArrangePartyIndicesPredicate = new TWParallel.ParallelForAuxPredicate(this.ParallelArrangePartyIndices);
			this._parallelTickArmiesPredicate = new TWParallel.ParallelForAuxPredicate(this.ParallelTickArmies);
			this._parallelTickMovingPartiesPredicate = new TWParallel.ParallelForAuxPredicate(this.ParallelTickMovingParties);
			this._parallelTickStationaryPartiesPredicate = new TWParallel.ParallelForAuxPredicate(this.ParallelTickStationaryParties);
			this._parallelTickSettlementVisualsPredicate = new TWParallel.ParallelForAuxPredicate(this.ParallelTickSettlementVisuals);
			this._parallelTickMobilePartyVisualsPredicate = new TWParallel.ParallelForAuxPredicate(this.ParallelTickMobilePartyVisuals);
			this._parallelCheckExitingSettlementsPredicate = new TWParallel.ParallelForAuxPredicate(this.ParallelCheckExitingSettlements);
		}

		// Token: 0x06000EA3 RID: 3747 RVA: 0x00044242 File Offset: 0x00042442
		internal void ValidateMobilePartyTickDataCache(int currentTotalMobilePartyCount)
		{
			if (this._currentTotalMobilePartyCapacity <= currentTotalMobilePartyCount)
			{
				this.InitializeCacheArrays();
			}
			this._currentFrameMovingPartyCount = -1;
			this._currentFrameStationaryPartyCount = -1;
			this._currentFrameMovingArmyLeaderCount = -1;
			this._gridChangeCount = -1;
			this._exitingSettlementCount = -1;
			this._dirtyPartyVisualCount = -1;
		}

		// Token: 0x06000EA4 RID: 3748 RVA: 0x00044280 File Offset: 0x00042480
		private void InitializeCacheArrays()
		{
			int num = (int)((float)this._currentTotalMobilePartyCapacity * 2f);
			this._cacheData = new CampaignTickCacheDataStore.PartyTickCachePerParty[num];
			this._gridChangeMobilePartyList = new MobileParty[num];
			this._exitingSettlementMobilePartyList = new MobileParty[num];
			this._dirtyPartiesList = new PartyBase[num];
			this._currentTotalMobilePartyCapacity = num;
			this._movingPartyIndices = new int[num];
			this._stationaryPartyIndices = new int[num];
			this._movingArmyLeaderPartyIndices = new int[num];
		}

		// Token: 0x06000EA5 RID: 3749 RVA: 0x000442F8 File Offset: 0x000424F8
		internal void InitializeDataCache()
		{
			this._currentFrameMovingArmyLeaderCount = Campaign.Current.MobileParties.Count;
			this._currentTotalMobilePartyCapacity = Campaign.Current.MobileParties.Count;
			this._currentFrameStationaryPartyCount = Campaign.Current.MobileParties.Count;
			this.InitializeCacheArrays();
		}

		// Token: 0x06000EA6 RID: 3750 RVA: 0x0004434C File Offset: 0x0004254C
		private void ParallelCheckExitingSettlements(int startInclusive, int endExclusive)
		{
			for (int i = startInclusive; i < endExclusive; i++)
			{
				Campaign.Current.MobileParties[i].CheckExitingSettlementParallel(ref this._exitingSettlementCount, ref this._exitingSettlementMobilePartyList);
			}
		}

		// Token: 0x06000EA7 RID: 3751 RVA: 0x00044388 File Offset: 0x00042588
		private void ParallelInitializeCachedPartyVariables(int startInclusive, int endExclusive)
		{
			for (int i = startInclusive; i < endExclusive; i++)
			{
				MobileParty mobileParty = Campaign.Current.MobileParties[i];
				this._cacheData[i].MobileParty = mobileParty;
				mobileParty.InitializeCachedPartyVariables(ref this._cacheData[i].LocalVariables);
			}
		}

		// Token: 0x06000EA8 RID: 3752 RVA: 0x000443DC File Offset: 0x000425DC
		private void ParallelCacheTargetPartyVariablesAtFrameStart(int startInclusive, int endExclusive)
		{
			for (int i = startInclusive; i < endExclusive; i++)
			{
				this._cacheData[i].MobileParty.Ai.CacheTargetPartyVariablesAtFrameStart(ref this._cacheData[i].LocalVariables);
			}
		}

		// Token: 0x06000EA9 RID: 3753 RVA: 0x00044424 File Offset: 0x00042624
		private void ParallelArrangePartyIndices(int startInclusive, int endExclusive)
		{
			for (int i = startInclusive; i < endExclusive; i++)
			{
				MobileParty.CachedPartyVariables localVariables = this._cacheData[i].LocalVariables;
				if (localVariables.IsMoving)
				{
					if (localVariables.IsArmyLeader)
					{
						int num = Interlocked.Increment(ref this._currentFrameMovingArmyLeaderCount);
						this._movingArmyLeaderPartyIndices[num] = i;
					}
					else
					{
						int num2 = Interlocked.Increment(ref this._currentFrameMovingPartyCount);
						this._movingPartyIndices[num2] = i;
					}
				}
				else
				{
					int num3 = Interlocked.Increment(ref this._currentFrameStationaryPartyCount);
					this._stationaryPartyIndices[num3] = i;
				}
			}
		}

		// Token: 0x06000EAA RID: 3754 RVA: 0x000444A4 File Offset: 0x000426A4
		private void ParallelTickArmies(int startInclusive, int endExclusive)
		{
			for (int i = startInclusive; i < endExclusive; i++)
			{
				int num = this._movingArmyLeaderPartyIndices[i];
				CampaignTickCacheDataStore.PartyTickCachePerParty partyTickCachePerParty = this._cacheData[num];
				MobileParty mobileParty = partyTickCachePerParty.MobileParty;
				MobileParty.CachedPartyVariables localVariables = partyTickCachePerParty.LocalVariables;
				mobileParty.TickForMovingArmyLeader(ref localVariables, this._currentDt, this._currentRealDt);
				mobileParty.TickForMobileParty2(ref localVariables, this._currentRealDt, ref this._gridChangeCount, ref this._gridChangeMobilePartyList);
				mobileParty.ValidateSpeed();
			}
		}

		// Token: 0x06000EAB RID: 3755 RVA: 0x00044514 File Offset: 0x00042714
		private void ParallelTickMovingParties(int startInclusive, int endExclusive)
		{
			for (int i = startInclusive; i < endExclusive; i++)
			{
				int num = this._movingPartyIndices[i];
				CampaignTickCacheDataStore.PartyTickCachePerParty partyTickCachePerParty = this._cacheData[num];
				MobileParty mobileParty = partyTickCachePerParty.MobileParty;
				MobileParty.CachedPartyVariables localVariables = partyTickCachePerParty.LocalVariables;
				mobileParty.TickForMovingMobileParty(ref localVariables, this._currentDt, this._currentRealDt);
				mobileParty.TickForMobileParty2(ref localVariables, this._currentRealDt, ref this._gridChangeCount, ref this._gridChangeMobilePartyList);
			}
		}

		// Token: 0x06000EAC RID: 3756 RVA: 0x00044580 File Offset: 0x00042780
		private void ParallelTickStationaryParties(int startInclusive, int endExclusive)
		{
			for (int i = startInclusive; i < endExclusive; i++)
			{
				int num = this._stationaryPartyIndices[i];
				CampaignTickCacheDataStore.PartyTickCachePerParty partyTickCachePerParty = this._cacheData[num];
				MobileParty mobileParty = partyTickCachePerParty.MobileParty;
				MobileParty.CachedPartyVariables localVariables = partyTickCachePerParty.LocalVariables;
				mobileParty.TickForStationaryMobileParty(ref localVariables, this._currentDt, this._currentRealDt);
				mobileParty.TickForMobileParty2(ref localVariables, this._currentRealDt, ref this._gridChangeCount, ref this._gridChangeMobilePartyList);
			}
		}

		// Token: 0x06000EAD RID: 3757 RVA: 0x000445EC File Offset: 0x000427EC
		private void ParallelTickSettlementVisuals(int startInclusive, int endExclusive)
		{
			for (int i = startInclusive; i < endExclusive; i++)
			{
				Settlement.All[i].Party.VisualTick(this._currentRealDt, this._currentDt, ref this._dirtyPartyVisualCount, ref this._dirtyPartiesList);
			}
		}

		// Token: 0x06000EAE RID: 3758 RVA: 0x00044634 File Offset: 0x00042834
		private void ParallelTickMobilePartyVisuals(int startInclusive, int endExclusive)
		{
			for (int i = startInclusive; i < endExclusive; i++)
			{
				Campaign.Current.MobileParties[i].Party.VisualTick(this._currentRealDt, this._currentDt, ref this._dirtyPartyVisualCount, ref this._dirtyPartiesList);
			}
		}

		// Token: 0x06000EAF RID: 3759 RVA: 0x00044680 File Offset: 0x00042880
		internal void Tick()
		{
			TWParallel.For(0, Campaign.Current.MobileParties.Count, this._parallelCheckExitingSettlementsPredicate, 16);
			Array.Sort<MobileParty>(this._exitingSettlementMobilePartyList, 0, this._exitingSettlementCount + 1, this._mobilePartyComparer);
			for (int i = 0; i < this._exitingSettlementCount + 1; i++)
			{
				LeaveSettlementAction.ApplyForParty(this._exitingSettlementMobilePartyList[i]);
			}
		}

		// Token: 0x06000EB0 RID: 3760 RVA: 0x000446E4 File Offset: 0x000428E4
		internal void RealTick(float dt, float realDt)
		{
			this._currentDt = dt;
			this._currentRealDt = realDt;
			this.ValidateMobilePartyTickDataCache(Campaign.Current.MobileParties.Count);
			int count = Campaign.Current.MobileParties.Count;
			TWParallel.For(0, count, this._parallelInitializeCachedPartyVariablesPredicate, 16);
			TWParallel.For(0, count, this._parallelCacheTargetPartyVariablesAtFrameStartPredicate, 16);
			TWParallel.For(0, count, this._parallelArrangePartyIndicesPredicate, 16);
			TWParallel.For(0, this._currentFrameMovingArmyLeaderCount + 1, this._parallelTickArmiesPredicate, 16);
			TWParallel.For(0, this._currentFrameMovingPartyCount + 1, this._parallelTickMovingPartiesPredicate, 16);
			TWParallel.For(0, this._currentFrameStationaryPartyCount + 1, this._parallelTickStationaryPartiesPredicate, 16);
			this.UpdateVisibilitiesAroundMainParty();
			Array.Sort<MobileParty>(this._gridChangeMobilePartyList, 0, this._gridChangeCount + 1, this._mobilePartyComparer);
			Campaign campaign = Campaign.Current;
			for (int i = 0; i < this._gridChangeCount + 1; i++)
			{
				campaign.MobilePartyLocator.UpdateLocator(this._gridChangeMobilePartyList[i]);
			}
			TWParallel.For(0, Settlement.All.Count, this._parallelTickSettlementVisualsPredicate, 16);
			TWParallel.For(0, Campaign.Current.MobileParties.Count, this._parallelTickMobilePartyVisualsPredicate, 16);
			for (int j = 0; j < this._dirtyPartyVisualCount + 1; j++)
			{
				PartyBase partyBase = this._dirtyPartiesList[j];
				partyBase.Visuals.ValidateIsDirty(partyBase, realDt, dt);
			}
			for (int k = this._fadingPartiesFlatten.Count - 1; k >= 0; k--)
			{
				this._fadingPartiesFlatten[k].TickFadingState(realDt, dt);
			}
		}

		// Token: 0x06000EB1 RID: 3761 RVA: 0x00044874 File Offset: 0x00042A74
		private void UpdateVisibilitiesAroundMainParty()
		{
			if (MobileParty.MainParty.CurrentNavigationFace.IsValid() && Campaign.Current.GetSimplifiedTimeControlMode() != CampaignTimeControlMode.Stop)
			{
				float seeingRange = MobileParty.MainParty.SeeingRange;
				LocatableSearchData<MobileParty> locatableSearchData = MobileParty.StartFindingLocatablesAroundPosition(MobileParty.MainParty.Position2D, seeingRange + 25f);
				for (MobileParty mobileParty = MobileParty.FindNextLocatable(ref locatableSearchData); mobileParty != null; mobileParty = MobileParty.FindNextLocatable(ref locatableSearchData))
				{
					if (!mobileParty.IsMilitia && !mobileParty.IsGarrison)
					{
						mobileParty.Party.UpdateVisibilityAndInspected(seeingRange, false);
					}
				}
				LocatableSearchData<Settlement> locatableSearchData2 = Settlement.StartFindingLocatablesAroundPosition(MobileParty.MainParty.Position2D, seeingRange + 25f);
				for (Settlement settlement = Settlement.FindNextLocatable(ref locatableSearchData2); settlement != null; settlement = Settlement.FindNextLocatable(ref locatableSearchData2))
				{
					settlement.Party.UpdateVisibilityAndInspected(seeingRange, false);
				}
			}
		}

		// Token: 0x06000EB2 RID: 3762 RVA: 0x00044939 File Offset: 0x00042B39
		internal void RegisterFadingVisual(IPartyVisual visual)
		{
			if (!this._fadingPartiesSet.Contains(visual))
			{
				this._fadingPartiesFlatten.Add(visual);
				this._fadingPartiesSet.Add(visual);
			}
		}

		// Token: 0x06000EB3 RID: 3763 RVA: 0x00044964 File Offset: 0x00042B64
		internal void UnregisterFadingVisual(IPartyVisual visual)
		{
			if (this._fadingPartiesSet.Contains(visual))
			{
				int num = this._fadingPartiesFlatten.IndexOf(visual);
				this._fadingPartiesFlatten[num] = this._fadingPartiesFlatten[this._fadingPartiesFlatten.Count - 1];
				this._fadingPartiesFlatten.Remove(this._fadingPartiesFlatten[this._fadingPartiesFlatten.Count - 1]);
				this._fadingPartiesSet.Remove(visual);
			}
		}

		// Token: 0x04000446 RID: 1094
		private CampaignTickCacheDataStore.PartyTickCachePerParty[] _cacheData;

		// Token: 0x04000447 RID: 1095
		private MobileParty[] _gridChangeMobilePartyList;

		// Token: 0x04000448 RID: 1096
		private MobileParty[] _exitingSettlementMobilePartyList;

		// Token: 0x04000449 RID: 1097
		private PartyBase[] _dirtyPartiesList;

		// Token: 0x0400044A RID: 1098
		private int[] _movingPartyIndices;

		// Token: 0x0400044B RID: 1099
		private int _currentFrameMovingPartyCount;

		// Token: 0x0400044C RID: 1100
		private int[] _stationaryPartyIndices;

		// Token: 0x0400044D RID: 1101
		private int _currentFrameStationaryPartyCount;

		// Token: 0x0400044E RID: 1102
		private int[] _movingArmyLeaderPartyIndices;

		// Token: 0x0400044F RID: 1103
		private int _currentFrameMovingArmyLeaderCount;

		// Token: 0x04000450 RID: 1104
		private int _currentTotalMobilePartyCapacity;

		// Token: 0x04000451 RID: 1105
		private int _gridChangeCount;

		// Token: 0x04000452 RID: 1106
		private int _exitingSettlementCount;

		// Token: 0x04000453 RID: 1107
		private int _dirtyPartyVisualCount;

		// Token: 0x04000454 RID: 1108
		private float _currentDt;

		// Token: 0x04000455 RID: 1109
		private float _currentRealDt;

		// Token: 0x04000456 RID: 1110
		private readonly TWParallel.ParallelForAuxPredicate _parallelInitializeCachedPartyVariablesPredicate;

		// Token: 0x04000457 RID: 1111
		private readonly TWParallel.ParallelForAuxPredicate _parallelCacheTargetPartyVariablesAtFrameStartPredicate;

		// Token: 0x04000458 RID: 1112
		private readonly TWParallel.ParallelForAuxPredicate _parallelArrangePartyIndicesPredicate;

		// Token: 0x04000459 RID: 1113
		private readonly TWParallel.ParallelForAuxPredicate _parallelTickArmiesPredicate;

		// Token: 0x0400045A RID: 1114
		private readonly TWParallel.ParallelForAuxPredicate _parallelTickMovingPartiesPredicate;

		// Token: 0x0400045B RID: 1115
		private readonly TWParallel.ParallelForAuxPredicate _parallelTickStationaryPartiesPredicate;

		// Token: 0x0400045C RID: 1116
		private readonly TWParallel.ParallelForAuxPredicate _parallelTickSettlementVisualsPredicate;

		// Token: 0x0400045D RID: 1117
		private readonly TWParallel.ParallelForAuxPredicate _parallelTickMobilePartyVisualsPredicate;

		// Token: 0x0400045E RID: 1118
		private readonly TWParallel.ParallelForAuxPredicate _parallelCheckExitingSettlementsPredicate;

		// Token: 0x0400045F RID: 1119
		private readonly List<IPartyVisual> _fadingPartiesFlatten;

		// Token: 0x04000460 RID: 1120
		private readonly CampaignTickCacheDataStore.MobilePartyComparer _mobilePartyComparer;

		// Token: 0x04000461 RID: 1121
		private readonly HashSet<IPartyVisual> _fadingPartiesSet;

		// Token: 0x020004C2 RID: 1218
		private struct PartyTickCachePerParty
		{
			// Token: 0x040014A1 RID: 5281
			internal MobileParty MobileParty;

			// Token: 0x040014A2 RID: 5282
			internal MobileParty.CachedPartyVariables LocalVariables;
		}

		// Token: 0x020004C3 RID: 1219
		private class MobilePartyComparer : IComparer<MobileParty>
		{
			// Token: 0x06004148 RID: 16712 RVA: 0x0013335C File Offset: 0x0013155C
			public int Compare(MobileParty x, MobileParty y)
			{
				return x.Id.InternalValue.CompareTo(y.Id.InternalValue);
			}
		}
	}
}
