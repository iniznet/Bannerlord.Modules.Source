using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD.FormationMarker
{
	public class MissionFormationMarkerVM : ViewModel
	{
		public MissionFormationMarkerVM(Mission mission, Camera missionCamera)
		{
			this._mission = mission;
			this._missionCamera = missionCamera;
			this._comparer = new MissionFormationMarkerVM.FormationMarkerDistanceComparer();
			this.Targets = new MBBindingList<MissionFormationMarkerTargetVM>();
			this._formationTargetsMap = new Dictionary<object, MissionFormationMarkerTargetVM>();
		}

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

		private void SortMarkersInList()
		{
			this.Targets.Sort(this._comparer);
		}

		private void RefreshFormationItemProperties()
		{
			foreach (MissionFormationMarkerTargetVM missionFormationMarkerTargetVM in this.Targets)
			{
				missionFormationMarkerTargetVM.Refresh();
			}
		}

		private void UpdateTargetStates(bool isEnabled)
		{
			foreach (MissionFormationMarkerTargetVM missionFormationMarkerTargetVM in this.Targets)
			{
				missionFormationMarkerTargetVM.IsEnabled = isEnabled;
			}
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

		private Mission _mission;

		private Camera _missionCamera;

		private Dictionary<object, MissionFormationMarkerTargetVM> _formationTargetsMap;

		private Vec3 _heightOffset = new Vec3(0f, 0f, 3f, -1f);

		private bool _prevIsEnabled;

		private MissionFormationMarkerVM.FormationMarkerDistanceComparer _comparer;

		private bool _fadeOutTimerStarted;

		private float _fadeOutTimer;

		private bool _isEnabled;

		private MBBindingList<MissionFormationMarkerTargetVM> _targets;

		public class FormationMarkerDistanceComparer : IComparer<MissionFormationMarkerTargetVM>
		{
			public int Compare(MissionFormationMarkerTargetVM x, MissionFormationMarkerTargetVM y)
			{
				return y.Distance.CompareTo(x.Distance);
			}
		}
	}
}
