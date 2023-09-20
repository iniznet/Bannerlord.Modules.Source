using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD.FormationMarker
{
	// Token: 0x020000EC RID: 236
	public class MissionSiegeEngineMarkerVM : ViewModel
	{
		// Token: 0x06001514 RID: 5396 RVA: 0x00044B3C File Offset: 0x00042D3C
		public MissionSiegeEngineMarkerVM(Mission mission, Camera missionCamera)
		{
			this._mission = mission;
			this._missionCamera = missionCamera;
			this._comparer = new MissionSiegeEngineMarkerVM.SiegeEngineMarkerDistanceComparer();
			this.Targets = new MBBindingList<MissionSiegeEngineMarkerTargetVM>();
		}

		// Token: 0x06001515 RID: 5397 RVA: 0x00044B94 File Offset: 0x00042D94
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
		}

		// Token: 0x06001516 RID: 5398 RVA: 0x00044C38 File Offset: 0x00042E38
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

		// Token: 0x06001517 RID: 5399 RVA: 0x00044CE0 File Offset: 0x00042EE0
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

		// Token: 0x06001518 RID: 5400 RVA: 0x00044D74 File Offset: 0x00042F74
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

		// Token: 0x06001519 RID: 5401 RVA: 0x00044EA0 File Offset: 0x000430A0
		private void SortMarkersInList()
		{
			this.Targets.Sort(this._comparer);
		}

		// Token: 0x0600151A RID: 5402 RVA: 0x00044EB4 File Offset: 0x000430B4
		private void RefreshSiegeEngineItemProperties()
		{
			foreach (MissionSiegeEngineMarkerTargetVM missionSiegeEngineMarkerTargetVM in this.Targets)
			{
				missionSiegeEngineMarkerTargetVM.Refresh();
			}
		}

		// Token: 0x0600151B RID: 5403 RVA: 0x00044F00 File Offset: 0x00043100
		private void UpdateTargetStates(bool isEnabled)
		{
			foreach (MissionSiegeEngineMarkerTargetVM missionSiegeEngineMarkerTargetVM in this.Targets)
			{
				missionSiegeEngineMarkerTargetVM.IsEnabled = isEnabled;
			}
		}

		// Token: 0x0600151C RID: 5404 RVA: 0x00044F4C File Offset: 0x0004314C
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

		// Token: 0x170006ED RID: 1773
		// (get) Token: 0x0600151D RID: 5405 RVA: 0x00044F6C File Offset: 0x0004316C
		// (set) Token: 0x0600151E RID: 5406 RVA: 0x00044F74 File Offset: 0x00043174
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

		// Token: 0x170006EE RID: 1774
		// (get) Token: 0x0600151F RID: 5407 RVA: 0x00044F99 File Offset: 0x00043199
		// (set) Token: 0x06001520 RID: 5408 RVA: 0x00044FA1 File Offset: 0x000431A1
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

		// Token: 0x04000A15 RID: 2581
		private Mission _mission;

		// Token: 0x04000A16 RID: 2582
		private Camera _missionCamera;

		// Token: 0x04000A17 RID: 2583
		private List<SiegeWeapon> _siegeEngines;

		// Token: 0x04000A18 RID: 2584
		private Vec3 _heightOffset = new Vec3(0f, 0f, 3f, -1f);

		// Token: 0x04000A19 RID: 2585
		private bool _prevIsEnabled;

		// Token: 0x04000A1A RID: 2586
		private MissionSiegeEngineMarkerVM.SiegeEngineMarkerDistanceComparer _comparer;

		// Token: 0x04000A1B RID: 2587
		private bool _fadeOutTimerStarted;

		// Token: 0x04000A1C RID: 2588
		private float _fadeOutTimer;

		// Token: 0x04000A1D RID: 2589
		private bool _isEnabled;

		// Token: 0x04000A1E RID: 2590
		private MBBindingList<MissionSiegeEngineMarkerTargetVM> _targets;

		// Token: 0x0200023A RID: 570
		public class SiegeEngineMarkerDistanceComparer : IComparer<MissionSiegeEngineMarkerTargetVM>
		{
			// Token: 0x06001B47 RID: 6983 RVA: 0x00057734 File Offset: 0x00055934
			public int Compare(MissionSiegeEngineMarkerTargetVM x, MissionSiegeEngineMarkerTargetVM y)
			{
				return y.Distance.CompareTo(x.Distance);
			}
		}
	}
}
