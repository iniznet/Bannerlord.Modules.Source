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
	public class CampaignTickCacheDataStore
	{
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

		internal void InitializeDataCache()
		{
			this._currentFrameMovingArmyLeaderCount = Campaign.Current.MobileParties.Count;
			this._currentTotalMobilePartyCapacity = Campaign.Current.MobileParties.Count;
			this._currentFrameStationaryPartyCount = Campaign.Current.MobileParties.Count;
			this.InitializeCacheArrays();
		}

		private void ParallelCheckExitingSettlements(int startInclusive, int endExclusive)
		{
			for (int i = startInclusive; i < endExclusive; i++)
			{
				Campaign.Current.MobileParties[i].CheckExitingSettlementParallel(ref this._exitingSettlementCount, ref this._exitingSettlementMobilePartyList);
			}
		}

		private void ParallelInitializeCachedPartyVariables(int startInclusive, int endExclusive)
		{
			for (int i = startInclusive; i < endExclusive; i++)
			{
				MobileParty mobileParty = Campaign.Current.MobileParties[i];
				this._cacheData[i].MobileParty = mobileParty;
				mobileParty.InitializeCachedPartyVariables(ref this._cacheData[i].LocalVariables);
			}
		}

		private void ParallelCacheTargetPartyVariablesAtFrameStart(int startInclusive, int endExclusive)
		{
			for (int i = startInclusive; i < endExclusive; i++)
			{
				this._cacheData[i].MobileParty.Ai.CacheTargetPartyVariablesAtFrameStart(ref this._cacheData[i].LocalVariables);
			}
		}

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

		private void ParallelTickSettlementVisuals(int startInclusive, int endExclusive)
		{
			for (int i = startInclusive; i < endExclusive; i++)
			{
				Settlement.All[i].Party.VisualTick(this._currentRealDt, this._currentDt, ref this._dirtyPartyVisualCount, ref this._dirtyPartiesList);
			}
		}

		private void ParallelTickMobilePartyVisuals(int startInclusive, int endExclusive)
		{
			for (int i = startInclusive; i < endExclusive; i++)
			{
				Campaign.Current.MobileParties[i].Party.VisualTick(this._currentRealDt, this._currentDt, ref this._dirtyPartyVisualCount, ref this._dirtyPartiesList);
			}
		}

		internal void Tick()
		{
			TWParallel.For(0, Campaign.Current.MobileParties.Count, this._parallelCheckExitingSettlementsPredicate, 16);
			Array.Sort<MobileParty>(this._exitingSettlementMobilePartyList, 0, this._exitingSettlementCount + 1, this._mobilePartyComparer);
			for (int i = 0; i < this._exitingSettlementCount + 1; i++)
			{
				LeaveSettlementAction.ApplyForParty(this._exitingSettlementMobilePartyList[i]);
			}
		}

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

		internal void RegisterFadingVisual(IPartyVisual visual)
		{
			if (!this._fadingPartiesSet.Contains(visual))
			{
				this._fadingPartiesFlatten.Add(visual);
				this._fadingPartiesSet.Add(visual);
			}
		}

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

		private CampaignTickCacheDataStore.PartyTickCachePerParty[] _cacheData;

		private MobileParty[] _gridChangeMobilePartyList;

		private MobileParty[] _exitingSettlementMobilePartyList;

		private PartyBase[] _dirtyPartiesList;

		private int[] _movingPartyIndices;

		private int _currentFrameMovingPartyCount;

		private int[] _stationaryPartyIndices;

		private int _currentFrameStationaryPartyCount;

		private int[] _movingArmyLeaderPartyIndices;

		private int _currentFrameMovingArmyLeaderCount;

		private int _currentTotalMobilePartyCapacity;

		private int _gridChangeCount;

		private int _exitingSettlementCount;

		private int _dirtyPartyVisualCount;

		private float _currentDt;

		private float _currentRealDt;

		private readonly TWParallel.ParallelForAuxPredicate _parallelInitializeCachedPartyVariablesPredicate;

		private readonly TWParallel.ParallelForAuxPredicate _parallelCacheTargetPartyVariablesAtFrameStartPredicate;

		private readonly TWParallel.ParallelForAuxPredicate _parallelArrangePartyIndicesPredicate;

		private readonly TWParallel.ParallelForAuxPredicate _parallelTickArmiesPredicate;

		private readonly TWParallel.ParallelForAuxPredicate _parallelTickMovingPartiesPredicate;

		private readonly TWParallel.ParallelForAuxPredicate _parallelTickStationaryPartiesPredicate;

		private readonly TWParallel.ParallelForAuxPredicate _parallelTickSettlementVisualsPredicate;

		private readonly TWParallel.ParallelForAuxPredicate _parallelTickMobilePartyVisualsPredicate;

		private readonly TWParallel.ParallelForAuxPredicate _parallelCheckExitingSettlementsPredicate;

		private readonly List<IPartyVisual> _fadingPartiesFlatten;

		private readonly CampaignTickCacheDataStore.MobilePartyComparer _mobilePartyComparer;

		private readonly HashSet<IPartyVisual> _fadingPartiesSet;

		private struct PartyTickCachePerParty
		{
			internal MobileParty MobileParty;

			internal MobileParty.CachedPartyVariables LocalVariables;
		}

		private class MobilePartyComparer : IComparer<MobileParty>
		{
			public int Compare(MobileParty x, MobileParty y)
			{
				return x.Id.InternalValue.CompareTo(y.Id.InternalValue);
			}
		}
	}
}
