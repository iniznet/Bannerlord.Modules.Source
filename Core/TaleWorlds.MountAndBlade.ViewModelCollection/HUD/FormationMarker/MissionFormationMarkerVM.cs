using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order;

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
		}

		public void Tick(float dt)
		{
			if (this.IsEnabled)
			{
				this.RefreshFormationListInMission();
				this.RefreshFormationPositions();
				this.RefreshFormationItemProperties();
				this.SortMarkersInList();
				this.RefreshTargetProperties();
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
						missionFormationMarkerTargetVM.IsFormationTargetRelevant = this.IsFormationTargetRelevant;
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
					MBWindowManager.WorldToScreen(this._missionCamera, medianPosition.GetGroundVec3() + this._heightOffset, ref num, ref num2, ref num3);
					missionFormationMarkerTargetVM.IsInsideScreenBoundaries = num <= Screen.RealScreenResolutionWidth && num2 <= Screen.RealScreenResolutionHeight && num + 200f >= 0f && num2 + 100f >= 0f;
					missionFormationMarkerTargetVM.WSign = MathF.Sign(num3);
				}
				if (!missionFormationMarkerTargetVM.IsTargetingAFormation && (!medianPosition.IsValid || num3 < 0f || !MathF.IsValidValue(num) || !MathF.IsValidValue(num2)))
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

		private void RefreshTargetProperties()
		{
			List<Formation> list = new List<Formation>();
			Agent main = Agent.Main;
			MBReadOnlyList<Formation> mbreadOnlyList;
			if (main == null)
			{
				mbreadOnlyList = null;
			}
			else
			{
				OrderController playerOrderController = main.Team.PlayerOrderController;
				mbreadOnlyList = ((playerOrderController != null) ? playerOrderController.SelectedFormations : null);
			}
			MBReadOnlyList<Formation> mbreadOnlyList2 = mbreadOnlyList;
			if (mbreadOnlyList2 != null)
			{
				for (int i = 0; i < mbreadOnlyList2.Count; i++)
				{
					if (mbreadOnlyList2[i].TargetFormation != null && OrderUIHelper.CanOrderHaveTarget(OrderUIHelper.GetActiveMovementOrderOfFormation(mbreadOnlyList2[i])))
					{
						list.Add(mbreadOnlyList2[i].TargetFormation);
					}
				}
			}
			for (int j = 0; j < this.Targets.Count; j++)
			{
				MissionFormationMarkerTargetVM missionFormationMarkerTargetVM = this.Targets[j];
				if (missionFormationMarkerTargetVM.TeamType == 2)
				{
					bool flag = list.Contains(missionFormationMarkerTargetVM.Formation);
					MissionFormationMarkerTargetVM missionFormationMarkerTargetVM2 = missionFormationMarkerTargetVM;
					MBReadOnlyList<Formation> focusedFormations = this._focusedFormations;
					missionFormationMarkerTargetVM2.SetTargetedState(focusedFormations != null && focusedFormations.Contains(missionFormationMarkerTargetVM.Formation), flag);
				}
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

		private void UpdateTargetStates(bool isEnabled, bool isFormationTargetRelevant)
		{
			foreach (MissionFormationMarkerTargetVM missionFormationMarkerTargetVM in this.Targets)
			{
				missionFormationMarkerTargetVM.IsEnabled = isEnabled;
				missionFormationMarkerTargetVM.IsFormationTargetRelevant = isFormationTargetRelevant;
			}
		}

		public void SetFocusedFormations(MBReadOnlyList<Formation> focusedFormations)
		{
			this._focusedFormations = focusedFormations;
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
					for (int i = 0; i < this.Targets.Count; i++)
					{
						this.Targets[i].IsEnabled = value;
					}
				}
			}
		}

		[DataSourceProperty]
		public bool IsFormationTargetRelevant
		{
			get
			{
				return this._isFormationTargetRelevant;
			}
			set
			{
				if (value != this._isFormationTargetRelevant)
				{
					this._isFormationTargetRelevant = value;
					base.OnPropertyChangedWithValue(value, "IsFormationTargetRelevant");
					for (int i = 0; i < this.Targets.Count; i++)
					{
						this.Targets[i].IsFormationTargetRelevant = value;
					}
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

		private readonly Mission _mission;

		private readonly Camera _missionCamera;

		private readonly MissionFormationMarkerVM.FormationMarkerDistanceComparer _comparer;

		private readonly Vec3 _heightOffset = new Vec3(0f, 0f, 3f, -1f);

		private bool _prevIsEnabled;

		private bool _fadeOutTimerStarted;

		private float _fadeOutTimer;

		private MBReadOnlyList<Formation> _focusedFormations;

		private bool _isEnabled;

		private bool _isFormationTargetRelevant;

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
