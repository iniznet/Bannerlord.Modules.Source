using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.MapSiege
{
	public class MapSiegeVM : ViewModel
	{
		private SiegeEvent Siege
		{
			get
			{
				return PlayerSiege.PlayerSiegeEvent;
			}
		}

		private BattleSideEnum PlayerSide
		{
			get
			{
				return PlayerSiege.PlayerSide;
			}
		}

		private Settlement Settlement
		{
			get
			{
				return this.Siege.BesiegedSettlement;
			}
		}

		public MapSiegeVM(Camera mapCamera)
		{
			this._mapCamera = mapCamera;
			this.PointsOfInterest = new MBBindingList<MapSiegePOIVM>();
			this._poiDistanceComparer = new MapSiegeVM.SiegePOIDistanceComparer();
			for (int i = 0; i < this.Settlement.Party.Visuals.GetAttackerBatteringRamSiegeEngineFrameCount(); i++)
			{
				MatrixFrame attackerBatteringRamSiegeEngineFrameAtIndex = this.Settlement.Party.Visuals.GetAttackerBatteringRamSiegeEngineFrameAtIndex(i);
				this.PointsOfInterest.Add(new MapSiegePOIVM(MapSiegePOIVM.POIType.AttackerRamSiegeMachine, attackerBatteringRamSiegeEngineFrameAtIndex, this._mapCamera, i, new Action<MapSiegePOIVM>(this.OnPOISelection)));
			}
			for (int j = 0; j < this.Settlement.Party.Visuals.GetAttackerRangedSiegeEngineFrameCount(); j++)
			{
				MatrixFrame attackerRangedSiegeEngineFrameAtIndex = this.Settlement.Party.Visuals.GetAttackerRangedSiegeEngineFrameAtIndex(j);
				this.PointsOfInterest.Add(new MapSiegePOIVM(MapSiegePOIVM.POIType.AttackerRangedSiegeMachine, attackerRangedSiegeEngineFrameAtIndex, this._mapCamera, j, new Action<MapSiegePOIVM>(this.OnPOISelection)));
			}
			for (int k = 0; k < this.Settlement.Party.Visuals.GetAttackerTowerSiegeEngineFrameCount(); k++)
			{
				MatrixFrame attackerTowerSiegeEngineFrameAtIndex = this.Settlement.Party.Visuals.GetAttackerTowerSiegeEngineFrameAtIndex(k);
				this.PointsOfInterest.Add(new MapSiegePOIVM(MapSiegePOIVM.POIType.AttackerTowerSiegeMachine, attackerTowerSiegeEngineFrameAtIndex, this._mapCamera, this.Settlement.Party.Visuals.GetAttackerBatteringRamSiegeEngineFrameCount() + k, new Action<MapSiegePOIVM>(this.OnPOISelection)));
			}
			for (int l = 0; l < this.Settlement.Party.Visuals.GetDefenderSiegeEngineFrameCount(); l++)
			{
				MatrixFrame defenderSiegeEngineFrameAtIndex = this.Settlement.Party.Visuals.GetDefenderSiegeEngineFrameAtIndex(l);
				this.PointsOfInterest.Add(new MapSiegePOIVM(MapSiegePOIVM.POIType.DefenderSiegeMachine, defenderSiegeEngineFrameAtIndex, this._mapCamera, l, new Action<MapSiegePOIVM>(this.OnPOISelection)));
			}
			for (int m = 0; m < this.Settlement.Party.Visuals.GetBreacableWallFrameCount(); m++)
			{
				MatrixFrame breacableWallFrameAtIndex = this.Settlement.Party.Visuals.GetBreacableWallFrameAtIndex(m);
				this.PointsOfInterest.Add(new MapSiegePOIVM(MapSiegePOIVM.POIType.WallSection, breacableWallFrameAtIndex, this._mapCamera, m, new Action<MapSiegePOIVM>(this.OnPOISelection)));
			}
			this.ProductionController = new MapSiegeProductionVM();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.PreparationTitleText = GameTexts.FindText("str_building_siege_camp", null).ToString();
			this.ProductionController.RefreshValues();
			this.PointsOfInterest.ApplyActionOnAllItems(delegate(MapSiegePOIVM x)
			{
				x.RefreshValues();
			});
		}

		private void OnPOISelection(MapSiegePOIVM poi)
		{
			if (this.ProductionController.LatestSelectedPOI != null)
			{
				this.ProductionController.LatestSelectedPOI.IsSelected = false;
			}
			SiegeEvent siege = this.Siege;
			if (siege != null && siege.IsPlayerSiegeEvent)
			{
				BesiegerCamp besiegerCamp = this.Siege.BesiegerCamp;
				bool flag;
				if (besiegerCamp == null)
				{
					flag = false;
				}
				else
				{
					MobileParty besiegerParty = besiegerCamp.BesiegerParty;
					bool? flag2 = ((besiegerParty != null) ? new bool?(besiegerParty.IsMainParty) : null);
					bool flag3 = true;
					flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
				}
				if (flag)
				{
					this.ProductionController.OnMachineSelection(poi);
				}
			}
		}

		public void OnSelectionFromScene(MatrixFrame frameOfEngine)
		{
			SiegeEvent siege = this.Siege;
			if (siege != null && siege.IsPlayerSiegeEvent)
			{
				BesiegerCamp besiegerCamp = this.Siege.BesiegerCamp;
				bool flag;
				if (besiegerCamp == null)
				{
					flag = false;
				}
				else
				{
					MobileParty besiegerParty = besiegerCamp.BesiegerParty;
					bool? flag2 = ((besiegerParty != null) ? new bool?(besiegerParty.IsMainParty) : null);
					bool flag3 = true;
					flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
				}
				if (flag)
				{
					IEnumerable<MapSiegePOIVM> enumerable = this.PointsOfInterest.Where((MapSiegePOIVM poi) => frameOfEngine.NearlyEquals(poi.MapSceneLocationFrame, 1E-05f));
					if (enumerable == null)
					{
						return;
					}
					MapSiegePOIVM mapSiegePOIVM = enumerable.FirstOrDefault<MapSiegePOIVM>();
					if (mapSiegePOIVM == null)
					{
						return;
					}
					mapSiegePOIVM.ExecuteSelection();
				}
			}
		}

		public void Update(float mapCameraDistanceValue)
		{
			SiegeEvent siege = this.Siege;
			this.IsPreparationsCompleted = (siege != null && siege.BesiegerCamp.IsPreparationComplete) || PlayerSiege.PlayerSide == 0;
			SiegeEvent siege2 = this.Siege;
			float? num;
			if (siege2 == null)
			{
				num = null;
			}
			else
			{
				SiegeEvent.SiegeEnginesContainer siegeEngines = siege2.BesiegerCamp.SiegeEngines;
				if (siegeEngines == null)
				{
					num = null;
				}
				else
				{
					SiegeEvent.SiegeEngineConstructionProgress siegePreparations = siegeEngines.SiegePreparations;
					num = ((siegePreparations != null) ? new float?(siegePreparations.Progress) : null);
				}
			}
			this.PreparationProgress = num ?? 0f;
			TWParallel.For(0, this.PointsOfInterest.Count, delegate(int startInclusive, int endExclusive)
			{
				for (int i = startInclusive; i < endExclusive; i++)
				{
					this.PointsOfInterest[i].RefreshDistanceValue(mapCameraDistanceValue);
					this.PointsOfInterest[i].RefreshPosition();
					this.PointsOfInterest[i].UpdateProperties();
				}
			}, 16);
			foreach (MapSiegePOIVM mapSiegePOIVM in this.PointsOfInterest)
			{
				mapSiegePOIVM.RefreshBinding();
			}
			this.ProductionController.Update();
			this.PointsOfInterest.Sort(this._poiDistanceComparer);
		}

		[DataSourceProperty]
		public float PreparationProgress
		{
			get
			{
				return this._preparationProgress;
			}
			set
			{
				if (value != this._preparationProgress)
				{
					this._preparationProgress = value;
					base.OnPropertyChangedWithValue(value, "PreparationProgress");
				}
			}
		}

		[DataSourceProperty]
		public bool IsPreparationsCompleted
		{
			get
			{
				return this._isPreparationsCompleted;
			}
			set
			{
				if (value != this._isPreparationsCompleted)
				{
					this._isPreparationsCompleted = value;
					base.OnPropertyChangedWithValue(value, "IsPreparationsCompleted");
				}
			}
		}

		[DataSourceProperty]
		public string PreparationTitleText
		{
			get
			{
				return this._preparationTitleText;
			}
			set
			{
				if (value != this._preparationTitleText)
				{
					this._preparationTitleText = value;
					base.OnPropertyChangedWithValue<string>(value, "PreparationTitleText");
				}
			}
		}

		[DataSourceProperty]
		public MapSiegeProductionVM ProductionController
		{
			get
			{
				return this._productionController;
			}
			set
			{
				if (value != this._productionController)
				{
					this._productionController = value;
					base.OnPropertyChangedWithValue<MapSiegeProductionVM>(value, "ProductionController");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<MapSiegePOIVM> PointsOfInterest
		{
			get
			{
				return this._pointsOfInterest;
			}
			set
			{
				if (value != this._pointsOfInterest)
				{
					this._pointsOfInterest = value;
					base.OnPropertyChangedWithValue<MBBindingList<MapSiegePOIVM>>(value, "PointsOfInterest");
				}
			}
		}

		private readonly Camera _mapCamera;

		private readonly MapSiegeVM.SiegePOIDistanceComparer _poiDistanceComparer;

		private MBBindingList<MapSiegePOIVM> _pointsOfInterest;

		private MapSiegeProductionVM _productionController;

		private float _preparationProgress;

		private string _preparationTitleText;

		private bool _isPreparationsCompleted;

		public class SiegePOIDistanceComparer : IComparer<MapSiegePOIVM>
		{
			public int Compare(MapSiegePOIVM x, MapSiegePOIVM y)
			{
				return y.LatestW.CompareTo(x.LatestW);
			}
		}
	}
}
