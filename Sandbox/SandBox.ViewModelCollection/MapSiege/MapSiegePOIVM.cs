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
	public class MapSiegePOIVM : ViewModel
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

		public MapSiegePOIVM.POIType Type { get; }

		public int MachineIndex { get; }

		public float LatestW
		{
			get
			{
				return this._latestW;
			}
		}

		public SiegeEvent.SiegeEngineConstructionProgress Machine { get; private set; }

		public MatrixFrame MapSceneLocationFrame { get; private set; }

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

		public void ExecuteSelection()
		{
			this._onSelection(this);
			this.IsSelected = true;
		}

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

		public void RefreshDistanceValue(float newDistance)
		{
			this._bindIsInVisibleRange = newDistance <= 20f;
		}

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

		private bool DetermineIfPOIIsPlayerSide()
		{
			MapSiegePOIVM.POIType type = this.Type;
			if (type > MapSiegePOIVM.POIType.DefenderSiegeMachine)
			{
				return type - MapSiegePOIVM.POIType.AttackerRamSiegeMachine <= 2 && this.PlayerSide == 1;
			}
			return this.PlayerSide == 0;
		}

		private bool IsInsideWindow()
		{
			return this._latestX <= Screen.RealScreenResolutionWidth && this._latestY <= Screen.RealScreenResolutionHeight && this._latestX + 200f >= 0f && this._latestY + 100f >= 0f;
		}

		public void ExecuteShowTooltip()
		{
			InformationManager.ShowTooltip(typeof(List<TooltipProperty>), new object[] { SandBoxUIHelper.GetSiegeEngineInProgressTooltip(this.Machine) });
		}

		public void ExecuteHideTooltip()
		{
			MBInformationManager.HideInformations();
		}

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

		private readonly Vec3 _mapSceneLocation;

		private readonly Camera _mapCamera;

		private readonly BattleSideEnum _thisSide;

		private readonly Action<MapSiegePOIVM> _onSelection;

		private float _latestX;

		private float _latestY;

		private float _latestW;

		private float _bindCurrentHitpoints;

		private float _bindMaxHitpoints;

		private float _bindWPos;

		private int _bindWSign;

		private int _bindMachineType = -1;

		private int _bindQueueIndex;

		private bool _bindIsInside;

		private bool _bindHasItem;

		private bool _bindIsConstructing;

		private Vec2 _bindPosition;

		private bool _bindIsInVisibleRange;

		private Vec2 _position;

		private bool _isInside;

		private float _currentHitpoints;

		private float _maxHitpoints;

		private bool _hasItem;

		private bool _isConstructing;

		private bool _isFireVersion;

		private bool _isInVisibleRange;

		private int _machineType = -1;

		private Color _sidePrimaryColor;

		private Color _sideSecondaryColor;

		private bool _isPlayerSidePOI;

		private int _queueIndex;

		private bool _isSelected;

		public enum POIType
		{
			WallSection,
			DefenderSiegeMachine,
			AttackerRamSiegeMachine,
			AttackerTowerSiegeMachine,
			AttackerRangedSiegeMachine
		}

		public enum MachineTypes
		{
			None = -1,
			Wall,
			BrokenWall,
			Ballista,
			Trebuchet,
			Ladder,
			Ram,
			SiegeTower,
			Mangonel
		}
	}
}
