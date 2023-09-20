using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD.FormationMarker
{
	// Token: 0x020000EB RID: 235
	public class MissionFormationMarkerVM : ViewModel
	{
		// Token: 0x06001509 RID: 5385 RVA: 0x000446B0 File Offset: 0x000428B0
		public MissionFormationMarkerVM(Mission mission, Camera missionCamera)
		{
			this._mission = mission;
			this._missionCamera = missionCamera;
			this._comparer = new MissionFormationMarkerVM.FormationMarkerDistanceComparer();
			this.Targets = new MBBindingList<MissionFormationMarkerTargetVM>();
			this._formationTargetsMap = new Dictionary<object, MissionFormationMarkerTargetVM>();
		}

		// Token: 0x0600150A RID: 5386 RVA: 0x00044714 File Offset: 0x00042914
		public void Tick(float dt)
		{
			if (this.IsEnabled)
			{
				this.RefreshFormationListInMission();
				this.RefreshFormationPositions();
				this.RefreshFormationItemProperties();
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
					this.RefreshFormationPositions();
				}
				else
				{
					this._fadeOutTimerStarted = false;
				}
			}
			this._prevIsEnabled = this.IsEnabled;
		}

		// Token: 0x0600150B RID: 5387 RVA: 0x000447B0 File Offset: 0x000429B0
		private void RefreshFormationListInMission()
		{
			IEnumerable<Formation> formationList = this._mission.Teams.SelectMany((Team t) => t.FormationsIncludingEmpty.WhereQ((Formation f) => f.CountOfUnits > 0));
			using (IEnumerator<Formation> enumerator = formationList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Formation formation = enumerator.Current;
					if (this.Targets.All((MissionFormationMarkerTargetVM t) => t.Formation != formation))
					{
						MissionFormationMarkerTargetVM missionFormationMarkerTargetVM = new MissionFormationMarkerTargetVM(formation);
						this.Targets.Add(missionFormationMarkerTargetVM);
						missionFormationMarkerTargetVM.IsEnabled = this.IsEnabled;
					}
				}
			}
			if (formationList.CountQ<Formation>() < this.Targets.Count)
			{
				foreach (MissionFormationMarkerTargetVM missionFormationMarkerTargetVM2 in this.Targets.WhereQ((MissionFormationMarkerTargetVM t) => !formationList.Contains(t.Formation)).ToList<MissionFormationMarkerTargetVM>())
				{
					this.Targets.Remove(missionFormationMarkerTargetVM2);
				}
			}
		}

		// Token: 0x0600150C RID: 5388 RVA: 0x000448F4 File Offset: 0x00042AF4
		private void RefreshFormationPositions()
		{
			for (int i = 0; i < this.Targets.Count; i++)
			{
				MissionFormationMarkerTargetVM missionFormationMarkerTargetVM = this.Targets[i];
				float num = 0f;
				float num2 = 0f;
				float num3 = 0f;
				WorldPosition medianPosition = missionFormationMarkerTargetVM.Formation.QuerySystem.MedianPosition;
				medianPosition.SetVec2(missionFormationMarkerTargetVM.Formation.QuerySystem.AveragePosition);
				if (medianPosition.IsValid)
				{
					MBWindowManager.WorldToScreenInsideUsableArea(this._missionCamera, medianPosition.GetGroundVec3() + this._heightOffset, ref num, ref num2, ref num3);
				}
				if (!medianPosition.IsValid || num3 < 0f || !MathF.IsValidValue(num) || !MathF.IsValidValue(num2))
				{
					num = -10000f;
					num2 = -10000f;
					num3 = 0f;
				}
				if (this._prevIsEnabled && this.IsEnabled)
				{
					missionFormationMarkerTargetVM.ScreenPosition = Vec2.Lerp(missionFormationMarkerTargetVM.ScreenPosition, new Vec2(num, num2), 0.9f);
				}
				else
				{
					missionFormationMarkerTargetVM.ScreenPosition = new Vec2(num, num2);
				}
				MissionFormationMarkerTargetVM missionFormationMarkerTargetVM2 = missionFormationMarkerTargetVM;
				Agent main = Agent.Main;
				missionFormationMarkerTargetVM2.Distance = ((main != null && main.IsActive()) ? Agent.Main.Position.Distance(medianPosition.GetGroundVec3()) : num3);
			}
		}

		// Token: 0x0600150D RID: 5389 RVA: 0x00044A3B File Offset: 0x00042C3B
		private void SortMarkersInList()
		{
			this.Targets.Sort(this._comparer);
		}

		// Token: 0x0600150E RID: 5390 RVA: 0x00044A50 File Offset: 0x00042C50
		private void RefreshFormationItemProperties()
		{
			foreach (MissionFormationMarkerTargetVM missionFormationMarkerTargetVM in this.Targets)
			{
				missionFormationMarkerTargetVM.Refresh();
			}
		}

		// Token: 0x0600150F RID: 5391 RVA: 0x00044A9C File Offset: 0x00042C9C
		private void UpdateTargetStates(bool isEnabled)
		{
			foreach (MissionFormationMarkerTargetVM missionFormationMarkerTargetVM in this.Targets)
			{
				missionFormationMarkerTargetVM.IsEnabled = isEnabled;
			}
		}

		// Token: 0x170006EB RID: 1771
		// (get) Token: 0x06001510 RID: 5392 RVA: 0x00044AE8 File Offset: 0x00042CE8
		// (set) Token: 0x06001511 RID: 5393 RVA: 0x00044AF0 File Offset: 0x00042CF0
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

		// Token: 0x170006EC RID: 1772
		// (get) Token: 0x06001512 RID: 5394 RVA: 0x00044B15 File Offset: 0x00042D15
		// (set) Token: 0x06001513 RID: 5395 RVA: 0x00044B1D File Offset: 0x00042D1D
		[DataSourceProperty]
		public MBBindingList<MissionFormationMarkerTargetVM> Targets
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
					base.OnPropertyChangedWithValue<MBBindingList<MissionFormationMarkerTargetVM>>(value, "Targets");
				}
			}
		}

		// Token: 0x04000A0B RID: 2571
		private Mission _mission;

		// Token: 0x04000A0C RID: 2572
		private Camera _missionCamera;

		// Token: 0x04000A0D RID: 2573
		private Dictionary<object, MissionFormationMarkerTargetVM> _formationTargetsMap;

		// Token: 0x04000A0E RID: 2574
		private Vec3 _heightOffset = new Vec3(0f, 0f, 3f, -1f);

		// Token: 0x04000A0F RID: 2575
		private bool _prevIsEnabled;

		// Token: 0x04000A10 RID: 2576
		private MissionFormationMarkerVM.FormationMarkerDistanceComparer _comparer;

		// Token: 0x04000A11 RID: 2577
		private bool _fadeOutTimerStarted;

		// Token: 0x04000A12 RID: 2578
		private float _fadeOutTimer;

		// Token: 0x04000A13 RID: 2579
		private bool _isEnabled;

		// Token: 0x04000A14 RID: 2580
		private MBBindingList<MissionFormationMarkerTargetVM> _targets;

		// Token: 0x02000236 RID: 566
		public class FormationMarkerDistanceComparer : IComparer<MissionFormationMarkerTargetVM>
		{
			// Token: 0x06001B3D RID: 6973 RVA: 0x00057684 File Offset: 0x00055884
			public int Compare(MissionFormationMarkerTargetVM x, MissionFormationMarkerTargetVM y)
			{
				return y.Distance.CompareTo(x.Distance);
			}
		}
	}
}
