using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD.FormationMarker
{
	public class MissionSiegeEngineMarkerVM : ViewModel
	{
		public bool IsInitialized { get; private set; }

		public MissionSiegeEngineMarkerVM(Mission mission, Camera missionCamera)
		{
			this._mission = mission;
			this._missionCamera = missionCamera;
			this._comparer = new MissionSiegeEngineMarkerVM.SiegeEngineMarkerDistanceComparer();
			this.Targets = new MBBindingList<MissionSiegeEngineMarkerTargetVM>();
		}

		public void InitializeWith(List<SiegeWeapon> siegeEngines)
		{
			this._siegeEngines = siegeEngines;
			for (int i = 0; i < this._siegeEngines.Count; i++)
			{
				SiegeWeapon engine = this._siegeEngines[i];
				BattleSideEnum side = this._mission.PlayerTeam.Side;
				if (!this.Targets.Any((MissionSiegeEngineMarkerTargetVM t) => t.Engine == engine))
				{
					MissionSiegeEngineMarkerTargetVM missionSiegeEngineMarkerTargetVM = new MissionSiegeEngineMarkerTargetVM(engine, engine.Side != side);
					this.Targets.Add(missionSiegeEngineMarkerTargetVM);
					missionSiegeEngineMarkerTargetVM.IsEnabled = this.IsEnabled;
				}
			}
			this.IsInitialized = true;
		}

		public void Tick(float dt)
		{
			if (this._siegeEngines != null)
			{
				if (this.IsEnabled)
				{
					this.RefreshSiegeEngineList();
					this.RefreshSiegeEnginePositions();
					this.RefreshSiegeEngineItemProperties();
					this.SortMarkersInList();
					this._fadeOutTimerStarted = false;
					this._fadeOutTimer = 0f;
					this._prevIsEnabled = this.IsEnabled;
				}
				else
				{
					if (this._prevIsEnabled)
					{
						this._fadeOutTimerStarted = true;
					}
					if (this._fadeOutTimerStarted)
					{
						this._fadeOutTimer += dt;
					}
					if (this._fadeOutTimer < 2f)
					{
						this.RefreshSiegeEnginePositions();
					}
					else
					{
						this._fadeOutTimerStarted = false;
					}
				}
				this._prevIsEnabled = this.IsEnabled;
			}
		}

		private void RefreshSiegeEngineList()
		{
			bool isDefender = this._mission.PlayerTeam.IsDefender;
			for (int i = this._siegeEngines.Count - 1; i >= 0; i--)
			{
				SiegeWeapon engine = this._siegeEngines[i];
				if (engine.DestructionComponent.IsDestroyed)
				{
					this._siegeEngines.RemoveAt(i);
					MissionSiegeEngineMarkerTargetVM missionSiegeEngineMarkerTargetVM = this.Targets.SingleOrDefault((MissionSiegeEngineMarkerTargetVM t) => t.Engine == engine);
					this.Targets.Remove(missionSiegeEngineMarkerTargetVM);
				}
			}
		}

		private void RefreshSiegeEnginePositions()
		{
			foreach (MissionSiegeEngineMarkerTargetVM missionSiegeEngineMarkerTargetVM in this.Targets)
			{
				float num = 0f;
				float num2 = 0f;
				float num3 = 0f;
				Vec3 globalPosition = missionSiegeEngineMarkerTargetVM.Engine.GameEntity.GlobalPosition;
				MBWindowManager.WorldToScreenInsideUsableArea(this._missionCamera, globalPosition + this._heightOffset, ref num, ref num2, ref num3);
				if (num3 < 0f || !MathF.IsValidValue(num) || !MathF.IsValidValue(num2))
				{
					num = -10000f;
					num2 = -10000f;
					num3 = 0f;
				}
				if (this._prevIsEnabled && this.IsEnabled)
				{
					missionSiegeEngineMarkerTargetVM.ScreenPosition = Vec2.Lerp(missionSiegeEngineMarkerTargetVM.ScreenPosition, new Vec2(num, num2), 0.9f);
				}
				else
				{
					missionSiegeEngineMarkerTargetVM.ScreenPosition = new Vec2(num, num2);
				}
				MissionSiegeEngineMarkerTargetVM missionSiegeEngineMarkerTargetVM2 = missionSiegeEngineMarkerTargetVM;
				Agent main = Agent.Main;
				missionSiegeEngineMarkerTargetVM2.Distance = ((main != null && main.IsActive()) ? Agent.Main.Position.Distance(globalPosition) : num3);
			}
		}

		private void SortMarkersInList()
		{
			this.Targets.Sort(this._comparer);
		}

		private void RefreshSiegeEngineItemProperties()
		{
			foreach (MissionSiegeEngineMarkerTargetVM missionSiegeEngineMarkerTargetVM in this.Targets)
			{
				missionSiegeEngineMarkerTargetVM.Refresh();
			}
		}

		private void UpdateTargetStates(bool isEnabled)
		{
			foreach (MissionSiegeEngineMarkerTargetVM missionSiegeEngineMarkerTargetVM in this.Targets)
			{
				missionSiegeEngineMarkerTargetVM.IsEnabled = isEnabled;
			}
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			List<SiegeWeapon> siegeEngines = this._siegeEngines;
			if (siegeEngines != null)
			{
				siegeEngines.Clear();
			}
			this._siegeEngines = null;
		}

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
					this.UpdateTargetStates(value);
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<MissionSiegeEngineMarkerTargetVM> Targets
		{
			get
			{
				return this._targets;
			}
			set
			{
				if (value != this._targets)
				{
					this._targets = value;
					base.OnPropertyChangedWithValue<MBBindingList<MissionSiegeEngineMarkerTargetVM>>(value, "Targets");
				}
			}
		}

		private Mission _mission;

		private Camera _missionCamera;

		private List<SiegeWeapon> _siegeEngines;

		private Vec3 _heightOffset = new Vec3(0f, 0f, 3f, -1f);

		private bool _prevIsEnabled;

		private MissionSiegeEngineMarkerVM.SiegeEngineMarkerDistanceComparer _comparer;

		private bool _fadeOutTimerStarted;

		private float _fadeOutTimer;

		private bool _isEnabled;

		private MBBindingList<MissionSiegeEngineMarkerTargetVM> _targets;

		public class SiegeEngineMarkerDistanceComparer : IComparer<MissionSiegeEngineMarkerTargetVM>
		{
			public int Compare(MissionSiegeEngineMarkerTargetVM x, MissionSiegeEngineMarkerTargetVM y)
			{
				return y.Distance.CompareTo(x.Distance);
			}
		}
	}
}
