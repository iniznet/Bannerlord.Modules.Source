using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.MapSiege
{
	// Token: 0x02000030 RID: 48
	public class MapSiegeProductionVM : ViewModel
	{
		// Token: 0x1700012B RID: 299
		// (get) Token: 0x060003B2 RID: 946 RVA: 0x00011500 File Offset: 0x0000F700
		private SiegeEvent Siege
		{
			get
			{
				return PlayerSiege.PlayerSiegeEvent;
			}
		}

		// Token: 0x1700012C RID: 300
		// (get) Token: 0x060003B3 RID: 947 RVA: 0x00011507 File Offset: 0x0000F707
		private BattleSideEnum PlayerSide
		{
			get
			{
				return PlayerSiege.PlayerSide;
			}
		}

		// Token: 0x1700012D RID: 301
		// (get) Token: 0x060003B4 RID: 948 RVA: 0x0001150E File Offset: 0x0000F70E
		private Settlement Settlement
		{
			get
			{
				return this.Siege.BesiegedSettlement;
			}
		}

		// Token: 0x1700012E RID: 302
		// (get) Token: 0x060003B5 RID: 949 RVA: 0x0001151B File Offset: 0x0000F71B
		// (set) Token: 0x060003B6 RID: 950 RVA: 0x00011523 File Offset: 0x0000F723
		public MapSiegePOIVM LatestSelectedPOI { get; private set; }

		// Token: 0x060003B7 RID: 951 RVA: 0x0001152C File Offset: 0x0000F72C
		public MapSiegeProductionVM()
		{
			this.PossibleProductionMachines = new MBBindingList<MapSiegeProductionMachineVM>();
		}

		// Token: 0x060003B8 RID: 952 RVA: 0x0001153F File Offset: 0x0000F73F
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.PossibleProductionMachines.ApplyActionOnAllItems(delegate(MapSiegeProductionMachineVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x060003B9 RID: 953 RVA: 0x00011574 File Offset: 0x0000F774
		public void Update()
		{
			if (this.IsEnabled && this.LatestSelectedPOI.Machine == null)
			{
				if (this.PossibleProductionMachines.Any((MapSiegeProductionMachineVM m) => m.IsReserveOption))
				{
					this.ExecuteDisable();
				}
			}
		}

		// Token: 0x060003BA RID: 954 RVA: 0x000115C8 File Offset: 0x0000F7C8
		public void OnMachineSelection(MapSiegePOIVM poi)
		{
			this.PossibleProductionMachines.Clear();
			this.LatestSelectedPOI = poi;
			MapSiegePOIVM latestSelectedPOI = this.LatestSelectedPOI;
			if (((latestSelectedPOI != null) ? latestSelectedPOI.Machine : null) != null)
			{
				this.PossibleProductionMachines.Add(new MapSiegeProductionMachineVM(new Action<MapSiegeProductionMachineVM>(this.OnPossibleMachineSelection), !this.LatestSelectedPOI.Machine.IsConstructed));
			}
			else
			{
				IEnumerable<SiegeEngineType> enumerable;
				switch (poi.Type)
				{
				case MapSiegePOIVM.POIType.DefenderSiegeMachine:
					enumerable = this.GetAllDefenderMachines();
					goto IL_A9;
				case MapSiegePOIVM.POIType.AttackerRamSiegeMachine:
					enumerable = this.GetAllAttackerRamMachines();
					goto IL_A9;
				case MapSiegePOIVM.POIType.AttackerTowerSiegeMachine:
					enumerable = this.GetAllAttackerTowerMachines();
					goto IL_A9;
				case MapSiegePOIVM.POIType.AttackerRangedSiegeMachine:
					enumerable = this.GetAllAttackerRangedMachines();
					goto IL_A9;
				}
				this.IsEnabled = false;
				return;
				IL_A9:
				using (IEnumerator<SiegeEngineType> enumerator = enumerable.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						SiegeEngineType desMachine = enumerator.Current;
						int num = this.Siege.GetSiegeEventSide(this.PlayerSide).SiegeEngines.ReservedSiegeEngines.Count((SiegeEvent.SiegeEngineConstructionProgress m) => m.SiegeEngine == desMachine);
						this.PossibleProductionMachines.Add(new MapSiegeProductionMachineVM(desMachine, num, new Action<MapSiegeProductionMachineVM>(this.OnPossibleMachineSelection)));
					}
				}
			}
			this.IsEnabled = true;
		}

		// Token: 0x060003BB RID: 955 RVA: 0x00011718 File Offset: 0x0000F918
		private void OnPossibleMachineSelection(MapSiegeProductionMachineVM machine)
		{
			if (this.LatestSelectedPOI.Machine == null || this.LatestSelectedPOI.Machine.SiegeEngine != machine.Engine)
			{
				ISiegeEventSide siegeEventSide = this.Siege.GetSiegeEventSide(this.PlayerSide);
				if (machine.IsReserveOption && this.LatestSelectedPOI.Machine != null)
				{
					bool isConstructed = this.LatestSelectedPOI.Machine.IsConstructed;
					siegeEventSide.SiegeEngines.RemoveDeployedSiegeEngine(this.LatestSelectedPOI.MachineIndex, this.LatestSelectedPOI.Machine.SiegeEngine.IsRanged, isConstructed);
				}
				else
				{
					SiegeEvent.SiegeEngineConstructionProgress siegeEngineConstructionProgress = siegeEventSide.SiegeEngines.ReservedSiegeEngines.FirstOrDefault((SiegeEvent.SiegeEngineConstructionProgress e) => e.SiegeEngine == machine.Engine);
					if (siegeEngineConstructionProgress == null)
					{
						float siegeEngineHitPoints = Campaign.Current.Models.SiegeEventModel.GetSiegeEngineHitPoints(PlayerSiege.PlayerSiegeEvent, machine.Engine, this.PlayerSide);
						siegeEngineConstructionProgress = new SiegeEvent.SiegeEngineConstructionProgress(machine.Engine, 0f, siegeEngineHitPoints);
					}
					if (siegeEventSide.SiegeStrategy != DefaultSiegeStrategies.Custom)
					{
						siegeEventSide.SetSiegeStrategy(DefaultSiegeStrategies.Custom);
					}
					siegeEventSide.SiegeEngines.DeploySiegeEngineAtIndex(siegeEngineConstructionProgress, this.LatestSelectedPOI.MachineIndex);
				}
				this.Siege.BesiegedSettlement.Party.Visuals.SetMapIconAsDirty();
				Game.Current.EventManager.TriggerEvent<PlayerStartEngineConstructionEvent>(new PlayerStartEngineConstructionEvent(machine.Engine));
			}
			this.IsEnabled = false;
		}

		// Token: 0x060003BC RID: 956 RVA: 0x000118A3 File Offset: 0x0000FAA3
		public void ExecuteDisable()
		{
			this.IsEnabled = false;
		}

		// Token: 0x060003BD RID: 957 RVA: 0x000118AC File Offset: 0x0000FAAC
		private IEnumerable<SiegeEngineType> GetAllDefenderMachines()
		{
			return Campaign.Current.Models.SiegeEventModel.GetAvailableDefenderSiegeEngines(PartyBase.MainParty);
		}

		// Token: 0x060003BE RID: 958 RVA: 0x000118C7 File Offset: 0x0000FAC7
		private IEnumerable<SiegeEngineType> GetAllAttackerRangedMachines()
		{
			return Campaign.Current.Models.SiegeEventModel.GetAvailableAttackerRangedSiegeEngines(PartyBase.MainParty);
		}

		// Token: 0x060003BF RID: 959 RVA: 0x000118E2 File Offset: 0x0000FAE2
		private IEnumerable<SiegeEngineType> GetAllAttackerRamMachines()
		{
			return Campaign.Current.Models.SiegeEventModel.GetAvailableAttackerRamSiegeEngines(PartyBase.MainParty);
		}

		// Token: 0x060003C0 RID: 960 RVA: 0x000118FD File Offset: 0x0000FAFD
		private IEnumerable<SiegeEngineType> GetAllAttackerTowerMachines()
		{
			return Campaign.Current.Models.SiegeEventModel.GetAvailableAttackerTowerSiegeEngines(PartyBase.MainParty);
		}

		// Token: 0x1700012F RID: 303
		// (get) Token: 0x060003C1 RID: 961 RVA: 0x00011918 File Offset: 0x0000FB18
		// (set) Token: 0x060003C2 RID: 962 RVA: 0x00011920 File Offset: 0x0000FB20
		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		// Token: 0x17000130 RID: 304
		// (get) Token: 0x060003C3 RID: 963 RVA: 0x0001193E File Offset: 0x0000FB3E
		// (set) Token: 0x060003C4 RID: 964 RVA: 0x00011946 File Offset: 0x0000FB46
		[DataSourceProperty]
		public MBBindingList<MapSiegeProductionMachineVM> PossibleProductionMachines
		{
			get
			{
				return this._possibleProductionMachines;
			}
			set
			{
				if (value != this._possibleProductionMachines)
				{
					this._possibleProductionMachines = value;
					base.OnPropertyChangedWithValue<MBBindingList<MapSiegeProductionMachineVM>>(value, "PossibleProductionMachines");
				}
			}
		}

		// Token: 0x040001F8 RID: 504
		private MBBindingList<MapSiegeProductionMachineVM> _possibleProductionMachines;

		// Token: 0x040001F9 RID: 505
		private bool _isEnabled;
	}
}
