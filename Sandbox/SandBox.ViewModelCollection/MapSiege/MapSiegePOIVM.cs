using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.ViewModelCollection.MapSiege
{
	// Token: 0x0200002F RID: 47
	public class MapSiegePOIVM : ViewModel
	{
		// Token: 0x17000115 RID: 277
		// (get) Token: 0x0600037D RID: 893 RVA: 0x00010A77 File Offset: 0x0000EC77
		private SiegeEvent Siege
		{
			get
			{
				return PlayerSiege.PlayerSiegeEvent;
			}
		}

		// Token: 0x17000116 RID: 278
		// (get) Token: 0x0600037E RID: 894 RVA: 0x00010A7E File Offset: 0x0000EC7E
		private BattleSideEnum PlayerSide
		{
			get
			{
				return PlayerSiege.PlayerSide;
			}
		}

		// Token: 0x17000117 RID: 279
		// (get) Token: 0x0600037F RID: 895 RVA: 0x00010A85 File Offset: 0x0000EC85
		private Settlement Settlement
		{
			get
			{
				return this.Siege.BesiegedSettlement;
			}
		}

		// Token: 0x17000118 RID: 280
		// (get) Token: 0x06000380 RID: 896 RVA: 0x00010A92 File Offset: 0x0000EC92
		public MapSiegePOIVM.POIType Type { get; }

		// Token: 0x17000119 RID: 281
		// (get) Token: 0x06000381 RID: 897 RVA: 0x00010A9A File Offset: 0x0000EC9A
		public int MachineIndex { get; }

		// Token: 0x1700011A RID: 282
		// (get) Token: 0x06000382 RID: 898 RVA: 0x00010AA2 File Offset: 0x0000ECA2
		public float LatestW
		{
			get
			{
				return this._latestW;
			}
		}

		// Token: 0x1700011B RID: 283
		// (get) Token: 0x06000383 RID: 899 RVA: 0x00010AAA File Offset: 0x0000ECAA
		// (set) Token: 0x06000384 RID: 900 RVA: 0x00010AB2 File Offset: 0x0000ECB2
		public SiegeEvent.SiegeEngineConstructionProgress Machine { get; private set; }

		// Token: 0x1700011C RID: 284
		// (get) Token: 0x06000385 RID: 901 RVA: 0x00010ABB File Offset: 0x0000ECBB
		// (set) Token: 0x06000386 RID: 902 RVA: 0x00010AC3 File Offset: 0x0000ECC3
		public MatrixFrame MapSceneLocationFrame { get; private set; }

		// Token: 0x06000387 RID: 903 RVA: 0x00010ACC File Offset: 0x0000ECCC
		public MapSiegePOIVM(MapSiegePOIVM.POIType type, MatrixFrame mapSceneLocation, Camera mapCamera, int machineIndex, Action<MapSiegePOIVM> onSelection)
		{
			this.Type = type;
			this._onSelection = onSelection;
			this._thisSide = ((this.Type == MapSiegePOIVM.POIType.AttackerRamSiegeMachine || this.Type == MapSiegePOIVM.POIType.AttackerTowerSiegeMachine || this.Type == MapSiegePOIVM.POIType.AttackerRangedSiegeMachine) ? 1 : 0);
			this.MapSceneLocationFrame = mapSceneLocation;
			this._mapSceneLocation = this.MapSceneLocationFrame.origin;
			this._mapCamera = mapCamera;
			this.MachineIndex = machineIndex;
			Color color;
			if (this._thisSide != 1)
			{
				IFaction mapFaction = this.Siege.BesiegedSettlement.MapFaction;
				color = Color.FromUint((mapFaction != null) ? mapFaction.Color : 0U);
			}
			else
			{
				IFaction mapFaction2 = this.Siege.BesiegerCamp.BesiegerParty.MapFaction;
				color = Color.FromUint((mapFaction2 != null) ? mapFaction2.Color : 0U);
			}
			this.SidePrimaryColor = color;
			Color color2;
			if (this._thisSide != 1)
			{
				IFaction mapFaction3 = this.Siege.BesiegedSettlement.MapFaction;
				color2 = Color.FromUint((mapFaction3 != null) ? mapFaction3.Color2 : 0U);
			}
			else
			{
				IFaction mapFaction4 = this.Siege.BesiegerCamp.BesiegerParty.MapFaction;
				color2 = Color.FromUint((mapFaction4 != null) ? mapFaction4.Color2 : 0U);
			}
			this.SideSecondaryColor = color2;
			this.IsPlayerSidePOI = this.DetermineIfPOIIsPlayerSide();
		}

		// Token: 0x06000388 RID: 904 RVA: 0x00010C04 File Offset: 0x0000EE04
		public void ExecuteSelection()
		{
			this._onSelection(this);
			this.IsSelected = true;
		}

		// Token: 0x06000389 RID: 905 RVA: 0x00010C1C File Offset: 0x0000EE1C
		public void UpdateProperties()
		{
			this.Machine = this.GetDesiredMachine();
			this._bindHasItem = this.Type == MapSiegePOIVM.POIType.WallSection || this.Machine != null;
			SiegeEvent.SiegeEngineConstructionProgress machine = this.Machine;
			this._bindIsConstructing = machine != null && !machine.IsConstructed;
			this.RefreshMachineType();
			this.RefreshHitpoints();
			this.RefreshQueueIndex();
		}

		// Token: 0x0600038A RID: 906 RVA: 0x00010C7C File Offset: 0x0000EE7C
		public void RefreshDistanceValue(float newDistance)
		{
			this._bindIsInVisibleRange = newDistance <= 20f;
		}

		// Token: 0x0600038B RID: 907 RVA: 0x00010C90 File Offset: 0x0000EE90
		public void RefreshPosition()
		{
			this._latestX = 0f;
			this._latestY = 0f;
			this._latestW = 0f;
			MBWindowManager.WorldToScreenInsideUsableArea(this._mapCamera, this._mapSceneLocation, ref this._latestX, ref this._latestY, ref this._latestW);
			this._bindWPos = this._latestW;
			this._bindWSign = (int)this._bindWPos;
			this._bindIsInside = this.IsInsideWindow();
			if (!this._bindIsInside)
			{
				this._bindPosition = new Vec2(-1000f, -1000f);
				return;
			}
			this._bindPosition = new Vec2(this._latestX, this._latestY);
		}

		// Token: 0x0600038C RID: 908 RVA: 0x00010D3C File Offset: 0x0000EF3C
		public void RefreshBinding()
		{
			this.Position = this._bindPosition;
			this.IsInside = this._bindIsInside;
			this.CurrentHitpoints = this._bindCurrentHitpoints;
			this.MaxHitpoints = this._bindMaxHitpoints;
			this.HasItem = this._bindHasItem;
			this.IsConstructing = this._bindIsConstructing;
			this.MachineType = this._bindMachineType;
			this.QueueIndex = this._bindQueueIndex;
			this.IsInVisibleRange = this._bindIsInVisibleRange;
		}

		// Token: 0x0600038D RID: 909 RVA: 0x00010DB8 File Offset: 0x0000EFB8
		private void RefreshHitpoints()
		{
			if (this.Siege == null)
			{
				this._bindCurrentHitpoints = 0f;
				this._bindMaxHitpoints = 0f;
				return;
			}
			MapSiegePOIVM.POIType type = this.Type;
			if (type == MapSiegePOIVM.POIType.WallSection)
			{
				MBReadOnlyList<float> settlementWallSectionHitPointsRatioList = this.Settlement.SettlementWallSectionHitPointsRatioList;
				this._bindMaxHitpoints = this.Settlement.MaxWallHitPoints / (float)this.Settlement.Party.Visuals.GetBreacableWallFrameCount();
				this._bindCurrentHitpoints = settlementWallSectionHitPointsRatioList[this.MachineIndex] * this._bindMaxHitpoints;
				this._bindMachineType = ((this._bindCurrentHitpoints <= 0f) ? 1 : 0);
				return;
			}
			if (type - MapSiegePOIVM.POIType.DefenderSiegeMachine > 3)
			{
				return;
			}
			this._bindCurrentHitpoints = ((this.Machine != null) ? (this.Machine.IsConstructed ? this.Machine.Hitpoints : this.Machine.Progress) : 0f);
			this._bindMaxHitpoints = ((this.Machine != null) ? (this.Machine.IsConstructed ? this.Machine.MaxHitPoints : 1f) : 0f);
		}

		// Token: 0x0600038E RID: 910 RVA: 0x00010ECC File Offset: 0x0000F0CC
		private void RefreshMachineType()
		{
			if (this.Siege == null)
			{
				this._bindMachineType = -1;
				return;
			}
			MapSiegePOIVM.POIType type = this.Type;
			if (type == MapSiegePOIVM.POIType.WallSection)
			{
				this._bindMachineType = 0;
				return;
			}
			if (type - MapSiegePOIVM.POIType.DefenderSiegeMachine > 3)
			{
				return;
			}
			this._bindMachineType = (int)((this.Machine != null) ? this.GetMachineTypeFromId(this.Machine.SiegeEngine.StringId) : MapSiegePOIVM.MachineTypes.None);
		}

		// Token: 0x0600038F RID: 911 RVA: 0x00010F2C File Offset: 0x0000F12C
		private void RefreshQueueIndex()
		{
			int num;
			if (this.Machine == null)
			{
				num = -1;
			}
			else
			{
				num = this.Siege.GetSiegeEventSide(this.PlayerSide).SiegeEngines.DeployedSiegeEngines.Where((SiegeEvent.SiegeEngineConstructionProgress e) => !e.IsConstructed).ToList<SiegeEvent.SiegeEngineConstructionProgress>().IndexOf(this.Machine);
			}
			this._bindQueueIndex = num;
		}

		// Token: 0x06000390 RID: 912 RVA: 0x00010F9C File Offset: 0x0000F19C
		private bool DetermineIfPOIIsPlayerSide()
		{
			MapSiegePOIVM.POIType type = this.Type;
			if (type > MapSiegePOIVM.POIType.DefenderSiegeMachine)
			{
				return type - MapSiegePOIVM.POIType.AttackerRamSiegeMachine <= 2 && this.PlayerSide == 1;
			}
			return this.PlayerSide == 0;
		}

		// Token: 0x06000391 RID: 913 RVA: 0x00010FD4 File Offset: 0x0000F1D4
		private bool IsInsideWindow()
		{
			return this._latestX <= Screen.RealScreenResolutionWidth && this._latestY <= Screen.RealScreenResolutionHeight && this._latestX + 200f >= 0f && this._latestY + 100f >= 0f;
		}

		// Token: 0x06000392 RID: 914 RVA: 0x00011026 File Offset: 0x0000F226
		public void ExecuteShowTooltip()
		{
			InformationManager.ShowTooltip(typeof(List<TooltipProperty>), new object[] { SandBoxUIHelper.GetSiegeEngineInProgressTooltip(this.Machine) });
		}

		// Token: 0x06000393 RID: 915 RVA: 0x0001104B File Offset: 0x0000F24B
		public void ExecuteHideTooltip()
		{
			MBInformationManager.HideInformations();
		}

		// Token: 0x06000394 RID: 916 RVA: 0x00011054 File Offset: 0x0000F254
		private MapSiegePOIVM.MachineTypes GetMachineTypeFromId(string id)
		{
			string text = id.ToLower();
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
			if (num > 746114623U)
			{
				if (num <= 1820818168U)
				{
					if (num != 808481256U)
					{
						if (num != 1241455715U)
						{
							if (num != 1820818168U)
							{
								return MapSiegePOIVM.MachineTypes.None;
							}
							if (!(text == "fire_onager"))
							{
								return MapSiegePOIVM.MachineTypes.None;
							}
							return MapSiegePOIVM.MachineTypes.Mangonel;
						}
						else
						{
							if (!(text == "ram"))
							{
								return MapSiegePOIVM.MachineTypes.None;
							}
							return MapSiegePOIVM.MachineTypes.Ram;
						}
					}
					else if (!(text == "fire_ballista"))
					{
						return MapSiegePOIVM.MachineTypes.None;
					}
				}
				else if (num <= 1898442385U)
				{
					if (num != 1839032341U)
					{
						if (num != 1898442385U)
						{
							return MapSiegePOIVM.MachineTypes.None;
						}
						if (!(text == "catapult"))
						{
							return MapSiegePOIVM.MachineTypes.None;
						}
						return MapSiegePOIVM.MachineTypes.Mangonel;
					}
					else
					{
						if (!(text == "trebuchet"))
						{
							return MapSiegePOIVM.MachineTypes.None;
						}
						return MapSiegePOIVM.MachineTypes.Trebuchet;
					}
				}
				else if (num != 2806198843U)
				{
					if (num != 4036530155U)
					{
						return MapSiegePOIVM.MachineTypes.None;
					}
					if (!(text == "ballista"))
					{
						return MapSiegePOIVM.MachineTypes.None;
					}
				}
				else
				{
					if (!(text == "onager"))
					{
						return MapSiegePOIVM.MachineTypes.None;
					}
					return MapSiegePOIVM.MachineTypes.Mangonel;
				}
				return MapSiegePOIVM.MachineTypes.Ballista;
			}
			if (num > 473034592U)
			{
				if (num <= 712590611U)
				{
					if (num != 695812992U)
					{
						if (num != 712590611U)
						{
							return MapSiegePOIVM.MachineTypes.None;
						}
						if (!(text == "siege_tower_level2"))
						{
							return MapSiegePOIVM.MachineTypes.None;
						}
					}
					else if (!(text == "siege_tower_level3"))
					{
						return MapSiegePOIVM.MachineTypes.None;
					}
				}
				else if (num != 729368230U)
				{
					if (num != 746114623U)
					{
						return MapSiegePOIVM.MachineTypes.None;
					}
					if (!(text == "fire_mangonel"))
					{
						return MapSiegePOIVM.MachineTypes.None;
					}
					return MapSiegePOIVM.MachineTypes.Mangonel;
				}
				else if (!(text == "siege_tower_level1"))
				{
					return MapSiegePOIVM.MachineTypes.None;
				}
				return MapSiegePOIVM.MachineTypes.SiegeTower;
			}
			if (num != 6339497U)
			{
				if (num != 390431385U)
				{
					if (num != 473034592U)
					{
						return MapSiegePOIVM.MachineTypes.None;
					}
					if (!(text == "mangonel"))
					{
						return MapSiegePOIVM.MachineTypes.None;
					}
				}
				else
				{
					if (!(text == "bricole"))
					{
						return MapSiegePOIVM.MachineTypes.None;
					}
					return MapSiegePOIVM.MachineTypes.Trebuchet;
				}
			}
			else
			{
				if (!(text == "ladder"))
				{
					return MapSiegePOIVM.MachineTypes.None;
				}
				return MapSiegePOIVM.MachineTypes.Ladder;
			}
			return MapSiegePOIVM.MachineTypes.Mangonel;
		}

		// Token: 0x06000395 RID: 917 RVA: 0x00011248 File Offset: 0x0000F448
		private SiegeEvent.SiegeEngineConstructionProgress GetDesiredMachine()
		{
			if (this.Siege != null)
			{
				switch (this.Type)
				{
				case MapSiegePOIVM.POIType.DefenderSiegeMachine:
					return this.Siege.GetSiegeEventSide(0).SiegeEngines.DeployedRangedSiegeEngines[this.MachineIndex];
				case MapSiegePOIVM.POIType.AttackerRamSiegeMachine:
				case MapSiegePOIVM.POIType.AttackerTowerSiegeMachine:
					return this.Siege.GetSiegeEventSide(1).SiegeEngines.DeployedMeleeSiegeEngines[this.MachineIndex];
				case MapSiegePOIVM.POIType.AttackerRangedSiegeMachine:
					return this.Siege.GetSiegeEventSide(1).SiegeEngines.DeployedRangedSiegeEngines[this.MachineIndex];
				}
				return null;
			}
			return null;
		}

		// Token: 0x1700011D RID: 285
		// (get) Token: 0x06000396 RID: 918 RVA: 0x000112DD File Offset: 0x0000F4DD
		// (set) Token: 0x06000397 RID: 919 RVA: 0x000112E5 File Offset: 0x0000F4E5
		public Vec2 Position
		{
			get
			{
				return this._position;
			}
			set
			{
				if (this._position != value)
				{
					this._position = value;
					base.OnPropertyChangedWithValue(value, "Position");
				}
			}
		}

		// Token: 0x1700011E RID: 286
		// (get) Token: 0x06000398 RID: 920 RVA: 0x00011308 File Offset: 0x0000F508
		// (set) Token: 0x06000399 RID: 921 RVA: 0x00011310 File Offset: 0x0000F510
		public Color SidePrimaryColor
		{
			get
			{
				return this._sidePrimaryColor;
			}
			set
			{
				if (this._sidePrimaryColor != value)
				{
					this._sidePrimaryColor = value;
					base.OnPropertyChangedWithValue(value, "SidePrimaryColor");
				}
			}
		}

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x0600039A RID: 922 RVA: 0x00011333 File Offset: 0x0000F533
		// (set) Token: 0x0600039B RID: 923 RVA: 0x0001133B File Offset: 0x0000F53B
		public Color SideSecondaryColor
		{
			get
			{
				return this._sideSecondaryColor;
			}
			set
			{
				if (this._sideSecondaryColor != value)
				{
					this._sideSecondaryColor = value;
					base.OnPropertyChangedWithValue(value, "SideSecondaryColor");
				}
			}
		}

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x0600039C RID: 924 RVA: 0x0001135E File Offset: 0x0000F55E
		// (set) Token: 0x0600039D RID: 925 RVA: 0x00011366 File Offset: 0x0000F566
		public int QueueIndex
		{
			get
			{
				return this._queueIndex;
			}
			set
			{
				if (this._queueIndex != value)
				{
					this._queueIndex = value;
					base.OnPropertyChangedWithValue(value, "QueueIndex");
				}
			}
		}

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x0600039E RID: 926 RVA: 0x00011384 File Offset: 0x0000F584
		// (set) Token: 0x0600039F RID: 927 RVA: 0x0001138C File Offset: 0x0000F58C
		public int MachineType
		{
			get
			{
				return this._machineType;
			}
			set
			{
				if (this._machineType != value)
				{
					this._machineType = value;
					base.OnPropertyChangedWithValue(value, "MachineType");
				}
			}
		}

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x060003A0 RID: 928 RVA: 0x000113AA File Offset: 0x0000F5AA
		// (set) Token: 0x060003A1 RID: 929 RVA: 0x000113B2 File Offset: 0x0000F5B2
		public float CurrentHitpoints
		{
			get
			{
				return this._currentHitpoints;
			}
			set
			{
				if (this._currentHitpoints != value)
				{
					this._currentHitpoints = value;
					base.OnPropertyChangedWithValue(value, "CurrentHitpoints");
				}
			}
		}

		// Token: 0x17000123 RID: 291
		// (get) Token: 0x060003A2 RID: 930 RVA: 0x000113D0 File Offset: 0x0000F5D0
		// (set) Token: 0x060003A3 RID: 931 RVA: 0x000113D8 File Offset: 0x0000F5D8
		public float MaxHitpoints
		{
			get
			{
				return this._maxHitpoints;
			}
			set
			{
				if (this._maxHitpoints != value)
				{
					this._maxHitpoints = value;
					base.OnPropertyChangedWithValue(value, "MaxHitpoints");
				}
			}
		}

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x060003A4 RID: 932 RVA: 0x000113F6 File Offset: 0x0000F5F6
		// (set) Token: 0x060003A5 RID: 933 RVA: 0x000113FE File Offset: 0x0000F5FE
		public bool IsPlayerSidePOI
		{
			get
			{
				return this._isPlayerSidePOI;
			}
			set
			{
				if (this._isPlayerSidePOI != value)
				{
					this._isPlayerSidePOI = value;
					base.OnPropertyChangedWithValue(value, "IsPlayerSidePOI");
				}
			}
		}

		// Token: 0x17000125 RID: 293
		// (get) Token: 0x060003A6 RID: 934 RVA: 0x0001141C File Offset: 0x0000F61C
		// (set) Token: 0x060003A7 RID: 935 RVA: 0x00011424 File Offset: 0x0000F624
		public bool IsFireVersion
		{
			get
			{
				return this._isFireVersion;
			}
			set
			{
				if (this._isFireVersion != value)
				{
					this._isFireVersion = value;
					base.OnPropertyChangedWithValue(value, "IsFireVersion");
				}
			}
		}

		// Token: 0x17000126 RID: 294
		// (get) Token: 0x060003A8 RID: 936 RVA: 0x00011442 File Offset: 0x0000F642
		// (set) Token: 0x060003A9 RID: 937 RVA: 0x0001144A File Offset: 0x0000F64A
		public bool IsInVisibleRange
		{
			get
			{
				return this._isInVisibleRange;
			}
			set
			{
				if (this._isInVisibleRange != value)
				{
					this._isInVisibleRange = value;
					base.OnPropertyChangedWithValue(value, "IsInVisibleRange");
				}
			}
		}

		// Token: 0x17000127 RID: 295
		// (get) Token: 0x060003AA RID: 938 RVA: 0x00011468 File Offset: 0x0000F668
		// (set) Token: 0x060003AB RID: 939 RVA: 0x00011470 File Offset: 0x0000F670
		public bool IsConstructing
		{
			get
			{
				return this._isConstructing;
			}
			set
			{
				if (this._isConstructing != value)
				{
					this._isConstructing = value;
					base.OnPropertyChangedWithValue(value, "IsConstructing");
				}
			}
		}

		// Token: 0x17000128 RID: 296
		// (get) Token: 0x060003AC RID: 940 RVA: 0x0001148E File Offset: 0x0000F68E
		// (set) Token: 0x060003AD RID: 941 RVA: 0x00011496 File Offset: 0x0000F696
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (this._isSelected != value)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
				}
			}
		}

		// Token: 0x17000129 RID: 297
		// (get) Token: 0x060003AE RID: 942 RVA: 0x000114B4 File Offset: 0x0000F6B4
		// (set) Token: 0x060003AF RID: 943 RVA: 0x000114BC File Offset: 0x0000F6BC
		public bool HasItem
		{
			get
			{
				return this._hasItem;
			}
			set
			{
				if (this._hasItem != value)
				{
					this._hasItem = value;
					base.OnPropertyChangedWithValue(value, "HasItem");
				}
			}
		}

		// Token: 0x1700012A RID: 298
		// (get) Token: 0x060003B0 RID: 944 RVA: 0x000114DA File Offset: 0x0000F6DA
		// (set) Token: 0x060003B1 RID: 945 RVA: 0x000114E2 File Offset: 0x0000F6E2
		public bool IsInside
		{
			get
			{
				return this._isInside;
			}
			set
			{
				if (this._isInside != value)
				{
					this._isInside = value;
					base.OnPropertyChangedWithValue(value, "IsInside");
				}
			}
		}

		// Token: 0x040001D7 RID: 471
		private readonly Vec3 _mapSceneLocation;

		// Token: 0x040001D8 RID: 472
		private readonly Camera _mapCamera;

		// Token: 0x040001D9 RID: 473
		private readonly BattleSideEnum _thisSide;

		// Token: 0x040001DA RID: 474
		private readonly Action<MapSiegePOIVM> _onSelection;

		// Token: 0x040001DB RID: 475
		private float _latestX;

		// Token: 0x040001DC RID: 476
		private float _latestY;

		// Token: 0x040001DD RID: 477
		private float _latestW;

		// Token: 0x040001DE RID: 478
		private float _bindCurrentHitpoints;

		// Token: 0x040001DF RID: 479
		private float _bindMaxHitpoints;

		// Token: 0x040001E0 RID: 480
		private float _bindWPos;

		// Token: 0x040001E1 RID: 481
		private int _bindWSign;

		// Token: 0x040001E2 RID: 482
		private int _bindMachineType = -1;

		// Token: 0x040001E3 RID: 483
		private int _bindQueueIndex;

		// Token: 0x040001E4 RID: 484
		private bool _bindIsInside;

		// Token: 0x040001E5 RID: 485
		private bool _bindHasItem;

		// Token: 0x040001E6 RID: 486
		private bool _bindIsConstructing;

		// Token: 0x040001E7 RID: 487
		private Vec2 _bindPosition;

		// Token: 0x040001E8 RID: 488
		private bool _bindIsInVisibleRange;

		// Token: 0x040001E9 RID: 489
		private Vec2 _position;

		// Token: 0x040001EA RID: 490
		private bool _isInside;

		// Token: 0x040001EB RID: 491
		private float _currentHitpoints;

		// Token: 0x040001EC RID: 492
		private float _maxHitpoints;

		// Token: 0x040001ED RID: 493
		private bool _hasItem;

		// Token: 0x040001EE RID: 494
		private bool _isConstructing;

		// Token: 0x040001EF RID: 495
		private bool _isFireVersion;

		// Token: 0x040001F0 RID: 496
		private bool _isInVisibleRange;

		// Token: 0x040001F1 RID: 497
		private int _machineType = -1;

		// Token: 0x040001F2 RID: 498
		private Color _sidePrimaryColor;

		// Token: 0x040001F3 RID: 499
		private Color _sideSecondaryColor;

		// Token: 0x040001F4 RID: 500
		private bool _isPlayerSidePOI;

		// Token: 0x040001F5 RID: 501
		private int _queueIndex;

		// Token: 0x040001F6 RID: 502
		private bool _isSelected;

		// Token: 0x02000091 RID: 145
		public enum POIType
		{
			// Token: 0x04000327 RID: 807
			WallSection,
			// Token: 0x04000328 RID: 808
			DefenderSiegeMachine,
			// Token: 0x04000329 RID: 809
			AttackerRamSiegeMachine,
			// Token: 0x0400032A RID: 810
			AttackerTowerSiegeMachine,
			// Token: 0x0400032B RID: 811
			AttackerRangedSiegeMachine
		}

		// Token: 0x02000092 RID: 146
		public enum MachineTypes
		{
			// Token: 0x0400032D RID: 813
			None = -1,
			// Token: 0x0400032E RID: 814
			Wall,
			// Token: 0x0400032F RID: 815
			BrokenWall,
			// Token: 0x04000330 RID: 816
			Ballista,
			// Token: 0x04000331 RID: 817
			Trebuchet,
			// Token: 0x04000332 RID: 818
			Ladder,
			// Token: 0x04000333 RID: 819
			Ram,
			// Token: 0x04000334 RID: 820
			SiegeTower,
			// Token: 0x04000335 RID: 821
			Mangonel
		}
	}
}
