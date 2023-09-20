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
	// Token: 0x02000033 RID: 51
	public class MapSiegeVM : ViewModel
	{
		// Token: 0x17000138 RID: 312
		// (get) Token: 0x060003D9 RID: 985 RVA: 0x00011B3A File Offset: 0x0000FD3A
		private SiegeEvent Siege
		{
			get
			{
				return PlayerSiege.PlayerSiegeEvent;
			}
		}

		// Token: 0x17000139 RID: 313
		// (get) Token: 0x060003DA RID: 986 RVA: 0x00011B41 File Offset: 0x0000FD41
		private BattleSideEnum PlayerSide
		{
			get
			{
				return PlayerSiege.PlayerSide;
			}
		}

		// Token: 0x1700013A RID: 314
		// (get) Token: 0x060003DB RID: 987 RVA: 0x00011B48 File Offset: 0x0000FD48
		private Settlement Settlement
		{
			get
			{
				return this.Siege.BesiegedSettlement;
			}
		}

		// Token: 0x060003DC RID: 988 RVA: 0x00011B58 File Offset: 0x0000FD58
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

		// Token: 0x060003DD RID: 989 RVA: 0x00011D94 File Offset: 0x0000FF94
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

		// Token: 0x060003DE RID: 990 RVA: 0x00011DF4 File Offset: 0x0000FFF4
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

		// Token: 0x060003DF RID: 991 RVA: 0x00011E88 File Offset: 0x00010088
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

		// Token: 0x060003E0 RID: 992 RVA: 0x00011F28 File Offset: 0x00010128
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

		// Token: 0x1700013B RID: 315
		// (get) Token: 0x060003E1 RID: 993 RVA: 0x00012058 File Offset: 0x00010258
		// (set) Token: 0x060003E2 RID: 994 RVA: 0x00012060 File Offset: 0x00010260
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

		// Token: 0x1700013C RID: 316
		// (get) Token: 0x060003E3 RID: 995 RVA: 0x0001207E File Offset: 0x0001027E
		// (set) Token: 0x060003E4 RID: 996 RVA: 0x00012086 File Offset: 0x00010286
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

		// Token: 0x1700013D RID: 317
		// (get) Token: 0x060003E5 RID: 997 RVA: 0x000120A4 File Offset: 0x000102A4
		// (set) Token: 0x060003E6 RID: 998 RVA: 0x000120AC File Offset: 0x000102AC
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

		// Token: 0x1700013E RID: 318
		// (get) Token: 0x060003E7 RID: 999 RVA: 0x000120CF File Offset: 0x000102CF
		// (set) Token: 0x060003E8 RID: 1000 RVA: 0x000120D7 File Offset: 0x000102D7
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

		// Token: 0x1700013F RID: 319
		// (get) Token: 0x060003E9 RID: 1001 RVA: 0x000120F5 File Offset: 0x000102F5
		// (set) Token: 0x060003EA RID: 1002 RVA: 0x000120FD File Offset: 0x000102FD
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

		// Token: 0x04000203 RID: 515
		private readonly Camera _mapCamera;

		// Token: 0x04000204 RID: 516
		private readonly MapSiegeVM.SiegePOIDistanceComparer _poiDistanceComparer;

		// Token: 0x04000205 RID: 517
		private MBBindingList<MapSiegePOIVM> _pointsOfInterest;

		// Token: 0x04000206 RID: 518
		private MapSiegeProductionVM _productionController;

		// Token: 0x04000207 RID: 519
		private float _preparationProgress;

		// Token: 0x04000208 RID: 520
		private string _preparationTitleText;

		// Token: 0x04000209 RID: 521
		private bool _isPreparationsCompleted;

		// Token: 0x02000097 RID: 151
		public class SiegePOIDistanceComparer : IComparer<MapSiegePOIVM>
		{
			// Token: 0x06000570 RID: 1392 RVA: 0x00014730 File Offset: 0x00012930
			public int Compare(MapSiegePOIVM x, MapSiegePOIVM y)
			{
				return y.LatestW.CompareTo(x.LatestW);
			}
		}
	}
}
